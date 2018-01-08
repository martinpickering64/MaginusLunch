using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Repository;

namespace MaginusLunch.Menu.Service.Validators
{
    public class MenuCommandValidatorFactory : IValidateCommandsFactory<BreadAggregate>,
        IValidateCommandsFactory<CalendarAggregate>,
        IValidateCommandsFactory<MenuOptionAggregate>,
        IValidateCommandsFactory<MenuCategoryAggregate>,
        IValidateCommandsFactory<LunchProviderAggregate>,
        IValidateCommandsFactory<FillingAggregate>
    {
        private readonly CalendarCommandValidator _calendarCommandValidator;
        private readonly BreadCommandValidator _breadCommandValidator;
        private readonly FillingCommandValidator _fillingCommandValidator;
        private readonly MenuOptionCommandValidator _menuOptionCommandValidator;
        private readonly MenuCategoryCommandValidator _menuCategoryCommandValidator;
        private readonly LunchProviderCommandValidator _lunchProviderCommandValidator;

        public MenuCommandValidatorFactory(ICalendarRepository calendarRepository,
            IBreadRepository breadRepository,
            IMenuOptionRepository menuOptionRepository,
            IMenuCategoryRepository menuCategoryRepository,
            IFillingRepository fillingRepository,
            ILunchProviderRepository lunchProviderRepository)
        {
            _calendarCommandValidator = new CalendarCommandValidator(calendarRepository);
            _breadCommandValidator = new BreadCommandValidator(calendarRepository, breadRepository);
            _fillingCommandValidator = new FillingCommandValidator(calendarRepository, fillingRepository);
            _menuOptionCommandValidator = new MenuOptionCommandValidator(calendarRepository, menuOptionRepository);
            _menuCategoryCommandValidator = new MenuCategoryCommandValidator(calendarRepository, menuCategoryRepository);
            _lunchProviderCommandValidator = new LunchProviderCommandValidator(calendarRepository, lunchProviderRepository);
        }

        IValidateCommands<CommandType, BreadAggregate> IValidateCommandsFactory<BreadAggregate>.GetValidatorFor<CommandType>()
        {
            return _breadCommandValidator as IValidateCommands<CommandType, BreadAggregate>;
        }

        IValidateCommands<CommandType, CalendarAggregate> IValidateCommandsFactory<CalendarAggregate>.GetValidatorFor<CommandType>()
        {
            return _calendarCommandValidator as IValidateCommands<CommandType, CalendarAggregate>;
        }

        IValidateCommands<CommandType, MenuOptionAggregate> IValidateCommandsFactory<MenuOptionAggregate>.GetValidatorFor<CommandType>()
        {
            return _menuOptionCommandValidator as IValidateCommands<CommandType, MenuOptionAggregate>;
        }

        IValidateCommands<CommandType, MenuCategoryAggregate> IValidateCommandsFactory<MenuCategoryAggregate>.GetValidatorFor<CommandType>()
        {
            return _menuCategoryCommandValidator as IValidateCommands<CommandType, MenuCategoryAggregate>;
        }

        IValidateCommands<CommandType, LunchProviderAggregate> IValidateCommandsFactory<LunchProviderAggregate>.GetValidatorFor<CommandType>()
        {
            return _lunchProviderCommandValidator as IValidateCommands<CommandType, LunchProviderAggregate>;
        }

        IValidateCommands<CommandType, FillingAggregate> IValidateCommandsFactory<FillingAggregate>.GetValidatorFor<CommandType>()
        {
            return _fillingCommandValidator as IValidateCommands<CommandType, FillingAggregate>;
        }
    }
}
