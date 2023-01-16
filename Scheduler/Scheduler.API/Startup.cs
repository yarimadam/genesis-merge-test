using System.Linq;
using CoreData.Infrastructure;
using CoreSvc;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Scheduler.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCoreConfigurationServices(Configuration, Environment);

            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = "smart";
                    sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddPolicyScheme("smart", "Authorization Bearer or OIDC", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        var authHeader = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

                        // ?? context.Request.Query[Constants.ACCESS_TOKEN_NAME].Any())
                        return authHeader?.StartsWith("Bearer ") == true
                            ? JwtBearerDefaults.AuthenticationScheme
                            : OpenIdConnectDefaults.AuthenticationScheme;
                    };
                })
                .AddCookie("OIDCCookie")
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = "OIDCCookie";

                    options.Authority = ConfigurationManager.IdentityUrl;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "scheduler";
                    options.ClientSecret = ConfigurationManager.GetIdentityServerSharedSecret(options.ClientId);
                    options.ResponseType = "code";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.AddCoreConfigurations(Configuration, Environment);

            app.UseHangfireDashboard("", new DashboardOptions
            {
                AppPath = null,
                DashboardTitle = Constants.SCHEDULER_DASHBOARD_NAME,
                Authorization = new[] { new HangfireAuthorizeFilter() },
                DisplayStorageConnectionString = false
            });
        }
    }
}