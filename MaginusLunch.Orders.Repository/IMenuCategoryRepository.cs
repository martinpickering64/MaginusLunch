using System;
using MaginusLunch.Orders.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Orders.Repository
{
    public interface IMenuCategoryRepository
    {
        MenuCategory GetMenuCategory(Guid id);
        Task<MenuCategory> GetMenuCategoryAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<MenuCategory> GetMenuCategories();
        Task<IEnumerable<MenuCategory>> GetMenuCategoriesAsync(CancellationToken cancellationToken = default(CancellationToken));
        void SaveMenuCategory(MenuCategory menuCategory);
        Task SaveMenuCategoryAsync(MenuCategory menuCategory, CancellationToken cancellationToken = default(CancellationToken));
    }
}
