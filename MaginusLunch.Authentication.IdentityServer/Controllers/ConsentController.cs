﻿using IdentityServer4.Services;
using IdentityServer4.Stores;
using MaginusLunch.Authentication.IdentityServer.Attributes;
using MaginusLunch.Authentication.IdentityServer.Models.ConsentViewModels;
using MaginusLunch.Authentication.IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MaginusLunch.Authentication.IdentityServer.Controllers
{
    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    [ResponseSecurityHeaders]
    public class ConsentController : Controller
    {
        private readonly ConsentService _consent;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<ConsentController> logger)
        {
            _consent = new ConsentService(interaction, clientStore, resourceStore, logger);
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _consent.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await _consent.ProcessConsent(model);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }
    }
}