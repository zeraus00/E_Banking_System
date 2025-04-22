using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class EmailChangeViewModel
    {
        [Required(AllowEmptyStrings=false, ErrorMessage = "Please provide an Email.")]
        [EmailAddress]
        public string? EmailNew { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide an Email.")]
        [EmailAddress]
        public string? EmailConfirm { get; set; }

    }
}
