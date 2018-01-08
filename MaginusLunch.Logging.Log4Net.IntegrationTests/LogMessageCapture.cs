namespace MaginusLunch.Logging.Log4Net.IntegrationTests
{
    using System.Collections.Generic;
    using log4net.Config;
    using log4net.Core;
    using log4net.Repository.Hierarchy;
    using log4netLogManager = log4net.LogManager;
    using MaginusLunchLogManager = MaginusLunch.Logging.LogManager;

    class LogMessageCapture
    {
        public static List<LoggingEvent> LoggingEvents = new List<LoggingEvent>();

        public static void ConfigureLogging()
        {
            var hierarchy = (Hierarchy)log4netLogManager.CreateRepository("default");
            hierarchy.Root.RemoveAllAppenders();

            var target = new TestAppender
            {
                Action = x => LoggingEvents.Add(x)
            };
            BasicConfigurator.Configure(hierarchy, target);
            MaginusLunchLogManager.Use<Log4NetFactory>();
        }
    }
}
