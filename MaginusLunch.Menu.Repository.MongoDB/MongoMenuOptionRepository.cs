using MaginusLunch.MongoDB;
using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using Polly.Retry;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Menu.Repository
{
    public class MongoMenuOptionRepository : MongoRepository<MenuOption>, IMenuOptionRepository
    {
        public MongoMenuOptionRepository(MongoRepositorySettings settings,
                                        RetryPolicy asyncRetryPolicy, 
                                        RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Task<MenuOption> GetMenuOptionAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public Task<IEnumerable<MenuOption>> GetAllMenuOptionsAsync(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken))
            => FindAsync(x => x.LunchProviderId == lunchProviderId, cancellationToken);

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
