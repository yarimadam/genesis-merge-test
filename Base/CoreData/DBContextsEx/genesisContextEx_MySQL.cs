using CoreData.Common;
using CoreData.DBContexts;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CoreData.DbContextsEx
{
    public partial class genesisContextEx_MySQL : genesisContext_MySQL
    {
        public genesisContextEx_MySQL()
        {
        }

        protected genesisContextEx_MySQL(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContextEx_MySQL(DbContextOptions<genesisContextEx_MySQL> options)
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
                // Pomelo provider throws error when UseMysql method called more than once.
                // optionsBuilder.UseMySql(ConfigurationManager.GetConnectionString("GenesisDB"));
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