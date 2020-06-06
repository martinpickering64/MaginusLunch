using System;
using System.Collections.Generic;

namespace MaginusLunch.Core.Aggregates
{
    public abstract class AggregateRoot : IAggregate, IEquatable<IAggregate>
    {
        public const long NO_VERSION_YET = -100;
        private readonly ICollection<Events.Event> _uncommittedEvents 
            = new LinkedList<Events.Event>();

        protected AggregateRoot() => Version = NO_VERSION_YET;

        /// <remarks>
        /// Will always fall back to using the ConventionBasedRouter 
        /// if no IRouteEvents handler specified.
        /// </remarks>
        protected AggregateRoot(IRouteEvents handler) : this()
        {
            RegisteredRoutes = handler ?? new ConventionBasedEventRouter();
            RegisteredRoutes.Register(this);
        }

        protected IRouteEvents RegisteredRoutes { get; set; }

        public Guid Id { get; protected set; }

        public long Version { get; protected set; }

        public void ApplyEvent(Events.Event anEvent)
        {
            RegisteredRoutes.Dispatch(anEvent);
            Version++;
        }

        public IEnumerable<Events.Event> UncommittedEvents 
            => _uncommittedEvents;

        public void ClearUncommittedEvents() 
            => _uncommittedEvents.Clear();

        protected void RaiseEvent(Events.Event anEvent)
        {
            ApplyEvent(anEvent);
            _uncommittedEvents.Add(anEvent);
        }


        /// <remarks>
        /// Caution: two instances of a given Aggregate Root (same Id)
        /// but at different Versions will generate the same HashCode.
        /// </remarks>
        public override int GetHashCode() => Id.GetHashCode();

        /// <remarks>
        /// Implementation Note: An Aggregate Root is only equal to another
        /// if their respective Id AND Version properties match.
        /// </remarks>
        public virtual bool Equals(IAggregate other) => null != other
                    && other.Id == Id
                    && other.Version == Version;

        public override bool Equals(object obj) => Equals(obj as IAggregate);

        public static bool operator ==(AggregateRoot leftAggregate, IAggregate rightAggregate) 
            => leftAggregate is object && leftAggregate.Equals(rightAggregate);

        public static bool operator !=(AggregateRoot leftAggregate, IAggregate rightAggregate) 
            => leftAggregate is null || !(leftAggregate.Equals(rightAggregate));
    }
}
