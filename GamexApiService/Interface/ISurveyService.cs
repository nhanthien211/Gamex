using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface ISurveyService {
        List<SurveyViewModel> GetSurveys(string exhibitionId, string companyId);
    }
}