using GamexEntity;
using GamexEntity.Enumeration;
using GamexRepository;
using GamexService.Interface;
using GamexService.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamexEntity.Constant;
using GamexService.Utilities;
using OfficeOpenXml;

namespace GamexService.Implement
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Exhibition> _exhibitionRepository;
        private readonly IRepository<Booth> _boothRepository;
        private readonly IRepository<Survey> _surveyRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<ProposedAnswer> _proposedAnswerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepository, IRepository<Exhibition> exhibitionRepository, 
            IRepository<Booth> boothRepository, IRepository<Survey> surveyRepository, 
            IRepository<Question> questionRepository, IRepository<ProposedAnswer> proposedAnswerRepository,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _exhibitionRepository = exhibitionRepository;
            _boothRepository = boothRepository;
            _surveyRepository = surveyRepository;
            _questionRepository = questionRepository;
            _proposedAnswerRepository = proposedAnswerRepository;
            _unitOfWork = unitOfWork;
        }

        public SelectCompanyViewModel SelectCompanyRegisterStatus(SelectCompanyViewModel model)
        {
            var company = _companyRepository.GetSingleProjection(
                c => new SelectCompanyViewModel
                {
                    CompanyTaxId = c.TaxNumber,
                    CompanyName = c.Name,
                    Status = c.StatusId,
                    CompanyId = c.CompanyId
                },
                c => string.Equals(c.TaxNumber, model.CompanyTaxId)
                );
            return company;
        }

        public bool RegisterNewCompany(CompanyRegisterViewModel model, string companyId)
        {
            Company company = new Company
            {
                CompanyId = companyId,
                Name =  model.Name,
                Email = model.Email,
                Phone =  model.Phone,
                Website = model.Website,
                TaxNumber = model.TaxNumber,
                StatusId = (int) CompanyStatusEnum.Pending
            };
            _companyRepository.Insert(company);
            try
            {
                int result = _unitOfWork.SaveChanges();
                return result > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool IsCompanyRegistered(string taxNumber)
        {
            var company = _companyRepository.GetSingleProjection(
                c => c.TaxNumber,
                c => string.Equals(c.TaxNumber, taxNumber)
            );
            if (!string.IsNullOrEmpty(company))
            {
                //company  registered
                return true;
            }
            return false;
        }

        public void RemoveCompany(string companyId)
        {
            var company = _companyRepository.GetById(companyId);
            _companyRepository.Delete(company);
            _unitOfWork.SaveChanges();
        }

        public ExhibitionDetailViewOnlyModel GetExhibitionDetail(string exhibitionId, string type)
        {
            switch (type)
            {
                case ExhibitionTypes.New:
                case ExhibitionTypes.Upcoming:
                    return _exhibitionRepository.GetSingleProjection(
                        e => new ExhibitionDetailViewOnlyModel
                        {
                            Description = e.Description,
                            Logo = e.Logo,
                            Name = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Address = e.Address,
                            ExhibitionId = e.ExhibitionId
                        },
                        e => e.ExhibitionId == exhibitionId && e.StartDate > DateTime.Now
                    );
                case ExhibitionTypes.Ongoing:
                    return _exhibitionRepository.GetSingleProjection(
                        e => new ExhibitionDetailViewOnlyModel
                        {
                            Description = e.Description,
                            Logo = e.Logo,
                            Name = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Address = e.Address,
                            ExhibitionId = e.ExhibitionId
                        },
                        e => e.ExhibitionId == exhibitionId && e.StartDate <= DateTime.Now 
                             && e.EndDate >= DateTime.Now
                    );
                case ExhibitionTypes.Past:
                    return _exhibitionRepository.GetSingleProjection(
                        e => new ExhibitionDetailViewOnlyModel
                        {
                            Description = e.Description,
                            Logo = e.Logo,
                            Name = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Address = e.Address,
                            ExhibitionId = e.ExhibitionId
                        },
                        e => e.ExhibitionId == exhibitionId && e.EndDate < DateTime.Now
                    );
                default:
                    return null;
            }
        }

        public string GetCompanyBoothInExhibition(string exhibitionId, string companyId)
        {
            var booth = _boothRepository.GetListProjection(
                b => b.BoothNumber,
                b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId
                ).ToList();
            var result = "";
            foreach (var number in booth)
            {
                result += number + " ";  
            }
            return result;
        }
    
        public bool IsCompanyHasJoinExhibition(string exhibitionId, string companyId)
        {
            return  _boothRepository.GetSingleProjection(
                        b => b.Id,
                            b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId 
                     ) != 0;
        }

        public bool JoinExhibition(string exhibitionId, string companyId)
        {
            var booth = new Booth
            {
                CompanyId = companyId,
                ExhibitionId = exhibitionId,
            };
            _boothRepository.Insert(booth);
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return result > 0;
        }

        public List<ExhibitionTableViewModel> LoadExhibitionDataTable(string type, string sortColumnDirection, string searchValue, int skip, int take, string companyId)
        {
            switch (type)
            {
                case ExhibitionTypes.New:
                    var exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => !e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.StartDate > DateTime.Now
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    var result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
                    }).ToList();
                    return result;
                case ExhibitionTypes.Upcoming:
                    exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.StartDate > DateTime.Now
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
                    }).ToList();
                    return result;
                case ExhibitionTypes.Ongoing:
                    exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now 
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
                    }).ToList();
                    return result;
                case ExhibitionTypes.Past:
                    exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.EndDate < DateTime.Now 
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
                    }).ToList();
                    return result;
                default:
                    return null;
            }
            
        }

        public bool QuitExhibition(string exhibitionId, string companyId)
        {
            var deleteList = _boothRepository.GetList(b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId);
            foreach (var record in deleteList)
            {
                _boothRepository.Delete(record);
            }

            var surveyList = _surveyRepository.GetList(s => s.ExhibitionId == exhibitionId && s.CompanyId == companyId);
            foreach (var survey in surveyList)
            {
                _surveyRepository.Delete(survey);
            }
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return result > 0;
        }

        public bool CreateSurvey(CreateSurveyViewModel model, string companyId, string accountId)
        {
            
            var survey = new Survey
            {
                Description = model.Description,
                Title = model.Title,
                ExhibitionId = model.ExhibitionId,
                Point = 100,
                CompanyId = companyId,
                AccountId = accountId,
                IsActive = true
            };
            _surveyRepository.Insert(survey);
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return result > 0;
        }

        public List<UpcomingSurveyViewModel> LoadUpcomingSurveyDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId, string exhibitionId)
        {
            var upcomingSurveyList = _surveyRepository.GetPagingProjection(
                e => new UpcomingSurveyViewModel
                {
                    ExhibitionId = e.ExhibitionId,                    
                    SurveyId = e.SurveyId,
                    SurveyTitle = e.Title
                },
                e => e.CompanyId == companyId && e.ExhibitionId == exhibitionId
                     && e.Title.Contains(searchValue),
                e => e.Title, sortColumnDirection, take, skip
            );
            return upcomingSurveyList.ToList();
        }

        public UpcomingSurveyDetailViewModel GetUpcomingSurveyDetail(string surveyId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(surveyId);
            }
            catch (Exception)
            {
                return null;
            }
            return _surveyRepository.GetSingleProjection(
                s => new UpcomingSurveyDetailViewModel
                {
                    Description = s.Description,
                    Title = s.Title,
                    SurveyId = s.SurveyId,
                }, 
                s => s.SurveyId == id
                );
        }

        public bool UpdateSurveyInfo(UpcomingSurveyDetailViewModel model)
        {
            var survey = _surveyRepository.GetById(model.SurveyId);
            if (survey == null)
            {
                return false;
            }
            _surveyRepository.Update(survey);
            survey.Title = model.Title;
            survey.Description = model.Description;
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return result >= 0;
        }

        public bool ValidateQuestionCreateField(string questionType, string id, string questionTitle = null, string[] answer = null)
        {
            if (string.IsNullOrEmpty(questionTitle))
            {
                return false;
            }
            if (questionTitle.Length <= 0 || questionTitle.Length > 1000)
            {
                return false;
            }
            if (answer != null && answer.Length > 0)
            {
                foreach (var check in answer)
                {
                    if (string.IsNullOrEmpty(check))
                    {
                        return false;
                    }
                    if (check.Length > 100)
                    {
                        return false;
                    }
                }
            }
            try
            {
                int surveyId = Convert.ToInt32(id);

                var survey = _surveyRepository.GetById(surveyId);
                if (survey == null)
                {
                    return false;
                }

                int type = Convert.ToInt32(questionType);
                if (type < 1 || type > 3)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool AddQuestionAndAnswer(string questionTitle, string[] answer, string id, string questionType)
        {
            int surveyId = Convert.ToInt32(id);
            int type = Convert.ToInt32(questionType);
            var question = new Question
            {
                Content = questionTitle,
                SurveyId = surveyId,
                QuestionType = type,
            };
            _questionRepository.Insert(question);
            int addQuestion;
            try
            {
                addQuestion = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            if (addQuestion > 0 && type != (int) QuestionTypeEnum.Text)
            {
                foreach (var option in answer)
                {
                    var proposedAnswer = new ProposedAnswer
                    {
                        Content = option,
                        QuestionId = question.QuestionId
                    };
                    _proposedAnswerRepository.Insert(proposedAnswer);
                }

                int addAnswer;
                try
                {
                    addAnswer = _unitOfWork.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return addAnswer > 0;
            }
            return addQuestion > 0;
        }

        public List<UpcomingSurveyQuestionViewModel> LoadUpcomingSurveyQuestionDataTable(string sortColumnDirection,
            string searchValue, int skip, int take, string surveyId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(surveyId);
            }
            catch (Exception)
            {
                return null;
            }

            var questionList = _questionRepository.GetPagingProjection(
                q => new
                {
                    Question = q.Content,
                    Type = q.QuestionType,
                    QuestionId = q.QuestionId
                },
                q => q.SurveyId == id, 
                q => q.QuestionType, sortColumnDirection, take, skip
                );
            var list = questionList.Select(q => new UpcomingSurveyQuestionViewModel
            {
                Question = q.Question,
                QuestionId = q.QuestionId,
                QuestionType = q.Type == (int) QuestionTypeEnum.Text ? "Text question" 
                    : q.Type == (int)  QuestionTypeEnum.SelectOne ? "Select one" : "Select Multiple",
                Type = q.Type
            });
            return list.ToList();
        }

        public SurveyQuestionDetailViewModel GetSurveyQuestionDetail(string questionId, string questionType)
        {
            int question, type;
            try
            {
                question = Convert.ToInt32(questionId);
                type = Convert.ToInt32(questionType);
            }
            catch (Exception)
            {
                return null;
            }

            switch (type)
            {
                case (int)QuestionTypeEnum.Text:

                    return _questionRepository.GetSingleProjection(
                        q => new SurveyQuestionDetailViewModel
                        {
                            QuestionType = q.QuestionType,
                            Question = q.Content,
                            QuestionId = q.QuestionId
                        },
                        q => q.QuestionId == question
                        );
                case (int)QuestionTypeEnum.SelectOne:
                case (int)QuestionTypeEnum.SelectMultiple:
                    return _questionRepository.GetSingleProjection(
                        q => new SurveyQuestionDetailViewModel
                        {
                            QuestionType = q.QuestionType,
                            Question = q.Content,
                            QuestionId = q.QuestionId,
                            Answers = q.ProposedAnswer.Select(p => new ProposedAnswerViewModel
                            {
                                Content = p.Content,
                            }).ToList()
                        },
                        q => q.QuestionId == question
                    );
                default:
                    return null;
            }
        }

        public bool UpdateSurveyQuestionDetail(SurveyQuestionDetailViewModel model)
        {
            var question = _questionRepository.GetById(model.QuestionId);
            if (question == null)
            {
                return false;
            }
            _questionRepository.Update(question);
            question.Content = model.Question;
            if (question.QuestionType != (int) QuestionTypeEnum.Text)
            {
                var answersList = _proposedAnswerRepository.GetList(p => p.QuestionId == question.QuestionId);
                if (answersList != null)
                {
                    foreach (var answer in answersList)
                    {
                        _proposedAnswerRepository.Delete(answer);
                    }
                }
                foreach (var answer in model.Answers)
                {
                    var proposedAnswer = new ProposedAnswer
                    {
                        Content = answer.Content,
                        QuestionId = question.QuestionId
                    };
                    _proposedAnswerRepository.Insert(proposedAnswer);
                }
            }
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return result >= 0;
        }

        public bool RemoveQuestion(string questionId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(questionId);
            }
            catch (Exception)
            {
                return false;
            }
            var question = _questionRepository.GetById(id);
            if (question == null)
            {
                return false;
            }
            _questionRepository.Delete(question);
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                return false;
            }
            return result > 0;
        }

        public bool RemoveSurvey(string surveyId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(surveyId);
            }
            catch (Exception)
            {
                return false;
            }
            var survey = _surveyRepository.GetById(id);
            if (survey == null)
            {
                return false;
            }
            _surveyRepository.Delete(survey);
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                return false;
            }
            return result > 0;
        }

        public CompanyProfileViewModel GetCompanyProfile(string companyId)
        {
            return _companyRepository.GetSingleProjection(
                c => new CompanyProfileViewModel
            {
                    Description = c.Description,
                    Email = c.Email,
                    TaxNumber = c.TaxNumber,
                    ImageUrl = c.Logo,
                    Address = c.Address,
                    CompanyName = c.Name,
                    Phone = c.Phone,
                    Website = c.Website,
                    Latitude = c.Location.Latitude,
                    Longitude = c.Location.Longitude
            },
                c => c.CompanyId == companyId
            );
        }

        public bool UpdateCompanyProfile(CompanyProfileViewModel model, string companyId)
        {
            var company = _companyRepository.GetById(companyId);
            if (company != null)
            {
                company.Description = model.Description;
                company.Address = model.Address;
                company.Email = model.Email;
                company.Logo = model.ImageUrl;
                company.Phone = model.Phone;
                company.Website = model.Website;
                if (model.Latitude.HasValue && model.Longitude.HasValue)
                {
                    company.Location = MyUtilities.CreateDbGeography(model.Longitude.Value, model.Latitude.Value);
                }
                else
                {
                    company.Location = null;
                }
                try
                {
                    var result = _unitOfWork.SaveChanges();
                    if (result >= 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public List<PastSurveyViewModel> LoadPastSurveyDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId, string exhibitionId)
        {
            var pastSurveyList = _surveyRepository.GetPagingProjection(
                s => new PastSurveyViewModel
                {
                    ExhibitionId = s.ExhibitionId,
                    SurveyId = s.SurveyId,
                    SurveyTitle = s.Title,
                    ResponseCount = s.SurveyParticipation.Count
                },
                s => s.CompanyId == companyId && s.ExhibitionId == exhibitionId
                                              && s.Title.Contains(searchValue),
                s => s.Title, sortColumnDirection, take, skip
            );
            return pastSurveyList.ToList();
        }

        public bool IsValidSurveyExportRequest(string surveyId, string companyId)
        {
            try
            {
                int id = Convert.ToInt32(surveyId);
                var survey = _surveyRepository.GetSingle(
                    s => s.SurveyId == id && s.CompanyId == companyId 
                         && s.Exhibition.EndDate < DateTime.Now && s.SurveyParticipation.Count > 0
                );
                return survey != null;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public Stream GetSurveyResponseExcelFile(string surveyId)
        {
            int id = Convert.ToInt32(surveyId);

            var surveyResult = _surveyRepository.GetSingleProjection(
                s => new SurveyResultViewModel
                {
                    SurveyTitle = s.Title,
                    Questions = s.Question.Select(q => q.Content).ToList(),
                    AnswerList = s.SurveyParticipation.Select(sp => new SurveyAnswerViewModel
                    {
                        ParticipantName = sp.AspNetUsers.LastName + " " + sp.AspNetUsers.FirstName,
                        TimeStamp = sp.CompleteDate,
                        QuestionList = s.SurveyAnswer.Where(sa => sa.AccountId == sp.AspNetUsers.Id).Select(sa => new
                        {
                            QuestionId = sa.Question.QuestionId,
                            Answer = sa.ProposedAnswerId != null ? sa.ProposedAnswer.Content : sa.Other
                        }).GroupBy(rs => rs.QuestionId).Select(g => new QuestionAnswerViewModel
                        {
                            QuestionId = g.Key,
                            QuestionAnswer = g.Select(v => v.Answer).ToList()
                        }).ToList()
                    }).ToList()
                },
                s => s.SurveyId == id
            );
            if (surveyResult == null || surveyResult.Questions.Count == 0 || surveyResult.AnswerList.Count == 0)
            {
                return null;
            }
            using (var excelPackage = new ExcelPackage(new MemoryStream()))
            {
                var workSheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                workSheet.Row(1).Style.Font.Bold = true;

                var headerRow = new List<string[]>();

                var row = new List<string> {"Full Name", "Timestamp"};

                foreach (var question in surveyResult.Questions)
                {
                    row.Add(question);
                }
                headerRow.Add(row.ToArray());
                string headerRange = "A1:" + char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";
                workSheet.Cells[headerRange].LoadFromArrays(headerRow);
                int currentRow = 2;
                foreach (var surveyAnswer in surveyResult.AnswerList)
                {
                    
                    var contentRow = new List<string[]>();
                    row = new List<string>
                    {
                        surveyAnswer.ParticipantName,
                        surveyAnswer.TimeStamp.ToString("HH:mm dddd, dd MMMM yyyy")
                    };
                    foreach (var answers in surveyAnswer.QuestionList)
                    {
                        row.Add(string.Join(", ", answers.QuestionAnswer));
                    }
                    contentRow.Add(row.ToArray());
                    string rowRange = "A" + currentRow + ":" + Char.ConvertFromUtf32(contentRow[0].Length + 64) + currentRow;
                    workSheet.Cells[rowRange].LoadFromArrays(contentRow);
                    currentRow++;
                }

                int columnCount = 2 + surveyResult.Questions.Count;
                for (int i = 1; i <= columnCount; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                excelPackage.Save();
                excelPackage.Stream.Position = 0;
                return excelPackage.Stream;
            }
        }
    }

    
}
