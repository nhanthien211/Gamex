using System;
using System.Collections.Generic;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class RewardHistoryService : IRewardHistoryService {
        private IRepository<RewardHistory> _rewardHistoryRepo;
        private IUnitOfWork _unitOfWork;

        public RewardHistoryService(IRepository<RewardHistory> rewardHistoryRepo, IUnitOfWork unitOfWork) {
            _rewardHistoryRepo = rewardHistoryRepo;
            _unitOfWork = unitOfWork;
        }

        public bool RecordExchangeHistory(string accountId, int rewardId) {
            var hasExchanged = _rewardHistoryRepo.GetSingle(
                rh => rh.AccountId.Equals(accountId) && rh.RewardId == rewardId) != null;

            if (hasExchanged) {
                return false;
            }

            _rewardHistoryRepo.Insert(new RewardHistory {
                AccountId = accountId,
                RewardId = rewardId,
                ExchangedDate = DateTime.Now
            });
            try {
                var result = _unitOfWork.SaveChanges();
                return result == 1;
            }
            catch (Exception e) {
                return false;
            }
        }

        public List<RewardHistoryViewModel> GetExchangeHistory(string accountId) {
            var history = _rewardHistoryRepo.GetPagingProjection(
                rh => new {
                    rh.ExchangedDate,
                    rh.Reward.Content
                },
                rh => rh.AccountId.Equals(accountId),
                rh => rh.ExchangedDate,
                "desc",
                0,
                0);
            return history.Select(h => new RewardHistoryViewModel {
                ExchangedDate = h.ExchangedDate.ToLongDateString(),
                RewardContent = h.Content
            }).ToList();
        }
    }
}