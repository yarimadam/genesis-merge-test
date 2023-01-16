using System;
using System.Text.RegularExpressions;

namespace CoreData
{
    public abstract class Constants : CoreType.Constants
    {
        public const string SIGNATURE_NAME = "Net Core Genesis";
        public const string SIGNATURE_EMAIL = "info@netcoregenesis.com";
        public const ushort CONCURRENT_BATCH_SIZE = 50;
        public const string SESSION_KEY_CLAIMS = "{claims}:";
        public const string SESSION_KEY_CONTEXT = "{context}:";
        public const string PARAMETER_KEY_PREFIX = "{parameters}:";
        public const string DEFAULT_LANGUAGE = "EN";
        public const string MSSQL_DATABASE_COLLATION_NAME = "Turkish_CI_AS";
        public const string SCHEDULER_DASHBOARD_NAME = "Scheduler";
        public const string SCHEDULER_DASHBOARD_TITLE = "Scheduler Dashboard";
        public const string CLIENT_RECEIVER_METHOD_NAME = "ReceiveMessage";
        public const string DEFAULT_IDENTITY_CLIENT_ID = "short.client";
        public const string ACCESS_TOKEN_NAME = "access_token";

        public static readonly Regex PACKAGE_LIST_PROJECTS = new Regex($@"'([\S]+?)'[\S\s]+?\[([\w\.]+)\]:\s+([\S\s]+?)(?:{Environment.NewLine}){{2,}}");
        public static readonly Regex PACKAGE_LIST_VERSIONS = new Regex(@"(?:\s+>\s+([\S]+)\s+(\(A\))?\s*([\S]+?(?:,\s*\))?)\s+([\S]+)+?)");
        public static readonly Regex NEW_LINE_REGEX = new Regex(@"\r\n?|\n");

        public static readonly TimeSpan RESET_PASSWORD_EXPIRATION = TimeSpan.FromHours(1);
        public static readonly TimeSpan USER_VERIFICATION_EXPIRATION = TimeSpan.FromHours(6);

        public static bool IsEFToolRuntime => Environment.GetEnvironmentVariable("EF_TOOL_RUNTIME") == true.ToString();

        #region Logging

        public const string LOG_FILE_PATH_PATTERN = "Logs/log-.txt";

        //public static readonly string ELASTIC_SEARCH_URL = "http://localhost:9200/";
        //public static readonly string ELASTIC_SEARCH_DEFAULT_INDEX = "log_history";
        //public static readonly string ELASTIC_SEARCH_CUSTOM_ALIAS = "custom_log";
        //public static readonly int ELASTIC_SEARCH_NUMBER_OF_SHARDS = 3;
        //public static readonly int ELASTIC_SEARCH_NUMBER_OF_REPLICAS = 1;

        #endregion

        #region Caching

        public static readonly TimeSpan CACHE_ABSOLUTE_EXPIRATION = TimeSpan.FromHours(24);
        public static readonly TimeSpan CACHE_DELEGATE_EXPIRATION = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan CACHE_LOCK_EXPIRATION = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan CACHE_LOCK_TIMEOUT = TimeSpan.FromSeconds(30);

        #endregion
    }
}