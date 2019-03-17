using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface IRewardService {
        List<RewardDetailViewModel> GetRewards();
        RewardDetailViewModel GetReward(int id);
        RewardContentViewModel GetRewardContent(int id);
        ServiceActionResult ExchangeReward(string accountId, int rewardId);
    }
}