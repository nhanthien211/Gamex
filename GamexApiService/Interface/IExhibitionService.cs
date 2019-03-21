using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {

    public interface IExhibitionService {
        List<ExhibitionShortViewModel> GetExhibitions(string list, string type, int take, int skip, string lat, string lng, string accountId);
        ExhibitionDetailViewModel GetExhibition(string accountId, string exhibitionId);
        ServiceActionResult CheckInExhibition(string accountId, string exhibitionId);
        bool HasCheckedIn(string accountId, string exhibitionId);
        bool IsOnGoing(string exhibitionId);

        List<ExhibitionShortViewModel> GetExhibitionListRouteLengthNear(string lat, string lng, List<ExhibitionShortViewModel> exhibitionList);
    }
}
