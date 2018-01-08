using MaginusLunch.Core.Aggregates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaginusLunch.Core.EventStore
{
    /// <summary>
    /// The persistence layer for an Event Store
    /// </summary>
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Obtain the most current revision of an Aggregate Root identified by the given Id
        /// from the Event Store.
        /// </summary>
        /// <typeparam name="TAggregate">The type of Aggregate Root targetted by the Repository instance.</typeparam>
        /// <param name="id">The identity of the Aggregate Root instance.</param>
        /// <returns>The Aggregate Root at its most current version.</returns>
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate;
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : class, IAggregate;

        /// <summary>
        /// Obtain an Aggregate Root identified by the given Id
        /// from the Event Store. The version of the Aggregate Root should equal 
        /// the given version.
        /// </summary>
        /// <typeparam name="TAggregate">The type of Aggregate Root targetted by the Repository instance.</typeparam>
        /// <param name="id">The identity of the Aggregate Root instance.</param>
        /// <param name="version">The targetted version of the resultant Aggregate.</param>
        /// <returns>The Aggregate Root.</returns>
        TAggregate GetById<TAggregate>(Guid id, long version) where TAggregate : class, IAggregate;
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id, long version) where TAggregate : class, IAggregate;

        /// <summary>
        /// Add the Aggregate Root's list of uncommitted events to the Aggregate Root's Event Store/stream
        /// </summary>
        /// <param name="aggregate">The Aggregate Root.</param>
        /// <param name="commitId">An identity for this commit to the Event Store.</param>
        /// <param name="updateHeaders">Any additional Headers/Metadata to save as well.</param>
        void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders);
        Task SaveAsync(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders);
    }
}
