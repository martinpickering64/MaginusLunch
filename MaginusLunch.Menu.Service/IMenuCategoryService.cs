using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface IMenuCategoryService
    {
        Task<MenuCategory> GetMenuCategory(Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<MenuCategory>> GetAllMenuCategories(CancellationToken cancellationToken = default(CancellationToken));
    }
}
