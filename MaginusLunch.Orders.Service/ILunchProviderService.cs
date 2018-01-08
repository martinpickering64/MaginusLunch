using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Service
{
    public interface ILunchProviderService
    {
        bool IsAvailable(Guid id, DateTime asOfDate);
        Task<bool> IsAvailableAsync(Guid id, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        bool IsWithdrawn(Guid id, DateTime asOfDate);
        Task<bool> IsWithdrawnAsync(Guid id, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
        LunchProvider GetLunchProvider(Guid id);
        Task<LunchProvider> GetLunchProviderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<LunchProvider> GetAvailableLunchProviders();
        Task<IEnumerable<LunchProvider>> GetAvailableLunchProvidersAsync(CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<LunchProvider> GetAvailableLunchProviders(DateTime asOfDate);
        Task<IEnumerable<LunchProvider>> GetAvailableLunchProvidersAsync(DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken));
    }
}
