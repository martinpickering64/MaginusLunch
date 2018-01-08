using EventStore.ClientAPI;
using MaginusLunch.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace MaginusLunch.Orders.Denormalizer.Console
{
    public static class EventExtensions
    {
        public const string EventClrTypeHeader = "EventClrTypeName";
        private static ILog _recordedEventLogger = LogManager.GetLogger(typeof(RecordedEvent));
        private static ILog _resolvedEventLogger = LogManager.GetLogger(typeof(ResolvedEvent));
        public static object DeserializeEvent(this RecordedEvent theEvent)
        {
            if (_recordedEventLogger.IsDebugEnabled)
            {
                var metaData = JObject.Parse(Encoding.UTF8.GetString(theEvent.Metadata));
                var eventData = JObject.Parse(Encoding.UTF8.GetString(theEvent.Data));
                var msg = string.Concat("Event on ",
                    theEvent.EventStreamId,
                    " :\r\n{\r\n   'MetaData' : ",
                    metaData,
                    ",\r\n   'EventData' : ",
                    eventData,
                    "\r\n}\r\n");
                _recordedEventLogger.Debug(msg);
            }
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(theEvent.Metadata))
                                                    .Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(theEvent.Data),
                                                    Type.GetType((string)eventClrTypeName));
        }
        public static object DeserializeEvent(this ResolvedEvent theEvent)
        {
            if (_resolvedEventLogger.IsDebugEnabled)
            {
                var metaData = JObject.Parse(Encoding.UTF8.GetString(theEvent.Event.Metadata));
                var eventData = JObject.Parse(Encoding.UTF8.GetString(theEvent.Event.Data));
                var msg = string.Concat("Event #",
                    theEvent.OriginalEventNumber,
                    " (",
                    theEvent.OriginalStreamId,
                    ") :\r\n{\r\n   'MetaData' : ",
                    metaData,
                    ",\r\n   'EventData' : ",
                    eventData,
                    "\r\n}\r\n");
                _resolvedEventLogger.Debug(msg);
            }
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(theEvent.Event.Metadata))
                                                    .Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(theEvent.Event.Data),
                                                    Type.GetType((string)eventClrTypeName));
        }
    }
}
