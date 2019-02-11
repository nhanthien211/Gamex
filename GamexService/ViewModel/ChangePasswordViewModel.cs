using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Please enter old password")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please enter new password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Re-enter Password")]
        [Compare("NewPassword", ErrorMessage = "Must match your new password")]
        public string ConfirmPassword { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}
