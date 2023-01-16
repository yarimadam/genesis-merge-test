using System;
using System.Data;
using CoreData.Infrastructure;
using CoreType.Types;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace CoreData.Common
{
    public static class ConnectionManager
    {
        public static IDbConnection GetConnection(DatabasePreference databasePreference = DatabasePreference.Default)
        {
            //When connection used once,object will be disposed.Recreate is needed.

            var connectionString = ConfigurationManager.GetConnectionString(databasePreference);

            switch (databasePreference)
            {
                case DatabasePreference.PostgreSQL:
                    return new NpgsqlConnection(connectionString);
                case DatabasePreference.MSSQL:
                    return new SqlConnection(connectionString);
                case DatabasePreference.MySQL:
                    return new MySqlConnection(connectionString);
                case DatabasePreference.Oracle:
                    // We need to switch to the GENESIS Schema as Oracle ManagedDataAccess client does not automatically use the specified User Id as the default Schema
                    // It's observed it uses default SYS/SYSTEM schema when logged in as SYSDBA Role 
                    var builder = new OracleConnectionStringBuilder(connectionString);
                    var cn = new OracleConnection(connectionString);
                    if (!String.IsNullOrEmpty(builder.UserID))
                        CurrentOracleSchema = builder.UserID;
                    //cn.Execute("ALTER SESSION SET current_schema=" + builder.UserID);

                    return cn;
                default:
                    throw new ArgumentException();
            }
        }

        public static IDbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            //When connection used once,object will be disposed.Recreate is needed.

            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    return new SqlConnection(connectionString);
                case DatabaseType.PostgreSQL:
                    return new NpgsqlConnection(connectionString);
                case DatabaseType.MySQL:
                    return new MySqlConnection(connectionString);
                case DatabaseType.Oracle:
                    // We need to switch to the GENESIS Schema as Oracle ManagedDataAccess client does not automatically use the specified User Id as the default Schema
                    // It's observed it uses default SYS/SYSTEM schema when logged in as SYSDBA Role 
                    var builder = new OracleConnectionStringBuilder(connectionString);
                    var cn = new OracleConnection(connectionString);
                    if (!String.IsNullOrEmpty(builder.UserID))
                        CurrentOracleSchema = builder.UserID;
                    //cn.Execute("ALTER SESSION SET current_schema=" + builder.UserID);

                    return cn;
                default:
                    throw new ArgumentException();
            }
        }

        public static string CurrentOracleSchema { get; private set; }
    }
}