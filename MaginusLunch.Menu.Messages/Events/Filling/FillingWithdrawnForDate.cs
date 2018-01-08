using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingWithdrawnForDate : MenuEvent
    {
        public FillingWithdrawnForDate()
            : base()
        { }

        public FillingWithdrawnForDate(Guid fillingId, DateTime withdrawnOn, string editorUserId)
            : base(fillingId, editorUserId)
        {
            WithdrawnOn = withdrawnOn;
        }
        public DateTime WithdrawnOn { get; set; }
    }
}
