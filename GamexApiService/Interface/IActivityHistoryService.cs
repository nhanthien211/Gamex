using System.Collections.Generic;
using GamexApiService.Models;
using GamexApiService.ViewModel;

namespace GamexApiService.Interface {
    public interface IActivityHistoryService {
        bool AddActivity(string accountId, string activity);
        List<ActivityHistoryViewModel> GetActivities(string accountId, int take, int skip);
    }
}