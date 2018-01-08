using System;

namespace MaginusLunch.Logging
{
    /// <summary>
    /// Responsible for the creation of <see cref="T:MagniusLunch.Logging.ILog" /> 
    /// instances and used as an extension point to redirect log event to
    /// an external library.
    /// </summary>
    /// <remarks>
    /// The default logging will be to the console and a rolling log file.
    /// </remarks>
    public static class LogManager
    {
        private static Lazy<ILoggerFactory> LoggerFactory;

        static LogManager()
        {
            Use<DefaultFactory>();
        }

        /// <summary>
        /// Used to inject an instance of <see cref="T:MagniusLunch.Logging.ILoggerFactory" /> 
        /// into <see cref="T:MagniusLunch.Logging.LogManager" />.
        /// </summary>
        public static T Use<T>() where T : LoggingFactoryDefinition, new()
        {
            T instance = Activator.CreateInstance<T>();
            LoggerFactory = new Lazy<ILoggerFactory>(new Func<ILoggerFactory>(instance.GetLoggingFactory));
            return instance;
        }

        /// <summary>
        /// An instance of <see cref="T:MagniusLunch.Logging.ILoggerFactory" /> 
        /// that will be used to construct <see cref="T:MagniusLunch.Logging.ILog" />s for static fields.
        /// </summary>
        /// <remarks>
        /// Replace this instance at application statup to redirect log event to the custom logging library.
        /// </remarks>
        public static void UseFactory(ILoggerFactory loggerFactory)
        {
            Guard.AgainstNull("loggerFactory", loggerFactory);
            LoggerFactory = new Lazy<ILoggerFactory>(() => loggerFactory);
        }

        /// <summary>
        /// Construct a <see cref="T:MagniusLunch.Logging.ILog" /> using <typeparamref name="T" /> as the name.
        /// </summary>
        public static ILog GetLogger<T>() 
            => GetLogger(typeof(T));

        /// <summary>
        /// Construct a <see cref="T:MagniusLunch.Logging.ILog" /> using <paramref name="type" /> as the name.
        /// </summary>
        public static ILog GetLogger(Type type)
        {
            Guard.AgainstNull("type", type);
            return LoggerFactory.Value.GetLogger(type);
        }

        /// <summary>
        /// Construct a <see cref="T:MagniusLunch.Logging.ILog" /> for <paramref name="name" />.
        /// </summary>
        public static ILog GetLogger(string name)
        {
            Guard.AgainstNullAndEmpty("name", name);
            return LoggerFactory.Value.GetLogger(name);
        }
    }
}
