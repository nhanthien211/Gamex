using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface IAccountService {
        bool EarnPoint(string accountId, int point);
        bool UsePoint(string accountId, int point);
        RewardPointViewModel GetPoint(string accountId);
    }
}