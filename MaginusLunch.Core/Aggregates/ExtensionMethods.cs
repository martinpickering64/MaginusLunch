namespace MaginusLunch.Core.Aggregates
{
    internal static class ExtensionMethods
    {
        public static void ThrowHandlerNotFound(this IAggregate aggregate, object eventMessage)
        {
            throw new HandlerForDomainEventNotFoundException(
                $"Aggregate of type '{aggregate.GetType().Name}' raised an event of type '{eventMessage.GetType().Name}' but not handler could be found to handle the message.");
        }
    }
}
