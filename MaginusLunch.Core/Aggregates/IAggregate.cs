using System;
using System.Collections;
using System.Collections.Generic;

namespace MaginusLunch.Core.Aggregates
{
    /// <summary>
    /// The public definition of a Class representing 
    /// an Aggregate in a DDD-CQRS application
    /// </summary>
    public interface IAggregate
    {
        /// <summary>
        /// The Identity of the Aggregate
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The current Version Number of the Aggregate's state.
        /// </summary>
        long Version { get; }

        /// <summary>
        /// Apply an Event to the Aggregate to transition it to a new state.
        /// </summary>
        /// <param name="anEvent"></param>
        void ApplyEvent(object anEvent);

        /// <summary>
        /// Access the list of uncommitted Events associated 
        /// with this Aggregate instance. An uncommitted event has 
        /// yet to be saved to the Event Store.
        /// </summary>
        /// <returns>The collection of uncommitted Events.</returns>
        ICollection<object> GetUncommittedEvents();

        /// <summary>
        /// Empty the list of uncommitted Events.
        /// </summary>
        void ClearUncommittedEvents();
    }
}
