using MaginusLunch.Core.Messages.Commands;
using MaginusLunch.Menu.Messages.Commands;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Menu.Service
{
    public interface IMenuService : 
        ICalendarService, IBreadService, IFillingService, 
        IMenuOptionService, IMenuCategoryService, ILunchProviderService
    {
        Task<CommandStatus> HandleForUserAsync(ClaimsPrincipal user, 
            MenuCommand command, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
