using MaginusLunch.MongoDB;
using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using Polly.Retry;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Menu.Repository
{
    public class MongoLunchProviderRepository : MongoRepository<LunchProvider>, ILunchProviderRepository
    {
        public MongoLunchProviderRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Task<LunchProvider> GetLunchProviderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public Task<IEnumerable<LunchProvider>> GetAllLunchProvidersAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => FindAsync(x => x.Status == LunchProviderStatus.Available, cancellationToken);

        public Task SaveLunchProviderAsync(LunchProvider LunchProvider, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (LunchProvider.Version == 0)
            {
                return InsertAsync(LunchProvider, cancellationToken);
            }
            else
            {
                return ReplaceAsync(LunchProvider, cancellationToken);
            }
        }
    }
}
