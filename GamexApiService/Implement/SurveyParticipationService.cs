using System;
using GamexApiService.Interface;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class SurveyParticipationService : ISurveyParticipationService {
        private IRepository<SurveyParticipation> _surveyParticipationRepo;
        private IUnitOfWork _unitOfWork;

        public SurveyParticipationService(
            IRepository<SurveyParticipation> surveyParticipationRepo,
            IUnitOfWork unitOfWork) {
            _surveyParticipationRepo = surveyParticipationRepo;
            _unitOfWork = unitOfWork;
        }

        public bool CompleteSurvey(string accountId, int surveyId) {
            var surveyParticipation = new SurveyParticipation {
                AccountId = accountId,
                SurveyId = surveyId,
                CompleteDate = DateTime.Now
            };
            _surveyParticipationRepo.Insert(surveyParticipation);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                return affectedRows == 1;
            } catch (Exception e) {
                return false;
            }
        }
    }
}