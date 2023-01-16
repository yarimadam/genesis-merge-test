using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CoreData.CacheManager;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using CoreData.Repositories;
using CoreType.Types;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Encoder = System.Drawing.Imaging.Encoder;

namespace CoreData.Common
{
    public static class Helper
    {
        public static DbContextOptions<T> GetContextOptions<T>() where T : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();

            DefaultDbContextOptionsBuilder(optionsBuilder);

            return optionsBuilder.Options;
        }

        public static void DefaultDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            // Prevent access to services if process is started by EF tools.
            if (!Constants.IsEFToolRuntime)
            {
                var cacheInterceptor = ServiceLocator.Current?.GetService<SecondLevelCacheInterceptor>();
                if (cacheInterceptor != null)
                    optionsBuilder.AddInterceptors(cacheInterceptor);
            }

            // TODO Convert to IsDevelopment()
#if DEBUG
            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
#endif
        }

        public static DatabaseType GetDatabaseType(this DbContext context)
        {
            //var connectionTypeName = context.Database.GetDbConnection().GetType().Name;
            // public const string PostgreSqlConnectionTypeName = "NpgsqlConnection";
            // public const string MySqlConnectionTypeName = "MySqlConnection";
            // public const string MssqlConnectionTypeName = "SqlConnection";
            // public const string OracleConnectionTypeName = "OracleConnection";

            var db = context.Database;

            if (db.IsSqlServer())
                return DatabaseType.MSSQL;
            if (db.IsNpgsql())
                return DatabaseType.PostgreSQL;
            if (db.IsMySql())
                return DatabaseType.MySQL;
            if (db.IsOracle())
                return DatabaseType.Oracle;

            throw new ArgumentException($"Connection type is unknown !");
        }

        public static TContext GetDbContext<TContext>(SessionContext session = null) where TContext : DbContext
        {
            var context = ServiceLocator.Current?.GetService<TContext>();

            if (context == null)
            {
                Log.Information("Specified context type({contextType}) could not be found on IoC.", typeof(TContext).Name);
                context = (TContext) Activator.CreateInstance(typeof(TContext), GetContextOptions<TContext>());
            }

            if (context is ContextBase contextBase)
                contextBase.Session = session ?? SessionAccessor.GetSession();

            // context.DatabaseType = context.GetDatabaseType();

            return context;
        }

        public static DbContext GetDbContext(Type contextType, SessionContext session = null)
        {
            return (DbContext) typeof(Helper)
                .GetMethods()
                .FirstOrDefault(x => x.Name == nameof(GetDbContext) && x.IsGenericMethod)?
                .MakeGenericMethod(contextType)
                .Invoke(null, new object[] { session });
        }

        public static GenesisContextBase GetGenesisContext(SessionContext session = null)
        {
            var databasePreference = ConfigurationManager.GenesisDatabaseType;
            GenesisContextBase context;

            switch (databasePreference)
            {
                case DatabasePreference.PostgreSQL:
                    context = GetDbContext<genesisContextEx_PostgreSQL>(session);
                    break;
                case DatabasePreference.MySQL:
                    context = GetDbContext<genesisContextEx_MySQL>(session);
                    break;
                case DatabasePreference.MSSQL:
                    context = GetDbContext<genesisContextEx_MSSQL>(session);
                    break;
                case DatabasePreference.Oracle:
                    context = GetDbContext<genesisContextEx_Oracle>(session);
                    break;
                default:
                    throw new ArgumentException();
            }

            return context;
        }

        public static bool HasAuthorizationClaim(AuthorizationClaims authorizationClaims, IList<string> resourceCodes,
            ActionType actionType)
        {
            if (authorizationClaims == null || resourceCodes == null)
                return false;

            foreach (var resourceCode in resourceCodes)
            {
                var resource = authorizationClaims.AuthResources.SingleOrDefault(x => x.ResourceCode.Equals(resourceCode));

                if (resource == null || resource.Status != 1)
                    continue;

                var action = resource.AuthActions.SingleOrDefault(x => x.ActionType == (int) actionType);

                if (action == null || action.Status != 1)
                    continue;

                var userHasAccess = action.AuthUserRights.Any(x => x.Status == 1);
                if (userHasAccess)
                    return true;
                //if (action.AuthUserRights.Count > 0)
                //{
                //    AuthUserRight userRight = action.AuthUserRights.Find(x => x.Status == 1);
                //    return userRight != null;
                //}
                //else
                //return action.Status == 1;
            }

            return false;
        }

        //TODO Disabled due to provider change (Should be only enabled for Devart Trial) 
        public static string AppendLicensePathToConnString(string connectionString, string licenseFileName = "Devart.Data.Oracle.key")
        {
            return connectionString;
            if (string.IsNullOrEmpty(connectionString))
                return connectionString;
            if (connectionString.Contains("license key"))
                return connectionString;

            var keyPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), licenseFileName)).Replace(@"\", @"\\");
            return string.Concat(connectionString.TrimEnd(';'), $";license key=trial:{keyPath}");
        }

        public static void RegisterDelegates()
        {
            var parameterRepository = new ParameterRepository();
            var communicationDefinitionsRepository = new CommunicationDefinitionsRepository();

            DistributedCache.RegisterDelegate("Parameters", parameterRepository.CacheAllParameters);
            DistributedCache.RegisterDelegate("Parameters_Individual", parameterRepository.GetLocalizedParam);
            DistributedCache.RegisterDelegate("CommunicationDefinitions",
                communicationDefinitionsRepository.CacheCommunicationDefinitions);
            DistributedCache.RegisterDelegate("CommunicationTemplates",
                communicationDefinitionsRepository.CacheCommunicationTemplates);
        }

        public static object SimplifyClaims(AuthorizationClaims allClaims)
        {
            return allClaims?.AuthResources?.Select(resource =>
            {
                return new
                {
                    resource.ResourceId,
                    resource.ResourceCode,
                    resource.ParentResourceCode,
                    resource.Status,
                    AuthActions = resource.AuthActions
                        .Select(action =>
                        {
                            var authUserRight = action.AuthUserRights.FirstOrDefault();
                            return new
                            {
                                action.ActionId,
                                action.ActionType,
                                action.Status,
                                AuthUserRight = new
                                {
                                    authUserRight?.RightId,
                                    authUserRight?.Status
                                }
                            };
                        })
                };
            });
        }

        public static string GenerateUniqueKey()
        {
            Guid guid = Guid.NewGuid();
            string rString = Convert.ToBase64String(guid.ToByteArray());
            rString = rString.Replace("=", "");
            return rString;
        }

        public static byte[] ResizeThumbnail(byte[] imageBytes, int size = 150, int quality = 75)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;
            var ms = new MemoryStream(imageBytes);

            var imageFromStreem = Image.FromStream(ms);
            using (var image = new Bitmap(imageFromStreem))
            {
                var width = Convert.ToInt32(image.Width * size / (double) image.Height);
                var height = size;

                var resized = new Bitmap(width, height);

                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(image, 0, 0, width, height);

                    var qualityParamId = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1) { Param = { [0] = new EncoderParameter(qualityParamId, quality) } };
                    var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                    var ms2 = new MemoryStream();
                    resized.Save(ms2, codec, encoderParameters);
                    var byteImage = ms2.ToArray();

                    return byteImage;
                }
            }
        }

        public static string GetHashedString(string source, bool verifyResult = true)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            using (var sha256Hash = SHA256.Create())
            {
                var hash = GetHash(sha256Hash, source);

                if (verifyResult && !VerifyHash(sha256Hash, source, hash))
                    throw new CryptographicUnexpectedOperationException();
                return hash;
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (var t in data) sBuilder.Append(t.ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);

            // Create a StringComparer an compare the hashes.
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}