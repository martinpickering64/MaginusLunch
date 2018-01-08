using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using System.Collections.Concurrent;

namespace MaginusLunch.Logging.Serilog.IntegrationTests
{
    [TestCategory("Integration")]
    [TestCategory("Serilog")]
    [TestClass]
    public class SerilogIntegrationTests
    {
        [TestMethod]
        public void Ensure_Serilog_messages_are_redirected()
        {
            var logs = new ConcurrentBag<LogEvent>();
            var eventSink = new SerilogEventSink
            {
                Action = logs.Add
            };
            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .WriteTo.Sink(eventSink)
                                .CreateLogger();
            LogManager.Use<SerilogFactory>();
            var log = LogManager.GetLogger("IntegrationTests");

            log.Error("An error log");

            Assert.IsTrue(logs.Count > 0);
        }
    }
}
