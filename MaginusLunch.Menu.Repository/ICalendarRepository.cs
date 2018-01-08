using MaginusLunch.Menu.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Repository
{
    public interface ICalendarRepository
    {
        Task<Calendar> GetCalendarAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<Calendar> GetCalendarAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task SaveCalendarAsync(Calendar calendar, CancellationToken cancellationToken = default(CancellationToken));
    }
}
