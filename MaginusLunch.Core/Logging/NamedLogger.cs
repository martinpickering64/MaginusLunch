using System;

namespace MaginusLunch.Logging
{
    internal class NamedLogger : ILog
    {
        private DefaultLoggerFactory _defaultLoggerFactory;
        private string _name;

        public bool IsDebugEnabled { get; internal set; }

        public bool IsInfoEnabled { get; internal set; }

        public bool IsWarnEnabled { get; internal set; }

        public bool IsErrorEnabled { get; internal set; }

        public bool IsFatalEnabled { get; internal set; }

        public bool IsVerboseEnabled { get; internal set; }

        public NamedLogger(string name, DefaultLoggerFactory defaultLoggerFactory)
        {
            _name = name;
            _defaultLoggerFactory = defaultLoggerFactory;
        }

        public void Debug(string message) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Debug, message);

        public void Debug(string message, Exception exception) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Debug, message + Environment.NewLine + exception);

        public void DebugFormat(string format, params object[] args) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Debug, string.Format(format, args));

        public void Info(string message) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Info, message);

        public void Info(string message, Exception exception) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Info, message + Environment.NewLine + exception);

        public void InfoFormat(string format, params object[] args) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Info, string.Format(format, args));

        public void Warn(string message) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Warn, message);

        public void Warn(string message, Exception exception) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Warn, message + Environment.NewLine + exception);

        public void WarnFormat(string format, params object[] args) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Warn, string.Format(format, args));

        public void Error(string message) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Error, message);

        public void Error(string message, Exception exception) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Error, message + Environment.NewLine + exception);

        public void ErrorFormat(string format, params object[] args) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Error, string.Format(format, args));

        public void Fatal(string message) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Fatal, message);

        public void Fatal(string message, Exception exception) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Fatal, message + Environment.NewLine + exception);

        public void FatalFormat(string format, params object[] args) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Fatal, string.Format(format, args));

        public void Verbose(string message) => 
            _defaultLoggerFactory.Write(_name, LogLevel.Verbose, message);

        public void Verbose(string message, Exception exception) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Verbose, message + Environment.NewLine + exception);

        public void VerboseFormat(string format, params object[] args) 
            => _defaultLoggerFactory.Write(_name, LogLevel.Verbose, string.Format(format, args));
    }
}
