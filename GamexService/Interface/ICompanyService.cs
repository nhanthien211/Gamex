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
        NewExhibitionDetailViewModel GetNewExhibitionDetail(string exhibitionId);
        bool IsCompanyHasJoinExhibition(string exhibitionId, string companyId);
        bool JoinExhibition(string exhibitionId, string companyId);
        List<CompanyViewExhibitionViewModel> LoadUpcomingExhibitionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId);
    }
}