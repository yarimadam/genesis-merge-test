using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreData.Common;
using CoreType.Types;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace CoreData.CacheManager
{
    public static class DistributedCache
    {
        public static IDatabase Database;

        private static List<(string DictionaryKey, Delegate DelegateMethod, TimeSpan Expiry)> delegatePairs =
            new List<(string DictionaryKey, Delegate DelegateMethod, TimeSpan Expiry)>();

        static DistributedCache()
        {
            try
            {
                Database = RedisConnectionFactory.GetDatabase();
            }
            catch (Exception ex)
            {
                throw new Exception("Redis not configured properly !", ex);
            }
        }

        public static string CreateClaimKey(int userId) => $"{Constants.SESSION_KEY_CLAIMS}{userId}";
        public static string CreateSessionKey(int userId) => $"{Constants.SESSION_KEY_CONTEXT}{userId}";
        public static string CreateParameterKey(string keyCode, string lang) => $"{Constants.PARAMETER_KEY_PREFIX}{{{keyCode}}}:{lang}";

        #region RegisterDelegate

        public static void RegisterDelegate(string dictionaryKey, Action delegateMethod, TimeSpan? expiry = null)
        {
            delegatePairs.Add((dictionaryKey, (Delegate) delegateMethod,
                expiry ?? Constants.CACHE_DELEGATE_EXPIRATION));
        }

        public static void RegisterDelegate<R>(string dictionaryKey, Func<R> delegateMethod, TimeSpan? expiry = null)
        {
            delegatePairs.Add((dictionaryKey, (Delegate) delegateMethod,
                expiry ?? Constants.CACHE_DELEGATE_EXPIRATION));
        }

        public static void RegisterDelegate<R>(string dictionaryKey, Func<bool, R> delegateMethod,
            TimeSpan? expiry = null)
        {
            delegatePairs.Add((dictionaryKey, (Delegate) delegateMethod,
                expiry ?? Constants.CACHE_DELEGATE_EXPIRATION));
        }

        public static void RegisterDelegate(string dictionaryKey, Func<string, string, string> delegateMethod,
            TimeSpan? expiry = null)
        {
            delegatePairs.Add((dictionaryKey, (Delegate) delegateMethod,
                expiry ?? Constants.CACHE_DELEGATE_EXPIRATION));
        }

        #endregion RegisterDelegate

        public static string Get(string dictionaryKey, bool ignoreDatabase = false, bool allowKeyAsDefault = true)
        {
            var result = Get<string>(dictionaryKey, ignoreDatabase);

            if (allowKeyAsDefault)
                return result ?? string.Empty;
            else
                return result;
        }

        public static T Get<T>(string dictionaryKey, bool ignoreDatabase = false)
        {
            return GetAsync<T>(dictionaryKey, ignoreDatabase).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<string> GetAsync(string dictionaryKey, bool ignoreDatabase = false,
            bool allowKeyAsDefault = true)
        {
            var result = await GetAsync<string>(dictionaryKey, ignoreDatabase);

            if (allowKeyAsDefault)
                return result ?? string.Empty;
            else
                return result;
        }

        public static async Task<T> GetAsync<T>(string dictionaryKey, bool ignoreDatabase = false)
        {
            try
            {
                var valueStr = "";

                try
                {
                    valueStr = await Database.StringGetAsync(dictionaryKey);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "DistributedCache.Get1-Exception");
                }

                if (!string.IsNullOrEmpty(valueStr))
                {
                    if (typeof(T) == typeof(string) && valueStr is T str)
                        return str;

                    return JsonConvert.DeserializeObject<T>(valueStr);
                }
                else if (delegatePairs.FindIndex(x => x.DictionaryKey == dictionaryKey) > 0)
                {
                    RedisValue token = Guid.NewGuid().ToString();
                    var lockKey = $"{dictionaryKey}_lock";
                    var filteredDelegatePairs = delegatePairs.Where(x => x.DictionaryKey == dictionaryKey);

                    if (LockTake(lockKey, token, Constants.CACHE_LOCK_EXPIRATION))
                    {
                        try
                        {
                            object result = null;
                            var expiry = Constants.CACHE_DELEGATE_EXPIRATION;

                            foreach (var delegatePair in filteredDelegatePairs)
                            {
                                try
                                {
                                    var delegateFunc = delegatePair.DelegateMethod;
                                    var parameters = delegateFunc.Method.GetParameters();
                                    expiry = delegatePair.Expiry;

                                    if (parameters.Length == 0)
                                        result = delegateFunc.DynamicInvoke();
                                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(bool))
                                        result = delegateFunc.DynamicInvoke(true);
                                }
                                catch (Exception e)
                                {
                                    Log.Fatal(e, "DelegatePair-Loop");
                                    throw;
                                }
                            }

                            if (result != null)
                                await Database.StringSetAsync(dictionaryKey,
                                    JsonConvert.SerializeObject(result), expiry);

                            if (result is T resultT)
                                return resultT;
                            else
                                return default;
                        }
                        finally
                        {
                            LockRelease(lockKey, token);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DistributedCache.GetAsync");
            }

            return default;
        }

        public static string GetParam(string keyCode, bool ignoreDatabase = false)
        {
            return GetParamAsync(keyCode, ignoreDatabase).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static string GetParam(string keyCode, string lang, bool ignoreDatabase = false)
        {
            return GetParamAsync(keyCode, ignoreDatabase, new List<string> { lang }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<string> GetParamAsync(string keyCode, bool ignoreDatabase = false)
        {
            var session = SessionAccessor.GetSession();
            var preferredLang = session?.PreferredLocale;

            var preferredLangs = new List<string>();
            if (!string.IsNullOrEmpty(preferredLang))
                preferredLangs.Add(preferredLang);

            return await GetParamAsync(keyCode, ignoreDatabase, preferredLangs);
        }

        public static async Task<string> GetParamAsync(string keyCode, bool ignoreDatabase, List<string> preferredLangs)
        {
            preferredLangs ??= new List<string>();

            if (!preferredLangs.Contains(Constants.DEFAULT_LANGUAGE))
                preferredLangs.Add(Constants.DEFAULT_LANGUAGE);

            foreach (string lang in preferredLangs)
            {
                try
                {
                    var tempDictionaryKey = CreateParameterKey(keyCode, lang);
                    var label = "";
                    try
                    {
                        label = await Database.StringGetAsync(tempDictionaryKey);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "DistributedCache.Get1-Exception");
                    }

                    if (!string.IsNullOrEmpty(label))
                        return label;
                    else if (!ignoreDatabase)
                    {
                        label = GetLabelFromDB(keyCode, lang);

                        if (!string.IsNullOrEmpty(label))
                        {
                            try
                            {
                                await Database.StringSetAsync(tempDictionaryKey, label, Constants.CACHE_ABSOLUTE_EXPIRATION);
                            }
                            catch (Exception ex)
                            {
                                Log.Fatal(ex, "DistributedCache.GetParamAsync-Exception");
                            }

                            return label;
                        }
                    }
                }
                catch (TimeoutException ex)
                {
                    Log.Fatal(ex, "DistributedCache.GetParamAsync-TimeoutException");

                    var label = GetLabelFromDB(keyCode, lang);
                    if (!string.IsNullOrEmpty(label))
                        return label;
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "DistributedCache.GetParamAsync");
                }
            }

            return keyCode ?? string.Empty;
        }

        public static RedisKey[] GetKeys(string pattern, int pageSize)
        {
            return GetKeysAsync(pattern, pageSize).GetAwaiter().GetResult();
        }

        public static async Task<RedisKey[]> GetKeysAsync(string pattern, int pageSize)
        {
            var endPoint = await Database.IdentifyEndpointAsync();
            var connection = RedisConnectionFactory.GetConnection();
            var databaseIndex = Database.Database;
            var server = connection.GetServer(endPoint);

            return server.Keys(databaseIndex, pattern, pageSize).ToArray();
        }

        public static bool Set(string dictionaryKey, Translations translations)
        {
            return SetAsync(dictionaryKey, translations).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<bool> SetAsync(string dictionaryKey, Translations translations)
        {
            if (translations == null) return false;

            PropertyInfo[] properties = typeof(Translations).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    object value = property.GetValue(translations);

                    var lang = property.Name;
                    var tempDictionaryKey = CreateParameterKey(dictionaryKey, lang);

                    if (value == null)
                    {
                        await Database.KeyDeleteAsync(tempDictionaryKey);
                        continue;
                    }

                    await Database.StringSetAsync(tempDictionaryKey, value.ToString(),
                        Constants.CACHE_ABSOLUTE_EXPIRATION);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "DistributedCache.Set");
                }
            }

            return true;
        }

        public static bool Set(string dictionaryKey, string value, TimeSpan? expiry = null)
        {
            return SetAsync(dictionaryKey, value, expiry).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<bool> SetAsync(string dictionaryKey, string value, TimeSpan? expiry = null)
        {
            try
            {
                return await Database.StringSetAsync(dictionaryKey, value, expiry ?? Constants.CACHE_ABSOLUTE_EXPIRATION);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DistributedCache.SetAsync");
            }

            return false;
        }

        public static bool Delete(params string[] keys)
        {
            return DeleteAsync(keys).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<bool> DeleteAsync(params string[] keys)
        {
            try
            {
                var deletedCount = await Database.KeyDeleteAsync(keys.Select(key => (RedisKey) key).ToArray());
                return keys.Length == deletedCount;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DistributedCache.DeleteAsync");
            }

            return false;
        }

        public static string GetLabelFromDB(string dictionaryKey, string lang)
        {
            if (delegatePairs.FindIndex(x => x.DictionaryKey == "Parameters_Individual") > 0)
            {
                var tuple = delegatePairs.Find(x => x.DictionaryKey == "Parameters_Individual");
                var delegateFunc = tuple.DelegateMethod;
                var parameters = delegateFunc.Method.GetParameters();
                object result = null;

                if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) &&
                    parameters[1].ParameterType == typeof(string))
                {
                    result = delegateFunc.DynamicInvoke(dictionaryKey, lang);

                    var tempDictionaryKey = CreateParameterKey(dictionaryKey, lang);

                    if (result is string resultStr)
                    {
                        Database.StringSet(tempDictionaryKey, resultStr, tuple.Expiry);
                        return resultStr;
                    }
                }
            }

            return null;
        }

        public static bool LockTake(string lockKey, string lockToken, TimeSpan? timeout = null,
            TimeSpan? cacheLockExpiration = null)
        {
            var retryDelay = TimeSpan.FromMilliseconds(250);

            timeout ??= Constants.CACHE_LOCK_TIMEOUT;
            cacheLockExpiration ??= Constants.CACHE_LOCK_EXPIRATION;

            while (timeout.Value > retryDelay)
            {
                //check for access to cache object, trying to lock it
                if (Database.LockTake(lockKey, lockToken, cacheLockExpiration.Value))
                    return true;

                timeout = timeout.Value.Subtract(retryDelay);
                Thread.Sleep(retryDelay); //sleep for 100 milliseconds for next lock try. you can play with that
            }

            return false;
        }

        public static void LockRelease(string lockKey, string token)
        {
            Database.LockRelease(lockKey, token);
        }

        #region Helpers

        public static SessionContext GetSession(int userId)
        {
            return GetSessionAsync(userId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static Task<SessionContext> GetSessionAsync(int userId)
        {
            return GetAsync<SessionContext>(CreateSessionKey(userId), true);
        }

        public static bool SetSession(int userId, SessionContext sessionContext)
        {
            return SetSessionAsync(userId, sessionContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static Task<bool> SetSessionAsync(int userId, SessionContext sessionContext)
        {
            return SetAsync(CreateSessionKey(userId), JsonConvert.SerializeObject(sessionContext), TimeSpan.FromMinutes(1));
        }

        public static AuthorizationClaims GetClaims(int userId)
        {
            return GetClaimsAsync(userId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static Task<AuthorizationClaims> GetClaimsAsync(int userId)
        {
            return GetAsync<AuthorizationClaims>(CreateClaimKey(userId), true);
        }

        public static bool SetClaims(int userId, AuthorizationClaims claims)
        {
            return SetClaimsAsync(userId, claims).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static Task<bool> SetClaimsAsync(int userId, AuthorizationClaims claims)
        {
            return SetAsync(CreateClaimKey(userId), JsonConvert.SerializeObject(claims), TimeSpan.MaxValue);
        }

        public static bool ClearClaims(int userId)
        {
            return Delete(CreateClaimKey(userId));
        }

        public static bool ClearClaims(params int[] userIds)
        {
            var userKeys = userIds.Select(CreateClaimKey).ToArray();

            return Delete(userKeys);
        }

        public static bool ClearAllClaims()
        {
            return ClearAllClaimsAsync().GetAwaiter().GetResult();
        }

        public static async Task<bool> ClearAllClaimsAsync()
        {
            try
            {
                var pattern = $"{CreateClaimKey(int.MaxValue).Replace(int.MaxValue.ToString(), "*")}";
                var foundKeys = await GetKeysAsync(pattern, int.MaxValue);
                if (foundKeys.Any())
                    await Database.KeyDeleteAsync(foundKeys, CommandFlags.FireAndForget);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "DistributedCache.ClearAllClaims");
                return false;
            }

            return true;
        }

        #endregion
    }
}