using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CoreData.DbContextsEx
{
    public partial class genesisContextEx_PostgreSQL : genesisContext_PostgreSQL
    {
        public genesisContextEx_PostgreSQL()
        {
        }

        protected genesisContextEx_PostgreSQL(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContextEx_PostgreSQL(DbContextOptions<genesisContextEx_PostgreSQL> options)
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
                optionsBuilder.UseNpgsql(ConfigurationManager.GetConnectionString("GenesisDB"),
                    b => b.SetPostgresVersion(9, 6));
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