using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface ISurveyService {
        List<SurveyShortViewModel> GetSurveys(string accountId, string exhibitionId, string companyId);
        SurveyDetailViewModel GetSurvey(int id);
        bool SubmitSurvey(string accountId, SurveyAnswerBindingModel surveyAnswerModel);
    }
}