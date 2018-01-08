namespace MaginusLunch.Logging.Serilog
{
    using System;
    using MaginusLunch.Logging;
    using SerilogLogger = global::Serilog.ILogger;
    using SerilogEvents = global::Serilog.Events;

    class Logger : ILog
    {
        SerilogLogger _logger;

        public Logger(SerilogLogger logger)
        {
            _logger = logger;
        }

        public void Verbose(string message)
        {
            _logger.Verbose("{LogText:l}", message);
        }

        public void Verbose(string message, Exception exception)
        {
            _logger.Verbose(exception, "{LogText:l}", message);
        }

        public void VerboseFormat(string format, params object[] args)
        {
            _logger.Verbose(format, args);
        }

        public void Debug(string message)
        {
            _logger.Debug("{LogText:l}", message);
        }

        public void Debug(string message, Exception exception)
        {
            _logger.Debug(exception, "{LogText:l}", message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.Debug(format, args);
        }

        public void Info(string message)
        {
            _logger.Information("{LogText:l}", message);
        }

        public void Info(string message, Exception exception)
        {
            _logger.Information(exception, "{LogText:l}", message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.Information(format, args);
        }

        public void Warn(string message)
        {
            _logger.Warning("{LogText:l}", message);
        }

        public void Warn(string message, Exception exception)
        {
            _logger.Warning(exception, "{LogText:l}", message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.Warning(format, args);
        }

        public void Error(string message)
        {
            _logger.Error("{LogText:l}", message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, "{LogText:l}", message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.Error(format, args);
        }

        public void Fatal(string message)
        {
            _logger.Fatal("{LogText:l}", message);
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, "{LogText:l}", message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.Fatal(format, args);
        }

        public bool IsVerboseEnabled => _logger.IsEnabled(SerilogEvents.LogEventLevel.Verbose);
        public bool IsDebugEnabled => _logger.IsEnabled(SerilogEvents.LogEventLevel.Debug);
        public bool IsInfoEnabled => _logger.IsEnabled(SerilogEvents.LogEventLevel.Information);
        public bool IsWarnEnabled => _logger.IsEnabled(SerilogEvents.LogEventLevel.Warning);
        public bool IsErrorEnabled => _logger.IsEnabled(SerilogEvents.LogEventLevel.Error);
        public bool IsFatalEnabled => _logger.IsEnabled(SerilogEvents.LogEventLevel.Fatal);
    }
}