using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Microservice.DataLib.DBContexts
{
    public partial class user_appContextEx : user_appContext
    {
        public user_appContextEx()
        {
        }

        public user_appContextEx(DbContextOptions<user_appContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder);
                optionsBuilder.UseLoggerFactory(LogFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}