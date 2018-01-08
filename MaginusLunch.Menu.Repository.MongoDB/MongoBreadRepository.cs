using MaginusLunch.MongoDB;
using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using Polly.Retry;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Menu.Repository
{
    public class MongoBreadRepository : MongoRepository<Bread>, IBreadRepository
    {
        public MongoBreadRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Task<Bread> GetBreadAsync(Guid id, 
            CancellationToken cancellationToken = default(CancellationToken)) => GetAsync(id, cancellationToken);

        public Task<IEnumerable<Bread>> GetAllBreadsAsync(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindAsync(x => x.LunchProviderId == lunchProviderId, cancellationToken);
        }

        public Task SaveBreadAsync(Bread Bread, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Bread.Version == 0)
            {
                return InsertAsync(Bread, cancellationToken);
            }
            else
            {
                return ReplaceAsync(Bread, cancellationToken);
            }
        }
    }
}
