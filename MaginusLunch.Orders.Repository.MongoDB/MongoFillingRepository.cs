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
    public class MongoFillingRepository : MongoRepository<Filling>, IFillingRepository
    {
        public MongoFillingRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Filling GetFilling(Guid id) => Get(id);

        public Task<Filling> GetFillingAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public IEnumerable<Filling> GetAllFillings(DateTime activeOnThisDate) => Find(x => x.Status == FillingStatus.Available)
                .Where(x => !(x.WithdrawnDates.Contains(activeOnThisDate.ToUniversalTime())));

        public async Task<IEnumerable<Filling>> GetAllFillingsAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == FillingStatus.Available, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate.ToUniversalTime()));
        }

        public IEnumerable<Filling> GetAllFillings(DateTime activeOnThisDate, Guid menuCategoryId) 
            => Find(x => x.Status == FillingStatus.Available && x.MenuCategories.Contains(menuCategoryId))
                .Where(x => !(x.WithdrawnDates.Contains(activeOnThisDate.ToUniversalTime())));

        public async Task<IEnumerable<Filling>> GetAllFillingsAsync(DateTime activeOnThisDate, Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == FillingStatus.Available
                                && x.MenuCategories.Contains(menuCategoryId), cancellationToken)
                    .ConfigureAwait(false);
            return f.Where(x => !(x.WithdrawnDates.Contains(activeOnThisDate.ToUniversalTime())));
        }

        public IEnumerable<Filling> GetAllFillings(Guid lunchProviderId, DateTime activeOnThisDate) 
            => Find(x => x.Status == FillingStatus.Available
                        && x.LunchProviderId == lunchProviderId)
                    .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<Filling>> GetAllFillingsAsync(Guid lunchProviderId, DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == FillingStatus.Available
                            && x.LunchProviderId == lunchProviderId, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }


        public IEnumerable<Filling> GetAllFillings(Guid lunchProviderId, DateTime activeOnThisDate, Guid menuCategoryId) 
            => Find(x => x.Status == FillingStatus.Available
                        && x.LunchProviderId == lunchProviderId
                        && x.MenuCategories.Contains(menuCategoryId))
                .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<Filling>> GetAllFillingsAsync(Guid lunchProviderId, DateTime activeOnThisDate, Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == FillingStatus.Available
                            && x.LunchProviderId == lunchProviderId
                            && x.MenuCategories.Contains(menuCategoryId), cancellationToken)
                    .ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }

        public void SaveFilling(Filling filling)
        {
            if (filling.Version == 0)
            {
                Insert(filling);
            }
            else
            {
                Replace(filling);
            }
        }

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
