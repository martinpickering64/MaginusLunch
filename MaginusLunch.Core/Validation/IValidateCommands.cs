using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Messages.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Core.Validation
{
    public interface IValidateCommands<TheCommand, TheAggregate>
        where TheCommand : AbstractCommand
        where TheAggregate : AggregateRoot
    {
        Task<ValidationStatus> IsValidAsync(TheCommand command, TheAggregate aggregate, CancellationToken cancellationToken = default(CancellationToken));
    }
}
