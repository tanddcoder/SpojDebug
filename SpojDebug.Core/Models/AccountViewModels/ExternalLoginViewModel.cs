using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
