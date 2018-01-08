using EventStore.ClientAPI;
using MaginusLunch.Core.EventStore;
using MaginusLunch.Core.ServiceManagement;
using MaginusLunch.GetEventStore;
using MaginusLunch.Logging;
using MaginusLunch.Logging.MicrosoftLogging;
using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace MaginusLunch.Orders.Denormalizer.Console
{
    public static class Program
    {
        public const string AppSettingsJsonFilename = "appsettings.json";
        public static IConfigurationRoot Configuration { get; set; }

        private static ILog _logger;

        public static void Main(string[] args)
        {
            System.Console.WriteLine("Maginus Lunch Orders Denormalizer.");
            Configure();
            if (Start())
            {
                System.Console.WriteLine("Press 'S' to stop me...");
                char keyPress = ' ';
                while (true)
                {
                    keyPress = System.Console.ReadKey().KeyChar;
                    if (keyPress.ToString().Equals("S", StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }
                }
                Stop();
                return;
            }
            System.Console.WriteLine("FATAL: failed to start the process.");
        }

        public static DaemonServiceBase TheTask;
        public static DenormalizerServiceSettings ServiceSettings;
        public static MongoRepositorySettings ReadRepositorySettings;

        public static void Configure()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(AppSettingsJsonFilename, optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            var msLoggerFactory = new Microsoft.Extensions.Logging.LoggerFactory()
                                        .AddConsole(Configuration.GetSection("Logging"))
                                        .AddDebug(); ;
            MicrosoftLoggerFactory.MSLoggerFactory = msLoggerFactory;
            LogManager.Use<MicrosoftLoggerFactory>();
            _logger = LogManager.GetLogger(typeof(Program));

            ServiceSettings = new DenormalizerServiceSettings();
            Configuration.GetSection(DenormalizerServiceSettings.DefaultSectionName)
                .Bind(ServiceSettings);
            ReadRepositorySettings = new MongoRepositorySettings();
            Configuration.GetSection(MongoRepositorySettings.DefaultMongoRepositorySectionName)
                .Bind(ReadRepositorySettings);

            #region Repositories

            var retryPolicy = Policy.Handle<MongoConnectionException>(i =>
                                i.InnerException.GetType() == typeof(IOException)
                                || i.InnerException.GetType() == typeof(SocketException))
                            .Retry(3);
            var asyncRetryPolicy = Policy.Handle<MongoConnectionException>(i =>
                        i.InnerException.GetType() == typeof(IOException)
                        || i.InnerException.GetType() == typeof(SocketException))
                    .RetryAsync(3);
            var lunchProvidersRepository = new MongoLunchProviderRepository(
                ReadRepositorySettings,
                asyncRetryPolicy,
                retryPolicy);
            var fillingRepository = new MongoFillingRepository(
                ReadRepositorySettings,
                asyncRetryPolicy,
                retryPolicy);
            var breadRepository = new MongoBreadRepository(
                ReadRepositorySettings,
                asyncRetryPolicy,
                retryPolicy);
            var menuOptionRepository = new MongoMenuOptionRepository(
                ReadRepositorySettings,
                asyncRetryPolicy,
                retryPolicy);
            var orderRepository = new MongoOrderRepository(
                ReadRepositorySettings,
                asyncRetryPolicy,
                retryPolicy);
            var calendarRepository = new MongoCalendarRepository(
                ReadRepositorySettings,
                asyncRetryPolicy,
                retryPolicy);
            var eventStore = CreateEventStoreConnection();
            var eventStoreRepository = CreateEventStoreRepository(eventStore);

            #endregion Repositories

            var eventHandlers = new OrderEventHandlers(
                orderRepository,
                lunchProvidersRepository,
                fillingRepository,
                breadRepository,
                menuOptionRepository,
                calendarRepository,
                eventStoreRepository);
            TheTask = new DenormalizerService(eventHandlers, eventStore, ServiceSettings);
        }

        public static IRepository CreateEventStoreRepository(IEventStoreConnection gesConnection)
        {
            var repository = new GetEventStoreRepository(eventStoreConnection: gesConnection,
                                    aggregateFactory: new DefaultAggregateFactory());
            return repository;
        }
        public static IEventStoreConnection CreateEventStoreConnection()
        {
            var settings = new GetEventStoreRepositorySettings();
            Configuration.GetSection(GetEventStoreRepositorySettings.DefaultGetEventStoreRepositorySectionName)
                .Bind(settings);
            _logger.DebugFormat("ConfigurationSettings bound to Section [{0}].", GetEventStoreRepositorySettings.DefaultGetEventStoreRepositorySectionName);
            var connectionSettings = EventStore.ClientAPI.ConnectionSettings.Create();
            if (settings.UseDebugLogger)
            {
                connectionSettings.UseDebugLogger();
                _logger.Debug("UseDebugLogger");
            }
            if (settings.EnableVerboseLogging)
            {
                connectionSettings.EnableVerboseLogging();
                _logger.Debug("EnableVerboseLogging");
            }
            if (!settings.DoNotFailOnNoServerResponse)
            {
                connectionSettings.FailOnNoServerResponse();
                _logger.Debug("FailOnNoServerResponse");
            }
            if (settings.LimitAttemptsForOperationTo > 0)
            {
                connectionSettings.LimitAttemptsForOperationTo(settings.LimitAttemptsForOperationTo);
                _logger.DebugFormat("LimitAttemptsForOperationTo [{0}].", settings.LimitAttemptsForOperationTo);
            }
            else
            {
                connectionSettings.LimitAttemptsForOperationTo(1);
                _logger.DebugFormat("LimitAttemptsForOperationTo [{0}].", 1);
            }
            if (settings.LimitReconnectionsTo > 0)
            {
                connectionSettings.LimitReconnectionsTo(settings.LimitReconnectionsTo);
                _logger.DebugFormat("LimitReconnectionsTo [{0}].", settings.LimitReconnectionsTo);
            }
            else
            {
                connectionSettings.LimitReconnectionsTo(5);
                _logger.DebugFormat("LimitReconnectionsTo [{0}].", 5);
            }
            if (settings.OperationTimeout > 0)
            {
                connectionSettings.SetOperationTimeoutTo(TimeSpan.FromSeconds(settings.OperationTimeout));
                _logger.DebugFormat("OperationTimeout [{0}] seconds.", settings.OperationTimeout);
            }
            else
            {
                connectionSettings.SetOperationTimeoutTo(TimeSpan.FromSeconds(5));
                _logger.DebugFormat("OperationTimeout [{0}] seconds.", 5);
            }
            if (settings.UseConsoleLogger)
            {
                connectionSettings.UseConsoleLogger();
                _logger.Debug("Using Console Logger.");
            }
            if (settings.ConnectionTimeout > 0)
            {
                connectionSettings.WithConnectionTimeoutOf(TimeSpan.FromSeconds(settings.ConnectionTimeout));
                _logger.DebugFormat("OperationTimeout [{0}] seconds.", settings.ConnectionTimeout);
            }
            else
            {
                connectionSettings.WithConnectionTimeoutOf(TimeSpan.FromSeconds(10));
                _logger.DebugFormat("OperationTimeout [{0}] seconds.", 10);
            }

            connectionSettings.SetDefaultUserCredentials(userCredentials: new EventStore.ClientAPI.SystemData.UserCredentials(
                        settings.Username,
                        settings.Password));
            var eventStoreServerAddress = IPAddress.Loopback;
            if (string.IsNullOrWhiteSpace(settings.HostName)
                && string.IsNullOrWhiteSpace(settings.IPAddress))
            {
                _logger.Warn("No HostName or IPAddress settings specified - reverting to LOOPBACK.");
            }
            else if (!string.IsNullOrWhiteSpace(settings.HostName))
            {
                _logger.DebugFormat("Using HostName [{0}].", settings.HostName);
                eventStoreServerAddress = Dns.GetHostAddresses(settings.HostName).First();
                _logger.DebugFormat("HostName [{0}] resolved to [{1}].", settings.HostName, eventStoreServerAddress.ToString());
            }
            else
            {
                _logger.DebugFormat("Using IPAddress [{0}].", settings.IPAddress);
                eventStoreServerAddress = IPAddress.Parse(settings.IPAddress);
                _logger.DebugFormat("IPAddress resolved to [{0}].", eventStoreServerAddress.ToString());
            }
            var port = settings.Port > 0 ? settings.Port : 1113;
            _logger.DebugFormat("Port is [{0}]", port);
            var connectionName = string.IsNullOrWhiteSpace(settings.ConnectionName)
                                                    ? "Maginus Lunch Order API"
                                                    : settings.ConnectionName;
            _logger.DebugFormat("ConnectionName is [{0}].", connectionName);
            var gesConnection = EventStore.ClientAPI.EventStoreConnection.Create(
                                connectionSettings,
                                new IPEndPoint(eventStoreServerAddress, port),
                                connectionName: connectionName);
            _logger.Debug("Connecting to EventStore...");
            gesConnection.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            _logger.Debug("EventStore connected.");
            return gesConnection;
        }

        public static bool Start()
        {
            return TheTask.Start();
        }

        public static bool Stop()
        {
            System.Console.WriteLine("Stopping...");
            var status = TheTask.Stop();
            System.Console.WriteLine("Stopped.");
            return status;
        }
    }
}
