using CoreData.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CoreSvc.Common
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder) => hostBuilder.UseSerilog();

        public static IHostBuilder ConfigureWebHost<TStartup>(this IHostBuilder builder) where TStartup : class
        {
            return builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TStartup>();
                webBuilder.UseUrls(ConfigurationManager.ApplicationUrl);
            });
        }
    }
}