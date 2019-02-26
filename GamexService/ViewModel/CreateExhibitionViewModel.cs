using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using GamexService.Utilities;

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

        [Required(ErrorMessage = "Required")]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Required")]
        [FileImage(ErrorMessage = "Upload file must be .JPG .PNG or .JPEG")]
        [FileLength(MaxSize = 10 * 1024 * 1024, ErrorMessage = "Upload File must be under 10MB")]
        [Display(Name = "Exhibition Cover Image")]
        public HttpPostedFileBase Logo { get; set; }

        public string Qr { get; set; }




    }
}
