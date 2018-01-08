using System;
using System.Collections.Generic;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Repository;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace MaginusLunch.Orders.Service
{
    public class MenuService : IMenuService
    {
        private static readonly Bread[] _emptyBread = new Bread[0];
        private static readonly Filling[] _emptyFillings = new Filling[0];
        private static readonly MenuOption[] _emptyMenuOptions = new MenuOption[0];

        private readonly ICalendarService _calSvc;
        private readonly ILunchProviderService _lunchProviderSvc;
        private readonly IFillingRepository _fillingRepository;
        private readonly IBreadRepository _breadRepository;
        private readonly IMenuOptionRepository _menuOptionRepository;

        public MenuService(ICalendarService calendarSvc, 
            ILunchProviderService lunchProviderSvc, 
            IFillingRepository fillingRepository,
            IBreadRepository breadRepository,
            IMenuOptionRepository menuOptionRepository)
        {
            _calSvc = calendarSvc;
            _lunchProviderSvc = lunchProviderSvc;
            _fillingRepository = fillingRepository;
            _breadRepository = breadRepository;
            _menuOptionRepository = menuOptionRepository;
        }

        public async Task<IEnumerable<Bread>> GetAvailableBreadAsync(Guid lunchProviderId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await IsDateEmbargoed(asOfDate, lunchProviderId, cancellationToken).ConfigureAwait(false))
            {
                return _emptyBread;
            }
            return await _breadRepository.GetAllBreadsAsync(
                lunchProviderId: lunchProviderId, 
                activeOnThisDate: asOfDate, 
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> IsBreadAvailableAsync(Guid breadId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var bread = await _breadRepository.GetBreadAsync(breadId, cancellationToken).ConfigureAwait(false);
            if (bread == null
                || bread.Status == BreadStatus.Withdrawn
                || bread.WithdrawnDates.Contains(asOfDate))
            {
                return false;
            }
            return await IsDateEmbargoed(asOfDate, bread.LunchProviderId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Filling>> GetAvailableFillingsAsync(Guid lunchProviderId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await IsDateEmbargoed(asOfDate, lunchProviderId, cancellationToken).ConfigureAwait(false))
            {
                return _emptyFillings;
            }
            return await _fillingRepository.GetAllFillingsAsync(lunchProviderId: lunchProviderId, activeOnThisDate: asOfDate, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        
        public async Task<IEnumerable<Filling>> GetAvailableFillingsAsync(Guid lunchProviderId, DateTime asOfDate, Guid menuCategoryId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await IsDateEmbargoed(asOfDate, lunchProviderId, cancellationToken).ConfigureAwait(false))
            {
                return _emptyFillings;
            }
            return await _fillingRepository.GetAllFillingsAsync(
                activeOnThisDate: asOfDate,
                lunchProviderId: lunchProviderId,
                menuCategoryId: menuCategoryId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> IsFillingAvailableAsync(Guid fillingId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filling = await _fillingRepository.GetFillingAsync(fillingId, cancellationToken).ConfigureAwait(false);
            if (filling == null
                || filling.Status == FillingStatus.Withdrawn
                || filling.WithdrawnDates.Contains(asOfDate))
            {
                return false;
            }
            return await IsDateEmbargoed(asOfDate, filling.LunchProviderId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MenuOption>> GetAvailableMenuOptionsAsync(Guid lunchProviderId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await IsDateEmbargoed(asOfDate, lunchProviderId, cancellationToken).ConfigureAwait(false))
            {
                return _emptyMenuOptions;
            }
            return await _menuOptionRepository.GetAllMenuOptionsAsync(
                activeOnThisDate: asOfDate, 
                lunchProviderId: lunchProviderId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> IsMenuOptionAvailableAsync(Guid menuOptionId, DateTime asOfDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var menuOption = await _fillingRepository.GetFillingAsync(menuOptionId, cancellationToken).ConfigureAwait(false);
            if (menuOption == null
                || menuOption.Status == FillingStatus.Withdrawn
                || menuOption.WithdrawnDates.Contains(asOfDate))
            {
                return false;
            }
            return await IsDateEmbargoed(asOfDate, menuOption.LunchProviderId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> CanHaveBreadAsync(Guid fillingId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filling = await _fillingRepository.GetFillingAsync(fillingId, cancellationToken).ConfigureAwait(false);
            return !filling.DisallowBread;
        }

        public async Task<bool> CanHaveMenuOptionAsync(Guid fillingId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filling = await _fillingRepository.GetFillingAsync(fillingId, cancellationToken).ConfigureAwait(false);
            return !filling.DisallowMenuOption;
        }

        private async Task<bool> IsDateEmbargoed(DateTime asOfDate, Guid lunchProviderId, CancellationToken cancellationToken)
        {
            var x = await Task.WhenAll(new[]
                                { _calSvc.IsAvailableAsync(asOfDate, cancellationToken),
                                  _lunchProviderSvc.IsAvailableAsync(lunchProviderId, asOfDate, cancellationToken)
                                }).ConfigureAwait(false);
            return !(x.Any(available => available == false));
        }
    }
}
