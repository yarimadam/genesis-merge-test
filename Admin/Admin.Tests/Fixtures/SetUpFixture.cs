using Admin.Svc;
using NUnit.Framework;

namespace Admin.Tests.Fixtures
{
    [SetUpFixture]
    public class SetUpFixture : CoreTests.Fixtures.SetUpFixture<Startup>
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