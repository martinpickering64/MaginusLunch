using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MaginusLunch.Orders.API
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
        #region done by CreateDefaultBuilder??
            //.UseKestrel()
            //.UseContentRoot(Directory.GetCurrentDirectory())
            //.ConfigureLogging((hostingContext, logging) =>
            //{
            //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            //    logging.AddConsole();
            //    logging.AddDebug();
            //})
        #endregion done by CreateDefaultBuilder??
            .UseStartup<Startup>()
                .Build();
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
