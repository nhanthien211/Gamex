using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class AttendedCompanyDetailViewModel
    {
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string ExhibitionId { get; set; }
        public List<BootViewModel> Booth { get; set; }
        public bool? IsSuccessful { get; set; }
    }

    public class BootViewModel
    {
        [Required(ErrorMessage = "Field required")]
        [StringLength(5, ErrorMessage = "Cannot exceed 5 characters")]
        public string BoothNumber { get; set; }
    }
}
