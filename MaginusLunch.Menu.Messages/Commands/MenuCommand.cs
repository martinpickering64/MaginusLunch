using MaginusLunch.Core.Messages.Commands;
using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public abstract class MenuCommand : AbstractCommand
    {
        public Guid Id { get; set; }

        public string EditorUserId { get; set; }
    }
}
