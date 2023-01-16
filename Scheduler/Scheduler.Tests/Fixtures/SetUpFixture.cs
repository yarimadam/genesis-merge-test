using NUnit.Framework;
using Scheduler.API;

namespace Scheduler.Tests.Fixtures
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