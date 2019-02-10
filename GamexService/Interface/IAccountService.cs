using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexEntity;

namespace GamexService.Interface
{
    public interface IAccountService
    {
        AspNetUsers GetLoginAccount(string id);
    }
}
