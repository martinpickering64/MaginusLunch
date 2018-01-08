using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public class CalendarCommandValidator : MenuCommandValidatorBase,
        IValidateCommands<CreateCalendar, CalendarAggregate>,
        IValidateCommands<ActivateCalendarForDate, CalendarAggregate>,
        IValidateCommands<CloseDayOnCalendar, CalendarAggregate>,
        IValidateCommands<OpenDayOnCalendar, CalendarAggregate>,
        IValidateCommands<WithdrawCalendarForDate, CalendarAggregate>
    {

        public CalendarCommandValidator(ICalendarRepository calendarRepository)
            : base(calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        async Task<ValidationStatus> IValidateCommands<CreateCalendar, CalendarAggregate>.IsValidAsync(CreateCalendar command, CalendarAggregate aggregate, CancellationToken cancellationToken)
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
            if (!returnStatus.IsValid)
            {
                return returnStatus;
            }
            if ((await _calendarRepository.GetCalendarAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"A Calendar for that Id already exists; Id =[{command.Id}]."));
            }
            return returnStatus;
        }

        async Task<ValidationStatus> IValidateCommands<WithdrawCalendarForDate, CalendarAggregate>.IsValidAsync(WithdrawCalendarForDate command, CalendarAggregate aggregate, CancellationToken cancellationToken)
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

        Task<ValidationStatus> IValidateCommands<OpenDayOnCalendar, CalendarAggregate>.IsValidAsync(OpenDayOnCalendar command, CalendarAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        Task<ValidationStatus> IValidateCommands<CloseDayOnCalendar, CalendarAggregate>.IsValidAsync(CloseDayOnCalendar command, CalendarAggregate aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommonValidation(command, aggregate, cancellationToken));
        }

        async Task<ValidationStatus> IValidateCommands<ActivateCalendarForDate, CalendarAggregate>.IsValidAsync(ActivateCalendarForDate command, CalendarAggregate aggregate, CancellationToken cancellationToken)
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
    }
}
