using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.DbContextsEx;
using CoreData.Infrastructure;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreSvc.Middlewares;
using EFCoreSecondLevelCacheInterceptor;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.HttpJob;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using Helper = CoreData.Common.Helper;

namespace CoreSvc
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.AddCoreConfigurations(Configuration, Environment);
        }
    }

    public static class ServiceCollectionExtensions
    {
        private static string CoreAssemblyName { get; } = typeof(Startup).Assembly.GetName().Name;
        private static string AssemblyName { get; } = Assembly.GetEntryAssembly()?.GetName().Name;

        public static IServiceCollection AddCoreConfigurationServices(this IServiceCollection services,
            IConfiguration Configuration, IWebHostEnvironment Environment, bool includeApplicationParts = false)
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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query[Constants.ACCESS_TOKEN_NAME];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            // Add framework services.
            services
                .AddControllersWithViews(options => { options.Filters.Add(typeof(GlobalLoggingFilter)); })
                .ConfigureApplicationPartManager(manager =>
                {
                    if (!includeApplicationParts)
                    {
                        var controllerFeatureProvider = manager.FeatureProviders
                            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerFeatureProvider>()
                            .FirstOrDefault();

                        if (controllerFeatureProvider != null)
                        {
                            manager.FeatureProviders.Remove(controllerFeatureProvider);
                            manager.FeatureProviders.Add(new ControllerFeatureProvider());
                        }
                    }
                })
                .AddNewtonsoftJson(
                    options =>
                    {
                        var settings = options.SerializerSettings;

                        settings.Error = JsonErrorLogger;
                        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                        // NodaTime support for workflow models
                        settings.DateParseHandling = DateParseHandling.None;
                    });

            services.AddSignalR(options => { options.EnableDetailedErrors = true; });
            // .AddJsonProtocol(options => options.PayloadSerializerSettings.ReferenceLoopHandling =
            //     Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddHangfire(configuration =>
            {
                configuration.UseRedisStorage(ConfigurationManager.RedisSettings.Url);
                configuration.UseSerilogLogProvider();
                configuration.UseConsole(); // Enables logging panel on scheduler dashboard
                configuration.UseHangfireHttpJob(new HangfireHttpJobOptions
                {
                    DashboardName = CoreData.Constants.SCHEDULER_DASHBOARD_NAME,
                    DashboardTitle = CoreData.Constants.SCHEDULER_DASHBOARD_TITLE,
                    MailOption = ConfigurationManager.GetSection("Hangfire:Mail").Get<MailOption>()
                });
            });

            services.AddHangfireConsoleExtensions(); // Redirects Serilog logs to Hangfire.Console 

            services.AddHttpContextAccessor();

            services.AddScoped<SessionAccessor>();
            services.AddScoped(serviceProvider => serviceProvider.GetService<SessionAccessor>().Session);
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddSingleton<HubContext>();
            services.AddTransient<UnitOfWork>();
            services.AddTransient(typeof(UnitOfWork<>));
            services.RegisterDerivedTypes<IGenericRepository>();
            services.RegisterDerivedTypes<IGenericService>();
            services.RegisterDerivedTypes<Controller>();

            services
                .AddDbContext<genesisContextEx_PostgreSQL>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient)
                .AddDbContext<genesisContextEx_MySQL>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient)
                .AddDbContext<genesisContextEx_MSSQL>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient)
                .AddDbContext<genesisContextEx_Oracle>(Helper.DefaultDbContextOptionsBuilder, ServiceLifetime.Transient);

            services
                .AddHttpClient("", c => { c.BaseAddress = new Uri(ConfigurationManager.ApplicationUrl); })
                .AddStandardHttpClientPolicies();

            services.AddSwaggerGen(options =>
            {
                // Use fully qualified schema names to prevent conflictions
                options.CustomSchemaIds(x => x.FullName);

                options.SwaggerDoc(AssemblyName, ConfigurationManager.GetSection("Swagger:Info").Get<OpenApiInfo>());

                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                    });

                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();

                var xmlCommentsPath = GetXmlCommentsPath();
                if (File.Exists(xmlCommentsPath))
                    options.IncludeXmlComments(xmlCommentsPath);

                if (AssemblyName != CoreAssemblyName)
                {
                    xmlCommentsPath = GetXmlCommentsPath(CoreAssemblyName);
                    if (File.Exists(xmlCommentsPath))
                        options.IncludeXmlComments(xmlCommentsPath);
                }
            });

            if (Environment.IsDevelopment())
                IdentityModelEventSource.ShowPII = true;

            ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());

            return services;
        }

        private static void JsonErrorLogger(object? sender, ErrorEventArgs args)
        {
            Log.Error(args.ErrorContext.Error, "Json conversion failed !");
        }

        public static IApplicationBuilder AddCoreConfigurations(this IApplicationBuilder app,
            IConfiguration Configuration, IWebHostEnvironment Environment)
        {
            app.UseSerilogRequestLogging();

            // compression devreye al
            app.UseResponseCompression();

            // CORS global policy - assign here or on each controller

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseWebSockets();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseFileServer();

            app.UseRouting();

            app.UseCookiePolicy();

            app.UseCors();

            app.UseMiddleware<EnableRequestBufferingMiddleware>();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CommunicationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalRHub>("/hub");
            });

            app.UseHangfireServer();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{AssemblyName}/swagger.json",
                    AssemblyName);
            });

            return app;
        }

        private static string GetXmlCommentsPath(string assemblyName = null)
        {
            assemblyName ??= AssemblyName;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                assemblyName + ".xml");
        }

        public static IServiceCollection RegisterDerivedTypes<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var type = typeof(T);

            // TODO Evaluate Original: Assembly.GetEntryAssembly()
            // Assembly.GetExecutingAssembly();
            var foundTypes = Assembly.GetCallingAssembly().DefinedTypes
                .Concat(Assembly.GetExecutingAssembly().DefinedTypes)
                .Where(x => !x.IsAbstract
                            && ((type.IsInterface && x.GetInterfaces().Contains(type))
                                || (!type.IsInterface && x.IsSubclassOf(type))))
                .GroupBy(x => x.AssemblyQualifiedName)
                .Select(x => x.First());

            foreach (var foundType in foundTypes)
                services.Add(new ServiceDescriptor(foundType, foundType, lifetime));

            return services;
        }
    }

    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddStandardHttpClientPolicies(this IHttpClientBuilder builder)
        {
            return builder
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddRetryPolicyHandler()
                .AddCircuitBreakerPolicyHandler();
        }

        public static IHttpClientBuilder AddRetryPolicyHandler(this IHttpClientBuilder builder)
        {
            return builder
                .AddPolicyHandler(
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                        .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        }

        public static IHttpClientBuilder AddCircuitBreakerPolicyHandler(this IHttpClientBuilder builder)
        {
            return builder
                .AddPolicyHandler(
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        }
    }
}