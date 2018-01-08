using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MaginusLunch.Logging.MicrosoftLogging;
using MaginusLunch.Core.Authentication;
using IdentityServer4.AccessTokenValidation;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using MaginusLunch.Menu.API.Extensions;

namespace MaginusLunch.Menu.API
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

        // This method gets called by the runtime. Use this method to add services to the container.
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
                                                ? "MaginusLunch.Menu"
                                                : authSettings.ApiName;
                });
            services.AddBespokeAuthorizationPolicies();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                //// Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                //c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //    Type = "oauth2",
                //    Flow = "implicit",
                //    AuthorizationUrl = authSettings.Authority,
                //    //Scopes = new Dictionary<string, string>
                //    //        {
                //    //            { "readAccess", "Access read operations" },
                //    //            { "writeAccess", "Access write operations" }
                //    //        }
                //});
                //// Assign scope requirements to operations based on AuthorizeAttribute
                //c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Maginus Lunch Menu API",
                    Version = "V1",
                    Description = "An API to access and manage the Maginus Lunch Menu",
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
                        Url = "https://www.maginus.com/"
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "MaginusLunch.Menu.API.xml");
                c.IncludeXmlComments(xmlPath);
            });


            services.AddBespokeMvc(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maginus Lunch Menu API");
            });

            app.UseMvc();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
