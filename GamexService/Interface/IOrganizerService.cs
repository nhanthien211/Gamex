using GamexService.ViewModel;
using System.Collections.Generic;

namespace GamexService.Interface
{
    public interface IOrganizerService
    {

        bool CreateExhibition(CreateExhibitionViewModel model, string exhibitionCode, string uploadUrl, string organizerId);

        List<ExhibitionTableViewModel> LoadExhibitionDataTable(string type, string sortColumnDirection,
            string searchValue, int skip, int take, string organizerId);

        ExhibitionDetailViewModel GetExhibitionDetail(string exhibitionId);
        ExhibitionDetailViewOnlyModel GetExhibitionDetailViewOnly(string exhibitionId);
        bool UpdateExhibitionDetail(ExhibitionDetailViewModel model);

        List<AttendedCompanyViewModel> LoadAttendedCompanyList(string sortColumnDirection, string searchValue, int skip,
            int take, string exhibitionId);
        AttendedCompanyDetailViewModel GetAttendedCompanyDetail(string exhibitionId, string companyId);
        bool AssignBoothToCompany(AttendedCompanyDetailViewModel model, string exhibitionId, string companyId);
    }
}
