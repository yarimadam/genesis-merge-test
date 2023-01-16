using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CoreData.DbContextsEx
{
    public partial class genesisContextEx_MSSQL : genesisContext_MSSQL
    {
        public genesisContextEx_MSSQL()
        {
        }

        protected genesisContextEx_MSSQL(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContextEx_MSSQL(DbContextOptions<genesisContextEx_MSSQL> options)
            : base(options)
        {
        }

        public virtual DbSet<SampleEmployee> SampleEmployee { get; set; }
        public virtual DbSet<SampleEmployeeTask> SampleEmployeeTask { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder);
                optionsBuilder.UseSqlServer(ConfigurationManager.GetConnectionString("GenesisDB"));
                optionsBuilder.UseLoggerFactory(LogFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Live Preview Models

            modelBuilder.ApplyConfiguration<SampleEmployee>(SampleEmployeeConfiguration);
            modelBuilder.ApplyConfiguration<SampleEmployeeTask>(SampleEmployeeTaskConfiguration);

            #endregion
        }
    }
}