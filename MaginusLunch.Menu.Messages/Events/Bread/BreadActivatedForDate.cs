using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class BreadActivatedForDate : MenuEvent
    {
        public BreadActivatedForDate()
            : base()
        {}

        public BreadActivatedForDate(Guid breadId, DateTime affectedDate, string editorUserId) 
            : base(breadId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
