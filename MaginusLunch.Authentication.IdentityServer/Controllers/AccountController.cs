using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MaginusLunch.Authentication.IdentityServer.Models;
using MaginusLunch.Authentication.IdentityServer.Models.AccountViewModels;
using MaginusLunch.Authentication.IdentityServer.Services;
using MaginusLunch.Authentication.IdentityServer.Attributes;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace MaginusLunch.Authentication.IdentityServer.Controllers
{
    [Authorize]
    [ResponseSecurityHeaders]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly AccountService _account;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IIdentityServerInteractionService interaction,
            IHttpContextAccessor httpContext,
            IClientStore clientStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _interaction = interaction;
            _clientStore = clientStore;

            _account = new AccountService(interaction, httpContext, clientStore);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = await _account.BuildLoginViewModelAsync(returnUrl);

            //if (vm.IsExternalLoginOnly)
            if (vm.IsExternalLoginOnly || vm.ExternalProviders.Count() == 1)
            {
                // only one option for logging in
                return ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await _account.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(vm.ExternalAuthenticationScheme,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException) // this is for the external providers that don't have signout
                {
                }
                catch (InvalidOperationException) // this is for Windows/Negotiate
                {
                }
            }

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            return View("LoggedOut", vm);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _userManager.RefreshExternalClaims(info);
                var theUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return Forbid();
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                user.AddExternalClaims(info.Principal);
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                var configuredRedirectUrl = Startup.Configuration.GetSection("RedirectionUrls")?["SafeHomeUrl"];
                return Redirect(string.IsNullOrWhiteSpace(configuredRedirectUrl)
                    ? "http://www.maginus.com/"
                    : configuredRedirectUrl);
            }
        }

        #endregion
    }





    //public class AccountController : Controller
    //{
    //    private readonly UserManager<ApplicationUser> _userManager;
    //    private readonly SignInManager<ApplicationUser> _signInManager;
    //    private readonly ILogger _logger;
    //    private readonly IIdentityServerInteractionService _interaction;
    //    private readonly IClientStore _clientStore;
    //    private readonly AccountService _account;

    //    public AccountController(
    //        UserManager<ApplicationUser> userManager,
    //        SignInManager<ApplicationUser> signInManager,
    //        ILoggerFactory loggerFactory,
    //        IIdentityServerInteractionService interaction,
    //        IHttpContextAccessor httpContext,
    //        IClientStore clientStore)
    //    {
    //        _userManager = userManager;
    //        _signInManager = signInManager;
    //        _logger = loggerFactory.CreateLogger<AccountController>();
    //        _interaction = interaction;
    //        _clientStore = clientStore;

    //        _account = new AccountService(interaction, httpContext, clientStore);
    //    }

    //    //
    //    // GET: /Account/Login
    //    [AllowAnonymous]
    //    [HttpGet]
    //    public async Task<IActionResult> Login(string returnUrl)
    //    {
    //        var vm = await _account.BuildLoginViewModelAsync(returnUrl);

    //        if (vm.IsExternalLoginOnly)
    //        {
    //            // only one option for logging in
    //            return ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
    //        }

    //        return View(vm);
    //    }

    //    /// <summary>
    //    /// Show logout page
    //    /// </summary>
    //    [AllowAnonymous]
    //    [HttpGet]
    //    public async Task<IActionResult> Logout(string logoutId)
    //    {
    //        var vm = await _account.BuildLogoutViewModelAsync(logoutId);

    //        if (vm.ShowLogoutPrompt == false)
    //        {
    //            // no need to show prompt
    //            return await Logout(vm);
    //        }

    //        return View(vm);
    //    }

    //    /// <summary>
    //    /// Handle logout page postback
    //    /// </summary>
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Logout(LogoutViewModel model)
    //    {
    //        var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
    //        if (vm.TriggerExternalSignout)
    //        {
    //            string url = Url.Action("Logout", new { logoutId = vm.LogoutId });
    //            try
    //            {
    //                // hack: try/catch to handle social providers that throw
    //                await HttpContext.Authentication.SignOutAsync(vm.ExternalAuthenticationScheme,
    //                    new AuthenticationProperties { RedirectUri = url });
    //            }
    //            catch (NotSupportedException) // this is for the external providers that don't have signout
    //            {
    //            }
    //            catch (InvalidOperationException) // this is for Windows/Negotiate
    //            {
    //            }
    //        }

    //        // delete authentication cookie
    //        await _signInManager.SignOutAsync();

    //        return View("LoggedOut", vm);
    //    }

    //    //
    //    // POST: /Account/ExternalLogin
    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    //    {
    //        // Request a redirect to the external login provider.
    //        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
    //        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    //        return Challenge(properties, provider);
    //    }

    //    //
    //    // GET: /Account/ExternalLoginCallback
    //    [HttpGet]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    //    {
    //        if (remoteError != null)
    //        {
    //            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
    //            return View(nameof(Login));
    //        }
    //        var info = await _signInManager.GetExternalLoginInfoAsync();
    //        if (info == null)
    //        {
    //            return RedirectToAction(nameof(Login));
    //        }

    //        // Sign in the user with this external login provider if the user already has a login.
    //        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
    //        if (result.Succeeded)
    //        {
    //            _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
    //            return RedirectToLocal(returnUrl);
    //        }
    //        else
    //        {
    //            // If the user does not have an account, then ask the user to create an account.
    //            ViewData["ReturnUrl"] = returnUrl;
    //            ViewData["LoginProvider"] = info.LoginProvider;
    //            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
    //        }
    //    }

    //    //
    //    // POST: /Account/ExternalLoginConfirmation
    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            // Get the information about the user from the external login provider
    //            var info = await _signInManager.GetExternalLoginInfoAsync();
    //            if (info == null)
    //            {
    //                return View("ExternalLoginFailure");
    //            }
    //            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
    //            var result = await _userManager.CreateAsync(user);
    //            if (result.Succeeded)
    //            {
    //                result = await _userManager.AddLoginAsync(user, info);
    //                if (result.Succeeded)
    //                {
    //                    await _signInManager.SignInAsync(user, isPersistent: false);
    //                    _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
    //                    return RedirectToLocal(returnUrl);
    //                }
    //            }
    //            AddErrors(result);
    //        }

    //        ViewData["ReturnUrl"] = returnUrl;
    //        return View(model);
    //    }

    //    #region Helpers

    //    private void AddErrors(IdentityResult result)
    //    {
    //        foreach (var error in result.Errors)
    //        {
    //            ModelState.AddModelError(string.Empty, error.Description);
    //        }
    //    }

    //    private IActionResult RedirectToLocal(string returnUrl)
    //    {
    //        if (Url.IsLocalUrl(returnUrl))
    //        {
    //            return Redirect(returnUrl);
    //        }
    //        else
    //        {
    //            var configuredRedirectUrl = Startup.Configuration.GetSection("RedirectionUrls")["SafeHomeUrl"];
    //            return Redirect(string.IsNullOrWhiteSpace(configuredRedirectUrl)
    //                ? "http://www.maginus.com/"
    //                : configuredRedirectUrl);
    //        }
    //    }

    //    #endregion
    //}
}
