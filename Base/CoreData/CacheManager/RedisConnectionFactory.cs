using System;
using System.Collections.Generic;
using CoreData.Infrastructure;
using StackExchange.Redis;

namespace CoreData.CacheManager
{
    public class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnectionFactory()
        {
            var connectionString = ConfigurationManager.RedisSettings.Url;

            if (connectionString == null)
            {
                throw new KeyNotFoundException($"Environment variable for 'REDIS' was not found.");
            }

            var options = ConfigurationOptions.Parse(connectionString);

            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;

        public static IDatabase GetDatabase() => GetConnection().GetDatabase(ConfigurationManager.RedisSettings.DatabaseIndex);
    }
}