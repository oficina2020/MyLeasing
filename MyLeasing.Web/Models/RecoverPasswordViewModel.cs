using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
