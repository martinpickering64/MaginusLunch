using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MaginusLunch.Authentication.IdentityServer.Data;
using MaginusLunch.Authentication.IdentityServer.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using MaginusLunch.Authentication.IdentityServer.Authentication;

namespace MaginusLunch.Authentication.IdentityServer
{
    public class Startup
    {
        private bool IsDevelopment;
        public Startup(IHostingEnvironment env)
        {
            IsDevelopment = env.IsDevelopment();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            if (IsDevelopment)
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(
                     IsDevelopment
                        ?"DevConnection" 
                        : "ReleaseConnectionString")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()                
                .AddIdentityServerUserClaimsPrincipalFactory();

            // For use with CachedPropertiesDataFormat. In load-balanced scenarios 
            // you should use a persistent cache such as Redis or SQL Server.
            services.AddDistributedMemoryCache();

            services.AddMvc();

            // Adds IdentityServer
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                //.AddSigningCredential("C48B4BDA6D64F52F680A4824139CE90177716176", System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser, NameType.Thumbprint)
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseIdentityServer();

            var schemeName = "MS";
            var dataProtectionProvider = app.ApplicationServices.GetRequiredService<IDataProtectionProvider>();
            var distributedCache = app.ApplicationServices.GetRequiredService<IDistributedCache>();
            var msDataProtector = dataProtectionProvider.CreateProtector(
            typeof(Microsoft.AspNetCore.Authentication.MicrosoftAccount.MicrosoftAccountMiddleware).FullName,
            typeof(string).FullName, schemeName, "v1");

            var msDataFormat = new CachedPropertiesDataFormat(distributedCache, msDataProtector);
            var msAuthOptions = new MicrosoftAccountOptions
            {
                AuthenticationScheme = "MS",
                DisplayName = "Microsoft",
                SignInScheme = "Identity.External",
                ClientId = "b7c6b710-fa57-43d1-8f4b-9fa26b34e93a",
                ClientSecret = "n8rgfyq5Dj4sssCGekBP8Zq",
                StateDataFormat = msDataFormat
            };
            app.UseMicrosoftAccountAuthentication(msAuthOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
