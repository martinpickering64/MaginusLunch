using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public interface IBreadRepository
    {
        Bread GetBread(Guid id);
        Task<Bread> GetBreadAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Bread> GetAllBreads(DateTime activeOnThisDate);
        Task<IEnumerable<Bread>> GetAllBreadsAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Bread> GetAllBreads(Guid lunchProviderId, DateTime activeOnThisDate);
        Task<IEnumerable<Bread>> GetAllBreadsAsync(Guid lunchProviderId, DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        void SaveBread(Bread Bread);
        Task SaveBreadAsync(Bread Bread, CancellationToken cancellationToken = default(CancellationToken));
    }
}
