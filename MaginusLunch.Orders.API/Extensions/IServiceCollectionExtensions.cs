using MaginusLunch.Core.Validation;
using MaginusLunch.GetEventStore;
using MaginusLunch.Logging;
using MaginusLunch.MongoDB;
using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Repository;
using MaginusLunch.Orders.Service;
using MaginusLunch.Orders.Service.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace MaginusLunch.Orders.API.Extensions
{
    /// <summary>
    /// A set of extensions that are used to configure the ASP.Net runtime services.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        private static ILog _logger = LogManager.GetLogger(typeof(IServiceCollectionExtensions));

        /// <summary>
        /// Adds the bespoke authorization policies to the Policy Repository.
        /// </summary>
        /// <param name="services">The collection of service descriptors being extended.</param>
        /// <returns>The now extended collection of service descriptors.</returns>
        public static IServiceCollection AddBespokeAuthorizationPolicies(this IServiceCollection services)
        {
            _logger.Debug("Adding Authorization Policies...");
            var maginusEmployeeRequirement = new Core.AspNet.Authorization.MaginusEmployeeRequirement();
            var orderCommandRequirement = new OrderCommandRequirement();
            var userRetrievalRequirement = new UserRetrievalRequirement();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Core.AspNet.Authorization.AuthorizationPolicies.IsMaginusEmployee, policy =>
                {
                    policy.Requirements.Add(maginusEmployeeRequirement);
                });
                _logger.DebugFormat("Policy {0} added.", Core.AspNet.Authorization.AuthorizationPolicies.IsMaginusEmployee);
                options.AddPolicy(AuthorizationPolicies.CanAddOrder, policy =>
                {
                    policy.Requirements.Add(orderCommandRequirement);
                });
                _logger.DebugFormat("Policy {0} added.", AuthorizationPolicies.CanAddOrder);
                options.AddPolicy(AuthorizationPolicies.WillAcceptOrderCommand, policy =>
                {
                    policy.Requirements.Add(orderCommandRequirement);
                });
                _logger.DebugFormat("Policy {0} added.", AuthorizationPolicies.WillAcceptOrderCommand);
                options.AddPolicy(AuthorizationPolicies.CanAccessOrders, policy =>
                {
                    policy.Requirements.Add(userRetrievalRequirement);
                });
                _logger.DebugFormat("Policy {0} added.", AuthorizationPolicies.CanAccessOrders);
            });
            _logger.Debug("Authorization Policies added.");
            services.AddSingleton<IAuthorizationHandler, Core.AspNet.Authorization.MaginusEmployeeHandler>();
            _logger.DebugFormat("IAuthorizationHandler {0} added.", typeof(Core.AspNet.Authorization.MaginusEmployeeHandler).Name);
            services.AddSingleton<IAuthorizationHandler, AddOrderCommandHandler>();
            _logger.DebugFormat("IAuthorizationHandler {0} added.", typeof(AddOrderCommandHandler).Name);
            services.AddSingleton<IAuthorizationHandler, OrderCommandHandler>();
            _logger.DebugFormat("IAuthorizationHandler {0} added.", typeof(OrderCommandHandler).Name);
            services.AddSingleton<IAuthorizationHandler, UserRetrievalHandler>();
            _logger.DebugFormat("IAuthorizationHandler {0} added.", typeof(UserRetrievalHandler).Name);
            _logger.Debug("Authorization Handlers added.");
            return services;
        }

        /// <summary>
        /// Configure the various MongDB Repositories we are using.
        /// </summary>
        /// <param name="services">The collection of service descriptors being extended.</param>
        /// <param name="configuration"></param>
        /// <returns>The now extended collection of service descriptors.</returns>
        public static IServiceCollection AddMongoDbRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            _logger.Debug("configuring MongoDbRepositories...");
            var readRepositorySettings = new MongoRepositorySettings();
            configuration.GetSection(MongoRepositorySettings.DefaultMongoRepositorySectionName)
                .Bind(readRepositorySettings);
            _logger.DebugFormat("ConfigurationSettings bound to Section [{0}].", MongoRepositorySettings.DefaultMongoRepositorySectionName);
            services.AddSingleton<IOrderRepository, MongoOrderRepository>((sp) =>
            {
                return new MongoOrderRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(IOrderRepository).Name, typeof(MongoOrderRepository).Name);
            services.AddSingleton<ICalendarRepository, MongoCalendarRepository>((sp) =>
            {
                return new MongoCalendarRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(ICalendarRepository).Name, typeof(MongoCalendarRepository).Name);
            services.AddSingleton<ILunchProviderRepository, MongoLunchProviderRepository>((sp) =>
            {
                return new MongoLunchProviderRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(ILunchProviderRepository).Name, typeof(MongoLunchProviderRepository).Name);
            services.AddSingleton<IMenuCategoryRepository, MongoMenuCategoryRepository>((sp) =>
            {
                return new MongoMenuCategoryRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(IMenuCategoryRepository).Name, typeof(MongoMenuCategoryRepository).Name);
            services.AddSingleton<IMenuOptionRepository, MongoMenuOptionRepository>((sp) =>
            {
                return new MongoMenuOptionRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(IMenuOptionRepository).Name, typeof(MongoMenuOptionRepository).Name);
            services.AddSingleton<IFillingRepository, MongoFillingRepository>((sp) =>
            {
                return new MongoFillingRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(IFillingRepository).Name, typeof(MongoFillingRepository).Name);
            services.AddSingleton<IBreadRepository, MongoBreadRepository>((sp) =>
            {
                return new MongoBreadRepository(readRepositorySettings, AsyncRetryPolicy, RetryPolicy);
            });
            _logger.DebugFormat("[{0}] is bound to [{1}].", typeof(IBreadRepository).Name, typeof(MongoBreadRepository).Name);
            _logger.Debug("MongoDbRepositories configured.");
            return services;
        }

        /// <summary>
        /// Configure MVC.
        /// </summary>
        /// <param name="services">The collection of service descriptors being extended.</param>
        /// <returns>The now extended collection of service descriptors.</returns>
        public static IServiceCollection AddBespokeMvc(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddMvc();

            services.AddMvc(options =>
            {
                var settings = new Core.AspNet.Authorization.TestingAuthorizationFilterSettings();
                configuration.GetSection(Core.AspNet.Authorization.TestingAuthorizationFilterSettings.DefaultTestingAuthorizationFilterSectionName)
                    .Bind(settings);
                if (settings.Enabled)
                {
                    options.Filters.Add(new Core.AspNet.Authorization.TestingAuthorizationFilter(string.IsNullOrWhiteSpace(settings.EmailClaimValue)
                        ? "martin.pickering@maginus.com"
                        : settings.EmailClaimValue));
                }
                options.Filters.Add(new AuthorizeFilter("IsMaginusEmployee"));
            });
            _logger.Debug("Global MVC AuthorizeFilter [IsMaginusEmployee] added.");
            return services;
        }

        /// <summary>
        /// Configure the GetEventStore Repository we are using.
        /// </summary>
        /// <param name="services">The collection of service descriptors being extended.</param>
        /// <param name="configuration"></param>
        /// <returns>The now extended collection of service descriptors.</returns>

        public static IServiceCollection AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            _logger.Debug("Configuring EventStore...");
            services.AddSingleton<EventStore.ClientAPI.IEventStoreConnection>((sp) =>
            {
                var settings = new GetEventStoreRepositorySettings();
                configuration.GetSection(GetEventStoreRepositorySettings.DefaultGetEventStoreRepositorySectionName)
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
            });
            _logger.DebugFormat("{0} service added.", typeof(EventStore.ClientAPI.IEventStoreConnection).Name);
            services.AddSingleton<Core.EventStore.IConstructAggregates>((sp) => new Core.EventStore.DefaultAggregateFactory());
            _logger.DebugFormat("{0} service added.", typeof(Core.EventStore.IConstructAggregates).Name);
            services.AddSingleton<Core.EventStore.IRepository, GetEventStoreRepository>();
            _logger.DebugFormat("{0} service added and bound to [{1}].", 
                                    typeof(Core.EventStore.IRepository).Name,
                                    typeof(GetEventStoreRepository).Name);
            _logger.Debug("EventStore configured.");
            return services;
        }
        
        /// <summary>
        /// Configure Local Services in the Service Layer.
        /// </summary>
        /// <param name="services">The collection of service descriptors being extended.</param>
        /// <returns>The now extended collection of service descriptors.</returns>
        public static IServiceCollection AddLocalServices(this IServiceCollection services)
        {
            _logger.Debug("Adding Local Services to Service Layer...");
            services.AddSingleton<ICalendarService, CalendarService>();
            _logger.DebugFormat("{0} service added and bound to [{1}].",
                                    typeof(ICalendarService).Name,
                                    typeof(CalendarService).Name);
            services.AddSingleton<ILunchProviderService, LunchProviderService>();
            _logger.DebugFormat("{0} service added and bound to [{1}].",
                                    typeof(ILunchProviderService).Name,
                                    typeof(LunchProviderService).Name);
            services.AddSingleton<IMenuService, MenuService>();
            _logger.DebugFormat("{0} service added and bound to [{1}].",
                                    typeof(IMenuService).Name,
                                    typeof(MenuService).Name);
            services.AddSingleton<IValidateCommandsFactory<OrderAggregate>, OrderCommandValidatorFactory>();
            _logger.DebugFormat("{0} added and bound to [{1}].",
                                    typeof(IValidateCommandsFactory<OrderAggregate>).Name,
                                    typeof(OrderCommandValidatorFactory).Name);
            services.AddSingleton<IOrderService, LocalOrderService>();
            _logger.DebugFormat("{0} service added and bound to [{1}].",
                                    typeof(IOrderService).Name,
                                    typeof(LocalOrderService).Name);
            _logger.Debug("Local Services added to Service Layer.");
            return services;
        }

        private static RetryPolicy AsyncRetryPolicy => 
            Policy.Handle<MongoConnectionException>(i =>
                i.InnerException.GetType() == typeof(IOException)
                || i.InnerException.GetType() == typeof(SocketException))
                .RetryAsync(3);

        private static RetryPolicy RetryPolicy => 
            Policy.Handle<MongoConnectionException>(i =>
                i.InnerException.GetType() == typeof(IOException)
                || i.InnerException.GetType() == typeof(SocketException))
                .Retry(3);

    }
}
