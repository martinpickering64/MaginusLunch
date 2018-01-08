using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MaginusLunch.Core.Messages.Commands;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System.Linq;
using MaginusLunch.Logging;
using MaginusLunch.Core.Validation;
using MaginusLunch.Core.Extensions;
using MaginusLunch.Core.EventStore;

namespace MaginusLunch.Menu.Service
{
    public class LocalMenuService : IMenuService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LocalMenuService));

        private IBreadRepository _breadRepository;
        private readonly ICalendarRepository _calendarRepository;
        private readonly IFillingRepository _fillingRepository;
        private readonly ILunchProviderRepository _lunchProviderRepository;
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuOptionRepository _menuOptionRepository;
        private readonly Core.EventStore.IRepository _eventStore;
        private readonly IValidateCommandsFactory<BreadAggregate> _breadValidatorFactory;
        private readonly IValidateCommandsFactory<CalendarAggregate> _calendarValidatorFactory;
        private readonly IValidateCommandsFactory<FillingAggregate> _fillingValidatorFactory;
        private readonly IValidateCommandsFactory<LunchProviderAggregate> _lunchProviderValidatorFactory;
        private readonly IValidateCommandsFactory<MenuCategoryAggregate> _menuCategoryValidatorFactory;
        private readonly IValidateCommandsFactory<MenuOptionAggregate> _menuOptionValidatorFactory;


        public LocalMenuService(IBreadRepository breadRepository, 
            ICalendarRepository calendarRepository,
            IFillingRepository fillingRepository,
            ILunchProviderRepository lunchProviderRepository,
            IMenuCategoryRepository menuCategoryRepository,
            IMenuOptionRepository menuOptionRepository,
            Core.EventStore.IRepository eventStore,
            IValidateCommandsFactory<BreadAggregate> breadValidatorFactory,
            IValidateCommandsFactory<CalendarAggregate> calendarValidatorFactory,
            IValidateCommandsFactory<FillingAggregate> fillingValidatorFactory,
            IValidateCommandsFactory<LunchProviderAggregate> lunchProviderValidatorFactory,
            IValidateCommandsFactory<MenuCategoryAggregate> menuCategoryValidatorFactory,
            IValidateCommandsFactory<MenuOptionAggregate> menuOptionValidatorFactory)
        {
            _breadRepository = breadRepository;
            _calendarRepository = calendarRepository;
            _fillingRepository = fillingRepository;
            _lunchProviderRepository = lunchProviderRepository;
            _menuCategoryRepository = menuCategoryRepository;
            _menuOptionRepository = menuOptionRepository;
            _eventStore = eventStore;
            _breadValidatorFactory = breadValidatorFactory;
            _calendarValidatorFactory = calendarValidatorFactory;
            _fillingValidatorFactory = fillingValidatorFactory;
            _lunchProviderValidatorFactory = lunchProviderValidatorFactory;
            _menuCategoryValidatorFactory = menuCategoryValidatorFactory;
            _menuOptionValidatorFactory = menuOptionValidatorFactory;
        }

        #region interfaces

        Task<Bread> IBreadService.GetBread(Guid breadId, CancellationToken cancellationToken)
            => _breadRepository.GetBreadAsync(breadId, cancellationToken);

        Task<IEnumerable<Bread>> IBreadService.GetAllBreads(Guid lunchProviderId, CancellationToken cancellationToken)
            => _breadRepository.GetAllBreadsAsync(lunchProviderId, cancellationToken);

        Task<Calendar> ICalendarService.GetCalendarAsync(CancellationToken cancellationToken)
            => _calendarRepository.GetCalendarAsync(cancellationToken);

        Task<IEnumerable<Filling>> IFillingService.GetAllFillings(Guid lunchProviderId, CancellationToken cancellationToken)
            => _fillingRepository.GetAllFillingsAsync(lunchProviderId, cancellationToken);

        Task<Filling> IFillingService.GetFilling(Guid fillingId, CancellationToken cancellationToken)
            => _fillingRepository.GetFillingAsync(fillingId, cancellationToken);

        Task<IEnumerable<LunchProvider>> ILunchProviderService.GetAllLunchProviders(CancellationToken cancellationToken)
            => _lunchProviderRepository.GetAllLunchProvidersAsync(cancellationToken);

        Task<LunchProvider> ILunchProviderService.GetLunchProvider(Guid lunchProviderId, CancellationToken cancellationToken)
            => _lunchProviderRepository.GetLunchProviderAsync(lunchProviderId, cancellationToken);

        Task<IEnumerable<MenuCategory>> IMenuCategoryService.GetAllMenuCategories(CancellationToken cancellationToken)
            => _menuCategoryRepository.GetAllMenuCategoriesAsync(cancellationToken);

        Task<MenuCategory> IMenuCategoryService.GetMenuCategory(Guid menuCategoryId, CancellationToken cancellationToken)
            => _menuCategoryRepository.GetMenuCategoryAsync(menuCategoryId, cancellationToken);

        Task<IEnumerable<MenuOption>> IMenuOptionService.GetAllMenuOptions(Guid lunchProviderId, CancellationToken cancellationToken)
            => _menuOptionRepository.GetAllMenuOptionsAsync(lunchProviderId, cancellationToken);

        Task<MenuOption> IMenuOptionService.GetMenuOption(Guid menuOptionId, CancellationToken cancellationToken)
            => _menuOptionRepository.GetMenuOptionAsync(menuOptionId, cancellationToken);

        Task<CommandStatus> IMenuService.HandleForUserAsync(ClaimsPrincipal user, MenuCommand command, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Must pass in a ClaimsPrincipal");
            }
            var emailClaim = user.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null)
            {
                throw new ArgumentException($"The ClaimsPrincipal does not have an Email Claim; {0}.",
                    user.Identity == null ? "anonymous user" : user.Identity.Name);
            }
            if (string.IsNullOrWhiteSpace(emailClaim.Value))
            {
                throw new ArgumentException($"The ClaimsPrincipal does not have a valid Email Claim as its null or empty; {0}.", user.Identity.Name);
            }
            command.EditorUserId = emailClaim.Value.EmailAddressToUserId();
            var commandType = command.GetType();
            switch (commandType.Name)
            {
                case "CreateBread":
                    return HandleCommandAsync(command as CreateBread, cancellationToken);
                case "ActivateBread":
                    return HandleCommandAsync(command as ActivateBread, cancellationToken);
                case "ActivateBreadForDate":
                    return HandleCommandAsync(command as ActivateBreadForDate, cancellationToken);
                case "ChangeNameOfBread":
                    return HandleCommandAsync(command as ChangeNameOfBread, cancellationToken);
                case "WithdrawBread":
                    return HandleCommandAsync(command as WithdrawBread, cancellationToken);
                case "WithdrawBreadForDate":
                    return HandleCommandAsync(command as WithdrawBreadForDate, cancellationToken);
                case "CreateCalendar":
                    return HandleCommandAsync(command as CreateCalendar, cancellationToken);
                case "ActivateCalendarForDate":
                    return HandleCommandAsync(command as ActivateCalendarForDate, cancellationToken);
                case "AmendOpenOrderDate":
                    return HandleCommandAsync(command as AmendOpenOrderDate, cancellationToken);
                case "CloseDayOnCalendar":
                    return HandleCommandAsync(command as CloseDayOnCalendar, cancellationToken);
                case "OpenDayOnCalendar":
                    return HandleCommandAsync(command as OpenDayOnCalendar, cancellationToken);
                case "WithdrawCalendarForDate":
                    return HandleCommandAsync(command as WithdrawCalendarForDate, cancellationToken);
                case "CreateLunchProvider":
                    return HandleCommandAsync(command as CreateLunchProvider, cancellationToken);
                case "ActivateLunchProvider":
                    return HandleCommandAsync(command as ActivateLunchProvider, cancellationToken);
                case "ActivateLunchProviderForDate":
                    return HandleCommandAsync(command as ActivateLunchProviderForDate, cancellationToken);
                case "ChangeNameOfLunchProvider":
                    return HandleCommandAsync(command as ChangeNameOfLunchProvider, cancellationToken);
                case "WithdrawLunchProvider":
                    return HandleCommandAsync(command as WithdrawLunchProvider, cancellationToken);
                case "WithdrawLunchProviderForDate":
                    return HandleCommandAsync(command as WithdrawLunchProviderForDate, cancellationToken);
                case "CreateMenuOption":
                    return HandleCommandAsync(command as CreateMenuOption, cancellationToken);
                case "ActivateMenuOption":
                    return HandleCommandAsync(command as ActivateMenuOption, cancellationToken);
                case "ActivateMenuOptionForDate":
                    return HandleCommandAsync(command as ActivateMenuOptionForDate, cancellationToken);
                case "ChangeNameOfMenuOption":
                    return HandleCommandAsync(command as ChangeNameOfMenuOption, cancellationToken);
                case "WithdrawMenuOption":
                    return HandleCommandAsync(command as WithdrawMenuOption, cancellationToken);
                case "WithdrawMenuOptionForDate":
                    return HandleCommandAsync(command as WithdrawMenuOptionForDate, cancellationToken);
                case "CreateFilling":
                    return HandleCommandAsync(command as CreateFilling, cancellationToken);
                case "ActivateFilling":
                    return HandleCommandAsync(command as ActivateFilling, cancellationToken);
                case "ActivateFillingForDate":
                    return HandleCommandAsync(command as ActivateFillingForDate, cancellationToken);
                case "ChangeNameOfFilling":
                    return HandleCommandAsync(command as ChangeNameOfFilling, cancellationToken);
                case "WithdrawFilling":
                    return HandleCommandAsync(command as WithdrawFilling, cancellationToken);
                case "WithdrawFillingForDate":
                    return HandleCommandAsync(command as WithdrawFillingForDate, cancellationToken);
                case "AllowBreadWithFilling":
                    return HandleCommandAsync(command as AllowBreadWithFilling, cancellationToken);
                case "DisallowBreadWithFilling":
                    return HandleCommandAsync(command as DisallowBreadWithFilling, cancellationToken);
                case "AllowMenuOptionsWithFilling":
                    return HandleCommandAsync(command as AllowMenuOptionsWithFilling, cancellationToken);
                case "DisallowMenuOptionsWithFilling":
                    return HandleCommandAsync(command as DisallowMenuOptionsWithFilling, cancellationToken);
                case "UpdateFilling":
                    return HandleCommandAsync(command as UpdateFilling, cancellationToken);
                case "CreateMenuCategory":
                    return HandleCommandAsync(command as CreateMenuCategory, cancellationToken);
                case "ChangeNameOfMenuCategory":
                    return HandleCommandAsync(command as ChangeNameOfMenuCategory, cancellationToken);
                default:
                    _logger.ErrorFormat("The Menu Command received is of an unknown type: {0}", commandType.Name);
                    var returnStatus = new CommandStatus();
                    returnStatus.Reasons.Add(new ValidationStatus.Reason(
                                                    CommandValidatorBase.UnknownCommandCode, 
                                                    $"Unknown Menu Command received [{commandType.Name}]"));
                    return Task.FromResult(returnStatus);
            }
        }

#endregion interfaces

        #region command handlers

        private async Task<CommandStatus> HandleCommandAsync(UpdateFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<UpdateFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Update(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(DisallowMenuOptionsWithFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<DisallowMenuOptionsWithFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.DisallowMenuOptionsWithFilling(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(AllowMenuOptionsWithFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<AllowMenuOptionsWithFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.AllowMenuOptionsWithFilling(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(DisallowBreadWithFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<DisallowBreadWithFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.DisallowBreadWithFilling(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(AllowBreadWithFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<AllowBreadWithFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.AllowBreadWithFilling(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawFillingForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<WithdrawFillingForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.WithdrawForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<WithdrawFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Withdraw(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ChangeNameOfFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<ChangeNameOfFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ChangeName(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateFillingForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<ActivateFillingForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ActivateForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateFilling command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<FillingAggregate>(command.Id);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<ActivateFilling>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Activate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ChangeNameOfMenuCategory command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<MenuCategoryAggregate>(command.Id);
            var validationStatus = await _menuCategoryValidatorFactory.GetValidatorFor<ChangeNameOfMenuCategory>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ChangeName(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawMenuOptionForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<MenuOptionAggregate>(command.Id);
            var validationStatus = await _menuOptionValidatorFactory.GetValidatorFor<WithdrawMenuOptionForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.WithdrawForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawMenuOption command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<MenuOptionAggregate>(command.Id);
            var validationStatus = await _menuOptionValidatorFactory.GetValidatorFor<WithdrawMenuOption>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Withdraw(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ChangeNameOfMenuOption command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<MenuOptionAggregate>(command.Id);
            var validationStatus = await _menuOptionValidatorFactory.GetValidatorFor<ChangeNameOfMenuOption>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ChangeName(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateMenuOptionForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<MenuOptionAggregate>(command.Id);
            var validationStatus = await _menuOptionValidatorFactory.GetValidatorFor<ActivateMenuOptionForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ActivateForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateMenuOption command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<MenuOptionAggregate>(command.Id);
            var validationStatus = await _menuOptionValidatorFactory.GetValidatorFor<ActivateMenuOption>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Activate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawLunchProviderForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<LunchProviderAggregate>(command.Id);
            var validationStatus = await _lunchProviderValidatorFactory.GetValidatorFor<WithdrawLunchProviderForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.WithdrawForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawLunchProvider command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<LunchProviderAggregate>(command.Id);
            var validationStatus = await _lunchProviderValidatorFactory.GetValidatorFor<WithdrawLunchProvider>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Withdraw(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ChangeNameOfLunchProvider command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<LunchProviderAggregate>(command.Id);
            var validationStatus = await _lunchProviderValidatorFactory.GetValidatorFor<ChangeNameOfLunchProvider>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ChangeName(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateLunchProvider command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<LunchProviderAggregate>(command.Id);
            var validationStatus = await _lunchProviderValidatorFactory.GetValidatorFor<ActivateLunchProvider>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Activate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateLunchProviderForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<LunchProviderAggregate>(command.Id);
            var validationStatus = await _lunchProviderValidatorFactory.GetValidatorFor<ActivateLunchProviderForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ActivateForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawCalendarForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<CalendarAggregate>(command.Id);
            var validationStatus = await _calendarValidatorFactory.GetValidatorFor<WithdrawCalendarForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.WithdrawForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(OpenDayOnCalendar command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<CalendarAggregate>(command.Id);
            var validationStatus = await _calendarValidatorFactory.GetValidatorFor<OpenDayOnCalendar>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.OpenDay(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CloseDayOnCalendar command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<CalendarAggregate>(command.Id);
            var validationStatus = await _calendarValidatorFactory.GetValidatorFor<CloseDayOnCalendar>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.CloseDay(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(AmendOpenOrderDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<CalendarAggregate>(command.Id);
            var validationStatus = await _calendarValidatorFactory.GetValidatorFor<AmendOpenOrderDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.AmendOpenOrderDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateCalendarForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<CalendarAggregate>(command.Id);
            var validationStatus = await _calendarValidatorFactory.GetValidatorFor<ActivateCalendarForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ActivateForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawBreadForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<BreadAggregate>(command.Id);
            var validationStatus = await _breadValidatorFactory.GetValidatorFor<WithdrawBreadForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.WithdrawForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(WithdrawBread command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<BreadAggregate>(command.Id);
            var validationStatus = await _breadValidatorFactory.GetValidatorFor<WithdrawBread>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Withdraw(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ChangeNameOfBread command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<BreadAggregate>(command.Id);
            var validationStatus = await _breadValidatorFactory.GetValidatorFor<ChangeNameOfBread>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ChangeName(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateBreadForDate command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<BreadAggregate>(command.Id);
            var validationStatus = await _breadValidatorFactory.GetValidatorFor<ActivateBreadForDate>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.ActivateForDate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(ActivateBread command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<BreadAggregate>(command.Id);
            var validationStatus = await _breadValidatorFactory.GetValidatorFor<ActivateBread>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.Activate(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CreateMenuCategory command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("CreateMenuCategory Command received with no Identity. Going to use identity [{0}].", command.Id, command.EditorUserId);
            }
            var newAggregate = new MenuCategoryAggregate(command);
            var validationStatus = await _menuCategoryValidatorFactory.GetValidatorFor<CreateMenuCategory>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CreateMenuOption command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("CreateMenuOption Command received with no Identity. Going to use identity [{0}].", command.Id, command.EditorUserId);
            }
            var newAggregate = new MenuOptionAggregate(command);
            var validationStatus = await _menuOptionValidatorFactory.GetValidatorFor<CreateMenuOption>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CreateFilling command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("CreateFilling Command received with no Identity. Going to use identity [{0}].", command.Id, command.EditorUserId);
            }
            var newAggregate = new FillingAggregate(command);
            var validationStatus = await _fillingValidatorFactory.GetValidatorFor<CreateFilling>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CreateLunchProvider command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("CreateLunchProvider Command received with no Identity. Going to use identity [{0}].", command.Id, command.EditorUserId);
            }
            var newAggregate = new LunchProviderAggregate(command);
            var validationStatus = await _lunchProviderValidatorFactory.GetValidatorFor<CreateLunchProvider>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CreateBread command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("CreateBread Command received with no Identity. Going to use identity [{0}].", command.Id, command.EditorUserId);
            }
            var newAggregate = new BreadAggregate(command);
            var validationStatus = await _breadValidatorFactory.GetValidatorFor<CreateBread>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        private async Task<CommandStatus> HandleCommandAsync(CreateCalendar command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("CreateCalendar Command received with no Identity. Going to use identity [{0}].", command.Id, command.EditorUserId);
            }
            var newAggregate = new CalendarAggregate(command);
            var validationStatus = await _calendarValidatorFactory.GetValidatorFor<CreateCalendar>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        #endregion command handlers
    }
}
