using System;
using System.Collections.Generic;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class ActivityHistoryService : IActivityHistoryService {
        private IRepository<ActivityHistory> _activityHistoryRepo;
        private IUnitOfWork _unitOfWork;

        public ActivityHistoryService(
            IRepository<ActivityHistory> activityHistoryRepo,
            IUnitOfWork unitOfWork) {
            _activityHistoryRepo = activityHistoryRepo;
            _unitOfWork = unitOfWork;
        }

        public bool AddActivity(string accountId, string activity) {
            _activityHistoryRepo.Insert(new ActivityHistory {
                AccountId = accountId,
                Activity = activity,
                Time = DateTime.Now
            });
            try {
                var result = _unitOfWork.SaveChanges();
                return result == 1;
            } catch (Exception ex) {
                return false;
            }
        }

        public List<ActivityHistoryViewModel> GetActivities(string accountId, int take, int skip) {
            var activityHistories = _activityHistoryRepo.GetPagingProjection(
                a => new {
                    a.AccountId,
                    a.Activity,
                    a.Time
                }, a => a.AccountId.Equals(accountId),
                a => a.Time,
                "desc",
                take,
                skip
            );

            return activityHistories.Select(a => new ActivityHistoryViewModel {
                AccountId = a.AccountId,
                Activity = a.Activity,
                Time = a.Time.ToString("f")
            }).ToList();
        }
    }
}