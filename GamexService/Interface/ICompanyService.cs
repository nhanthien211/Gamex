﻿using GamexService.ViewModel;
using System.Collections.Generic;
using System.IO;

namespace GamexService.Interface
{
    public interface ICompanyService
    {
        SelectCompanyViewModel SelectCompanyRegisterStatus(SelectCompanyViewModel model);
        bool RegisterNewCompany(CompanyRegisterViewModel model, string companyId);
        bool IsCompanyRegistered(string taxNumber);
        void RemoveCompany(string companyId);

        ExhibitionDetailViewOnlyModel GetExhibitionDetail(string exhibitionId, string type);
        bool IsCompanyHasJoinExhibition(string exhibitionId, string companyId);
        bool JoinExhibition(string exhibitionId, string companyId);
        
        List<ExhibitionTableViewModel> LoadExhibitionDataTable(string type, string sortColumnDirection, string searchValue, int skip, int take, string companyId);
        bool QuitExhibition(string exhibitionId, string companyId);
        bool CreateSurvey(CreateSurveyViewModel model, string companyId, string accountId);

        List<UpcomingSurveyViewModel> LoadUpcomingSurveyDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId, string exhibitionId);
        UpcomingSurveyDetailViewModel GetUpcomingSurveyDetail(string surveyId);
        bool UpdateSurveyInfo(UpcomingSurveyDetailViewModel model);
        bool ValidateQuestionCreateField(string questionType, string id, string questionTitle = null, string[] answer = null);
        bool AddQuestionAndAnswer(string questionTitle, string[] answer, string id, string questionType);
        List<UpcomingSurveyQuestionViewModel> LoadUpcomingSurveyQuestionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string surveyId);
        SurveyQuestionDetailViewModel GetSurveyQuestionDetail(string questionId, string questionType);
        bool UpdateSurveyQuestionDetail(SurveyQuestionDetailViewModel model);
        bool RemoveQuestion(string questionId);
        bool RemoveSurvey(string surveyId);
        CompanyProfileViewModel GetCompanyProfile(string companyId);
        bool UpdateCompanyProfile(CompanyProfileViewModel model, string companyId);
        string GetCompanyBoothInExhibition(string exhibitionId, string companyId);
        List<PastSurveyViewModel> LoadPastSurveyDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId, string exhibitionId);
        bool IsValidSurveyExportRequest(string surveyId, string companyId);
        Stream GetSurveyResponseExcelFile(string surveyId);

        List<EmployeeRequestViewModel> LoadEmployeeRequestDatatable(string sortColumnDirection, string searchValue, int skip, int take, string companyId);
    }
}