using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CoreData.DbContextsEx
{
    public partial class genesisContextEx_Oracle : genesisContext_Oracle
    {
        public genesisContextEx_Oracle()
        {
        }

        protected genesisContextEx_Oracle(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContextEx_Oracle(DbContextOptions<genesisContextEx_Oracle> options)
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
                optionsBuilder.UseOracle(ConfigurationManager.GetConnectionString("GenesisDB"));
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