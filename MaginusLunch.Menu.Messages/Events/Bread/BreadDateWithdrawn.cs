using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class BreadDateWithdrawn : MenuEvent
    {
        public BreadDateWithdrawn(): base()
        {}

        public BreadDateWithdrawn(Guid breadId, DateTime affectedDate, string editorUserId) 
            : base(breadId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
