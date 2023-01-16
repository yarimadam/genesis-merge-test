using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Oracle.ManagedDataAccess.Client;

namespace CoreData.DBContexts
{
    public partial class genesisContext_Oracle : GenesisContextBase
    {
        public genesisContext_Oracle()
        {
        }

        protected genesisContext_Oracle(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContext_Oracle(DbContextOptions<genesisContext_Oracle> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseOracle(ConfigurationManager.GetConnectionString("GenesisDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var builder = new OracleConnectionStringBuilder(ConfigurationManager.GetConnectionString("GenesisDB"));
            if (!string.IsNullOrEmpty(builder.UserID))
                modelBuilder.HasDefaultSchema(builder.UserID);
        }

        protected override void CoreParametersConfiguration(EntityTypeBuilder<CoreParameters> builder)
        {
            base.CoreParametersConfiguration(builder);

            builder.Property(e => e.Translations)
                .HasColumnType("clob");
        }

        protected override void NotificationSettingsConfiguration(EntityTypeBuilder<NotificationSettings> builder)
        {
            base.NotificationSettingsConfiguration(builder);

            builder.Property(e => e.Data)
                .HasColumnType("clob");
        }

        protected override void TransactionLogsConfiguration(EntityTypeBuilder<TransactionLogs> builder)
        {
            base.TransactionLogsConfiguration(builder);

            builder.Property(e => e.CurrentClaims)
                .HasColumnType("clob");

            builder.Property(e => e.Request)
                .HasColumnType("clob");

            builder.Property(e => e.Response)
                .HasColumnType("clob");
        }
    }
}