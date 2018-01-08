using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface IBreadService
    {
        Task<Bread> GetBread(Guid breadId, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Bread>> GetAllBreads(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
