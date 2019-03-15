using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface IRewardService {
        List<RewardShortViewModel> GetRewards();
        RewardDetailViewModel GetReward(int id);
        ServiceActionResult ExchangeReward(string accountId, int rewardId);
    }
}