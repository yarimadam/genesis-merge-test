using System;
using CoreData.Common;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreData.DBContextFactories
{
    // Design time class to prevent timeout on database creation. Do not remove.
    public class genesisContextFactory_MSSQL : IDesignTimeDbContextFactory<genesisContextEx_MSSQL>
    {
        public genesisContextEx_MSSQL CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<genesisContextEx_MSSQL>();

            optionsBuilder.UseSqlServer(ConfigurationManager.GetConnectionString("GenesisDB"),
                b => b.CommandTimeout((int) TimeSpan.FromMinutes(2).TotalSeconds));

            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomSqlServerMigrationsSqlGenerator>();

            return new genesisContextEx_MSSQL(optionsBuilder.Options);
        }
    }
}