using System;
using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreSvc;
using CoreTests.Infrastructure.WebHosting;
using CoreType.DBModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Serilog;

namespace CoreTests.Fixtures
{
    internal class SetUpFixture : SetUpFixture<Startup>
    {
    }

    public abstract class SetUpFixture<TStartup, TContext> : SetUpFixture<TStartup>
        where TStartup : class
        where TContext : DbContext
    {
        private readonly Type _contextType = typeof(TContext);

        [OneTimeSetUp]
        public new async Task Setup()
        {
            TestContext.CustomContext = SetupDbContext(_contextType);
        }

        [OneTimeTearDown]
        public new async Task TearDown()
        {
            TearDownDbContext(TestContext.CustomContext, _contextType);
        }
    }

    public abstract class SetUpFixture<TStartup> : BasicSetUpFixture
        where TStartup : class
    {
        private const string KeepDatabaseAliveKey = "KeepDatabaseAlive";

        protected static bool KeepDatabaseAlive => ConfigurationManager.GetValue<bool>(KeepDatabaseAliveKey);
        protected static CustomWebApplicationFactory<TStartup> WebApplicationFactory;

        [OneTimeSetUp]
        public new async Task Setup()
        {
            await CacheShouldBeAvailable();
            await ConfigurationManagerShouldBeAvailable();

            Helper.RegisterDelegates();

            TestContext.GenesisContext = (GenesisContextBase) SetupDbContext();

            await SeedActiveTenant();
            await SeedActiveUser();

            WebApplicationFactory = new CustomWebApplicationFactory<TStartup>();

            // Renew context with newly created user's session
            TestContext.GenesisContext = Helper.GetGenesisContext();
        }

        [OneTimeTearDown]
        public new async Task TearDown()
        {
            TearDownDbContext(TestContext.GenesisContext);

            WebApplicationFactory?.Dispose();
        }

        public DbContext SetupDbContext(Type contextType = null)
        {
            var context = contextType == null
                ? Helper.GetGenesisContext()
                : Helper.GetDbContext(contextType);

            var lockToken = Guid.NewGuid().ToString();

            try
            {
                Log.Debug("SetupDbContext_PreLock");

                if (DistributedCache.LockTake(TestContext.AssemblyCountInSutRedisLockKey, lockToken))
                {
                    Log.Debug("SetupDbContext_IncrementRunningAssemblyCount_{contextType}", contextType?.FullName);

                    var runningAssemblyCount = IncrementRunningAssemblyCount(contextType?.FullName);

                    Log.Debug("SetupDbContext_RunningAssemblyCount_{runningAssemblyCount}", runningAssemblyCount);

                    if (runningAssemblyCount == 1)
                    {
                        Log.Debug("SetupDbContext_FirstRunningAssembly");

                        if (!KeepDatabaseAlive)
                        {
                            Log.Information("SetupDbContext_EnsureDeleted_Begin");
                            context.Database.EnsureDeleted();
                            Log.Information("SetupDbContext_EnsureDeleted_End");
                        }

                        Log.Information("SetupDbContext_EnsureCreated_Begin");
                        context.Database.EnsureCreated();
                        Log.Information("SetupDbContext_EnsureCreated_End");
                    }
                }
            }
            finally
            {
                DistributedCache.LockRelease(TestContext.AssemblyCountInSutRedisLockKey, lockToken);
                Log.Debug("SetupDbContext_PostLock");
            }

            return context;
        }

        public void TearDownDbContext(DbContext context, Type contextType = null)
        {
            var lockToken = Guid.NewGuid().ToString();

            try
            {
                Log.Debug("TearDownDbContext_PreLock");

                if (DistributedCache.LockTake(TestContext.AssemblyCountInSutRedisLockKey, lockToken))
                {
                    Log.Debug("TearDownDbContext_DecrementRunningAssemblyCount_{contextType}", contextType?.FullName);

                    var runningAssemblyCount = DecrementRunningAssemblyCount(contextType?.FullName);

                    Log.Debug("TearDownDbContext_RunningAssemblyCount_{runningAssemblyCount}", runningAssemblyCount);

                    if (runningAssemblyCount <= 0)
                    {
                        Log.Debug("TearDownDbContext_LastRunningAssembly");

                        if (!KeepDatabaseAlive)
                        {
                            Log.Information("TearDownDbContext_EnsureDeleted_Begin");
                            context?.Database.EnsureDeleted();
                            Log.Information("TearDownDbContext_EnsureDeleted_End");
                        }
                    }
                }
            }
            finally
            {
                DistributedCache.LockRelease(TestContext.AssemblyCountInSutRedisLockKey, lockToken);
                Log.Debug("TearDownDbContext_PostLock");
            }

            context?.Dispose();
        }

        private static async Task CacheShouldBeAvailable()
        {
            await DistributedCache.Database.PingAsync();
        }

        private static async Task ConfigurationManagerShouldBeAvailable()
        {
            ConfigurationManager.GetConnectionString().Should().NotBeNullOrEmpty();
        }

        private static async Task SeedActiveTenant()
        {
            var genesisContext = TestContext.GenesisContext;
            var activeTenant = genesisContext.Set<Tenant>()
                .IgnoreQueryFilters()
                .FirstOrDefault(tenant => tenant.TenantName == TestContext.ActiveTenant.TenantName);

            if (activeTenant == null)
            {
                genesisContext.Add(TestContext.ActiveTenant);

                var rowsAffected = await genesisContext.SaveChangesAsync();

                Assert.AreEqual(1, rowsAffected);

                TestContext.ActiveUser.TenantId = TestContext.ActiveTenant.TenantId;
            }
            else
                TestContext.ActiveUser.TenantId = activeTenant.TenantId;
        }

        private static async Task SeedActiveUser()
        {
            var genesisContext = TestContext.GenesisContext;
            var activeUser = genesisContext.Set<CoreUsers>()
                .IgnoreQueryFilters()
                .FirstOrDefault(user => user.Email == TestContext.ActiveUser.Email);

            if (activeUser == null)
            {
                activeUser = TestContext.ActiveUser.Map<CoreUsers>();

                activeUser.Password = Helper.GetHashedString(activeUser.Password);

                genesisContext.Add(activeUser);

                var rowsAffected = await genesisContext.SaveChangesAsync();

                Assert.AreEqual(1, rowsAffected);
            }

            TestContext.ActiveUser.UserId = activeUser.UserId;
        }

        protected static long IncrementRunningAssemblyCount(string suffix = "", int incrementBy = 1)
        {
            if (!string.IsNullOrEmpty(suffix))
                suffix = "_" + suffix;

            int newCount;
            var currentCount = DistributedCache.Get<int?>(TestContext.AssemblyCountInSutRedisKey + suffix);

            Log.Debug("RunningAssemblyCount_{suffix}_{currentCount}", suffix, currentCount);

            if (incrementBy < 0 && (currentCount ?? 0) + incrementBy <= 0)
            {
                newCount = 0;
                DistributedCache.Delete(TestContext.AssemblyCountInSutRedisKey + suffix);

                Log.Debug("RunningAssemblyCount_{suffix}_{currentCount}_Deleted", suffix, currentCount);
            }
            else
            {
                newCount = (currentCount ?? 0) + incrementBy;
                DistributedCache.Set(TestContext.AssemblyCountInSutRedisKey + suffix, newCount.ToString(), TimeSpan.FromMinutes(10));

                Log.Debug("RunningAssemblyCount_{suffix}_{currentCount}_{newCount}", suffix, currentCount, newCount);
            }

            return newCount;
        }

        protected static long DecrementRunningAssemblyCount(string suffix = "", int decrementBy = 1) => IncrementRunningAssemblyCount(suffix, decrementBy * -1);
    }
}