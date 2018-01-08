using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class BreadCreated : MenuEvent
    {
        public BreadCreated()
            : this (Guid.Empty, string .Empty, Guid.Empty, string.Empty)
        { }

        public BreadCreated(Guid breadId, string name, Guid lunchProviderId, string editorUserId)
            : base(breadId, editorUserId)
        {
            Name = name;
            LunchProviderId = lunchProviderId;
            Status = BreadStatus.Available;
        }
        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }

        public BreadStatus Status { get; set; }
    }
}
