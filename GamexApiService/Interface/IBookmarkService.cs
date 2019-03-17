namespace GamexApiService.Interface {
    public interface IBookmarkService {
        bool AddBookmarkAccount(string srcAccountId, string tgtAccountId);
        bool RemoveBookmarkAccount(string srcAccountId, string tgtAccountId);
        bool AddBookmarkCompany(string accountId, string companyId);
        bool RemoveBookmarkCompany(string accountId, string companyId);
        bool AddBookmarkExhibition(string accountId, string exhibitionId);
        bool RemoveBookmarkExhibition(string accountId, string exhibitionId);
    }
}