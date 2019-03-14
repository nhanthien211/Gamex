using GamexService.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace GamexService.ViewModel
{
    public class CreateExhibitionViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Exhibition Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Cannot exceed 1000 characters")]
        [Display(Name = "Exhibition Description")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Start date required")]
        [Display(Name = "From")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}" )]
        [DataType(DataType.DateTime)]
        [StartDate (ErrorMessage = "Start date must be after current date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date required")]
        [Display(Name = "To")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        [DataType(DataType.DateTime)]
        [EndDate(StartDateProperty = "StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Required")]
        [FileImage(ErrorMessage = "Upload file must be .JPG .PNG or .JPEG")]
        [FileLength(MaxSize = 10 * 1024 * 1024, ErrorMessage = "Upload File must be under 10MB")]
        [Display(Name = "Exhibition Cover Image")]
        public HttpPostedFileBase Logo { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
