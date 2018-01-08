using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface IMenuOptionService
    {
        Task<MenuOption> GetMenuOption(Guid menuOptionId, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<MenuOption>> GetAllMenuOptions(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
