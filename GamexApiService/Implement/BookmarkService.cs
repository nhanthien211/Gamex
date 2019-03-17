using System;
using GamexApiService.Interface;
using GamexEntity;
using GamexRepository;

namespace GamexApiService.Implement {
    public class BookmarkService : IBookmarkService {

        private IRepository<AccountBookmark> _accountBookmarkRepo;
        private IRepository<CompanyBookmark> _companyBookmarkRepo;
        private IRepository<ExhibitionAttendee> _exhibitionBookmarkRepo;
        private IUnitOfWork _unitOfWork;

        public BookmarkService(
            IRepository<AccountBookmark> accountBookmarkRepo,
            IRepository<CompanyBookmark> companyBookmarkRepo,
            IRepository<ExhibitionAttendee> exhibitionBookmarkRepo,
            IUnitOfWork unitOfWork) {
            _accountBookmarkRepo = accountBookmarkRepo;
            _companyBookmarkRepo = companyBookmarkRepo;
            _exhibitionBookmarkRepo = exhibitionBookmarkRepo;
            _unitOfWork = unitOfWork;
        }

        public bool AddBookmarkAccount(string srcAccountId, string tgtAccountId) {
            var bookmark = new AccountBookmark {
                AccountId = srcAccountId,
                AccountBookmark1 = tgtAccountId,
                BookmarkDate = DateTime.Now
            };
            _accountBookmarkRepo.Insert(bookmark);
            try {
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool RemoveBookmarkAccount(string srcAccountId, string tgtAccountId) {
            throw new System.NotImplementedException();
        }

        public bool AddBookmarkCompany(string accountId, string companyId) {
            throw new System.NotImplementedException();
        }

        public bool RemoveBookmarkCompany(string accountId, string companyId) {
            throw new System.NotImplementedException();
        }

        public bool AddBookmarkExhibition(string accountId, string exhibitionId) {
            throw new System.NotImplementedException();
        }

        public bool RemoveBookmarkExhibition(string accountId, string exhibitionId) {
            throw new System.NotImplementedException();
        }
    }
}