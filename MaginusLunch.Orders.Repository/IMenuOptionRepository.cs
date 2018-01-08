using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public interface IMenuOptionRepository
    {
        MenuOption GetMenuOption(Guid id);
        Task<MenuOption> GetMenuOptionAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<MenuOption> GetAllMenuOptions(DateTime activeOnThisDate);
        Task<IEnumerable<MenuOption>> GetAllMenuOptionsAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<MenuOption> GetAllMenuOptions(Guid lunchProviderId, DateTime activeOnThisDate);
        Task<IEnumerable<MenuOption>> GetAllMenuOptionsAsync(Guid lunchProviderId, DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        void SaveMenuOption(MenuOption MenuOption);
        Task SaveMenuOptionAsync(MenuOption MenuOption, CancellationToken cancellationToken = default(CancellationToken));
    }
}
