using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public class MenuOptionCommandValidator : MenuCommandValidatorBase,
        IValidateCommands<CreateMenuOption, MenuOptionAggregate>,
        IValidateCommands<ActivateMenuOptionForDate, MenuOptionAggregate>,
        IValidateCommands<ActivateMenuOption, MenuOptionAggregate>,
        IValidateCommands<WithdrawMenuOption, MenuOptionAggregate>,
        IValidateCommands<WithdrawMenuOptionForDate, MenuOptionAggregate>,
        IValidateCommands<ChangeNameOfMenuOption, MenuOptionAggregate>
    {
        private readonly IMenuOptionRepository _menuOptionRepository;

        public MenuOptionCommandValidator(ICalendarRepository calendarRepository, IMenuOptionRepository menuOptionRepository)
            : base(calendarRepository)
        {
            _menuOptionRepository = menuOptionRepository;
        }

        async Task<ValidationStatus> IValidateCommands<CreateMenuOption, MenuOptionAggregate>.IsValidAsync(CreateMenuOption command, MenuOptionAggregate aggregate, CancellationToken cancellationToken)
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
            if ((await _menuOptionRepository.GetMenuOptionAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"A Menu Option for that Id already exists; Id =[{command.Id}]."));
            }
            return returnStatus;
        }

        async Task<ValidationStatus> IValidateCommands<WithdrawMenuOptionForDate, MenuOptionAggregate>.IsValidAsync(WithdrawMenuOptionForDate command, MenuOptionAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = CommonValidation(command, aggregate, cancellationToken);
            if (returnStatus.IsValid)
            {
                var aReason = await WithdrawlDateValidation(command.AffectedDate, cancellationToken).ConfigureAwait(false);
                if (aReason != null)
                {
                    returnStatus.Reasons.Add(aReason);
                }
            }
            return returnStatus;
        }

        async Task<ValidationStatus> IValidateCommands<ActivateMenuOptionForDate, MenuOptionAggregate>.IsValidAsync(ActivateMenuOptionForDate command, MenuOptionAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = CommonValidation(command, aggregate, cancellationToken);
            if (returnStatus.IsValid)
            {
                var aReason = await WithdrawlDateValidation(command.AffectedDate, cancellationToken).ConfigureAwait(false);
                if (aReason != null)
                {
                    returnStatus.Reasons.Add(aReason);
                }
            }
            return returnStatus;
        }

        Task<ValidationStatus> IValidateCommands<ActivateMenuOption, MenuOptionAggregate>.IsValidAsync(ActivateMenuOption command, MenuOptionAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<WithdrawMenuOption, MenuOptionAggregate>.IsValidAsync(WithdrawMenuOption command, MenuOptionAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<ChangeNameOfMenuOption, MenuOptionAggregate>.IsValidAsync(ChangeNameOfMenuOption command, MenuOptionAggregate aggregate, CancellationToken cancellationToken)
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
