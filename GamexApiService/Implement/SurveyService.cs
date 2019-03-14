using System;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;
using System.Collections.Generic;
using System.Linq;

namespace GamexApiService.Implement {
    public class SurveyService : ISurveyService {
        private IRepository<Survey> _surveyRepo;
        private IRepository<SurveyAnswer> _surveyAnswerRepo;
        private IRepository<SurveyParticipation> _surveyParticipationRepo;
        private IUnitOfWork _unitOfWork;

        public SurveyService(
            IRepository<Survey> surveyRepo,
            IRepository<SurveyAnswer> surveyAnswerRepo,
            IRepository<SurveyParticipation> surveyParticipationRepo,
            IUnitOfWork unitOfWork) {
            _surveyRepo = surveyRepo;
            _surveyAnswerRepo = surveyAnswerRepo;
            _surveyParticipationRepo = surveyParticipationRepo;
            _unitOfWork = unitOfWork;
        }

        public List<SurveyShortViewModel> GetSurveys(string accountId, string exhibitionId, string companyId) {
            var surveys = _surveyRepo.GetList(
                s => s.IsActive && s.ExhibitionId.Equals(exhibitionId) && s.CompanyId.Equals(companyId));

            var takenSurveys = _surveyParticipationRepo.GetList(
                sp => sp.AccountId.Equals(accountId));

            var surveyViewModel = surveys.Select(s => new SurveyShortViewModel() {
                SurveyId = s.SurveyId,
                Title = s.Title,
                Description = s.Description,
                Point = s.Point,
                IsTaken = takenSurveys.Where(takenSurvey => takenSurvey.SurveyId == s.SurveyId).ToList().Count > 0
            }).ToList();

            return surveyViewModel;
        }

        public SurveyDetailViewModel GetSurvey(int surveyId) {
            var survey = _surveyRepo.GetSingle(s => s.SurveyId == surveyId, s => s.Question.Select(q => q.ProposedAnswer));
            return new SurveyDetailViewModel() {
                SurveyId = survey.SurveyId,
                Title = survey.Title,
                Point = survey.Point,
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

        public bool SubmitSurvey(string accountId, SurveyAnswerBindingModel surveyAnswerModel) {
            var surveyId = surveyAnswerModel.SurveyId;
            surveyAnswerModel.SurveyAnswers.ForEach(surveyAnswer => {
                var surveyAnswerRecord = new SurveyAnswer {
                    AccountId = accountId,
                    SurveyId = surveyId,
                    QuestionId = surveyAnswer.QuestionId,
                    ProposedAnswerId = surveyAnswer.ProposedAnswerId,
                    Other = surveyAnswer.Other
                };
                _surveyAnswerRepo.Insert(surveyAnswerRecord);
            });

            try {
                var affectedRows = _unitOfWork.SaveChanges();
                return affectedRows == surveyAnswerModel.SurveyAnswers.Count;
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}