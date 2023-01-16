using CoreData.Common;
using CoreData.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoreData.DBContexts
{
    public partial class genesisContext_MSSQL : GenesisContextBase
    {
        public genesisContext_MSSQL()
        {
        }

        protected genesisContext_MSSQL(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContext_MSSQL(DbContextOptions<genesisContext_MSSQL> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(ConfigurationManager.GetConnectionString("GenesisDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}