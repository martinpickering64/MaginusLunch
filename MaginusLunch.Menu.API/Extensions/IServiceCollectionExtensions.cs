using MaginusLunch.Core.Validation;
using MaginusLunch.GetEventStore;
using MaginusLunch.Logging;
using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
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

namespace MaginusLunch.Menu.API.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Core.AspNet.Authorization.AuthorizationPolicies.IsMaginusEmployee, policy =>
                {
                    policy.Requirements.Add(maginusEmployeeRequirement);
                });
                _logger.DebugFormat("Policy {0} added.", Core.AspNet.Authorization.AuthorizationPolicies.IsMaginusEmployee);
            });
            _logger.Debug("Authorization Policies added.");
            services.AddSingleton<IAuthorizationHandler, Core.AspNet.Authorization.MaginusEmployeeHandler>();
            _logger.DebugFormat("IAuthorizationHandler {0} added.", typeof(Core.AspNet.Authorization.MaginusEmployeeHandler).Name);
            _logger.Debug("Authorization Handlers added.");
            return services;
        }

        /// <summary>
        /// Configure MVC.
        /// </summary>
        /// <param name="services">The collection of service descriptors being extended.</param>
        /// <param name="configuration"></param>
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
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
