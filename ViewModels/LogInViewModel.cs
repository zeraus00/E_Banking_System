using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class LogInViewModel
    {
        [Required(AllowEmptyStrings=false, ErrorMessage = "Please provide an Email.")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings=false, ErrorMessage = "Please provide a Password.")]
        public string? Password { get; set; }
    }
}
