using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;
using System.Collections.Generic;
using System.Linq;

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

        public List<SurveyShortViewModel> GetSurveys(string exhibitionId, string companyId) {
            var surveys = _surveyRepo.GetList(
                s => s.IsActive && s.ExhibitionId.Equals(exhibitionId) && s.CompanyId.Equals(companyId));

            var surveyViewModel = surveys.Select(s => new SurveyShortViewModel() {
                SurveyId = s.SurveyId,
                Title = s.Title,
                Description = s.Description,
                Point = s.Point
            }).ToList();

            return surveyViewModel;
        }

        public SurveyDetailViewModel GetSurvey(int surveyId) {
            var survey = _surveyRepo.GetById(surveyId);
            return new SurveyDetailViewModel() {
                SurveyId = survey.SurveyId,
                Title = survey.Title,
                Questions = survey.Question.Select(q => new QuestionViewModel {
                    QuestionId = q.QuestionId,
                    QuestionType = q.QuestionType,
                    Content = q.Content,
                    ProposedAnswers = q.ProposedAnswer.Select(a => new ProposedAnswerViewModel {
                        ProposedAnswerId = a.ProposedAnswerId,
                        Content = a.Content
                    }).ToList()
                }).ToList()
            };
        }
    }
}