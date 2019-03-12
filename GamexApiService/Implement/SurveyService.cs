using System.Collections.Generic;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class SurveyService : ISurveyService {
        private IRepository<Survey> _surveyRepo;
        private IUnitOfWork _unitOfWork;

        public SurveyService(
            IRepository<Survey> surveyRepo,
            IUnitOfWork unitOfWork) {
            _surveyRepo = surveyRepo;
            _unitOfWork = unitOfWork;
        }

        public List<SurveyViewModel> GetSurveys(string exhibitionId, string companyId) {
            var surveys = _surveyRepo.GetList(
                s => s.IsActive && s.ExhibitionId.Equals(exhibitionId) && s.CompanyId.Equals(companyId));

            var surveyViewModel = surveys.Select(s => new SurveyViewModel {
                SurveyId = s.SurveyId,
                ExhibitionId = s.ExhibitionId,
                CompanyId = s.CompanyId,
                AccountId = s.AccountId,
                Description = s.Description,
                Point = s.Point,
                Title = s.Title,
                Questions = s.Question.Select(q => new QuestionViewModel{
                    QuestionId = q.QuestionId,
                    SurveyId = q.SurveyId,
                    QuestionType = q.QuestionType,
                    Content = q.Content,
                    ProposedAnswers = q.ProposedAnswer.Select(a => new ProposedAnswerViewModel {
                        ProposedAnswerId = a.ProposedAnswerId,
                        QuestionId = a.QuestionId,
                        Content = a.Content
                    }).ToList()
                }).ToList()
            }).ToList();

            return surveyViewModel;
        }
    }
}