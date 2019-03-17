using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface ICompanyService {
        CompanyViewModel GetCompany(string accountId, string companyId);
    }
}