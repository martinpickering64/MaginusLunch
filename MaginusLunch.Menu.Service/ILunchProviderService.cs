using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface ILunchProviderService
    {
        Task<LunchProvider> GetLunchProvider(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<LunchProvider>> GetAllLunchProviders(CancellationToken cancellationToken = default(CancellationToken));
    }
}
