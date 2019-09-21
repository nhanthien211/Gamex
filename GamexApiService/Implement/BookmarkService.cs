using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexEntity.Constant;
using GamexRepository;

namespace GamexApiService.Implement {
    public class BookmarkService : IBookmarkService {

        private IRepository<CompanyBookmark> _companyBookmarkRepo;
        private IRepository<ExhibitionAttendee> _exhibitionBookmarkRepo;
        private IRepository<AspNetUsers> _accountRepo;
        private IRepository<Company> _companyRepo;
        private IRepository<Exhibition> _exhibtionRepo;
        private IUnitOfWork _unitOfWork;

        public BookmarkService(
            IRepository<CompanyBookmark> companyBookmarkRepo,
            IRepository<ExhibitionAttendee> exhibitionBookmarkRepo,
            IRepository<Company> companyRepo,
            IRepository<Exhibition> exhibitionRepo,
            IRepository<AspNetUsers> accountRepo,
            IUnitOfWork unitOfWork) {
            _companyBookmarkRepo = companyBookmarkRepo;
            _exhibitionBookmarkRepo = exhibitionBookmarkRepo;
            _accountRepo = accountRepo;
            _companyRepo = companyRepo;
            _exhibtionRepo = exhibitionRepo;
            _unitOfWork = unitOfWork;
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

            var exhibitionAttendeeRow = _exhibitionBookmarkRepo.GetSingle(eb =>
                                           eb.AccountId.Equals(accountId) && eb.ExhibitionId.Equals(exhibitionId));

            if (exhibitionAttendeeRow?.BookmarkDate != null) {
                return new ServiceActionResult { Ok = false, Message = "Bookmark failed: You've already bookmarked this exhibition!" };
            }

            if (exhibitionAttendeeRow == null) {
                _exhibitionBookmarkRepo.Insert(bookmark);
            }
            else {
                _exhibitionBookmarkRepo.Update(exhibitionAttendeeRow);
                exhibitionAttendeeRow.BookmarkDate = DateTime.Now;
            }
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
            var exhibitionAttendeeRow = _exhibitionBookmarkRepo.GetSingle(
                eb => eb.AccountId.Equals(accountId) && eb.ExhibitionId.Equals(exhibitionId));

            if (exhibitionAttendeeRow?.BookmarkDate == null) {
                return new ServiceActionResult { Ok = false, Message = "Remove bookmark failed: You haven't bookmarked this exhibition yet!" };
            }

            if (exhibitionAttendeeRow?.CheckinTime == null) {
                _exhibitionBookmarkRepo.Delete(exhibitionAttendeeRow);
            }
            else {
                _exhibitionBookmarkRepo.Update(exhibitionAttendeeRow);
                exhibitionAttendeeRow.BookmarkDate = null;
            }
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
            list.AddRange(GetBookmarkCompanies(accountId));
            list.AddRange(GetBookmarkExhibitions(accountId));
            return list;
        }
    }
}