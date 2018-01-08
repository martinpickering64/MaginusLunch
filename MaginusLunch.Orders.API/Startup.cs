using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4.AccessTokenValidation;
using MaginusLunch.Logging.MicrosoftLogging;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using MaginusLunch.Orders.API.Extensions;
using MaginusLunch.Core.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;

namespace MaginusLunch.Orders.API
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
            ;
            Configuration = builder.Build();
            MicrosoftLoggerFactory.MSLoggerFactory = loggerFactory;
            MaginusLunch.Logging.LogManager.Use<MicrosoftLoggerFactory>();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var authSettings = new IdentityServerAuthenticationSettings();
            Configuration.GetSection(IdentityServerAuthenticationSettings.DefaultIdentityServerAuthenticationSettingsSectionName)
                .Bind(authSettings);
            if (string.IsNullOrWhiteSpace(authSettings.Authority)) { authSettings.Authority = "https://localhost:44318"; }
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = authSettings.Authority;
                    options.RequireHttpsMetadata = authSettings.RequireHttpsMetadata;
                    options.ApiName = string.IsNullOrWhiteSpace(authSettings.ApiName)
                                                ? "MaginusLunch.Orders"
                                                : authSettings.ApiName;
                });
            services.AddBespokeAuthorizationPolicies();
            services.AddBespokeMvc(Configuration);
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = authSettings.Authority,
                    //Scopes = new Dictionary<string, string>
                    //        {
                    //            { "readAccess", "Access read operations" },
                    //            { "writeAccess", "Access write operations" }
                    //        }
                });
                // Assign scope requirements to operations based on AuthorizeAttribute
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Maginus Lunch Orders API",
                    Version = "V1",
                    Description = "An API to order your Maginus Lunch by",
                    TermsOfService = "https://lunch.maginus.com/termsOfService/",
                    Contact = new Contact
                    {
                        Name = "Martin Pickering",
                        Email = "martin.pickering@maginus.com",
                        Url = "https://maginus.com/footer-links/management-team/"
                    },
                    License = new License
                    {
                        Name = "For Maginus use only",
                        Url = "https://www.maginus.com/" }
                });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "MaginusLunch.Orders.API.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddMongoDbRepositories(configuration: Configuration);
            services.AddEventStore(configuration: Configuration);
            services.AddLocalServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            #region done by CreateDefaultBuilder??
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            #endregion done by CreateDefaultBuilder??
            app.UseAuthentication();

            app.UseStaticFiles();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maginus Lunch Orders API");
            });

            app.UseMvc();
        }

        // SecurityRequirementsOperationFilter.cs
        public class SecurityRequirementsOperationFilter : IOperationFilter
        {
            private readonly IOptions<AuthorizationOptions> authorizationOptions;

            public SecurityRequirementsOperationFilter(IOptions<AuthorizationOptions> authorizationOptions)
            {
                this.authorizationOptions = authorizationOptions;
            }

            public void Apply(Operation operation, OperationFilterContext context)
            {
                var controllerPolicies = context.ApiDescription.ControllerAttributes()
                    .OfType<AuthorizeAttribute>()
                    .Select(attr => attr.Policy);
                var actionPolicies = context.ApiDescription.ActionAttributes()
                    .OfType<AuthorizeAttribute>()
                    .Select(attr => attr.Policy);
                var policies = controllerPolicies.Union(actionPolicies).Distinct();
                var requiredClaimTypes = policies
                    .Select(x => this.authorizationOptions.Value.GetPolicy(x))
                    .SelectMany(x => x.Requirements)
                    .OfType<ClaimsAuthorizationRequirement>()
                    .Select(x => x.ClaimType);

                if (requiredClaimTypes.Any())
                {
                    operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                    operation.Responses.Add("403", new Response { Description = "Forbidden" });

                    operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                    operation.Security.Add(
                        new Dictionary<string, IEnumerable<string>>
                        {
                    { "oauth2", requiredClaimTypes }
                        });
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
