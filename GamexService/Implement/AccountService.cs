using GamexEntity;
using GamexEntity.Enumeration;
using GamexRepository;
using GamexService.Interface;

namespace GamexService.Implement
{
    public class AccountService : IAccountService
    {
        private IRepository<AspNetUsers> _aspNetUsersRepository;
        private IUnitOfWork _unitOfWork;

        public AccountService(IRepository<AspNetUsers> _aspNetUsersRepository, IUnitOfWork _unitOfWork)
        {
            this._aspNetUsersRepository = _aspNetUsersRepository;
            this._unitOfWork = _unitOfWork;
        }

        public AspNetUsers GetLoginAccount(string id)
        {
            var account = _aspNetUsersRepository.GetSingle(u =>
                (u.Email == id || u.UserName == id)
                && u.StatusId == (int) AccountStatusEnum.ACTIVE);
            return account;
        }
    }
}
