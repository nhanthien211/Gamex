namespace GamexApiService.Interface {
    public interface ISurveyParticipationService {
        bool CompleteSurvey(string accountId, int surveyId);
    }
}