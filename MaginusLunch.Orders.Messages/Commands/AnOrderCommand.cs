using MaginusLunch.Core.Messages.Commands;
using System;

namespace MaginusLunch.Orders.Messages.Commands
{
    public abstract class AnOrderCommand : AbstractCommand
    {
        public Guid Id { get; set; }

        public string OrderingUserId { get; set; }
    }
}
