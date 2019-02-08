using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexEntity;
using GamexRepository;
using GamexService.ViewModel;

namespace GamexService.Implement
{
    public class AccountService
    {
        private IRepository<AspNetUsers> _aspNetUsersRepository;
        private IUnitOfWork _unitOfWork;

        public AccountService(IRepository<AspNetUsers> _aspNetUsersRepository, IUnitOfWork _unitOfWork)
        {
            this._aspNetUsersRepository = _aspNetUsersRepository;
            this._unitOfWork = this._unitOfWork;
        }

        public LoginViewModel GetLoginAccount(string id)
        {
            return null;
        }
    }
}
