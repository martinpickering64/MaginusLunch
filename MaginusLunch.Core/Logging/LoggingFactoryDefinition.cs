namespace MaginusLunch.Logging
{
    /// <summary>Base class for logging definitions.</summary>
    public abstract class LoggingFactoryDefinition
    {
        /// <summary>
        /// Constructs an instance of <see cref="T:MaginusLunch.Logging.ILoggerFactory" /> 
        /// for use by <see cref="M:MaginusLunch.Logging.LogManager.Use``1" />.
        /// </summary>
        protected internal abstract ILoggerFactory GetLoggingFactory();
    }
}
