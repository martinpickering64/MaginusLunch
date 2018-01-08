using EventStore.ClientAPI;
using MaginusLunch.Core.EventStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MaginusLunch.GetEventStore.IntegrationTests
{
    ///
    /// GetEventStore Projection for all Events on TestAggregate instances...
    /// 
    /// fromCategory("testAggregate").foreachStream().when({
    ///    $any: function(s, ev) {
    ///       linkTo("allTestAggregate-events", ev);
    ///    }
    /// });
    /// 

    [TestCategory("Integration Tests")]
    [TestClass]
    public class IRepositoryTests
    {
        [TestMethod]
        public void Cant_get_aggregate_with_version_less_than_zero()
        {
            var gesConnection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            var repository = new GetEventStoreRepository(eventStoreConnection: gesConnection,
                aggregateFactory: new DefaultAggregateFactory());

            Assert.ThrowsException<InvalidOperationException>(() => repository.GetById<TestAggregate>(Guid.NewGuid(), -1));
        }

        [TestMethod]
        public void When_stream_is_not_found()
        {
            var testAggregate = new TestAggregate(Guid.NewGuid());
            var gesConnection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            var repository = new GetEventStoreRepository(eventStoreConnection: gesConnection,
                aggregateFactory: new DefaultAggregateFactory());

            Assert.ThrowsException<InvalidOperationException>(() => repository.GetById<TestAggregate>(testAggregate.Id));
        }

        [TestMethod]
        public void When_an_new_aggregate_is_saved_a_new_stream_is_created()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var command = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(command);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            var streamName = StreamNameForAggregate(command.Id);
            try
            {
                var events = gesConnection
                    .ReadStreamEventsForwardAsync(streamName, 0, 100, false)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.AreEqual(SliceReadStatus.Success, events.Status);
                Assert.AreEqual(1, events.Events.Length);
            }
            finally
            {
                DeleteTestStream(gesConnection, command.Id);
            }

        }

        [TestMethod]
        public void When_an_aggregate_is_saved_its_uncommitted_events_are_committed()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var command = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(command);
            var numberOfUncommittedEvents = testAggregate.GetUncommittedEvents().Count;
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);
            try
            {
                Assert.AreNotEqual(numberOfUncommittedEvents, testAggregate.GetUncommittedEvents().Count);
                Assert.AreEqual(0, testAggregate.GetUncommittedEvents().Count);
            }
            finally
            {
                DeleteTestStream(gesConnection, command.Id);
            }

        }

        [TestMethod]
        public void When_an_new_aggregate_is_saved_eventData_uses_json_serialization()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var command = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(command);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            var streamName = StreamNameForAggregate(command.Id);
            try
            {
                var events = gesConnection
                    .ReadStreamEventsForwardAsync(streamName, 0, 100, false)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                var theEvent = events.Events[0].Event;
                Assert.IsTrue(theEvent.IsJson);
            }
            finally
            {
                DeleteTestStream(gesConnection, command.Id);
            }

        }

        [TestMethod]
        public void When_an_new_aggregate_is_saved_the_new_stream_contains_TestCreatedEvent()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var command = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(command);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            var streamName = StreamNameForAggregate(command.Id);
            try
            {
                var events = gesConnection
                    .ReadStreamEventsForwardAsync(streamName, 0, 100, false)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                var theEvent = events.Events[0].Event;
                Assert.AreEqual("TestCreatedEvent", theEvent.EventType);
                var eventAsObject = DeserializeEvent(theEvent.Metadata, theEvent.Data);
                Assert.IsInstanceOfType(eventAsObject, typeof(TestCreatedEvent));
            }
            finally
            {
                DeleteTestStream(gesConnection, command.Id);
            }

        }

        [TestMethod]
        public void When_an_new_aggregate_is_saved_the_TestCreatedEvent_has_the_correct_state()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var command = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(command);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            var streamName = StreamNameForAggregate(command.Id);
            try
            {
                var events = gesConnection
                    .ReadStreamEventsForwardAsync(streamName, 0, 100, false)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                var theEvent = events.Events[0].Event;
                var eventAsObject = DeserializeEvent(theEvent.Metadata, theEvent.Data);
                var realEvent = eventAsObject as TestCreatedEvent;
                Assert.AreEqual(command.Id, realEvent.Id);
                Assert.AreEqual(command.Name, realEvent.Name);
            }
            finally
            {
                DeleteTestStream(gesConnection, command.Id);
            }

        }

        [TestMethod]
        public void Can_get_a_newly_created_aggregate()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var command = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(command);
            var commitId = Guid.NewGuid();
            repository.Save(testAggregate, commitId);
            try
            {
                var retrievedAggregate = repository.GetById<TestAggregate>(command.Id);

                Assert.IsNotNull(retrievedAggregate);
                Assert.IsInstanceOfType(retrievedAggregate, typeof(TestAggregate));
                Assert.AreEqual(testAggregate.Id, retrievedAggregate.Id);
                Assert.AreEqual(testAggregate.Name, retrievedAggregate.Name);
                Assert.AreEqual(testAggregate, retrievedAggregate);
                Assert.AreEqual(1, retrievedAggregate.Version);
            }
            finally
            {
                DeleteTestStream(gesConnection, command.Id);
            }
        }

        [TestMethod]
        public void When_an_aggregate_has_multiple_uncommitted_events_they_are_all_saved()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var createCommand = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(createCommand);
            var renameCommand = new UpdateNameCommand
            {
                Id = testAggregate.Id,
                Name = "First Amendment"
            };
            testAggregate.UpdateName(renameCommand);
            renameCommand.Name = "Second Amendment";
            testAggregate.UpdateName(renameCommand);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            var streamName = StreamNameForAggregate(createCommand.Id);
            try
            {
                Assert.AreEqual(0, testAggregate.GetUncommittedEvents().Count);
                var events = gesConnection
                    .ReadStreamEventsForwardAsync(streamName, 0, 100, false)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.AreEqual(3, events.Events.Length);
                Assert.AreEqual("TestCreatedEvent", events.Events[0].Event.EventType);
                Assert.AreEqual("NameUpdatedEvent", events.Events[1].Event.EventType);
                Assert.AreEqual("NameUpdatedEvent", events.Events[2].Event.EventType);
            }
            finally
            {
                DeleteTestStream(gesConnection, createCommand.Id, 2);
            }

        }

        [TestMethod]
        public void When_an_aggregate_has_multiple_events_it_is_restored_back_to_the_expected_state()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var createCommand = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(createCommand);
            var renameCommand = new UpdateNameCommand
            {
                Id = testAggregate.Id,
                Name = "First Amendment"
            };
            testAggregate.UpdateName(renameCommand);
            renameCommand.Name = "Second Amendment";
            testAggregate.UpdateName(renameCommand);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            try
            {
                var retrievedAggregate = repository.GetById<TestAggregate>(testAggregate.Id);

                Assert.AreEqual(testAggregate.Id, retrievedAggregate.Id);
                Assert.AreEqual(testAggregate.Name, retrievedAggregate.Name);
                Assert.AreEqual(testAggregate, retrievedAggregate);
                Assert.AreEqual(3, retrievedAggregate.Version);
            }
            finally
            {
                DeleteTestStream(gesConnection, createCommand.Id, 2);
            }

        }

        [TestMethod]
        public void When_an_aggregate_has_multiple_uncommitted_events_they_are_all_saved_with_the_same_commitId()
        {
            var gesConnection = CreateAndOpenConnection(GetSettings());
            var repository = GetRepository(gesConnection);
            var createCommand = new CreateTestCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };
            var testAggregate = new TestAggregate(createCommand);
            var renameCommand = new UpdateNameCommand
            {
                Id = testAggregate.Id,
                Name = "First Amendment"
            };
            testAggregate.UpdateName(renameCommand);
            renameCommand.Name = "Second Amendment";
            testAggregate.UpdateName(renameCommand);
            var commitId = Guid.NewGuid();

            repository.Save(testAggregate, commitId);

            var streamName = StreamNameForAggregate(createCommand.Id);
            try
            {
                var events = gesConnection
                    .ReadStreamEventsForwardAsync(streamName, 0, 100, false)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.IsTrue(events.Events.Any(e => GetCommitId(e.OriginalEvent.Metadata) == commitId.ToString()), "An event was not stored using the expected CommitId Header.");
            }
            finally
            {
                DeleteTestStream(gesConnection, createCommand.Id, 2);
            }

        }

        private string GetCommitId(byte[] metadata)
        {
            const string CommitIdHeader = "CommitId";
            JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };
            var commitId = (string)(JObject.Parse(Encoding.UTF8.GetString(metadata))
                                                    .Property(CommitIdHeader).Value);
            return commitId;
        }

        private object DeserializeEvent(byte[] metadata, byte[] data)
        {
            const string EventClrTypeHeader = "EventClrTypeName";
            JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata))
                                                    .Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data),
                                                    Type.GetType((string)eventClrTypeName));
        }

        private ConnectionSettings GetSettings()
        {
            return ConnectionSettings.Create()
                .UseDebugLogger()
                .EnableVerboseLogging()
                .FailOnNoServerResponse()
                .LimitAttemptsForOperationTo(1)
                .SetDefaultUserCredentials(
                    userCredentials: new EventStore.ClientAPI.SystemData.UserCredentials("admin", "changeit"));
        }

        private IEventStoreConnection CreateAndOpenConnection(ConnectionSettings settings)
        {
            var gesConnection = EventStoreConnection.Create(
                                    settings,
                                    new IPEndPoint(IPAddress.Loopback, 1113),
                                    connectionName: "Integration Tests");
            gesConnection.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return gesConnection;
        }

        private IRepository GetRepository(IEventStoreConnection gesConnection)
        {
            return new GetEventStoreRepository(eventStoreConnection: gesConnection,
                aggregateFactory: new DefaultAggregateFactory());
        }

        private string StreamNameForAggregate(Guid aggregateId)
        {
            return $"testAggregate-{aggregateId.ToString("N")}";
        }

        private void DeleteTestStream(IEventStoreConnection gesConnection, Guid aggregateId, long expectedVersion = 0)
        {
            var streamName = StreamNameForAggregate(aggregateId);
            //var deleteResult = gesConnection.DeleteStreamAsync(streamName, expectedVersion, true, null)
            //    .ConfigureAwait(false).GetAwaiter().GetResult();
            Debug.Print($"Stream {streamName} deleted.");
        }
    }
}
