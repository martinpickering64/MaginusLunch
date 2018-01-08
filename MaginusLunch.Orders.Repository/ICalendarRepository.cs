using MaginusLunch.Orders.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public interface ICalendarRepository
    {
        Calendar GetCalendar(Guid id);
        Task<Calendar> GetCalendarAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Calendar GetCalendar();
        Task<Calendar> GetCalendarAsync(CancellationToken cancellationToken = default(CancellationToken));
        void SaveCalendar(Calendar calendar);
        Task SaveCalendarAsync(Calendar calendar, CancellationToken cancellationToken = default(CancellationToken));
    }
}
