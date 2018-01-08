using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public class FillingCommandValidator : MenuCommandValidatorBase,
        IValidateCommands<CreateFilling, FillingAggregate>,
        IValidateCommands<ActivateFillingForDate, FillingAggregate>,
        IValidateCommands<ActivateFilling, FillingAggregate>,
        IValidateCommands<WithdrawFilling, FillingAggregate>,
        IValidateCommands<WithdrawFillingForDate, FillingAggregate>,
        IValidateCommands<ChangeNameOfFilling, FillingAggregate>,
        IValidateCommands<AllowBreadWithFilling, FillingAggregate>,
        IValidateCommands<AllowMenuOptionsWithFilling, FillingAggregate>,
        IValidateCommands<DisallowBreadWithFilling, FillingAggregate>,
        IValidateCommands<DisallowMenuOptionsWithFilling, FillingAggregate>,
        IValidateCommands<UpdateFilling, FillingAggregate>
    {
        private readonly IFillingRepository _fillingRepository;

        public FillingCommandValidator(ICalendarRepository calendarRepository, IFillingRepository fillingRepository)
            : base(calendarRepository)
        {
            _fillingRepository = fillingRepository;
        }

        async Task<ValidationStatus> IValidateCommands<CreateFilling, FillingAggregate>.IsValidAsync(CreateFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
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
            if ((await _fillingRepository.GetFillingAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"A Filling for that Id already exists; Id =[{command.Id}]."));
            }
            return returnStatus;
        }

        async Task<ValidationStatus> IValidateCommands<WithdrawFillingForDate, FillingAggregate>.IsValidAsync(WithdrawFillingForDate command, FillingAggregate aggregate, CancellationToken cancellationToken)
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

        async Task<ValidationStatus> IValidateCommands<ActivateFillingForDate, FillingAggregate>.IsValidAsync(ActivateFillingForDate command, FillingAggregate aggregate, CancellationToken cancellationToken)
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

        Task<ValidationStatus> IValidateCommands<ActivateFilling, FillingAggregate>.IsValidAsync(ActivateFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<WithdrawFilling, FillingAggregate>.IsValidAsync(WithdrawFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<ChangeNameOfFilling, FillingAggregate>.IsValidAsync(ChangeNameOfFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = CommonValidation(command, aggregate, cancellationToken);
            if (returnStatus.IsValid
                && string.IsNullOrWhiteSpace(command.NewName))
            {
                returnStatus.Reasons.Add(AggregateNameInvalid);
            }
            return Task.FromResult(returnStatus);
        }

        Task<ValidationStatus> IValidateCommands<AllowBreadWithFilling, FillingAggregate>.IsValidAsync(AllowBreadWithFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<AllowMenuOptionsWithFilling, FillingAggregate>.IsValidAsync(AllowMenuOptionsWithFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<DisallowMenuOptionsWithFilling, FillingAggregate>.IsValidAsync(DisallowMenuOptionsWithFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<DisallowBreadWithFilling, FillingAggregate>.IsValidAsync(DisallowBreadWithFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<UpdateFilling, FillingAggregate>.IsValidAsync(UpdateFilling command, FillingAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }
    }
}
