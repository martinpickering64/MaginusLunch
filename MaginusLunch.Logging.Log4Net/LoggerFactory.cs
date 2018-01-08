namespace MaginusLunch.Logging.Log4Net
{
    using System;
    using Logging;
    using System.Linq;

    class LoggerFactory : ILoggerFactory
    {

        public ILog GetLogger(Type type)
        {
            var logger = log4net.LogManager.GetLogger(type);
            return new Logger(logger);
        }

        /// <remarks>
        /// This is a hack as log4net on .NetStandard does not support a call to
        /// log4net.LogManager.GetLogger(string name)
        /// </remarks>
        public ILog GetLogger(string name)
        {
            var repo = log4net.LogManager.GetAllRepositories().First();
            var logger = log4net.LogManager.GetLogger(repo.Name, name);
            return new Logger(logger);
        }
    }
}