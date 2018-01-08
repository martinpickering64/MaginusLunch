using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class LunchProviderActivated : MenuEvent
    {
        public LunchProviderActivated()
            : base()
        { }

        public LunchProviderActivated(Guid lunchProviderId, string editorUserId)
            : base(lunchProviderId, editorUserId)
        { }
    }
}
