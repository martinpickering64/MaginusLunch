using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public abstract class MenuEvent
    {
        public MenuEvent() : this(Guid.Empty, string.Empty)
        { }

        public MenuEvent(Guid id, string editorUserId)
        {
            Id = id;
            EditorUserId = editorUserId; 
        }

        public Guid Id { get; set; }
        public string EditorUserId { get; set; }

    }
}
