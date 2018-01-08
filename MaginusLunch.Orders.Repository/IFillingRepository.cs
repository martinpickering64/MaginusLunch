using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public interface IFillingRepository
    {
        Filling GetFilling(Guid id);
        Task<Filling> GetFillingAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Filling> GetAllFillings(DateTime activeOnThisDate);
        Task<IEnumerable<Filling>> GetAllFillingsAsync(DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Filling> GetAllFillings(DateTime activeOnThisDate, Guid menuCategoryId);
        Task<IEnumerable<Filling>> GetAllFillingsAsync(DateTime activeOnThisDate, Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Filling> GetAllFillings(Guid lunchProviderId, DateTime activeOnThisDate);
        Task<IEnumerable<Filling>> GetAllFillingsAsync(Guid lunchProviderId, DateTime activeOnThisDate, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Filling> GetAllFillings(Guid lunchProviderId, DateTime activeOnThisDate, Guid menuCategoryId);
        Task<IEnumerable<Filling>> GetAllFillingsAsync(Guid lunchProviderId, DateTime activeOnThisDate, Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken));
        void SaveFilling(Filling filling);
        Task SaveFillingAsync(Filling filling, CancellationToken cancellationToken = default(CancellationToken));
    }
}
