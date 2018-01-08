using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MaginusLunch.Logging.MicrosoftLogging.IntegrationTests
{
    [TestClass]
    public class MicrosoftLoggingIntegrationTests
    {
        [TestMethod]
        public void Ensure_log_messages_are_redirected_to_Microsoft_Logging()
        {
            var msLoggerFactory = new Microsoft.Extensions.Logging.LoggerFactory(new[] { IntegrationTestLoggerProvider.Instance });
            MicrosoftLoggerFactory.MSLoggerFactory = msLoggerFactory;
            LogManager.Use<MicrosoftLoggerFactory>();
            var log = LogManager.GetLogger("IntegrationTests");

            log.Error("An error log");

            Assert.IsTrue(IntegrationTestLogger.Instance.Logs.Count() > 0);
        }

    }
}
