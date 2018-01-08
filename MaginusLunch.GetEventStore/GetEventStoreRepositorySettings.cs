using System;
using System.Collections.Generic;
using System.Text;

namespace MaginusLunch.GetEventStore
{
    public class GetEventStoreRepositorySettings
    {
        public const string DefaultGetEventStoreRepositorySectionName = "GetEventStoreRepository";

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionName { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set;}
        public int Port { get; set; }
        public bool UseDebugLogger { get; set; }
        public bool EnableVerboseLogging { get; set; }
        public bool DoNotFailOnNoServerResponse { get; set; }
        public int LimitAttemptsForOperationTo { get; set; }
        public int LimitReconnectionsTo { get; set; }
        public int OperationTimeout { get; set; }
        public bool UseConsoleLogger { get; set; }
        public int ConnectionTimeout { get; set; }
    }
}
