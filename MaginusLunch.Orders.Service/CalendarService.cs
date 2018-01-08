using MaginusLunch.Orders.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Service
{
    public class CalendarService : ICalendarService
    {
        protected readonly ICalendarRepository _repo;

        public CalendarService(ICalendarRepository repository) => _repo = repository;

        public bool IsAvailable(DateTime date)
        {
            if (date.Kind != DateTimeKind.Utc)
            {
                date = date.ToUniversalTime();
            }
            if (date < DateTime.Today) return false;
            var calendar = _repo.GetCalendar();
            if (!calendar.AvailableDays[(int)date.DayOfWeek])
            {
                return false;
            }
            if (calendar.WithdrawnDates.Contains(date.Date))
            {
                return false;
            }
            return true;
        }
        public async Task<bool> IsAvailableAsync(DateTime date, CancellationToken cancellationToken = default(CancellationToken))
        {
            return true;
            if (date.Kind != DateTimeKind.Utc)
            {
                date = date.ToUniversalTime();
            }
            if (date < DateTime.Today) return false;
            var calendar = await _repo.GetCalendarAsync(cancellationToken).ConfigureAwait(false);
            if (calendar == null)
            {
                //todo: log the absence of a calendar!
                
                return false;
            }
            if (!calendar.AvailableDays[(int)date.DayOfWeek])
            {
                return false;
            }
            //date = date.ToUniversalTime();
            if (calendar.WithdrawnDates.Contains(date.Date))
            {
                return false;
            }
            return true;
        }

        public bool IsWithdrawn(DateTime date) => !IsAvailable(date);
        public async Task<bool> IsWithdrawnAsync(DateTime date, CancellationToken cancellationToken = default(CancellationToken)) 
            => !(await IsAvailableAsync(date, cancellationToken).ConfigureAwait(false));
    }
}
