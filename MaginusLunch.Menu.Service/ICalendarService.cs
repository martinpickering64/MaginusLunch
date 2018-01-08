using MaginusLunch.Menu.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface ICalendarService
    {
        Task<Calendar> GetCalendarAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
