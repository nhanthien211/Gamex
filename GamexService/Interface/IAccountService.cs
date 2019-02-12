using GamexService.ViewModel;

namespace GamexService.Interface
{
    public interface IAccountService
    {
        LoginViewModel GetLoginAccountUsername(string id);
        ProfileViewModel GetProfileView(string userId);
    }
}
