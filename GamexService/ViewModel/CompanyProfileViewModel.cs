using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GamexService.Utilities;

namespace GamexService.ViewModel
{
    public class CompanyProfileViewModel
    {
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Tax Number")]
        public string TaxNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        [Display(Name = "Email *")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [RegularExpression("/^\\+?([0-9]){10,}$", ErrorMessage = "Invalid phone")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [StringLength(256, ErrorMessage = "Website cannot exceed 256 characters")]
        [Display(Name = "Website")]
        [RegularExpression("^((https?):[/][/])?([\\da-z.-]+)[.]([a-z.]{2,6})([/\\w.-]*)*[/]?$", ErrorMessage = "Invalid website format")]
        public string Website { get; set; }


        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Address cannot exceed 1000 characters")]
        public string Description { get; set; }

        [FileImage(ErrorMessage = "Upload file must be .JPG .PNG or .JPEG")]
        [FileLength(MaxSize = 10 * 1024 * 1024, ErrorMessage = "Upload File must be under 10MB")]
        [Display(Name = "Logo")]
        public HttpPostedFileBase Logo { get; set; }

        public string ImageUrl { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
