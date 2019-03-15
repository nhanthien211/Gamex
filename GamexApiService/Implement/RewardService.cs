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
        private IUnitOfWork _unitOfWork;

        public RewardService(
            IRepository<Reward> rewardRepo,
            IUnitOfWork unitOfWork) {
            _rewardRepo = rewardRepo;
            _unitOfWork = unitOfWork;
        }

        public List<RewardShortViewModel> GetRewards() {
            var now = DateTime.Now;
            var rewards = _rewardRepo.GetList(r => r.IsActive && r.StartDate <= now && now <= r.EndtDate);
            return rewards.Select(r => new RewardShortViewModel() {
                RewardId = r.RewardId,
                PointCost = r.PointCost,
                Content = r.Content
            }).ToList();
        }

        public RewardDetailViewModel GetReward(int id) {
            throw new System.NotImplementedException();
        }

        public ServiceActionResult ExchangeReward(string accountId, int rewardId) {
            throw new System.NotImplementedException();
        }
    }
}