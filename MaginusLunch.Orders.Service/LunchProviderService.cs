using System;
using System.Collections.Generic;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Repository;
using System.Threading.Tasks;
using System.Threading;

namespace MaginusLunch.Orders.Service
{
    public class LunchProviderService : ILunchProviderService
    {
        private readonly ILunchProviderRepository _repo;
        private readonly ICalendarService _calendarService;
        private static readonly LunchProvider[] _empty = new LunchProvider[0];

        public LunchProviderService(ILunchProviderRepository repo, ICalendarService calendarSvc)
        {
            _repo = repo;
            _calendarService = calendarSvc;
        }

        public bool IsAvailable(Guid id, DateTime asOfDate)
        {
            var lunchProvider = _repo.GetLunchProvider(id);
            if (lunchProvider == null
                || lunchProvider.Status == LunchProviderStatus.Withdrawn
                || lunchProvider.WithdrawnDates.Contains(asOfDate))
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsAvailableAsync(Guid id, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var lunchProvider = await _repo.GetLunchProviderAsync(id, cancellationToken).ConfigureAwait(false);
            if (lunchProvider == null
                || lunchProvider.Status == LunchProviderStatus.Withdrawn
                || lunchProvider.WithdrawnDates.Contains(asOfDate))
            {
                return false;
            }
            return true;
        }

        public bool IsWithdrawn(Guid id, DateTime asOfDate)
            => !IsAvailable(id, asOfDate);

        public async Task<bool> IsWithdrawnAsync(Guid id, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
            => !(await IsAvailableAsync(id, asOfDate, cancellationToken).ConfigureAwait(false));

        public LunchProvider GetLunchProvider(Guid id) => _repo.GetLunchProvider(id);

        public Task<LunchProvider> GetLunchProviderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
            => _repo.GetLunchProviderAsync(id, cancellationToken);
        public IEnumerable<LunchProvider> GetAvailableLunchProviders() => _repo.GetAllLunchProviders();

        public Task<IEnumerable<LunchProvider>> GetAvailableLunchProvidersAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => _repo.GetAllLunchProvidersAsync(cancellationToken);

        public IEnumerable<LunchProvider> GetAvailableLunchProviders(DateTime asOfDate)
        {
            if (_calendarService.IsWithdrawn(asOfDate)) { return _empty; }
            return _repo.GetAllLunchProviders(asOfDate);
        }

        public async Task<IEnumerable<LunchProvider>> GetAvailableLunchProvidersAsync(DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await _calendarService.IsWithdrawnAsync(asOfDate, cancellationToken).ConfigureAwait(false)) { return _empty; }
            return (await _repo.GetAllLunchProvidersAsync(asOfDate, cancellationToken).ConfigureAwait(false));
        }
    }
}
