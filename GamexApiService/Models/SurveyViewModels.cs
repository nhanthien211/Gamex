using System.Collections.Generic;

namespace GamexApiService.Models {
    public class SurveyViewModel {
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int  Point { get; set; }
        public string AccountId { get; set; }
        public string CompanyId { get; set; }
        public string ExhibitionId { get; set; }

        public List<QuestionViewModel> Questions { get; set; }
    }
}