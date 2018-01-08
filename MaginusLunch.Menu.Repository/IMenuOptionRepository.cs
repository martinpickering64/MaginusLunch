using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Repository
{
    public interface IMenuOptionRepository
    {
        Task<MenuOption> GetMenuOptionAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<MenuOption>> GetAllMenuOptionsAsync(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveMenuOptionAsync(MenuOption MenuOption, CancellationToken cancellationToken = default(CancellationToken));
    }
}
