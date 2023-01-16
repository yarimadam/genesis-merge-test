using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Bogus;
using CoreData.Common;
using CoreData.Infrastructure;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

namespace CoreTests.Fixtures
{
    [SetUpFixture]
    public abstract class BasicSetUpFixture
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);

            ConfigureLogging();

            Randomizer.Seed = new Random(Constants.RANDOMIZER_SEED);
        }

        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(Debugger.IsAttached ? LogEventLevel.Information : LogEventLevel.Error)
                .ReadFrom.Configuration(ConfigurationManager.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.NUnitOutput()
                .Filter.ByExcluding(logEvent => logEvent.IsSuppressed())
                .CreateLogger();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            Log.CloseAndFlush();
        }
    }
}