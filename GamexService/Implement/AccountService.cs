
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
        private IUnitOfWork _unitOfWork;

        public AccountService(IRepository<AspNetUsers> aspNetUsersRepository, IUnitOfWork unitOfWork)
        {
            _aspNetUsersRepository = aspNetUsersRepository;
            _unitOfWork = unitOfWork;
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
                a => string.Equals(a.UserName, id) || string.Equals(a.Email, id));
            
            LoginViewModel model = new LoginViewModel();
            if (account == null)
            {
                model.ErrorMessage = "Invalid Username or Password";
                return model;
            }

            if (account.StatusId == (int) AccountStatusEnum.Active)
            {
                model.Id = account.Username;
                model.UserId = account.UserId;
                return model;
            }
            model.ErrorMessage = "Account is currently disabled";
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
                  u => string.Equals(u.Id, userId));
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
