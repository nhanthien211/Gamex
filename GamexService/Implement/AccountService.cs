
using System;
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
        private IRepository<AspNetRoles> _aspNetRolesRepository;
        
        private IUnitOfWork _unitOfWork;

        public AccountService(IRepository<AspNetUsers> _aspNetUsersRepository, IUnitOfWork _unitOfWork)
        {
            this._aspNetUsersRepository = _aspNetUsersRepository;
            this._unitOfWork = _unitOfWork;
        }

        public LoginViewModel GetLoginAccount(string id)
        {
            var account = _aspNetUsersRepository.GetSingleProjection(
                a => new
                {
                    Username = a.UserName,
                    StatusId = a.StatusId,
                    UserId = a.Id
                },
                a => a.UserName == id || a.Email == id);
            LoginViewModel model = new LoginViewModel();
            if (account.StatusId == (int) AccountStatusEnum.ACTIVE)
            {
                model.Id = account.Username;
                model.UserId = account.UserId;
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

        public bool UpdateProfile(ProfileViewModel model, string id)
        {
            var account = _aspNetUsersRepository.GetById(id);
            if (account != null)
            {
                _aspNetUsersRepository.Update(account);
                account.FirstName = model.FirstName;
                account.LastName = model.LastName;
                account.UserName = model.Username;
                account.Email = model.Email;

                try
                {
                    int result = _unitOfWork.SaveChanges();
                    return result >= 0;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }
        
    }
}
