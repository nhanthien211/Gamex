using System.Collections.Generic;
using GamexService.ViewModel;

namespace GamexService.Interface
{
    public interface ICompanyService
    {
        SelectCompanyViewModel SelectCompanyRegisterStatus(SelectCompanyViewModel model);
        bool RegisterNewCompany(CompanyRegisterViewModel model);
        bool IsCompanyRegistered(string taxNumber);
        List<CompanyTableViewModel> LoadCompanyJoinRequestDataTable(string sortColumnDirection, string searchValue, int skip, int take);
        void ApproveOrRejectCompanyRequest(int companyId, bool isApproved);
        List<CompanyTableViewModel> LoadCompanyDataTable(string sortColumnDirection, string searchValue, int skip, int take);
        int GetCompanyId(string taxNumber);
        void RemoveCompany(int companyId);
    }
}