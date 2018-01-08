using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public interface ILunchProviderRepository
    {
        LunchProvider GetLunchProvider(Guid id);
        Task<LunchProvider> GetLunchProviderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<LunchProvider> GetAllLunchProviders();
        Task<IEnumerable<LunchProvider>> GetAllLunchProvidersAsync(CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<LunchProvider> GetAllLunchProviders(DateTime activeOnThisDate);
        Task<IEnumerable<LunchProvider>> GetAllLunchProvidersAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        void SaveLunchProvider(LunchProvider LunchProvider);
        Task SaveLunchProviderAsync(LunchProvider LunchProvider, CancellationToken cancellationToken = default(CancellationToken));
    }
}
