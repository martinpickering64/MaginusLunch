using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingNameChanged : MenuEvent
    {
        public FillingNameChanged() 
            : base()
        { }

        public FillingNameChanged(Guid fillingId, string newName, string editorUserId) 
            : base(fillingId, editorUserId)
        {
            Name = newName;
        }

        public string Name { get; set; }
    }
}
