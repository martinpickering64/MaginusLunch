using Microsoft.Extensions.Logging;

namespace MaginusLunch.Logging.MicrosoftLogging.IntegrationTests
{
    public class IntegrationTestLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        public static IntegrationTestLoggerProvider Instance { get; } = new IntegrationTestLoggerProvider();

        private IntegrationTestLoggerProvider()
        { }

        public ILogger CreateLogger(string categoryName)
        {
            return IntegrationTestLogger.Instance;
        }

        public void Dispose()
        {
        }
    }
}
