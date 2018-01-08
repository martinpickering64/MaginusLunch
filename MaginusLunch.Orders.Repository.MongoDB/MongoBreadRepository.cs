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
    public class MongoBreadRepository : MongoRepository<Bread>, IBreadRepository
    {
        public MongoBreadRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Bread GetBread(Guid id) => Get(id);

        public Task<Bread> GetBreadAsync(Guid id, 
            CancellationToken cancellationToken = default(CancellationToken)) => GetAsync(id, cancellationToken);

        public IEnumerable<Bread> GetAllBreads(DateTime activeOnThisDate) => Find(x => x.Status == BreadStatus.Available)
                            .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<Bread>> GetAllBreadsAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == BreadStatus.Available, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }

        public IEnumerable<Bread> GetAllBreads(Guid lunchProviderId, DateTime activeOnThisDate) 
            => Find(x => x.Status == BreadStatus.Available
                         && x.LunchProviderId == lunchProviderId)
                .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<Bread>> GetAllBreadsAsync(Guid lunchProviderId, DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == BreadStatus.Available
                            && x.LunchProviderId == lunchProviderId, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }

        public void SaveBread(Bread Bread)
        {
            if (Bread.Version == 0)
            {
                Insert(Bread);
            }
            else
            {
                Replace(Bread);
            }
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
