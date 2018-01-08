using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface IFillingService
    {
        Task<Filling> GetFilling(Guid fillingId, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Filling>> GetAllFillings(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
