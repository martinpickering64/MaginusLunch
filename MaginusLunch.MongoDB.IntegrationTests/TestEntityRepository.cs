using Polly.Retry;

namespace MaginusLunch.MongoDB.IntegrationTests
{
    public class TestEntityRepository : MongoRepository<TestEntity>
    {
        public TestEntityRepository(MongoRepositorySettings settings,
                                    RetryPolicy asyncRetryPolicy,
                                    RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }
    }
}
