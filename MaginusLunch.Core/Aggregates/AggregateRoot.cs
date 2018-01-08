using System;
using System.Collections.Generic;

namespace MaginusLunch.Core.Aggregates
{
    public abstract class AggregateRoot : IAggregate, IEquatable<IAggregate>
    {
        private readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();

        private IRouteEvents _registeredRoutes;

        protected AggregateRoot()
        {
            Version = -100;
        }

        protected AggregateRoot(IRouteEvents handler) : this()
        {
            if (handler == null) { return; }
            RegisteredRoutes = handler;
            RegisteredRoutes.Register(this);
        }

        /// <remarks>
        /// Will always fall back to using the ConventionBasedRouter if no IEventRouter set-up.
        /// </remarks>
        protected IRouteEvents RegisteredRoutes
        {
            get
            {
                return _registeredRoutes ?? (_registeredRoutes = new ConventionBasedEventRouter(this));
            }
            set
            {
                _registeredRoutes = value; 
            }
        }

        public Guid Id { get; protected set; }

        public long Version { get; protected set; }

        public void ApplyEvent(object @event)
        {
            RegisteredRoutes.Dispatch(@event);
            Version++;
        }

        public ICollection<object> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public virtual bool Equals(IAggregate other)
        {
            return null != other && other.Id == Id;
        }

        protected void RaiseEvent(object anEvent)
        {
            ApplyEvent(anEvent);
            _uncommittedEvents.Add(anEvent);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }
    }
}
