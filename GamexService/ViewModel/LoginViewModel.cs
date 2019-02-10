using System;
using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public String ErrorMessage { get; set; }
    }
}
