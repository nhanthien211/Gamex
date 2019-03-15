using GamexApiService.ViewModel;
using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {

    public interface IExhibitionService {
        List<ExhibitionShortViewModel> GetExhibitions(string type, int take, int skip, string lat, string lng, string accountId);
        ExhibitionViewModel GetExhibition(string exhibitionId);
        ServiceActionResult CheckInExhibition(string accountId, string exhibitionId);
        bool HasCheckedIn(string accountId, string exhibitionId);
        bool IsOnGoing(string exhibitionId);
    }
}
