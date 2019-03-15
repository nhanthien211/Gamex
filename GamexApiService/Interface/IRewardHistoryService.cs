using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface IRewardHistoryService {
        bool RecordExchangeHistory(string accountId, int rewardId);
        List<RewardHistoryViewModel> GetExchangeHistory(string accountId);
    }
}