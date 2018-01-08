using System;
using Serilog.Core;
using Serilog.Events;

namespace MaginusLunch.Logging.Serilog.IntegrationTests
{
    public class SerilogEventSink : ILogEventSink
    {
        public Action<LogEvent> Action { get; set; }
        public void Emit(LogEvent logEvent)
        {
            Action(logEvent);
        }
    }
}
