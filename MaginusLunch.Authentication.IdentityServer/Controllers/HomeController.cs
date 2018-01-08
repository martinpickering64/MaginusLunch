using Microsoft.AspNetCore.Mvc;
using MaginusLunch.Authentication.IdentityServer.Attributes;
using IdentityServer4.Services;
using System.Threading.Tasks;
using MaginusLunch.Authentication.IdentityServer.Models;

namespace MaginusLunch.Authentication.IdentityServer.Controllers
{
    [ResponseSecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        public IActionResult Index()
        {
            return Unauthorized();
            //return View();
        }

        /// <summary>
        /// retrieve error details from identityserver
        /// </summary>
        /// <param name="errorId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel
            {
                Error = await _interaction.GetErrorContextAsync(errorId)
            };
            return View("Error", vm);
        }
    }
}
