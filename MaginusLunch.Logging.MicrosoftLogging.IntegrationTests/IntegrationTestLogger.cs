using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MaginusLunch.Logging.MicrosoftLogging.IntegrationTests
{
    /// <summary>Minimalistic logger for integration testing.</summary>
    public class IntegrationTestLogger : ILogger
    {
        public static IntegrationTestLogger Instance { get; } = new IntegrationTestLogger();

        private IntegrationTestLogger()
        {
            _logs = new List<string>();
        }

        private List<string> _logs;
        public IEnumerable<string> Logs { get { return _logs; } }

        public IDisposable BeginScope<TState>(TState state)
        {
            return (IDisposable)NullScope.Instance;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            _logs.Add($"LogLevel: {logLevel};  {formatter(state, exception)}");
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return true;
        }
    }
    /// <summary>An empty scope without any logic</summary>
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope()
        {}

        public void Dispose()
        {
        }
    }
}
