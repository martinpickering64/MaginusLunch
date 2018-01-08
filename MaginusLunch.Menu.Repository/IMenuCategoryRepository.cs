using System;
using MaginusLunch.Menu.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Menu.Repository
{
    public interface IMenuCategoryRepository
    {
        Task<MenuCategory> GetMenuCategoryAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<MenuCategory>> GetAllMenuCategoriesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task SaveMenuCategoryAsync(MenuCategory menuCategory, CancellationToken cancellationToken = default(CancellationToken));
    }
}
