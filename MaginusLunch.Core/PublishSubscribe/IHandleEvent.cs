using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Core.PublishSubscribe
{
    public interface IHandleEvent<TEvent> where TEvent : class
    {
        void Handle(TEvent theEvent);
        Task HandleAsync(TEvent theEvent, CancellationToken cancellationToken = default(CancellationToken));
    }
}
