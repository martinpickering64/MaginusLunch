using System;
using System.Runtime.Serialization;

namespace MaginusLunch.Core.EventStore
{
    [Serializable]
    public class AggregateVersionException : Exception
    {
        public readonly Guid Id;
        public readonly Type Type;
        public readonly long AggregateVersion;
        public readonly long RequestedVersion;

        public AggregateVersionException(Guid id, Type type, long aggregateVersion, long requestedVersion)
            : this($"Requested version {requestedVersion} of aggregate '{id}' (type {type.Name}) - aggregate version is {aggregateVersion}")
        {
            Id = id;
            Type = type;
            AggregateVersion = aggregateVersion;
            RequestedVersion = requestedVersion;
        }

        #region Required for compatibility - not to be used
        protected AggregateVersionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        protected AggregateVersionException()
        {
        }

        protected AggregateVersionException(string message) : base(message)
        {
        }

        protected AggregateVersionException(string message, Exception innerException) : base(message, innerException)
        {
        }
        #endregion
    }
}
