using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }

        public string UserId { get; set; }

    }
}
