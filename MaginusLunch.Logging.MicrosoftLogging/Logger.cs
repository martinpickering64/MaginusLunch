namespace MaginusLunch.Logging.MicrosoftLogging
{
    using System;
    using Microsoft.Extensions.Logging;

    class Logger : ILog
    {
        private Microsoft.Extensions.Logging.ILogger _logger;

        public Logger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Verbose(string message) => _logger.LogTrace(message);

        public void Verbose(string message, Exception exception) => _logger.LogTrace(message: message, exception: exception);

        public void VerboseFormat(string format, params object[] args) => _logger.LogTrace(format, args);

        public void Debug(string message) => _logger.LogDebug(message);

        public void Debug(string message, Exception exception) => _logger.LogDebug(message: message, exception: exception);

        public void DebugFormat(string format, params object[] args) => _logger.LogDebug(format, args);

        public void Info(string message) => _logger.LogInformation(message);

        public void Info(string message, Exception exception) => _logger.LogInformation(message: message, exception: exception);

        public void InfoFormat(string format, params object[] args) => _logger.LogInformation(format, args);

        public void Warn(string message) => _logger.LogWarning(message);

        public void Warn(string message, Exception exception) => _logger.LogWarning(message: message, exception: exception);

        public void WarnFormat(string format, params object[] args) => _logger.LogWarning(format, args);

        public void Error(string message) => _logger.LogError(message);

        public void Error(string message, Exception exception) => _logger.LogError(message: message, exception: exception);

        public void ErrorFormat(string format, params object[] args) => _logger.LogError(format, args);

        public void Fatal(string message) => _logger.LogCritical(message);

        public void Fatal(string message, Exception exception) => _logger.LogCritical(message: message, exception: exception);

        public void FatalFormat(string format, params object[] args) => _logger.LogCritical(format, args);

        public bool IsVerboseEnabled => _logger.IsEnabled(LogLevel.Trace);
        public bool IsDebugEnabled => _logger.IsEnabled(LogLevel.Debug);
        public bool IsInfoEnabled => _logger.IsEnabled(LogLevel.Information);
        public bool IsWarnEnabled => _logger.IsEnabled(LogLevel.Warning);
        public bool IsErrorEnabled => _logger.IsEnabled(LogLevel.Error);
        public bool IsFatalEnabled => _logger.IsEnabled(LogLevel.Critical);
    }
}
