using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class ContactNumberChangeViewModel
    {
        [Required(ErrorMessage = "Please provide a number.")]
        public string? ContactNew { get; set; }
        [Required(ErrorMessage = "Please provide a number.")]
        public string? ContactConfirm { get; set; }
    }
}
