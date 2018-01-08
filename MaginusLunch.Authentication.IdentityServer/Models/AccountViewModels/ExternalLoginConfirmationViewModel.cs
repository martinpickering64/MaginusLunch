using System.ComponentModel.DataAnnotations;

namespace MaginusLunch.Authentication.IdentityServer.Models.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
