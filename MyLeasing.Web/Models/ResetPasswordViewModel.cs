using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Token { get; set; }

    }
}