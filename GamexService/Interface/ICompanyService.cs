using System.Collections.Generic;
using GamexService.ViewModel;

namespace GamexService.Interface
{
    public interface ICompanyService
    {
        SelectCompanyViewModel SelectCompanyRegisterStatus(SelectCompanyViewModel model);
        bool RegisterNewCompany(CompanyRegisterViewModel model, string companyId);
        bool IsCompanyRegistered(string taxNumber);
        void RemoveCompany(string companyId);

        List<CompanyViewExhibitionViewModel> LoadNewExhibitionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId);
        ExhibitionDetailViewOnlyModel GetExhibitionDetail(string exhibitionId);
        bool IsCompanyHasJoinExhibition(string exhibitionId, string companyId);
        bool JoinExhibition(string exhibitionId, string companyId);
        
        List<CompanyViewExhibitionViewModel> LoadUpcomingExhibitionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId);
        bool QuitExhibition(string exhibitionId, string companyId);
        bool CreateSurvey(CreateSurveyViewModel model, string companyId, string accountId);

        List<UpcomingSurveyViewModel> LoadUpcomingSurveyDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId, string exhibitionId);
        UpcomingSurveyDetailViewModel GetUpcomingSurveyDetail(string surveyId);
        bool UpdateSurveyInfo(UpcomingSurveyDetailViewModel model);
    }
}