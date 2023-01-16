using NUnit.Framework;
using Microservice.DataLib.DBContexts;
using Microservice.API;

namespace Microservice.Tests.Fixtures
{
    [SetUpFixture]
    public class SetUpFixture : CoreTests.Fixtures.SetUpFixture<Startup, user_appContext>
    {
        [OneTimeSetUp]
        public new void Setup()
        {
        }

        [OneTimeTearDown]
        public new void TearDown()
        {
        }
    }
}