using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class LunchProviderActivatedForDate : MenuEvent
    {
        public LunchProviderActivatedForDate(): base()
        {}

        public LunchProviderActivatedForDate(Guid lunchProviderId, DateTime affectedDate, string editorUserId)
            : base(lunchProviderId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
