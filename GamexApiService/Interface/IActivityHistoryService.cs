using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface IActivityHistoryService {
        bool AddActivity(string accountId, string activity);
        List<ActivityHistoryViewModel> GetActivities(string accountId, int take, int skip);
    }
}