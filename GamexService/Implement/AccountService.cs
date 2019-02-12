
using System.Data.Entity;
using GamexEntity;
using GamexEntity.Enumeration;
using GamexRepository;
using System.Linq;
using GamexService.Interface;
using GamexService.ViewModel;

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

        public LoginViewModel GetLoginAccountUsername(string id)
        {
            var account = _aspNetUsersRepository.GetSingleProjection(
                a => new
                {
                    Username = a.UserName,
                    StatusId = a.StatusId
                },
                a => a.UserName == id || a.Email == id);
            LoginViewModel model = new LoginViewModel();
            if (account.StatusId == (int) AccountStatusEnum.ACTIVE)
            {
                model.Id = account.Username;
            }
            else
            {
                model.ErrorMessage = "Account is currently disabled";
            }
            return model;
        }

        public ProfileViewModel GetProfileView(string userId)
        {
            return _aspNetUsersRepository.GetSingleProjection(
                u => new ProfileViewModel
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.UserName
                },
                  u => u.Id == userId);
        }
        
    }
}
