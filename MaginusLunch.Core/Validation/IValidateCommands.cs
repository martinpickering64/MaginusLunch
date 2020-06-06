using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Core.Validation
{
    public interface IValidateCommands<TheCommand, TheAggregate>
        where TheCommand : Commands.Command
        where TheAggregate : Aggregates.AggregateRoot
    {
        Task<ValidationStatus> IsValidAsync(TheCommand command, TheAggregate aggregate, CancellationToken cancellationToken = default(CancellationToken));
    }
}
