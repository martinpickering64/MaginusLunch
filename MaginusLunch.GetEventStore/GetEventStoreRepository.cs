using MaginusLunch.Core.EventStore;
using System;
using MaginusLunch.Core.Aggregates;
using System.Collections.Generic;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace MaginusLunch.GetEventStore
{
    public class GetEventStoreRepository : IRepository
    {
        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        //private readonly Func<object, Task> _publishOnTheBus;
        private readonly Func<Type, Guid, string> _aggregateIdToStreamName;
        private readonly IConstructAggregates _aggregateFactory;
        private readonly IEventStoreConnection _eventStoreConnection;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            // TypeNameHandling should be used with caution when the application 
            // deserializes JSON from an external source. Incoming types should be 
            // validated with a custom SerializationBinder when deserializing with 
            // a value other than 'None'.
            TypeNameHandling = TypeNameHandling.None
        };

        /// <summary>
        /// Build the GetEvetStore Repository
        /// </summary>
        /// <param name="eventStoreConnection">the GetEventStore to use</param>
        /// <param name="aggregateFactory">The strategy for building Aggregate Roots</param>
        /// <param name="publishOnTheBus">The strategy for publishing Events to Subscribers (via a ServiceBus)</param>
        public GetEventStoreRepository(IEventStoreConnection eventStoreConnection, 
            IConstructAggregates aggregateFactory)
            //,Func<object, Task> publishOnTheBus)
        {
            _eventStoreConnection = eventStoreConnection;
            _aggregateIdToStreamName = (type, guid) => 
                    $"{char.ToLowerInvariant(type.Name[0])}{type.Name.Substring(1)}-{guid.ToString("N")}";
            _aggregateFactory = aggregateFactory;
            //_publishOnTheBus = publishOnTheBus;
        }

        public Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return GetByIdAsync<TAggregate>(id, long.MaxValue);
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return GetById<TAggregate>(id, long.MaxValue);
        }

        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id, long version) where TAggregate : class, IAggregate
        {
            if (version <= 0)
            {
                throw new InvalidOperationException("Cannot get version <= 0");
            }
            var streamName = _aggregateIdToStreamName(typeof(TAggregate), id);
            var aggregate = ConstructAggregate<TAggregate>(id);
            long sliceStart = 0;
            StreamEventsSlice currentSlice;
            do
            {
                var sliceCount = sliceStart + ReadPageSize <= version
                                    ? ReadPageSize
                                    : (int)(version - sliceStart + 1);
                currentSlice = await _eventStoreConnection.ReadStreamEventsForwardAsync(
                    streamName, sliceStart, sliceCount, false)
                    .ConfigureAwait(false);
                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                {
                    throw new AggregateNotFoundException(id, typeof(TAggregate));
                }
                if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                {
                    throw new AggregateDeletedException(id, typeof(TAggregate));
                }
                sliceStart = currentSlice.NextEventNumber;
                foreach (var evnt in currentSlice.Events)
                {
                    var realEvent = DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data);
                    aggregate.ApplyEvent(realEvent);
                }
            } while (version >= currentSlice.NextEventNumber
                                && !currentSlice.IsEndOfStream);

            if (aggregate.Version != version && version < Int32.MaxValue)
            {
                throw new AggregateVersionException(id, typeof(TAggregate), aggregate.Version, version);
            }
            return aggregate;
        }

        public TAggregate GetById<TAggregate>(Guid id, long version) where TAggregate : class, IAggregate
        {
            return GetByIdAsync<TAggregate>(id, version)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            SaveAsync(aggregate, commitId, updateHeaders).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task SaveAsync(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, commitId},
                {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };
            updateHeaders(commitHeaders);
            var streamName = _aggregateIdToStreamName(aggregate.GetType(), aggregate.Id);
            var newEvents = aggregate.GetUncommittedEvents().Cast<object>().ToList();
            var originalVersion = aggregate.Version - newEvents.Count - 1;
            var expectedVersion = originalVersion < 0 ? ExpectedVersion.NoStream : originalVersion;
            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e, commitHeaders)).ToList();
            if (eventsToSave.Count < WritePageSize)
            {
                await _eventStoreConnection
                        .AppendToStreamAsync(streamName, expectedVersion, eventsToSave)
                        .ConfigureAwait(false);
            }
            else
            {
                using (var transaction = await _eventStoreConnection
                    .StartTransactionAsync(streamName, expectedVersion)
                    .ConfigureAwait(false))
                {
                    var position = 0;
                    while (position < eventsToSave.Count)
                    {
                        var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);
                        await transaction.WriteAsync(pageEvents).ConfigureAwait(false);
                        position += WritePageSize;
                    }
                    await transaction.CommitAsync().ConfigureAwait(false);
                }
            }
            //if (newEvents.Count > 1)
            //{
            //    var tasks = new List<Task>();
            //    foreach (var eventToPublish in newEvents)
            //    {
            //        tasks.Add(_publishOnTheBus(eventToPublish));
            //    }
            //    await Task.WhenAll(tasks).ConfigureAwait(false);
            //}
            //else if (newEvents.Count == 1)
            //{ 
            //    await _publishOnTheBus(newEvents[0]);
            //}
            aggregate.ClearUncommittedEvents();
        }

        /// <summary>
        /// Using the supplied strategy, build an Aggregate Root
        /// </summary>
        /// <typeparam name="TAggregate">The type of aggregate we are after</typeparam>
        /// <param name="id">the identity to apply to the aggregate instance</param>
        /// <returns>the aggregate instance</returns>
        private TAggregate ConstructAggregate<TAggregate>(Guid id)
        {
            return (TAggregate)_aggregateFactory.Build(typeof(TAggregate), id);
        }

        private static object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }

        private static EventData ToEventData(Guid eventId, object evnt, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));
            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {
                    EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName
                }
            };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = evnt.GetType().Name;
            return new EventData(eventId, typeName, true, data, metadata);
        }

        #region IDisposable Support

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
