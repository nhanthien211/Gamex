using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexApiService.ViewModel;

namespace GamexApiService.Interface
{
    public interface IExhibitionService
    {
        List<ExhibitionShortViewModel> GetExhibitions(string type, int take, int skip, string lat, string lng, string accountId);
        ExhibitionViewModel GetExhibition(string exhibitionId);
        bool CheckInExhibition(string accountId, string exhibitionId);
    }
}
