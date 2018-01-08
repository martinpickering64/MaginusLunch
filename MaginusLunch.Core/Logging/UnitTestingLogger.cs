using MaginusLunch.Logging;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Core.Logging
{
    public class UnitTestingLogger : ILog
    {
        public static readonly IList<string> LogMsgs = new List<string>();
        private static object _lock = new object();
        private LogLevel _currentLevel;
        private readonly string _name;

        public UnitTestingLogger(string name)
        {
            _name = name;
            _currentLevel = LogLevel.Verbose;
        }

        public bool IsVerboseEnabled => (int)_currentLevel <= (int)LogLevel.Verbose;

        public bool IsDebugEnabled => (int)_currentLevel <= (int)LogLevel.Debug;

        public bool IsInfoEnabled => (int)_currentLevel <= (int)LogLevel.Info;

        public bool IsWarnEnabled => (int)_currentLevel <= (int)LogLevel.Warn;

        public bool IsErrorEnabled => (int)_currentLevel <= (int)LogLevel.Error;

        public bool IsFatalEnabled => (int)_currentLevel <= (int)LogLevel.Fatal;

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                Write(LogLevel.Debug, message);
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                Write(LogLevel.Debug, message, exception);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
            {

                Write(LogLevel.Debug, string.Format(format, args));
            }
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                Write(LogLevel.Error, message);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Write(LogLevel.Error, message, exception);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Write(LogLevel.Error, string.Format(format, args));
            }
        }

        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                Write(LogLevel.Fatal, message);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Write(LogLevel.Fatal, message, exception);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Write(LogLevel.Fatal, string.Format(format, args));
            }
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                Write(LogLevel.Info, message);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                Write(LogLevel.Info, message, exception);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Write(LogLevel.Info, string.Format(format, args));
            }
        }

        public void Verbose(string message)
        {
            if (IsVerboseEnabled)
            {
                Write(LogLevel.Verbose, message);
            }
        }

        public void Verbose(string message, Exception exception)
        {
            if (IsVerboseEnabled)
            {
                Write(LogLevel.Verbose, message, exception);
            }
        }

        public void VerboseFormat(string format, params object[] args)
        {
            if (IsVerboseEnabled)
            {
                Write(LogLevel.Verbose, string.Format(format, args));
            }
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                Write(LogLevel.Warn, message);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Write(LogLevel.Warn, message, exception);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Write(LogLevel.Warn, string.Format(format, args));
            }
        }

        private void Write(LogLevel logLevel, string message)
        {
            var msg = $"{logLevel.ToString().ToUpper().PadRight(5)} {_name} {message}";
            lock (_lock)
            {
                LogMsgs.Add(msg);
            }
        }

        private void Write(LogLevel level, string message, Exception ex)
        {
            message = $"{message}/n/rException: {ex.Source}[{ex.HResult}] {ex.Message}";
            Write(level, message);
        }
    }

    public class UnitTestLoggerLoggerFactory : ILoggerFactory
    {

        public ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        public ILog GetLogger(string name)
        {
            return new UnitTestingLogger(name);
        }
    }

    public class UnitTestLoggerFactory : LoggingFactoryDefinition
    {
        protected internal override ILoggerFactory GetLoggingFactory()
        {
            return new UnitTestLoggerLoggerFactory();
        }
    }


}
