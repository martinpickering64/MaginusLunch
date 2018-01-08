using System;
using System.Collections.Generic;
using MaginusLunch.Orders.Domain;
using MaginusLunch.MongoDB;
using Polly.Retry;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Orders.Repository
{
    public class MongoMenuCategoryRepository : MongoRepository<MenuCategory>, IMenuCategoryRepository
    {
        public MongoMenuCategoryRepository(MongoRepositorySettings settings,
                                                RetryPolicy asyncRetryPolicy, 
                                                RetryPolicy retryPolicy) 
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        public IEnumerable<MenuCategory> GetMenuCategories() => FindAll();

        public Task<IEnumerable<MenuCategory>> GetMenuCategoriesAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => FindAllAsync(cancellationToken);

        public MenuCategory GetMenuCategory(Guid id) => Get(id);

        public Task<MenuCategory> GetMenuCategoryAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        public void SaveMenuCategory(MenuCategory menuCategory)
        {
            if (menuCategory.Version == 0)
            {
                Insert(menuCategory);
            }
            else
            {
                Replace(menuCategory);
            }
        }

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
