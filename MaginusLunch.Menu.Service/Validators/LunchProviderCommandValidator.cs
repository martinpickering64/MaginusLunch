using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public class LunchProviderCommandValidator : MenuCommandValidatorBase,
        IValidateCommands<CreateLunchProvider, LunchProviderAggregate>,
        IValidateCommands<ActivateLunchProviderForDate, LunchProviderAggregate>,
        IValidateCommands<ActivateLunchProvider, LunchProviderAggregate>,
        IValidateCommands<WithdrawLunchProvider, LunchProviderAggregate>,
        IValidateCommands<WithdrawLunchProviderForDate, LunchProviderAggregate>,
        IValidateCommands<ChangeNameOfLunchProvider, LunchProviderAggregate>
    {
        private readonly ILunchProviderRepository _lunchProviderRepository;

        public LunchProviderCommandValidator(ICalendarRepository calendarRepository, ILunchProviderRepository lunchProviderRepository)
            : base(calendarRepository)
        {
            _lunchProviderRepository = lunchProviderRepository;
        }

        async Task<ValidationStatus> IValidateCommands<CreateLunchProvider, LunchProviderAggregate>.IsValidAsync(CreateLunchProvider command, LunchProviderAggregate aggregate, CancellationToken cancellationToken)
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
            if ((await _lunchProviderRepository.GetLunchProviderAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"A Lunch Provider for that Id already exists; Id =[{command.Id}]."));
            }
            return returnStatus;
        }

        async Task<ValidationStatus> IValidateCommands<WithdrawLunchProviderForDate, LunchProviderAggregate>.IsValidAsync(WithdrawLunchProviderForDate command, LunchProviderAggregate aggregate, CancellationToken cancellationToken)
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

        async Task<ValidationStatus> IValidateCommands<ActivateLunchProviderForDate, LunchProviderAggregate>.IsValidAsync(ActivateLunchProviderForDate command, LunchProviderAggregate aggregate, CancellationToken cancellationToken)
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

        Task<ValidationStatus> IValidateCommands<ActivateLunchProvider, LunchProviderAggregate>.IsValidAsync(ActivateLunchProvider command, LunchProviderAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<WithdrawLunchProvider, LunchProviderAggregate>.IsValidAsync(WithdrawLunchProvider command, LunchProviderAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<ChangeNameOfLunchProvider, LunchProviderAggregate>.IsValidAsync(ChangeNameOfLunchProvider command, LunchProviderAggregate aggregate, CancellationToken cancellationToken)
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
