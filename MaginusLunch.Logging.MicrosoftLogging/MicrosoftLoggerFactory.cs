namespace MaginusLunch.Logging.MicrosoftLogging
{
    using MaginusLunch.Logging;
    using System;
    using MsLogging = Microsoft.Extensions.Logging;

    public class MicrosoftLoggerFactory : LoggingFactoryDefinition
    {
        public static MsLogging.ILoggerFactory MSLoggerFactory;
        public MicrosoftLoggerFactory()
        {
            if (MSLoggerFactory == null)
            {
                throw new InvalidOperationException("Please initialize MicrosoftLoggerFactory.MSLoggerFactory before calling LogManager.Use<MicrosoftLoggerFactory>();");
            }
        }

        /// <summary>
        /// <see cref="LoggingFactoryDefinition.GetLoggingFactory"/>.
        /// </summary>
        protected override ILoggerFactory GetLoggingFactory()
        {
            return new LoggerFactory(MSLoggerFactory);
        }
    }
}
