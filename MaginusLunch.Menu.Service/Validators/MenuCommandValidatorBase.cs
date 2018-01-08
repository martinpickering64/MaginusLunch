using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Validation;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service.Validators
{
    public abstract class MenuCommandValidatorBase : CommandValidatorBase
    {
        public const int EditorUserIdNullCode = 201;

        public static ValidationStatus.Reason EditorUserIdNull 
            = new ValidationStatus.Reason(EditorUserIdNullCode,
                    "All Menu Commands must have an EditorUserId value specified.");

        protected ICalendarRepository _calendarRepository;

        protected MenuCommandValidatorBase(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        public ValidationStatus CommonValidation(MenuCommand command, AggregateRoot aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = new ValidationStatus();
            if (aggregate == null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdDoesNotExistCode,
                    $"An {aggregate.GetType().Name} for that Id does not exist; Id =[{command.Id}]."));
            }
            else if (command.Id != aggregate.Id)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateRootIdInvalidCode,
                    $"The Id for the {command.GetType().Name} Command ({command.Id}) does not match with  Id for the {aggregate.GetType().Name} ({aggregate.Id}). Contact Support!"));
            }
            return returnStatus;
        }

        public async Task<ValidationStatus.Reason> WithdrawlDateValidation(DateTime date, CancellationToken cancellationToken)
        {
            var calendar = await _calendarRepository.GetCalendarAsync(cancellationToken).ConfigureAwait(false);
            if (calendar == null)
            {
                return CalendarMissing;
            }
            if (date.Kind != DateTimeKind.Utc)
            {
                date = date.ToUniversalTime();
            }
            if (date.Date < calendar.OrdersStillOpenBeyond.Date)
            {
                return WihtdrawlDateTooOld;
            }
            return null;
        }
    }
}
