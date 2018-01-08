using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaginusLunch.Logging.Log4Net.IntegrationTests
{
    [TestCategory("Integration")]
    [TestCategory("Log4Net")]
    [TestClass]
    public class Log4NetIntegrationTests
    {
        [TestMethod]
        public void Ensure_Log4Net_messages_are_redirected()
        {
            LogMessageCapture.ConfigureLogging();
            var logger = LogManager.GetLogger("IntegrationTest");
            var actual = LogMessageCapture.LoggingEvents;

            logger.Error("Some Error Log");

            Assert.IsTrue(actual.Count > 0);
        }
    }
}
