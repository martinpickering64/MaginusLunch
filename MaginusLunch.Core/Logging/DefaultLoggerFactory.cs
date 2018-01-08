using System;
using System.Diagnostics;

namespace MaginusLunch.Logging
{
    internal class DefaultLoggerFactory : ILoggerFactory
    {
        private object _locker = new object();
        private readonly LogLevel _filterLevel;
        private readonly bool _isDebugEnabled;
        private readonly bool _isErrorEnabled;
        private readonly bool _isFatalEnabled;
        private readonly bool _isInfoEnabled;
        private readonly bool _isWarnEnabled;
        private readonly RollingLogger _rollingLogger;

        public DefaultLoggerFactory(LogLevel filterLevel, string loggingDirectory)
        {
            _filterLevel = filterLevel;
            _rollingLogger = new RollingLogger(loggingDirectory, 10, 10485760L);
            _isDebugEnabled = LogLevel.Debug >= filterLevel;
            _isInfoEnabled = LogLevel.Info >= filterLevel;
            _isWarnEnabled = LogLevel.Warn >= filterLevel;
            _isErrorEnabled = LogLevel.Error >= filterLevel;
            _isFatalEnabled = LogLevel.Fatal >= filterLevel;
        }

        public ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        public ILog GetLogger(string name)
        {
            NamedLogger namedLogger = new NamedLogger(name, this);
            int num1 = _isDebugEnabled ? 1 : 0;
            namedLogger.IsDebugEnabled = num1 != 0;
            int num2 = _isInfoEnabled ? 1 : 0;
            namedLogger.IsInfoEnabled = num2 != 0;
            int num3 = _isWarnEnabled ? 1 : 0;
            namedLogger.IsWarnEnabled = num3 != 0;
            int num4 = _isErrorEnabled ? 1 : 0;
            namedLogger.IsErrorEnabled = num4 != 0;
            int num5 = _isFatalEnabled ? 1 : 0;
            namedLogger.IsFatalEnabled = num5 != 0;
            return (ILog)namedLogger;
        }

        public void Write(string name, LogLevel messageLevel, string message)
        {
            if (messageLevel < _filterLevel) { return; }
            var msg = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {messageLevel.ToString().ToUpper().PadRight(5)} {name} {message}";
            lock (_locker)
            {
                _rollingLogger.Write(msg);
                ColouredConsoleLogger.Write(msg, messageLevel);
                Trace.WriteLine(msg);
            }
        }
    }
}
