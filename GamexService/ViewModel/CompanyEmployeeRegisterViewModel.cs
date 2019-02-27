using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexService.Utilities;

namespace GamexService.ViewModel
{
    public class CompanyEmployeeRegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username *")]
        [Username(ErrorMessage = "At lease 6 characters and only letters, digit, underscore, hyphen and dot allowed")]
        [StringLength(256, ErrorMessage = "Maximum 256 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        [Display(Name = "Email *")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Re-enter Password")]
        [Compare("Password", ErrorMessage = "Must match your password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        [Display(Name = "First Name *")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        public string CompanyId { get; set; }

        public string CompanyName { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
