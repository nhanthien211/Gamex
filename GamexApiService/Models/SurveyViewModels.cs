using System.Collections.Generic;

namespace GamexApiService.Models {
    public class SurveyShortViewModel {
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public int Point { get; set; }
        public string Description { get; set; }
        public bool IsTaken { get; set; }
    }

    public class SurveyDetailViewModel {
        public int SurveyId { get; set; }
        public string ExhibitionId { get; set; }
        public string Title { get; set; }
        public int Point { get; set; }

        public List<QuestionViewModel> Questions { get; set; }
    }
}