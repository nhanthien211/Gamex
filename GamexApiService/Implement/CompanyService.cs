using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class CompanyService : ICompanyService {
        private IRepository<Company> _companyRepo;
        private IRepository<CompanyBookmark> _companyBookmarkRepo;
        private IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepo,
            IRepository<CompanyBookmark> companyBookmarkRepo,
            IUnitOfWork unitOfWork) {
            _companyRepo = companyRepo;
            _companyBookmarkRepo = companyBookmarkRepo;
            _unitOfWork = unitOfWork;
        }

        private bool HasBookmarked(string accountId, string companyId) {
            return _companyBookmarkRepo.GetSingle(cb =>
                       cb.AccountId.Equals(accountId) && cb.CompanyBookmark1.Equals(companyId)
                                                      && cb.BookmarkDate != null) != null;
        }

        public CompanyViewModel GetCompany(string accountId, string companyId) {
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
                TaxNumber = company.TaxNumber,
                IsBookmarked = HasBookmarked(accountId, companyId)
            };
        }
    }
}