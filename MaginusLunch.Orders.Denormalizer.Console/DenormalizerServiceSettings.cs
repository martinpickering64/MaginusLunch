using System;

namespace MaginusLunch.Orders.Denormalizer.Console
{
    public class DenormalizerServiceSettings
    {
        public const string DefaultSectionName = "OrdersDenormalizer";
        public const string DefaultStreamName = "MaginusLunch-Orders";
        public const string DefaultGroupName = "Denormalizer";
        public static readonly TimeSpan DefaultTimeAllowedToShutdown = TimeSpan.FromSeconds(20);
        public DenormalizerServiceSettings()
        {
            StreamName = DefaultStreamName;
            GroupName = DefaultGroupName;
            TimeAllowedToShutdown = DefaultTimeAllowedToShutdown;
        }
        public string StreamName { get; set; }
        public string GroupName { get; set; }
        public TimeSpan TimeAllowedToShutdown { get; set; }
    }
}
