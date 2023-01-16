using System;
using System.Net.Http;
using Bogus;
using CoreData.Common;
using CoreTests.Fixtures.Controllers;
using CoreType.DBModels;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace CoreTests
{
    public static class TestContext
    {
        public static readonly string AssemblyCountInSutRedisKey = $"{Constants.TEST_CACHE_KEY_PREFIX}AssemblyCountInSUT";
        public static readonly string AssemblyCountInSutRedisLockKey = $"{AssemblyCountInSutRedisKey}_Lock";

        public static readonly Tenant ActiveTenant = TenantControllerTest.EntityFaker
            .UseSeed(1)
            .Generate();

        public static readonly CoreUsers ActiveUser = CoreUserControllerTest.EntityFaker
            .UseSeed(1)
            .Generate();

        public static readonly Faker Faker = new Faker();

        #region DB

        public static GenesisContextBase GenesisContext;
        public static DbContext CustomContext;

        #endregion

        #region WebApp

        public static TestServer Server;
        private static HttpClient _client;
        public static HttpClient Client => _client ??= Server.CreateClient();
        public static IServiceProvider ServiceProvider;

        public static string Token;

        #endregion
    }
}