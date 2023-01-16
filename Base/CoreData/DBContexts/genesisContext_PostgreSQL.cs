using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreData.DBContexts
{
    public partial class genesisContext_PostgreSQL : GenesisContextBase
    {
        public genesisContext_PostgreSQL()
        {
        }

        protected genesisContext_PostgreSQL(DbContextOptions options)
            : base(options)
        {
        }

        public genesisContext_PostgreSQL(DbContextOptions<genesisContext_PostgreSQL> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(ConfigurationManager.GetConnectionString("GenesisDB"),
                b => b.SetPostgresVersion(9, 6));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void CommunicationTemplatesConfiguration(EntityTypeBuilder<CommunicationTemplates> builder)
        {
            base.CommunicationTemplatesConfiguration(builder);

            builder.Property(e => e.EmailBody)
                .HasColumnType("character varying");

            builder.Property(e => e.SmsBody)
                .HasColumnType("character varying");
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