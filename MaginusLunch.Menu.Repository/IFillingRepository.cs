using MaginusLunch.Menu.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Repository
{
    public interface IFillingRepository
    {
        Task<Filling> GetFillingAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Filling>> GetAllFillingsAsync(Guid lunchProviderId, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveFillingAsync(Filling filling, CancellationToken cancellationToken = default(CancellationToken));
    }
}
