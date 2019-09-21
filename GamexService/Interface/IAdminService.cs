using GamexService.ViewModel;
using System.Collections.Generic;

namespace GamexService.Interface
{
    public interface IAdminService
    {
        List<CompanyTableViewModel> LoadCompanyJoinRequestDataTable(string sortColumnDirection, string searchValue, int skip, int take);
        bool ApproveOrRejectCompanyRequest(string companyId, bool isApproved, ref string userId);
        List<CompanyTableViewModel> LoadCompanyDataTable(string sortColumnDirection, string searchValue, int skip, int take);
        List<OrganizerTableViewModel> LoadOrganizerDataTable(string sortColumnDirection, string searchValue, int skip, int take);
    }
}
