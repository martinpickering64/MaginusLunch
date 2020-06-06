using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MaginusLunch.Core.Aggregates
{
    public class ConventionBasedEventRouter : IRouteEvents
    {
        private readonly IDictionary<Type, Action<Events.Event>> _handlers 
            = new Dictionary<Type, Action<Events.Event>>();

        private readonly bool _throwOnApplyNotFound;

        private IAggregate _registered;

        public ConventionBasedEventRouter()
            : this (true)
        { }

        public ConventionBasedEventRouter(bool throwOnApplyNotFound) 
            => _throwOnApplyNotFound = throwOnApplyNotFound;

        public virtual void Register(IAggregate aggregate)
        {
            _registered = aggregate ?? throw new ArgumentNullException("aggregate");
            foreach (var apply in GetInstanceMethods(aggregate, "Apply"))
            {
                _handlers.Add(apply.EventType, m => apply.Method.Invoke(aggregate, new[] { m }));
            }
        }

        public virtual void Dispatch(Events.Event anEvent)
        {
            if (anEvent == null) { throw new ArgumentNullException("anEvent"); }
            if (_handlers.TryGetValue(anEvent.GetType(), out Action<Events.Event> handler))
            {
                handler(anEvent);
            }
            else if (_throwOnApplyNotFound)
            {
                _registered.ThrowHandlerNotFound(anEvent);
            }
        }

        /// <summary>
        /// Get instance methods that are named Apply with one 
        /// parameter and return void. This is the 
        /// convention followed by this Router.
        /// </summary>
        private IEnumerable<MethodData> GetInstanceMethods(IAggregate aggregate, string template) 
            => aggregate.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == template
                        && m.GetParameters().Length == 1
                        && m.ReturnParameter.ParameterType == typeof(void))
                .Select(m => new MethodData
                {
                    Method = m,
                    EventType = m.GetParameters().Single().ParameterType
                });

        private class MethodData
        {
            public MethodInfo Method { get; set; }
            public Type EventType { get; set; }
        }
    }
}
