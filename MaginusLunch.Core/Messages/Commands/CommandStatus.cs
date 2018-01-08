using System.Collections.Generic;
using MaginusLunch.Core.Validation;

namespace MaginusLunch.Core.Messages.Commands
{
    public class CommandStatus : ValidationStatus
    {
        public static CommandStatus CommandOk = new CommandStatus();

        public CommandStatus() : this (new ValidationStatus())
        { }

        public CommandStatus(ValidationStatus incorpratedState)
        {
            _incorporatedState = incorpratedState;
        }

        public AckNackStatus AckNack
        {
            get
            {
                return IsValid ? AckNackStatus.ACK : AckNackStatus.NACK;
            }
        }

        public override bool IsValid => _incorporatedState.IsValid;

        public override IList<Reason> Reasons => _incorporatedState.Reasons;

        public enum AckNackStatus
        {
            ACK = 1,
            NACK = 2
        }

        private ValidationStatus _incorporatedState;
    }
}
