using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Messages.Commands;

namespace MaginusLunch.Core.Validation
{
    public interface IValidateCommandsFactory<TheAggregate>
        where TheAggregate : AggregateRoot
    {
        IValidateCommands<CommandType, TheAggregate> GetValidatorFor<CommandType>() where CommandType : AbstractCommand;

    }
}
