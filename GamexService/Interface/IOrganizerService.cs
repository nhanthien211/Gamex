using GamexService.ViewModel;
using System.Collections.Generic;
using System.IO;

namespace GamexService.Interface
{
    public interface IOrganizerService
    {

        bool CreateExhibition(CreateExhibitionViewModel model, string exhibitionCode, string uploadUrl, string organizerId);

        List<ExhibitionTableViewModel> LoadExhibitionDataTable(string type, string sortColumnDirection,
            string searchValue, int skip, int take, string organizerId);

        ExhibitionDetailViewModel GetExhibitionDetail(string exhibitionId, string organizerId);
        ExhibitionDetailViewOnlyModel GetExhibitionDetailViewOnly(string exhibitionId, string organizerId);
        bool UpdateExhibitionDetail(ExhibitionDetailViewModel model);

        List<AttendedCompanyViewModel> LoadAttendedCompanyList(string sortColumnDirection, string searchValue, int skip,
            int take, string exhibitionId);
        AttendedCompanyDetailViewModel GetAttendedCompanyDetail(string exhibitionId, string companyId);
        bool AssignBoothToCompany(AttendedCompanyDetailViewModel model, string exhibitionId, string companyId);
        PastExhibitionViewModel GetPastExhibitionDetail(string exhibitionId, string organizerId);
        bool IsValidExhibitionExportRequest(string exhibitionId, string organizerId);
        Stream GetExhibitionReportExcelFile(string exhibitionId, string organizerId);
        ExhibitionNotificationViewModel GetExhibitionDetailForNotification(string exhibitionId, string organizerId);
    }
}
