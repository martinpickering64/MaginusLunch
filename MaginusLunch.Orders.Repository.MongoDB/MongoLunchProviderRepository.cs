using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using Polly.Retry;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace MaginusLunch.Orders.Repository
{
    public class MongoLunchProviderRepository : MongoRepository<LunchProvider>, ILunchProviderRepository
    {
        public MongoLunchProviderRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public LunchProvider GetLunchProvider(Guid id) => Get(id);

        public Task<LunchProvider> GetLunchProviderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public IEnumerable<LunchProvider> GetAllLunchProviders() => Find(x => x.Status == LunchProviderStatus.Available);

        public Task<IEnumerable<LunchProvider>> GetAllLunchProvidersAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => FindAsync(x => x.Status == LunchProviderStatus.Available, cancellationToken);

        public IEnumerable<LunchProvider> GetAllLunchProviders(DateTime activeOnThisDate) => Find(x => x.Status == LunchProviderStatus.Available)
                            .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<LunchProvider>> GetAllLunchProvidersAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == LunchProviderStatus.Available, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }

        public void SaveLunchProvider(LunchProvider LunchProvider)
        {
            if (LunchProvider.Version == 0)
            {
                Insert(LunchProvider);
            }
            else
            {
                Replace(LunchProvider);
            }
        }

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
