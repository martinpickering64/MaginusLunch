using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class LunchProviderWithdrawn : MenuEvent
    {
        public LunchProviderWithdrawn()
            : base()
        { }

        public LunchProviderWithdrawn(Guid lunchProviderId, string editorUserId)
            : base(lunchProviderId, editorUserId)
        { }
    }
}
