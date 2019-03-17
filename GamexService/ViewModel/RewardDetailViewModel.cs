using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexService.Utilities;

namespace GamexService.ViewModel
{
    public class RewardDetailViewModel
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
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date required")]
        [Display(Name = "To")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        [DataType(DataType.DateTime)]
        [EndDate(StartDateProperty = "StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public int? RewardId { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
