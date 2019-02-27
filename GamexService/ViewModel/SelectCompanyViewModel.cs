using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class SelectCompanyViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters")]
        [Display(Name = "Find your company by tax identification")]
        public string CompanyTaxId { get; set; }

        public string CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string ErrorMessage { get; set; }

        public int Status { get; set; }
    }
}
