using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class LunchProviderDateWithdrawn : MenuEvent
    {
        public LunchProviderDateWithdrawn()
            : base()
        {}

        public LunchProviderDateWithdrawn(Guid lunchProviderId, DateTime affectedDate, string editorUserId)
            : base(lunchProviderId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
