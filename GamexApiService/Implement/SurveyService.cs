using System;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
            var survey = _surveyRepo.GetSingle(
                s => s.SurveyId == surveyId,
                q => q.Question.Select(a => a.ProposedAnswer));
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
            var insertedRowCount = 0;

            surveyAnswerModel.SurveyAnswers.ForEach(surveyAnswer => {
                var surveyAnswerRecord = new SurveyAnswer {
                    AccountId = accountId,
                    SurveyId = surveyId,
                    QuestionId = surveyAnswer.QuestionId
                };
                if (surveyAnswer.ProposedAnswerId != null) {
                    // question that has only one answer
                    surveyAnswerRecord.ProposedAnswerId = surveyAnswer.ProposedAnswerId;
                    _surveyAnswerRepo.Insert(surveyAnswerRecord);
                    ++insertedRowCount;
                } else if (surveyAnswer.ProposedAnswerIds != null) {
                    // question that has many answers
                    surveyAnswer.ProposedAnswerIds.ForEach(proposedAnswerId => {
                        var surveyAnswerRecordMultiple = surveyAnswerRecord.Clone();
                        surveyAnswerRecordMultiple.ProposedAnswerId = proposedAnswerId;
                        _surveyAnswerRepo.Insert(surveyAnswerRecordMultiple);
                        ++insertedRowCount;
                    });
                } else {
                    // question type: text
                    surveyAnswerRecord.Other = surveyAnswer.Other;
                    _surveyAnswerRepo.Insert(surveyAnswerRecord);
                    ++insertedRowCount;
                }
            });

            try {
                var affectedRows = _unitOfWork.SaveChanges();
                return affectedRows == insertedRowCount;
            } catch (Exception ex) {
                return false;
            }
        }
    }

    static class SystemExtension {
        public static T Clone<T>(this T source) {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
