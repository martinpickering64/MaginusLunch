using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Service
{
    public interface IMenuService
    {
        Task<IEnumerable<Filling>> GetAvailableFillingsAsync(Guid lunchProviderId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Filling>> GetAvailableFillingsAsync(Guid lunchProviderId, DateTime asOfDate, Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsFillingAvailableAsync(Guid fillingId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Bread>> GetAvailableBreadAsync(Guid lunchProviderId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsBreadAvailableAsync(Guid breadId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<MenuOption>> GetAvailableMenuOptionsAsync(Guid lunchProviderId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsMenuOptionAvailableAsync(Guid menuOptionId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> CanHaveBreadAsync(Guid fillingId, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> CanHaveMenuOptionAsync(Guid fillingId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
