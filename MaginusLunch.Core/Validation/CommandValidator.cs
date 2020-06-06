namespace MaginusLunch.Core.Validation
{
    public abstract class CommandValidator
    {
        public const int AggregateIdInvalidCode = 10;
        public const int AggregateVersionInvalidCode = 11;
        public const int AggregateWithIdExistsCode = 12;
        public const int AggregateWithIdDoesNotExistCode = 404;
        public const int RouteDataFailsToValidateCode = 15;
        public const int AggregateNameInvalidCode = 16;
        public const int UnknownCommandCode = 403;

        public static ValidationStatus.Reason AggregateIdInvalid
            = new ValidationStatus.Reason(AggregateIdInvalidCode, "The Id of the Aggregate Root is invalid.");
        public static ValidationStatus.Reason AggregateVersionInvalid
            = new ValidationStatus.Reason(AggregateVersionInvalidCode, "The Version of the Aggregate Root is invalid.");
        public static ValidationStatus.Reason AggregateWithIdExists
            = new ValidationStatus.Reason(AggregateWithIdExistsCode, "An Aggregate Root with this Id already exists.");
        public static ValidationStatus.Reason AggregateWithIdDoesNotExist
            = new ValidationStatus.Reason(AggregateWithIdDoesNotExistCode, "An Aggregate Root with this Id does not exist.");
        public static ValidationStatus.Reason RouteDataFailsToValidate
            = new ValidationStatus.Reason(RouteDataFailsToValidateCode, "The Route Data for the Request fails to validate.");
        public static ValidationStatus.Reason AggregateNameInvalid
            = new ValidationStatus.Reason(AggregateNameInvalidCode, "The Name property of the Aggregate Root is invalid.");
        public static ValidationStatus.Reason UnkownCommand
            = new ValidationStatus.Reason(UnknownCommandCode, "An unknown Command has been received.");
    }
}
