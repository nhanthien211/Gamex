
using System;
using System.Collections.Generic;
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
                    UserId = a.Id,
                },
                a => string.Equals(a.UserName, id) || string.Equals(a.Email, id));
            
            LoginViewModel model = new LoginViewModel();
            if (account == null)
            {
                model.ErrorMessage = "Invalid Username or Password";
                return model;
            }

            switch (account.StatusId)
            {
                case (int)AccountStatusEnum.Pending:
                    model.ErrorMessage = "Your registration request is being reviewed";
                    break;
                case (int)AccountStatusEnum.Deactive:
                    model.ErrorMessage = "Account is currently disabled";
                    break;
                case (int)AccountStatusEnum.Active:
                    model.Id = account.Username;
                    model.UserId = account.UserId;
                    return model;
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
                  u => string.Equals(u.Id, userId));
        }

        public bool IsUsernameDuplicate(string username)
        {
            return _aspNetUsersRepository.GetSingleProjection(u => u.Id, 
                       u => u.Email.Equals(username, StringComparison.CurrentCultureIgnoreCase)
                        || u.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)) != null;
        }

        public bool IsUsernameDuplicate(string username, string id)
        {
            return _aspNetUsersRepository.GetSingleProjection(u => u.Id,
                       u => !u.Id.Equals(id) && (u.Email.Equals(username, StringComparison.CurrentCultureIgnoreCase)
                            || u.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase))) != null;
        }

        //        public List<AspNetUsers> Test(string role)
        //        {
        //            return _aspNetUsersRepository.GetList(
        //                u => u.AspNetRoles.Select(r => r.Name).Contains("Company")).ToList();
        //        }
    }
}
