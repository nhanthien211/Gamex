﻿using GamexService.ViewModel;

namespace GamexService.Interface
{
    public interface IAccountService
    {
        LoginViewModel GetLoginAccount(string id);
        ProfileViewModel GetProfileView(string userId);
        bool IsUsernameDuplicate(string username);
        bool IsUsernameDuplicate(string username, string id);
    }
}
