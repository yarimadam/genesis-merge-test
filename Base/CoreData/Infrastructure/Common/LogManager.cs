using CoreData.Common;
using Hangfire.Console.Extensions.Serilog;
using Serilog;
using Serilog.Events;

namespace CoreData.Infrastructure
{
    public static class LogManager
    {
        public static void Initialize(bool isConsoleApplication = false)
        {
            if (!Log.Logger.IsEnabled(LogEventLevel.Fatal))
                Log.Logger = isConsoleApplication
                    ? consoleAppLoggerConfig.CreateLogger()
                    : loggerConfig.CreateLogger();
        }

        private static readonly LoggerConfiguration _baseLoggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .ReadFrom.Configuration(ConfigurationManager.Configuration)
            .Enrich.FromLogContext();

        private static LoggerConfiguration _loggerConfig;

        private static LoggerConfiguration loggerConfig
        {
            get
            {
                if (_loggerConfig == null)
                {
                    _loggerConfig = _baseLoggerConfiguration
                        .WriteTo.Console()
                        .WriteTo.Hangfire().Enrich.WithHangfireContext();

                    _loggerConfig = WriteToFile(_loggerConfig);

                    _loggerConfig = WriteToLogStash(_loggerConfig);

                    _loggerConfig = Filter(_loggerConfig);
                }

                return _loggerConfig;
            }
        }

        private static LoggerConfiguration _consoleAppLoggerConfig;

        private static LoggerConfiguration consoleAppLoggerConfig
        {
            get
            {
                if (_consoleAppLoggerConfig == null)
                {
                    _consoleAppLoggerConfig = _baseLoggerConfiguration;

                    _consoleAppLoggerConfig = WriteToFile(_consoleAppLoggerConfig);

                    _consoleAppLoggerConfig = WriteToLogStash(_consoleAppLoggerConfig);

                    _consoleAppLoggerConfig = Filter(_consoleAppLoggerConfig);
                }

                return _consoleAppLoggerConfig;
            }
        }

        private static LoggerConfiguration WriteToFile(LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration.WriteTo.File(Constants.LOG_FILE_PATH_PATTERN,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information);
        }

        private static LoggerConfiguration WriteToLogStash(LoggerConfiguration loggerConfiguration)
        {
            if (ConfigurationManager.ElkSettings?.Enabled == true)
                return loggerConfiguration.WriteTo.Http(
                    ConfigurationManager.ElkSettings.LogstashUrl,
                    restrictedToMinimumLevel: LogEventLevel.Information
                );

            return loggerConfiguration;
        }

        private static LoggerConfiguration Filter(LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration.Filter.ByExcluding(logEvent => logEvent.IsSuppressed());
        }

        //private static readonly ConnectionSettings elasticConnSettings =
        //    new ConnectionSettings(new Uri(Constants.ELASTIC_SEARCH_URL))
        //        .DefaultIndex(Constants.ELASTIC_SEARCH_DEFAULT_INDEX);

        //private static readonly ElasticClient elasticClient = new ElasticClient(elasticConnSettings);

        //public static IIndexResponse LogToElastic<T>(T log) where T : LogBase
        //{
        //    if (!elasticClient.IndexExists(Constants.ELASTIC_SEARCH_CUSTOM_ALIAS).Exists)
        //    {
        //        var indexSettings = new IndexSettings();
        //        indexSettings.NumberOfReplicas = Constants.ELASTIC_SEARCH_NUMBER_OF_REPLICAS;
        //        indexSettings.NumberOfShards = Constants.ELASTIC_SEARCH_NUMBER_OF_REPLICAS;

        //        var createIndexDescriptor = new CreateIndexDescriptor(Constants.ELASTIC_SEARCH_DEFAULT_INDEX)
        //            .Mappings(ms => ms
        //                .Map<T>(m => m.AutoMap())
        //            )
        //            .InitializeUsing(new IndexState() { Settings = indexSettings })
        //            .Aliases(a => a.Alias(Constants.ELASTIC_SEARCH_CUSTOM_ALIAS));

        //        elasticClient.CreateIndex(createIndexDescriptor);
        //    }

        //    return elasticClient.Index(log, idx => idx.Index(Constants.ELASTIC_SEARCH_DEFAULT_INDEX));
        //}
    }
}