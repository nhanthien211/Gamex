using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexService.ViewModel;

namespace GamexService.Interface
{
    public interface IAdminService
    {
        List<CompanyTableViewModel> LoadCompanyJoinRequestDataTable(string sortColumnDirection, string searchValue, int skip, int take);
        bool ApproveOrRejectCompanyRequest(int companyId, bool isApproved, ref string userId);
        List<CompanyTableViewModel> LoadCompanyDataTable(string sortColumnDirection, string searchValue, int skip, int take);
        List<OrganizerTableViewModel> LoadOrganizerDataTable(string sortColumnDirection, string searchValue, int skip, int take);
    }
}
