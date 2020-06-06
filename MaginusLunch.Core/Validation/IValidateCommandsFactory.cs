namespace MaginusLunch.Core.Validation
{
    public interface IValidateCommandsFactory<TheAggregate>
        where TheAggregate : Aggregates.AggregateRoot
    {
        IValidateCommands<CommandType, TheAggregate> ValidatorFor<CommandType>() 
            where CommandType : Commands.Command;

    }
}
