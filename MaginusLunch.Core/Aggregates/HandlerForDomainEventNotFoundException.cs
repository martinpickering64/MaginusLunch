using System;
using System.Runtime.Serialization;

namespace MaginusLunch.Core.Aggregates
{
    [Serializable]
    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException()
        { }

        public HandlerForDomainEventNotFoundException(string message)
            : base(message)
        { }

        public HandlerForDomainEventNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected HandlerForDomainEventNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
