using System.Collections.Generic;
using GamexApiService.Models;

namespace GamexApiService.Interface {
    public interface IBookmarkService {
        ServiceActionResult AddBookmarkAccount(string srcAccountId, string tgtAccountId);
        ServiceActionResult RemoveBookmarkAccount(string srcAccountId, string tgtAccountId);
        ServiceActionResult AddBookmarkCompany(string accountId, string companyId);
        ServiceActionResult RemoveBookmarkCompany(string accountId, string companyId);
        ServiceActionResult AddBookmarkExhibition(string accountId, string exhibitionId);
        ServiceActionResult RemoveBookmarkExhibition(string accountId, string exhibitionId);

        List<BookmarkViewModel> GetBookmarkAccounts(string accountId);
        List<BookmarkViewModel> GetBookmarkCompanies(string accountId);
        List<BookmarkViewModel> GetBookmarkExhibitions(string accountId);

        List<BookmarkViewModel> GetBookmarks(string accountId);
    }
}