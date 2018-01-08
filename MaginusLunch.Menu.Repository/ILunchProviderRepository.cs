using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Repository
{
    public interface ILunchProviderRepository
    {
        Task<LunchProvider> GetLunchProviderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<LunchProvider>> GetAllLunchProvidersAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task SaveLunchProviderAsync(LunchProvider LunchProvider, CancellationToken cancellationToken = default(CancellationToken));
    }
}
