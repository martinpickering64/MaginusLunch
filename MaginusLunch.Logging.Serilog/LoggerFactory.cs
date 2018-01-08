namespace MaginusLunch.Logging.Serilog
{
    using System;
    using MaginusLunch.Logging;
    using SerilogLogger = global::Serilog.ILogger;

    class LoggerFactory : ILoggerFactory
    {
        private SerilogLogger _logger;

        public LoggerFactory(SerilogLogger logger)
        {
            _logger = logger;
        }

        public ILog GetLogger(Type type)
        {
            var contextLogger = _logger.ForContext(type);
            return new Logger(contextLogger);
        }

        public ILog GetLogger(string name)
        {
            var contextLogger = _logger.ForContext("SourceContext", name);
            return new Logger(contextLogger);
        }
    }
}