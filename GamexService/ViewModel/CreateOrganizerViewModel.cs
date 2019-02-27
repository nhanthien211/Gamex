using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class CreateOrganizerViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        [Display(Name = "Email *")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        [Display(Name = "First Name *")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        public string ErrorMessage { get; set; }

    }
}
