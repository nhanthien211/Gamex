using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class CompanyRegisterViewModel
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters")]
        [Display(Name = "Company Name *")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tax number is required")]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        [Display(Name = "Tax Number *")]
        public string TaxNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        [Display(Name = "Email *")]
        public string Email { get; set; }

        
        [RegularExpression("/^\\+?([0-9]){10,}$", ErrorMessage = "Invalid phone")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [StringLength(256, ErrorMessage = "Website cannot exceed 256 characters")]
        [Display(Name = "Website")]
        [RegularExpression("^((https?):[/][/])?([\\da-z.-]+)[.]([a-z.]{2,6})([/\\w.-]*)*[/]?$", ErrorMessage = "Invalid website format")]
        public string Website { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        [Display(Name = "Your Email *")]
        public string EmployeeEmail { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        [Display(Name = "Your First Name *")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        [Display(Name = "Your Last Name *")]
        public string LastName { get; set; }

        public bool? IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }
    }
}

