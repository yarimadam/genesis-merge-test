using System;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreData.DBContextFactories
{
    // Design time class to prevent timeout on database creation. Do not remove.
    public class genesisContextFactory_PostgreSQL : IDesignTimeDbContextFactory<genesisContextEx_PostgreSQL>
    {
        public genesisContextEx_PostgreSQL CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<genesisContextEx_PostgreSQL>();

            optionsBuilder.UseNpgsql(ConfigurationManager.GetConnectionString("GenesisDB"),
                b =>
                {
                    b.SetPostgresVersion(9, 6);
                    b.CommandTimeout((int) TimeSpan.FromMinutes(2).TotalSeconds);
                });

            return new genesisContextEx_PostgreSQL(optionsBuilder.Options);
        }
    }
}