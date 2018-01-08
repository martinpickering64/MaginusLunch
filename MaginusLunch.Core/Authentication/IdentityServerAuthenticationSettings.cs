namespace MaginusLunch.Core.Authentication
{
    public class IdentityServerAuthenticationSettings
    {
        public const string DefaultIdentityServerAuthenticationSettingsSectionName = "IdentityServerAuthentication";

        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string ApiName { get; set; }
    }
}
