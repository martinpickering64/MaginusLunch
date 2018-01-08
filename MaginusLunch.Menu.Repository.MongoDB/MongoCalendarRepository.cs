using MaginusLunch.MongoDB;
using MaginusLunch.Menu.Domain;
using Polly.Retry;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Repository
{
    public class MongoCalendarRepository : MongoRepository<Calendar>, ICalendarRepository
    {
        public MongoCalendarRepository(MongoRepositorySettings settings,
                                    RetryPolicy asyncRetryPolicy,
                                    RetryPolicy retryPolicy)
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Task<Calendar> GetCalendarAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync(id, cancellationToken);
        }

        public async Task<Calendar> GetCalendarAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var x = await FindAllAsync(0, 1, cancellationToken).ConfigureAwait(false);
            return x.FirstOrDefault();
        }

        public Task SaveCalendarAsync(Calendar calendar, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (calendar.Version == 0)
            {
                return InsertAsync(calendar, cancellationToken);
            }
            else
            {
                return ReplaceAsync(calendar, cancellationToken);
            }
        }
    }
}
