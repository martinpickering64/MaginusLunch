namespace MaginusLunch.Logging.MicrosoftLogging
{
    using System;
    using MaginusLunch.Logging;
    using MsLogging = Microsoft.Extensions.Logging;

    public class LoggerFactory : ILoggerFactory
    {
        private readonly MsLogging.ILoggerFactory _msLoggerFactory;
        public LoggerFactory(MsLogging.ILoggerFactory msLoggerFactory)
        {
            _msLoggerFactory = msLoggerFactory;
        }
        public ILog GetLogger(Type type)
        {
            var logger = MsLogging.LoggerFactoryExtensions.CreateLogger(_msLoggerFactory, type);
            return new Logger(logger);
        }

        public ILog GetLogger(string name)
        {
            var logger = _msLoggerFactory.CreateLogger(name);
            return new Logger(logger);
        }
    }
}
