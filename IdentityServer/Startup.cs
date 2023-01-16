using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CoreData.Common;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using CoreSvc.Filters;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // compression eklendi (https için- http'de güvenlik açığı var)
            services.Configure<GzipCompressionProviderOptions>(options =>
                options.Level = System.IO.Compression.CompressionLevel.Optimal);

            services.AddResponseCompression(options => { options.EnableForHttps = true; });

            services.AddEFSecondLevelCache(options => options.UseEasyCachingCoreProvider("EasyCacheRedisProvider").DisableLogging(!Environment.IsDevelopment()));

            services.AddEasyCaching(option =>
                option.UseRedis(config =>
                    config.DBConfig.Configuration = ConfigurationManager.RedisSettings.Url, "EasyCacheRedisProvider"));

            // Required due to some browsers cookie policy changes.
            services.ConfigureSameSiteCookies();

            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    corsBuilder => corsBuilder
                        .WithOrigins(ConfigurationManager.GetAsArray("AllowedCorsOrigins"))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services
                .AddControllersWithViews(options => { options.Filters.Add(typeof(GlobalLoggingFilter)); })
                .AddNewtonsoftJson(
                    options => options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var builder = services
                .AddIdentityServer(options =>
                {
                    // When validating token externally with "connect/userinfo" returns unauthorized without IssuerUri below even if ValidateIssuer is false.
                    // https://github.com/IdentityServer/IdentityServer4/issues/501
                    options.IssuerUri = ConfigurationManager.IdentityUrl;
                    options.Authentication.CookieLifetime = TimeSpan.FromHours(10);
                })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddInMemoryPersistedGrants();

            if (Environment.IsDevelopment())
                builder.AddDeveloperSigningCredential();
            else
            {
                var fileName = Path.Combine(Environment.WebRootPath, "idsvr.pfx");
                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Signing Certificate is missing!");

                var cert = new X509Certificate2(fileName, "g7k9Qb3Qht6eVrfc");
                builder.AddSigningCredential(cert);
            }

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = ConfigurationManager.IdentityUrl;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidIssuer = ConfigurationManager.IdentityUrl,
                        ValidateAudience = false,
                        ValidAudience = "genesisAPI",
                        ValidateLifetime = true
                    };
                });

            services.AddControllersWithViews();

            services.AddHttpContextAccessor();

            services.AddScoped<SessionAccessor>();
            services.AddScoped(serviceProvider => serviceProvider.GetService<SessionAccessor>().Session);

            services
                .AddDbContext<genesisContextEx_PostgreSQL>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient)
                .AddDbContext<genesisContextEx_MySQL>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient)
                .AddDbContext<genesisContextEx_MSSQL>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient)
                .AddDbContext<genesisContextEx_Oracle>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient);

            ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();

            // compression devreye al
            app.UseResponseCompression();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseCookiePolicy();

            // CORS global policy - assign here or on each controller
            app.UseCors();

            app.UseIdentityServer();
            // UseIdentityServer include a call to UseAuthentication()
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}