using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamexService.ViewModel
{
    public class CreateSurveyViewModel
    {   
        [Required(ErrorMessage = "Title required")]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description required")]
        [StringLength(1000, ErrorMessage = "Cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string ExhibitionId { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
