using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuOptionActivatedForDate : MenuEvent
    {
        public MenuOptionActivatedForDate()
            : base()
        {}

        public MenuOptionActivatedForDate(Guid menuOptionId, DateTime affectedDate, string editorUserId) 
            : base(menuOptionId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
