using System;
using System.Collections.Generic;

namespace GamexService.ViewModel
{
    public class SurveyResultViewModel
    {
        public string SurveyTitle { get; set; }
        public List<string> Questions { get; set; }
        public List<SurveyAnswerViewModel> AnswerList { get; set; }
    }

    public class SurveyAnswerViewModel
    {
        
        public string ParticipantName { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<QuestionAnswerViewModel> QuestionList { get; set; }
    }

    public class QuestionAnswerViewModel
    {
        public int QuestionId { get; set; }
        public List<string> QuestionAnswer { get; set; }
    }

}
