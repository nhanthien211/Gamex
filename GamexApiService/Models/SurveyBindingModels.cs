using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamexApiService.Models {
    public class SurveyQuestionAnswerBindingModel {
        [Required]
        public int QuestionId { get; set; }

        public int? ProposedAnswerId { get; set; }
        public List<int> ProposedAnswerIds { get; set; }
        public string Other { get; set; }
    }


    public class SurveyAnswerBindingModel {
        [Required] 
        public int SurveyId { get; set; }

        public List<SurveyQuestionAnswerBindingModel> SurveyAnswers { get; set; }
    }
}