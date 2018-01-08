namespace MaginusLunch.Logging.Serilog
{
    using MaginusLunch.Logging;
    using SerilogLogger = global::Serilog.ILogger;
    using SerilogLog = global::Serilog.Log;

    /// <summary>
    /// Configure logging messages to use Serilog.  
    /// Use by calling <see cref="LogManager.Use{T}"/> the T is <see cref="SerilogFactory"/>.
    /// </summary>
    public class SerilogFactory : LoggingFactoryDefinition
    {
        SerilogLogger _loggerToUse;

        /// <summary>
        /// <see cref="LoggingFactoryDefinition.GetLoggingFactory"/>.
        /// </summary>
        protected override ILoggerFactory GetLoggingFactory()
        {
            return new LoggerFactory(_loggerToUse ?? SerilogLog.Logger);
        }

        /// <summary>
        /// Specify an instance of <see cref="ILogger"/> to use. 
        /// If not specified then the default is <see cref="Log.Logger"/>.
        /// </summary>
        public void WithLogger(SerilogLogger logger)
        {
            Guard.AgainstNull("logger", logger);
            _loggerToUse = logger;
        }
    }
}