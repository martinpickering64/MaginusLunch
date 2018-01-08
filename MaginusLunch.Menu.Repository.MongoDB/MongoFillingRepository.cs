using MaginusLunch.MongoDB;
using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using Polly.Retry;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Menu.Repository
{
    public class MongoFillingRepository : MongoRepository<Filling>, IFillingRepository
    {
        public MongoFillingRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Task<Filling> GetFillingAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public Task<IEnumerable<Filling>> GetAllFillingsAsync(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken)) 
            => FindAsync(x => x.LunchProviderId == lunchProviderId, cancellationToken);

        public Task SaveFillingAsync(Filling filling, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (filling.Version == 0)
            {
                return InsertAsync(filling, cancellationToken);
            }
            else
            {
                return ReplaceAsync(filling, cancellationToken);
            }
        }
    }
}
