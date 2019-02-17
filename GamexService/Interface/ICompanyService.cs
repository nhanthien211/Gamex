using System.Collections.Generic;
using GamexService.ViewModel;

namespace GamexService.Interface
{
    public interface ICompanyService
    {
        SelectCompanyViewModel SelectCompanyRegisterStatus(SelectCompanyViewModel model);
        bool RegisterNewCompany(CompanyRegisterViewModel model);
        bool IsCompanyRegistered(string taxNumber);
    }
}