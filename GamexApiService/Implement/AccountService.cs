using System;
using GamexApiService.Interface;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class AccountService : IAccountService {
        private IRepository<AspNetUsers> _accountRepo;
        private IUnitOfWork _unitOfWork;

        public AccountService(IRepository<AspNetUsers> accountRepo, IUnitOfWork unitOfWork) {
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
        }

        public bool EarnPoint(string accountId, int point) {
            var user = _accountRepo.GetById(accountId);
            user.Point += point;

            try {
                var affectedRows = _unitOfWork.SaveChanges();
                return affectedRows == 1;
            }
            catch (Exception e) {
                return false;
            }
        }
    }
}