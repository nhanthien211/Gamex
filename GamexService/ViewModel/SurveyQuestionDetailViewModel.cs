using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class SurveyQuestionDetailViewModel
    {
        [Required(ErrorMessage = "Field required")]
        [StringLength(1000, ErrorMessage = "Cannot exceed 1000 characters")]
        public string Question { get; set; }
        public int QuestionId { get; set; }
        public int QuestionType { get; set; }
        public List<ProposedAnswerViewModel> Answers { get; set; }
        public string ExhibitionId { get; set; }
        public string SurveyId { get; set; }
        public bool? IsSuccessful { get; set; }
    }

    public class ProposedAnswerViewModel
    {
        [Required(ErrorMessage = "Field required")]
        [StringLength(100, ErrorMessage = "Cannot exceed 1000 characters")]
        public string Content { get; set; }
    }
}
