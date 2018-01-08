namespace MaginusLunch.Core.Validation
{
    public abstract class CommandValidatorBase
    {
        public const int CalendarMissingCode = 5;
        public const int WihtdrawlDateTooOldCode = 6;
        public const int AggregateRootIdInvalidCode = 10;
        public const int AggregateVersionInvalidCode = 11;
        public const int AggregateWithIdExistsCode = 12;
        public const int AggregateWithIdDoesNotExistCode = 14;
        public const int RouteDataFailsToValidateCode = 15;
        public const int AggregateNameInvalidCode = 16;
        public const int UnknownCommandCode = 403;

        public static ValidationStatus.Reason CalendarMissing
            = new ValidationStatus.Reason(CalendarMissingCode, "Failed to find or retrieve any Calendar from the Repositories.");
        public static ValidationStatus.Reason WihtdrawlDateTooOld
            = new ValidationStatus.Reason(WihtdrawlDateTooOldCode, "The date specified is older than the Open Order Date; acting on this command would be ineffectual.");
        public static ValidationStatus.Reason RouteDataFailsToValidate 
            = new ValidationStatus.Reason(RouteDataFailsToValidateCode, "The Route Data for the Request fails to validate.");
        public static ValidationStatus.Reason AggregateNameInvalid
            = new ValidationStatus.Reason(AggregateNameInvalidCode, "The Name property of the Aggregate Root is invalid.");
        public static ValidationStatus.Reason UnkownCommand
            = new ValidationStatus.Reason(UnknownCommandCode, "Unknown Command received!");
    }
}
