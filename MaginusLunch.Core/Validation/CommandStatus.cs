using MaginusLunch.Core.Validation;

namespace MaginusLunch.Core.Messages.Commands
{
    public class CommandStatus : ValidationStatus
    {
        public static CommandStatus CommandOk = new CommandStatus();

        public AckNackStatus AckNack => IsValid ? AckNackStatus.ACK : AckNackStatus.NACK;

        public enum AckNackStatus
        {
            ACK = 1,
            NACK = 2
        }
    }
}
