using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingActivatedForDate : MenuEvent
    {
        public FillingActivatedForDate(): base()
        {}

        public FillingActivatedForDate(Guid fillingId, DateTime activatedOn, string editorUserId) 
            : base(fillingId, editorUserId)
        {
            ActivatedOn = activatedOn;
        }

        public DateTime ActivatedOn { get; set; }
    }
}
