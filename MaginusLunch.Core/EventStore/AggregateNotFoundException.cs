using System;
using System.Runtime.Serialization;

namespace MaginusLunch.Core.EventStore
{
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
        public readonly Guid Id;
        public readonly Type Type;

        public AggregateNotFoundException(Guid id, Type type)
            : this($"Aggregate '{id}' (type {type.Name}) was not found.")
        {
            Id = id;
            Type = type;
        }

        #region Required for compatibility - not to be used
        protected AggregateNotFoundException()
        {
        }

        protected AggregateNotFoundException(string message) : base(message)
        {
        }

        protected AggregateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AggregateNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }
        #endregion
    }
}
