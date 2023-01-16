using System;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreData.DBContextFactories
{
    // Design time class to prevent timeout on database creation. Do not remove.
    public class genesisContextFactory_MySQL : IDesignTimeDbContextFactory<genesisContextEx_MySQL>
    {
        public genesisContextEx_MySQL CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<genesisContextEx_MySQL>();
            var connectionString = ConfigurationManager.GetConnectionString("GenesisDB");

            optionsBuilder.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString),
                b => b.CommandTimeout((int) TimeSpan.FromMinutes(2).TotalSeconds));

            return new genesisContextEx_MySQL(optionsBuilder.Options);
        }
    }
}