using System;
using System.Collections.Generic;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class RewardService : IRewardService {
        private IRepository<Reward> _rewardRepo;
        private IRepository<AspNetUsers> _accountRepo;
        private IRepository<RewardHistory> _rewardHistoryRepo;
        private IUnitOfWork _unitOfWork;

        public RewardService(
            IRepository<Reward> rewardRepo,
            IRepository<AspNetUsers> accountRepo,
            IRepository<RewardHistory> rewardHistoryRepo,
            IUnitOfWork unitOfWork) {
            _rewardRepo = rewardRepo;
            _accountRepo = accountRepo;
            _rewardHistoryRepo = rewardHistoryRepo;
            _unitOfWork = unitOfWork;
        }

        public List<RewardShortViewModel> GetRewards() {
            var now = DateTime.Now;
            var rewards = _rewardRepo.GetList(r => r.IsActive
                                                   && r.StartDate <= now && now <= r.EndDate
                                                   && r.Quantity > 0);
            return rewards.Select(r => new RewardShortViewModel() {
                RewardId = r.RewardId,
                PointCost = r.PointCost,
                Content = r.Content
            }).ToList();
        }

        public RewardDetailViewModel GetReward(int id) {
            var reward = _rewardRepo.GetById(id);
            return new RewardDetailViewModel() {
                RewardId = reward.RewardId,
                Description = reward.Description,
                PointCost = reward.PointCost,
                Quantity = reward.Quantity,
                StartDate = reward.StartDate.ToLongDateString(),
                EndDate = reward.EndDate.ToLongDateString()
            };
        }

        public RewardContentViewModel GetRewardContent(int id) {
            var reward = _rewardRepo.GetById(id);
            return new RewardContentViewModel {
                RewardId = reward.RewardId,
                Content = reward.Content,
                PointCost = reward.PointCost
            };
        }

        public ServiceActionResult ExchangeReward(string accountId, int rewardId) {
            var reward = _rewardRepo.GetById(rewardId);
            var account = _accountRepo.GetById(accountId);
            var now = DateTime.Now;
            if (!reward.IsActive || reward.StartDate > now || reward.EndDate < now || reward.Quantity <= 0) {
                return new ServiceActionResult() { Ok = false, Message = "Exchange reward failed: reward is not available!" };
            }

            if (account.Point < reward.PointCost) {
                return new ServiceActionResult { Ok = false, Message = "Exchange reward failed: not enough point to exchange this reward!" };
            }

            // check if user has exchanged
            var hasExchanged = _rewardHistoryRepo.GetSingle(
                                   rh => rh.AccountId.Equals(accountId) && rh.RewardId == rewardId) != null;

            if (hasExchanged) {
                return new ServiceActionResult { Ok = false, Message = "Exchange reward failed: you have already exchanged this reward!" };
            }

            --reward.Quantity;
            account.Point -= reward.PointCost;
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 2) {
                    return new ServiceActionResult { Ok = true, Message = "Exchange reward success!" };
                }

                return new ServiceActionResult { Ok = false, Message = "Exchange reward failed!" };
            } catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }
    }
}