using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using FluentAssertions;
using NUnit.Framework;

namespace CoreTests.Fixtures.Infrastructure
{
    [Category("Infrastructure")]
    [Category("DistributedCache")]
    public class DistributedCacheFixture
    {
        private const int LENGTH = 80;
        private const int SET_COUNT = Constants.DEFAULT_ENTITY_GENERATION_COUNT * 10;

        public static readonly List<KeyValuePair<string, string>> KeyValueSet = TestContext.Faker
            .Make(SET_COUNT, () => new KeyValuePair<string, string>(
                TestContext.Faker.Internet.Password(LENGTH, prefix: Constants.TEST_CACHE_KEY_PREFIX),
                TestContext.Faker.Internet.Password(LENGTH)
            ))
            .ToList();

        [Test, Order(1)]
        [TestCaseSource(nameof(KeyValueSet))]
        public async Task Set_ValidPair_ReturnsTrue(KeyValuePair<string, string> pair)
        {
            // Arrange
            var (key, value) = pair;

            // Act
            var result = await DistributedCache.SetAsync(key, value);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(KeyValueSet))]
        public async Task Get_ValidPair_ReturnsSameValue(KeyValuePair<string, string> pair)
        {
            // Arrange
            var (key, value) = pair;

            // Act
            var result = await DistributedCache.GetAsync(key);

            // Assert
            result.Should().Be(value);
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(KeyValueSet))]
        public async Task Delete_ValidPair_ReturnsTrue(KeyValuePair<string, string> pair)
        {
            // Arrange
            var (key, _) = pair;

            // Act
            var result = await DistributedCache.DeleteAsync(key);

            // Assert
            result.Should().BeTrue();
        }
    }
}