using NUnit.Framework;
using CoreData.DBContexts;
using CoreSvc;

namespace CoreTests.Fixtures
{
    [SetUpFixture]
    public class SetUpFixture : CoreTests.Fixtures.SetUpFixture<Startup, genesisContext_PostgreSQL>
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