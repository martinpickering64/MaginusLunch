using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public class MenuCategoryCommandValidator : MenuCommandValidatorBase,
        IValidateCommands<CreateMenuCategory, MenuCategoryAggregate>,
        IValidateCommands<ChangeNameOfMenuCategory, MenuCategoryAggregate>
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;

        public MenuCategoryCommandValidator(ICalendarRepository calendarRepository, IMenuCategoryRepository menuCategoryRepository)
            : base(calendarRepository)
        {
            _menuCategoryRepository = menuCategoryRepository;
        }

        async Task<ValidationStatus> IValidateCommands<CreateMenuCategory, MenuCategoryAggregate>.IsValidAsync(CreateMenuCategory command, MenuCategoryAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = new ValidationStatus();
            if (command.Id != aggregate.Id)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateRootIdInvalidCode,
                    $"The Id for the Command ({command.Id}) does not match with Id for the new {aggregate.GetType().Name} ({aggregate.Id}). Contact Support!"));
                return returnStatus;
            }
            if (aggregate.Version > 0)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateVersionInvalidCode,
                    $"Cannot create the same {aggregate.GetType().Name} more than once. Version attempted was {aggregate.Version}."));
            }
            if (string.IsNullOrWhiteSpace(command.EditorUserId))
            {
                returnStatus.Reasons.Add(EditorUserIdNull);
            }
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                returnStatus.Reasons.Add(AggregateNameInvalid);
            }
            if (!returnStatus.IsValid)
            {
                return returnStatus;
            }
            if ((await _menuCategoryRepository.GetMenuCategoryAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"A Menu Category for that Id already exists; Id =[{command.Id}]."));
            }
            return returnStatus;
        }

        Task<ValidationStatus> IValidateCommands<ChangeNameOfMenuCategory, MenuCategoryAggregate>.IsValidAsync(ChangeNameOfMenuCategory command, MenuCategoryAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = CommonValidation(command, aggregate, cancellationToken);
            if (returnStatus.IsValid
                && string.IsNullOrWhiteSpace(command.NewName))
            {
                returnStatus.Reasons.Add(AggregateNameInvalid);
            }
            return Task.FromResult(returnStatus);
        }
    }
}
