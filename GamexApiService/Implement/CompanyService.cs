using GamexApiService.Interface;
using GamexApiService.ViewModel;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class CompanyService : ICompanyService {
        private IRepository<Company> _companyRepo;
        private IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepo, IUnitOfWork unitOfWork) {
            _companyRepo = companyRepo;
            _unitOfWork = unitOfWork;
        }

        public CompanyViewModel GetCompany(string companyId) {
            var company = _companyRepo.GetById(companyId);
            return new CompanyViewModel {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Description = company.Description,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Logo = company.Logo,
                Website = company.Website,
                TaxNumber = company.TaxNumber
            };
        }
    }
}