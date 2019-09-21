using GamexService.ViewModel;
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

        CompanyProfileViewModel GetCompanyProfile(string companyId);
        bool UpdateCompanyProfile(CompanyProfileViewModel model, string companyId);
        string GetCompanyBoothInExhibition(string exhibitionId, string companyId);


        List<EmployeeRequestViewModel> LoadEmployeeRequestDatatable(string sortColumnDirection, string searchValue, int skip, int take, string companyId);
    }
}