using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class BreadNameChanged : MenuEvent
    {
        public BreadNameChanged() 
            : base()
        { }

        public BreadNameChanged(Guid breadId, string newName, string editorUserId) 
            : base(breadId, editorUserId)
        {
            Name = newName;
        }

        public string Name { get; set; }
    }
}
