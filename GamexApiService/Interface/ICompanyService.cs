using GamexApiService.ViewModel;

namespace GamexApiService.Interface {
    public interface ICompanyService {
        CompanyViewModel GetCompany(string companyId);
    }
}