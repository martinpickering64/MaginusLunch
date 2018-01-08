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
    public class MongoMenuOptionRepository : MongoRepository<MenuOption>, IMenuOptionRepository
    {
        public MongoMenuOptionRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public MenuOption GetMenuOption(Guid id) => Get(id);

        public Task<MenuOption> GetMenuOptionAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public IEnumerable<MenuOption> GetAllMenuOptions(DateTime activeOnThisDate) => Find(x => x.Status == MenuOptionStatus.Available)
                .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<MenuOption>> GetAllMenuOptionsAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == MenuOptionStatus.Available, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }

        public IEnumerable<MenuOption> GetAllMenuOptions(Guid lunchProviderId, DateTime activeOnThisDate) 
            => Find(x => x.Status == MenuOptionStatus.Available
                         && x.LunchProviderId == lunchProviderId)
                   .Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));

        public async Task<IEnumerable<MenuOption>> GetAllMenuOptionsAsync(Guid lunchProviderId, DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await FindAsync(x => x.Status == MenuOptionStatus.Available
                            && x.LunchProviderId == lunchProviderId, cancellationToken).ConfigureAwait(false);
            return f.Where(x => !x.WithdrawnDates.Contains(activeOnThisDate));
        }

        public void SaveMenuOption(MenuOption MenuOption)
        {
            if (MenuOption.Version == 0)
            {
                Insert(MenuOption);
            }
            else
            {
                Replace(MenuOption);
            }
        }

        public Task SaveMenuOptionAsync(MenuOption MenuOption, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (MenuOption.Version == 0)
            {
                return InsertAsync(MenuOption, cancellationToken);
            }
            else
            {
                return ReplaceAsync(MenuOption, cancellationToken);
            }
        }
    }
}
