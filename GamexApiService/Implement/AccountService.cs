using System;
using System.Collections.Generic;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexEntity.Constant;
using GamexRepository;

namespace GamexApiService.Implement {
    public class AccountService : IAccountService {
        private IRepository<AspNetUsers> _accountRepo;
        private IRepository<AspNetRoles> _roleRepo;
        private IUnitOfWork _unitOfWork;

        public AccountService(IRepository<AspNetUsers> accountRepo, 
            IRepository<AspNetRoles> roleRepo,
            IUnitOfWork unitOfWork) {
            _accountRepo = accountRepo;
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
        }


        public bool EarnPoint(string accountId, int point) {
            var user = _accountRepo.GetById(accountId);
            user.Point += point;
            user.TotalPointEarned += point;

            try {
                var affectedRows = _unitOfWork.SaveChanges();
                return affectedRows == 1;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool UsePoint(string accountId, int point) {
            var user = _accountRepo.GetById(accountId);

            if (user.Point < point) {
                return false;
            }

            user.Point -= point;

            try {
                var affectedRows = _unitOfWork.SaveChanges();
                return affectedRows == 1;
            }
            catch (Exception e) {
                return false;
            }
        }

        public RewardPointViewModel GetPoint(string accountId) {
            var user = _accountRepo.GetById(accountId);
            return new RewardPointViewModel {
                Point = user.Point,
                TotalPointEarned = user.TotalPointEarned
            };
        }
       
        public LeaderBoardViewModel GetLeaderBoardAccounts(string accountId) {
            var userRole = _roleRepo.GetSingle(r => r.Name.Equals(AccountRole.User));

            var accounts = _accountRepo.GetAll()
                .OrderByDescending(a => a.TotalPointEarned)
                .ToList();

            accounts = accounts.Where(a => a.AspNetRoles.Contains(userRole)).ToList();

            var list = accounts.Select(a => new AccountRankingViewModel {
                Fullname = a.FirstName + " " + a.LastName,
                TotalPointEarned = a.TotalPointEarned
            }).Take(10).ToList();
            var user = accounts.FirstOrDefault(a => a.Id.Equals(accountId));
            var userRanking = accounts.IndexOf(user) + 1;

            return new LeaderBoardViewModel {
                LeaderBoard = list,
                CurrentUserRank = userRanking,
                CurrentUserTotalPoint = user.TotalPointEarned
            };
        }
    }
}