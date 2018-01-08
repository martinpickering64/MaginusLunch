using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public class BreadCommandValidator : MenuCommandValidatorBase,
        IValidateCommands<CreateBread, BreadAggregate>,
        IValidateCommands<ActivateBreadForDate, BreadAggregate>,
        IValidateCommands<ActivateBread, BreadAggregate>,
        IValidateCommands<WithdrawBread, BreadAggregate>,
        IValidateCommands<WithdrawBreadForDate, BreadAggregate>,
        IValidateCommands<ChangeNameOfBread, BreadAggregate>
    {
        private readonly IBreadRepository _breadRepository;

        public BreadCommandValidator(ICalendarRepository calendarRepository, IBreadRepository breadRepository)
            : base(calendarRepository)
        {
            _breadRepository = breadRepository;
        }

        async Task<ValidationStatus> IValidateCommands<CreateBread, BreadAggregate>.IsValidAsync(CreateBread command, BreadAggregate aggregate, CancellationToken cancellationToken)
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
            if ((await _breadRepository.GetBreadAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"A Bread for that Id already exists; Id =[{command.Id}]."));
            }
            return returnStatus;
        }

        async Task<ValidationStatus> IValidateCommands<WithdrawBreadForDate, BreadAggregate>.IsValidAsync(WithdrawBreadForDate command, BreadAggregate aggregate, CancellationToken cancellationToken)
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

        async Task<ValidationStatus> IValidateCommands<ActivateBreadForDate, BreadAggregate>.IsValidAsync(ActivateBreadForDate command, BreadAggregate aggregate, CancellationToken cancellationToken)
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

        Task<ValidationStatus> IValidateCommands<ActivateBread, BreadAggregate>.IsValidAsync(ActivateBread command, BreadAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<WithdrawBread, BreadAggregate>.IsValidAsync(WithdrawBread command, BreadAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<ChangeNameOfBread, BreadAggregate>.IsValidAsync(ChangeNameOfBread command, BreadAggregate aggregate, CancellationToken cancellationToken)
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
