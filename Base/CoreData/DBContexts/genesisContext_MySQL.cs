using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreData.DBContexts
{
    public partial class genesisContext_MySQL : GenesisContextBase
    {
        public genesisContext_MySQL()
        {
        }

        protected genesisContext_MySQL(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContext_MySQL(DbContextOptions<genesisContext_MySQL> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseMySql(ConfigurationManager.GetConnectionString("GenesisDB"),
                ServerVersion.AutoDetect(ConfigurationManager.GetConnectionString("GenesisDB")));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void CoreParametersConfiguration(EntityTypeBuilder<CoreParameters> builder)
        {
            base.CoreParametersConfiguration(builder);

            builder.Property(e => e.Translations)
                .HasColumnType("json");
        }

        protected override void NotificationSettingsConfiguration(EntityTypeBuilder<NotificationSettings> builder)
        {
            base.NotificationSettingsConfiguration(builder);

            builder.Property(e => e.Data)
                .HasColumnType("json");
        }

        protected override void TransactionLogsConfiguration(EntityTypeBuilder<TransactionLogs> builder)
        {
            base.TransactionLogsConfiguration(builder);

            builder.Property(e => e.CurrentClaims)
                .HasColumnType("json");

            builder.Property(e => e.Request)
                .HasColumnType("json");

            builder.Property(e => e.Response)
                .HasColumnType("json");
        }
    }
}