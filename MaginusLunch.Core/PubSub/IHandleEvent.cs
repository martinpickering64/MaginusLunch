using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Core.PubSub
{
    public interface IHandleEvent<TEvent> where TEvent : Events.Event
    {
        void Handle(TEvent theEvent);
        Task HandleAsync(TEvent theEvent, CancellationToken cancellationToken = default);
    }
}
