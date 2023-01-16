using System.Linq;
using CoreData.Common;
using CoreSvc.Common;
using CoreType.Models;
using CoreType.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CoreTests.Infrastructure.WebHosting
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public CustomWebApplicationFactory()
        {
            TestContext.Server = Server;
            TestContext.ServiceProvider = Server.Services.CreateScope().ServiceProvider;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseContentRoot(".")
                .ConfigureServices(ConfigureServices);

            base.ConfigureWebHost(builder);
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            // new WebHostBuilder().ConfigureServices(service => new Startup().ConfigureServices(service));

            return Host.CreateDefaultBuilder()
                .ConfigureLogging()
                .ConfigureWebHost<TStartup>();
        }

        // protected override void ConfigureWebHost(IWebHostBuilder builder)
        // {
        //     builder.ConfigureServices(ConfigureServices);
        // }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                // .RemoveAll<SessionAccessor>();
                .RemoveAll<SessionContext>();

            services
                // .AddScoped<SessionAccessor>()
                .AddScoped(serviceProvider => new SessionContext
                {
                    CurrentUser = new LoggedInUser
                    {
                        UserId = TestContext.ActiveUser.UserId,
                        Name = TestContext.ActiveUser.Name,
                        Surname = TestContext.ActiveUser.Surname,
                        Email = TestContext.ActiveUser.Email,
                        TenantId = TestContext.ActiveUser.TenantId,
                        TenantType = (int) TenantType.SystemOwner
                    }
                });

            ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());
        }
    }
}