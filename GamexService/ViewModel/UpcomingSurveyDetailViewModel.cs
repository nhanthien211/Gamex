using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class UpcomingSurveyDetailViewModel
    {
        [Required(ErrorMessage = "Title required")]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description required")]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public int SurveyId { get; set; }

        public string ExhibitionId { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}
