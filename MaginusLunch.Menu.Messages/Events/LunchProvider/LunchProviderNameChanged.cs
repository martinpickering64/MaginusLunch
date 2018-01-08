using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class LunchProviderNameChanged : MenuEvent
    {
        public LunchProviderNameChanged() 
            : base()
        { }

        public LunchProviderNameChanged(Guid lunchProviderId, string newName, string editorUserId) 
            : base(lunchProviderId, editorUserId)
        {
            Name = newName;
        }

        public string Name { get; set; }
    }
}
