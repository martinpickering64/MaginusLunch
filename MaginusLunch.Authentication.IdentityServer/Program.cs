﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MaginusLunch.Authentication.IdentityServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                //.UseUrls("http://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                //.UseApplicationInsights()
                .Build();
            host.Run();
        }
    }
}
