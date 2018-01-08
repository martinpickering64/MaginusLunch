using System;
using System.Runtime.Serialization;

namespace MaginusLunch.Core.Aggregates
{
    [Serializable]
    public class AggregateDeletedException : Exception
    {
        public readonly Guid Id;
        public readonly Type Type;

        public AggregateDeletedException(Guid id, Type type)
            : this($"Aggregate '{id}' (type {type.Name}) was deleted.")
        {
            Id = id;
            Type = type;
        }

        #region Required for compatibility - not to be used
        protected AggregateDeletedException()
        {
        }

        protected AggregateDeletedException(string message) : base(message)
        {
        }

        protected AggregateDeletedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AggregateDeletedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }
        #endregion
    }
}
