using System;
using System.Collections.Generic;
using MaginusLunch.Menu.Domain;
using MaginusLunch.MongoDB;
using Polly.Retry;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Menu.Repository
{
    public class MongoMenuCategoryRepository : MongoRepository<MenuCategory>, IMenuCategoryRepository
    {
        public MongoMenuCategoryRepository(MongoRepositorySettings settings,
                                                RetryPolicy asyncRetryPolicy, 
                                                RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public Task<IEnumerable<MenuCategory>> GetAllMenuCategoriesAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => FindAllAsync(cancellationToken);

        public Task<MenuCategory> GetMenuCategoryAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public Task SaveMenuCategoryAsync(MenuCategory menuCategory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (menuCategory.Version == 0)
            {
                return InsertAsync(menuCategory, cancellationToken);
            }
            else
            {
                return ReplaceAsync(menuCategory, cancellationToken);
            }
        }
    }
}
