using System;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreData.DBContextFactories
{
    // Design time class to prevent timeout on database creation. Do not remove.
    public class genesisContextFactory_Oracle : IDesignTimeDbContextFactory<genesisContextEx_Oracle>
    {
        public genesisContextEx_Oracle CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<genesisContextEx_Oracle>();

            optionsBuilder.UseOracle(ConfigurationManager.GetConnectionString("GenesisDB"),
                b => b.CommandTimeout((int) TimeSpan.FromMinutes(2).TotalSeconds));

            return new genesisContextEx_Oracle(optionsBuilder.Options);
        }
    }
}