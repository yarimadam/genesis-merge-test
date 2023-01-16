namespace CoreTests
{
    public abstract class Constants : CoreSvc.Constants
    {
        // Default cache prefix for all keys.
        public const string TEST_CACHE_KEY_PREFIX = "{SUT}:";

        // Default entity count for testing. 
        public const int DEFAULT_ENTITY_GENERATION_COUNT = 1;

        // Set a constant randomizer seed if you wish to generate repeatable data sets.
        public static readonly int RANDOMIZER_SEED = TestContext.Faker.Random.Int();
    }
}