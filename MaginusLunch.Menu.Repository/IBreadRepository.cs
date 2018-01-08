using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Repository
{
    public interface IBreadRepository
    {
        Task<Bread> GetBreadAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Bread>> GetAllBreadsAsync(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveBreadAsync(Bread Bread, CancellationToken cancellationToken = default(CancellationToken));
    }
}
