
using System.Data.Entity;
using GamexEntity;
using GamexEntity.Enumeration;
using GamexRepository;
using System.Linq;
using GamexService.Interface;

namespace GamexService.Implement
{
    public class AccountService : IAccountService
    {
        private IRepository<AspNetUsers> _aspNetUsersRepository;
        private IUnitOfWork _unitOfWork;
        private GamexContext game;

        public AccountService(IRepository<AspNetUsers> _aspNetUsersRepository, IUnitOfWork _unitOfWork)
        {
            this._aspNetUsersRepository = _aspNetUsersRepository;
            this._unitOfWork = _unitOfWork;
        }

        public AspNetUsers GetLoginAccount(string id)
        {
            var account = _aspNetUsersRepository.GetSingle(
                u => (u.Email == id || u.UserName == id)
                && u.StatusId == (int) AccountStatusEnum.ACTIVE,
                "AspNetRoles");
            return account;
        }
    }
}
