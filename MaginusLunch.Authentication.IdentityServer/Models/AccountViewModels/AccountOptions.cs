using System;

namespace MaginusLunch.Authentication.IdentityServer.Models.AccountViewModels
{
    public static class AccountOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = true;

        public static bool WindowsAuthenticationEnabled = false;
        // specify the Windows authentication schemes you want to use for authentication
        public static readonly string[] WindowsAuthenticationSchemes = new string[] { "Negotiate", "NTLM" };
        public static readonly string WindowsAuthenticationDisplayName = "Windows";

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
