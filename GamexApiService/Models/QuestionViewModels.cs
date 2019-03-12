using System.Collections.Generic;

namespace GamexApiService.Models {
    public class QuestionViewModel {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public int SurveyId { get; set; }
        public int QuestionType { get; set; }
        public List<ProposedAnswerViewModel> ProposedAnswers { get; set; }
    }
}