using System.ComponentModel.DataAnnotations;

namespace CartApp.Models
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}