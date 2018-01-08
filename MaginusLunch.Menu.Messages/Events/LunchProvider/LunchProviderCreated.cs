using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class LunchProviderCreated : MenuEvent
    {
        public LunchProviderCreated()
            : base()
        { }

        public LunchProviderCreated(Guid lunchProviderId, string name, string editorUserId)
            : base(lunchProviderId, editorUserId)
        {
            Name = name;
            Status = LunchProviderStatus.Available;
        }
        public string Name { get; set; }

        public LunchProviderStatus Status { get; set; }
    }
}
