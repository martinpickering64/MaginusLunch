using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuOptionCreated : MenuEvent
    {
        public MenuOptionCreated()
            : base()
        { }

        public MenuOptionCreated(Guid MenuOptionId, string name, Guid lunchProviderId, string editorUserId)
            : base(MenuOptionId, editorUserId)
        {
            Name = name;
            LunchProviderId = lunchProviderId;
            Status = MenuOptionStatus.Available;
        }
        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }

        public MenuOptionStatus Status { get; set; }
    }
}
