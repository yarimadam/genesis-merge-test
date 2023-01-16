using System;
using System.Collections.Generic;
using System.Linq;
using CoreData.Common;
using CoreType.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CoreData.Infrastructure
{
    public static class ConfigurationManager
    {
        private static IConfiguration _configuration;

        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

                    IConfigurationBuilder builder = new ConfigurationBuilder()
                        .AddJsonFile("coresettings.json", true, true)
                        .AddJsonFile($"coresettings.{environmentName}.json", true, true)
                        .AddJsonFile("coresettings.genesis.json", true, true)
                        .AddJsonFile($"coresettings.genesis.{environmentName}.json", true, true)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddJsonFile("appsettings.genesis.json", true, true)
                        .AddJsonFile($"appsettings.genesis.{environmentName}.json", true, true);

#if DEBUG
                    builder
                        .AddJsonFile("testsettings.json", true, true)
                        .AddJsonFile("testsettings.genesis.json", true, true)
                        .AddJsonFile("testsettings.project.json", true, true)
                        .AddJsonFile("testsettings.project.genesis.json", true, true);
#endif

                    _configuration = builder.Build();

                    if (environmentName == Environments.Development)
                    {
                        var configurationOutput = ((IConfigurationRoot) _configuration).GetDebugView();
                        Log.Debug($"-Configuration Output-\n{configurationOutput}");
                    }
                }

                return _configuration;
            }
            set => _configuration = value;
        }

        public static string GetValue(string key)
        {
            return GetValue<string>(key);
        }

        public static T GetValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }

        public static string GetConnectionString(string key)
        {
            return GetConnectionString(Enum.Parse<DatabasePreference>(key));
        }

        public static string GetConnectionString(DatabasePreference databasePreference = DatabasePreference.Default)
        {
            if (databasePreference == DatabasePreference.Default)
                databasePreference = DefaultDatabaseType;

            var connectionString = Configuration.GetConnectionString(databasePreference.ToString());

            if (databasePreference == DatabasePreference.GenesisDB)
                databasePreference = GenesisDatabaseType;

            if (databasePreference == DatabasePreference.WorkflowDB)
                databasePreference = WorkflowDatabaseType;

            if (databasePreference == DatabasePreference.Oracle)
                connectionString = Helper.AppendLicensePathToConnString(connectionString);

            return connectionString;
        }

        public static string[] GetAsArray(string key)
        {
            return Configuration.GetSection(key).AsEnumerable().Where(x => x.Value != null).Select(x => x.Value).ToArray();
        }

        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }

        #region Parameters

        private static KafkaSettings _kafkaSettings;
        public static KafkaSettings KafkaSettings => _kafkaSettings ??= Configuration.GetSection("Kafka").Get<KafkaSettings>();

        private static RedisSettings _redisSettings;
        public static RedisSettings RedisSettings => _redisSettings ??= Configuration.GetSection("Redis").Get<RedisSettings>();

        private static ElkSettings _elkSettings;
        public static ElkSettings ElkSettings => _elkSettings ??= Configuration.GetSection("ELK").Get<ElkSettings>();

        private static MailSettings _mailSettings;
        public static MailSettings MailSettings => _mailSettings ??= Configuration.GetSection("MailSettings").Get<MailSettings>();

        private static EncryptionSettings _encryptionSettings;
        public static EncryptionSettings EncryptionSettings => _encryptionSettings ??= Configuration.GetSection("Encryption").Get<EncryptionSettings>();

        private static HangfireSettings _hangfireSettings;
        public static HangfireSettings HangfireSettings => _hangfireSettings ??= Configuration.GetSection("Hangfire").Get<HangfireSettings>();

        private static string _identityUrl;
        public static string IdentityUrl => _identityUrl ??= Configuration.GetValue<string>("IdentityUrl");

        private static Dictionary<string, string> _identityServerSharedSecrets;

        public static Dictionary<string, string> IdentityServerSharedSecrets =>
            _identityServerSharedSecrets ??= Configuration.GetSection("IdentityServerSharedSecrets").Get<Dictionary<string, string>>();

        public static string GetIdentityServerSharedSecret(string clientId)
        {
            if (!string.IsNullOrEmpty(clientId) && IdentityServerSharedSecrets.ContainsKey(clientId))
                return IdentityServerSharedSecrets[clientId];

            return null;
        }

        private static List<string> _allowedCorsOrigins;
        public static List<string> AllowedCorsOrigins => _allowedCorsOrigins ??= ConfigurationManager.GetSection("AllowedCorsOrigins").Get<List<string>>();

        private static string _applicationUrl;
        public static string ApplicationUrl => _applicationUrl ??= Configuration.GetValue<string>("ApplicationUrl");

        private static DatabasePreference? _genesisDatabaseType;
        public static DatabasePreference GenesisDatabaseType => _genesisDatabaseType ??= Configuration.GetValue<DatabasePreference>("GenesisDBType");

        private static DatabasePreference? _defaultDatabaseType;
        public static DatabasePreference DefaultDatabaseType => _defaultDatabaseType ??= Configuration.GetValue<DatabasePreference>("DefaultDatabase");

        private static Uri _frontendBaseUri;
        public static Uri FrontendBaseUri => _frontendBaseUri ??= new Uri(Configuration.GetValue<string>("FrontendBaseUrl"));

        private static string _forgotPasswordRelativeUri;
        public static string ForgotPasswordRelativeUri => _forgotPasswordRelativeUri ??= Configuration.GetValue<string>("ForgotPasswordRelativeUrl");

        private static Uri _forgotPasswordUri;
        public static Uri ForgotPasswordUri => _forgotPasswordUri ??= new Uri(FrontendBaseUri, ForgotPasswordRelativeUri);

        private static string _registerRelativeUri;
        public static string RegisterRelativeUri => _registerRelativeUri ??= Configuration.GetValue<string>("RegisterRelativeUrl");

        private static Uri _registerUri;
        public static Uri RegisterUri => _registerUri ??= new Uri(FrontendBaseUri, RegisterRelativeUri);

        private static DatabasePreference? _workflowDatabaseType;
        public static DatabasePreference WorkflowDatabaseType => _workflowDatabaseType ??= Configuration.GetValue<DatabasePreference?>("WorkflowDBType") ?? GenesisDatabaseType;

        private static string _workflowDatabaseSchema;
        public static string WorkflowDatabaseSchema => _workflowDatabaseSchema ??= Configuration.GetValue<string>("WorkflowDBSchema");

        #endregion
    }
}