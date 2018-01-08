using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Service
{
    public interface ICalendarService
    {
        bool IsAvailable(DateTime date);
        Task<bool> IsAvailableAsync(DateTime date, CancellationToken cancellationToken = default(CancellationToken));
        bool IsWithdrawn(DateTime date);
        Task<bool> IsWithdrawnAsync(DateTime date, CancellationToken cancellationToken = default(CancellationToken));
    }
}
