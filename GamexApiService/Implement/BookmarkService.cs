using System;
using System.Collections.Generic;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexEntity.Constant;
using GamexRepository;

namespace GamexApiService.Implement {
    public class BookmarkService : IBookmarkService {

        private IRepository<AccountBookmark> _accountBookmarkRepo;
        private IRepository<CompanyBookmark> _companyBookmarkRepo;
        private IRepository<ExhibitionAttendee> _exhibitionBookmarkRepo;
        private IRepository<AspNetUsers> _accountRepo;
        private IRepository<Company> _companyRepo;
        private IRepository<Exhibition> _exhibtionRepo;
        private IUnitOfWork _unitOfWork;

        public BookmarkService(
            IRepository<AccountBookmark> accountBookmarkRepo,
            IRepository<CompanyBookmark> companyBookmarkRepo,
            IRepository<ExhibitionAttendee> exhibitionBookmarkRepo,
            IRepository<Company> companyRepo,
            IRepository<Exhibition> exhibitionRepo,
            IRepository<AspNetUsers> accountRepo,
            IUnitOfWork unitOfWork) {
            _accountBookmarkRepo = accountBookmarkRepo;
            _companyBookmarkRepo = companyBookmarkRepo;
            _exhibitionBookmarkRepo = exhibitionBookmarkRepo;
            _accountRepo = accountRepo;
            _companyRepo = companyRepo;
            _exhibtionRepo = exhibitionRepo;
            _unitOfWork = unitOfWork;
        }

        private bool HasBookmarkedAccount(string srcAccountId, string tgtAccountId) {
            return _accountBookmarkRepo.GetSingle(
                                    ab => ab.AccountId.Equals(srcAccountId) && ab.AccountBookmark1.Equals(tgtAccountId)
                                                                            && ab.BookmarkDate != null) != null;
        }

        public ServiceActionResult AddBookmarkAccount(string srcAccountId, string tgtAccountId) {
            var bookmark = new AccountBookmark {
                AccountId = srcAccountId,
                AccountBookmark1 = tgtAccountId,
                BookmarkDate = DateTime.Now
            };


            if (HasBookmarkedAccount(srcAccountId, tgtAccountId)) {
                return new ServiceActionResult() { Ok = false, Message = "Bookmark failed: You've already bookmarked this account!" };
            }

            _accountBookmarkRepo.Insert(bookmark);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    var tgtAccount = _accountRepo.GetById(tgtAccountId);
                    return new BookmarkServiceActionResult {
                        Ok = true,
                        Message = "Bookmark account " + tgtAccount.UserName + " success!",
                        TgtAccount = tgtAccount
                    };
                }

                return ServiceActionResult.ErrorResult;
            } catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }

        public ServiceActionResult RemoveBookmarkAccount(string srcAccountId, string tgtAccountId) {
            var bookmark = _accountBookmarkRepo.GetSingle(
                ab => ab.AccountId.Equals(srcAccountId) && ab.AccountBookmark1.Equals(tgtAccountId)
                                                        && ab.BookmarkDate != null);


            if (bookmark == null) {
                return new ServiceActionResult 
                    { Ok = false, Message = "Remove bookmark failed: You haven't bookmarked this account yet!" };
            }

            _accountBookmarkRepo.Delete(bookmark);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    var tgtAccount = _accountRepo.GetById(tgtAccountId);
                    return new BookmarkServiceActionResult {
                        Ok = true,
                        Message = "Remove bookmark account " + tgtAccount.UserName + " success!",
                        TgtAccount = tgtAccount
                    };
                }
                return ServiceActionResult.ErrorResult;
            }
            catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }

        public ServiceActionResult AddBookmarkCompany(string accountId, string companyId) {
            var bookmark = new CompanyBookmark() {
                AccountId = accountId,
                CompanyBookmark1 = companyId,
                BookmarkDate = DateTime.Now
            };

            var hasBookmarkedCompany = _companyBookmarkRepo.GetSingle(cb =>
                                           cb.AccountId.Equals(accountId) && cb.CompanyBookmark1.Equals(companyId)
                                                                          && cb.BookmarkDate != null) != null;

            if (hasBookmarkedCompany) {
                return new ServiceActionResult { Ok = false, Message = "Bookmark failed: You've already bookmarked this company!" };
            }

            _companyBookmarkRepo.Insert(bookmark);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    var company = _companyRepo.GetById(companyId);
                    return new BookmarkServiceActionResult {
                        Ok = true,
                        Message = "Bookmark company " + company.Name + " success!",
                        Company = company
                    };
                }

                return ServiceActionResult.ErrorResult;
            } catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }

        public ServiceActionResult RemoveBookmarkCompany(string accountId, string companyId) {
            var bookmark = _companyBookmarkRepo.GetSingle(
                cb => cb.AccountId.Equals(accountId) && cb.CompanyBookmark1.Equals(companyId)
                                                        && cb.BookmarkDate != null);


            if (bookmark == null) {
                return new ServiceActionResult { Ok = false, Message = "Remove bookmark failed: You haven't bookmarked this company yet!" };
            }

            _companyBookmarkRepo.Delete(bookmark);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    var company = _companyRepo.GetById(companyId);
                    return new BookmarkServiceActionResult {
                        Ok = true,
                        Message = "Remove bookmark company " + company.Name + " success!",
                        Company = company
                    };
                }
                return ServiceActionResult.ErrorResult;
            } catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }

        public ServiceActionResult AddBookmarkExhibition(string accountId, string exhibitionId) {
            var bookmark = new ExhibitionAttendee {
                AccountId = accountId,
                ExhibitionId = exhibitionId,
                BookmarkDate = DateTime.Now
            };

            var hasBookmarkedExhibition = _exhibitionBookmarkRepo.GetSingle(eb =>
                                           eb.AccountId.Equals(accountId) && eb.ExhibitionId.Equals(exhibitionId)
                                                                          && eb.BookmarkDate != null) != null;

            if (hasBookmarkedExhibition) {
                return new ServiceActionResult { Ok = false, Message = "Bookmark failed: You've already bookmarked this exhibition!" };
            }

            _exhibitionBookmarkRepo.Insert(bookmark);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    var exhibition = _exhibtionRepo.GetById(exhibitionId);
                    return new BookmarkServiceActionResult {
                        Ok = true,
                        Message = "Bookmark exhibition " + exhibition.Name + " success!",
                        Exhibition = exhibition
                    };
                }

                return ServiceActionResult.ErrorResult;
            } catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }

        public ServiceActionResult RemoveBookmarkExhibition(string accountId, string exhibitionId) {
            var bookmark = _exhibitionBookmarkRepo.GetSingle(
                eb => eb.AccountId.Equals(accountId) && eb.ExhibitionId.Equals(exhibitionId)
                                                     && eb.BookmarkDate != null);


            if (bookmark == null) {
                return new ServiceActionResult { Ok = false, Message = "Remove bookmark failed: You haven't bookmarked this exhibition yet!" };
            }

            _exhibitionBookmarkRepo.Delete(bookmark);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    var exhibition = _exhibtionRepo.GetById(exhibitionId);
                    return new BookmarkServiceActionResult {
                        Ok = true,
                        Message = "Remove bookmark exhibition " + exhibition.Name + " success!",
                        Exhibition = exhibition
                    };
                }
                return ServiceActionResult.ErrorResult;
            } catch (Exception e) {
                return ServiceActionResult.ErrorResult;
            }
        }

        public List<BookmarkViewModel> GetBookmarkAccounts(string accountId) {
            var bookmarks = _accountBookmarkRepo.GetList(ab =>
                ab.AccountId.Equals(accountId));

            return bookmarks.Select(bookmark => new BookmarkViewModel {
                TargetType = BookmarkTypes.Attendee,
                TargetId = bookmark.AccountBookmark1,
                TargetName = bookmark.AspNetUsers1.UserName,
                BookmarkDate = bookmark.BookmarkDate.ToString("f")
            }).OrderBy(b => b.TargetName).ToList();
        }

        public List<BookmarkViewModel> GetBookmarkCompanies(string accountId) {
            var bookmarks = _companyBookmarkRepo.GetList(cb =>
                cb.AccountId.Equals(accountId));
            return bookmarks.Select(b => new BookmarkViewModel {
                TargetType = BookmarkTypes.Company,
                TargetId = b.CompanyBookmark1,
                TargetName = b.Company.Name,
                BookmarkDate = b.BookmarkDate.ToString("f")
            }).OrderBy(b => b.TargetName).ToList();
        }

        public List<BookmarkViewModel> GetBookmarkExhibitions(string accountId) {
            var bookmarks = _exhibitionBookmarkRepo.GetList(eb =>
                eb.AccountId.Equals(accountId) && eb.BookmarkDate != null);
            return bookmarks.Select(b => new BookmarkViewModel {
                TargetType = BookmarkTypes.Exhibition,
                TargetId = b.ExhibitionId,
                TargetName = b.Exhibition.Name,
                BookmarkDate = b.BookmarkDate?.ToString("f")
            }).OrderBy(b => b.TargetName).ToList();
        }

        public List<BookmarkViewModel> GetBookmarks(string accountId) {
            var list = new List<BookmarkViewModel>();
            list.AddRange(GetBookmarkAccounts(accountId));
            list.AddRange(GetBookmarkCompanies(accountId));
            list.AddRange(GetBookmarkExhibitions(accountId));
            return list;
        }
    }
}