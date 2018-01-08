using System;
using System.IO;

namespace MaginusLunch.Logging
{
    /// <summary>
    /// The default <see cref="T:NServiceBus.Logging.LoggingFactoryDefinition" />.
    /// </summary>
    public class DefaultFactory : LoggingFactoryDefinition
    {
        private Lazy<string> _directory;
        private Lazy<LogLevel> _level;

        /// <summary>
        /// Initializes a new instance of <see cref="T:MaginusLunch.Logging.DefaultFactory" />.
        /// </summary>
        public DefaultFactory()
        {
            _directory = new Lazy<string>(new Func<string>(FindDefaultLoggingDirectory));
            _level = new Lazy<LogLevel>(LogLevel.Info);
        }

        /// <summary>
        /// <see cref="M:MaginusLunch.Logging.LoggingFactoryDefinition.GetLoggingFactory" />.
        /// </summary>
        protected internal override ILoggerFactory GetLoggingFactory()
        {
            var defaultLoggerFactory = new DefaultLoggerFactory(_level.Value, _directory.Value);
            defaultLoggerFactory.Write(GetType().Name, 
                LogLevel.Debug, 
                $"Logging to '{_directory}' with level {_level}");
            return (ILoggerFactory)defaultLoggerFactory;
        }

        /// <summary>
        /// Controls the <see cref="T:MaginusLunch.Logging.LogLevel" />.
        /// </summary>
        public void Level(LogLevel level)
        {
            _level = new Lazy<LogLevel>((Func<LogLevel>)(() => level));
        }

        /// <summary>The directory to log files to.</summary>
        public void Directory(string directory)
        {
            Guard.AgainstNullAndEmpty("directory", directory);
            if (!System.IO.Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(string.Format("Could not find logging directory: '{0}'", directory));
            }
            _directory = new Lazy<string>(() => directory);
        }

        public static string FindDefaultLoggingDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
