using System;
using System.ComponentModel.DataAnnotations;
using GamexService.Utilities;

namespace GamexService.ViewModel
{
    public class CreateRewardViewModel
    {
        [Display(Name = "Reward Description")]
        [Required(ErrorMessage = "Field required")]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        public string Description { get; set; }

        [Display(Name = "Reward Code")]
        [Required(ErrorMessage = "Field required")]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Must be positive integer")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Point Cost")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Must be positive integer")]
        public int PointCost { get; set; }

        [Required(ErrorMessage = "Start date required")]
        [Display(Name = "From")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        [DataType(DataType.DateTime)]
        [StartDate(ErrorMessage = "Start date must be after current date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date required")]
        [Display(Name = "To")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        [DataType(DataType.DateTime)]
        [EndDate(StartDateProperty = "StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
