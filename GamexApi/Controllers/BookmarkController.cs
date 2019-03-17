using System.Collections.Generic;
using System.Web.Http;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity.Constant;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class BookmarkController : ApiController {
        private IBookmarkService _bookmarkService;
        private IActivityHistoryService _activityHistoryService;

        public BookmarkController(
            IBookmarkService bookmarkService,
            IActivityHistoryService activityHistoryService) {
            _bookmarkService = bookmarkService;
            _activityHistoryService = activityHistoryService;
        }

        [HttpPost]
        [Route("bookmark/account")]
        public IHttpActionResult AddBookmarkAccount(BookmarkBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _bookmarkService.AddBookmarkAccount(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var activity = "Bookmarked account " + ((BookmarkServiceActionResult)result).TgtAccount.UserName;
            _activityHistoryService.AddActivity(accountId, activity);
            return Ok(new { message = result.Message });
        }

        [HttpDelete]
        [Route("bookmark/account")]
        public IHttpActionResult RemoveBookmarkAccount(BookmarkBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _bookmarkService.RemoveBookmarkAccount(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var activity = "Removed bookmark account " + ((BookmarkServiceActionResult)result).TgtAccount.UserName;
            _activityHistoryService.AddActivity(accountId, activity);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("bookmark/company")]
        public IHttpActionResult AddBookmarkCompany(BookmarkBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _bookmarkService.AddBookmarkCompany(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var activity = "Bookmarked company " + ((BookmarkServiceActionResult)result).Company.Name;
            _activityHistoryService.AddActivity(accountId, activity);
            return Ok(new { message = result.Message });
        }

        [HttpDelete]
        [Route("bookmark/company")]
        public IHttpActionResult RemoveBookmarkCompany(BookmarkBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _bookmarkService.RemoveBookmarkCompany(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var activity = "Removed bookmark company " + ((BookmarkServiceActionResult)result).Company.Name;
            _activityHistoryService.AddActivity(accountId, activity);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("bookmark/exhibition")]
        public IHttpActionResult AddBookmarkExhibition(BookmarkBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _bookmarkService.AddBookmarkExhibition(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var activity = "Bookmarked exhibition " + ((BookmarkServiceActionResult)result).Exhibition.Name;
            _activityHistoryService.AddActivity(accountId, activity);
            return Ok(new { message = result.Message });
        }

        [HttpDelete]
        [Route("bookmark/exhibition")]
        public IHttpActionResult RemoveBookmarkExhibition(BookmarkBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _bookmarkService.RemoveBookmarkExhibition(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var activity = "Removed bookmark exhibition " + ((BookmarkServiceActionResult)result).Exhibition.Name;
            _activityHistoryService.AddActivity(accountId, activity);
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        [Route("bookmarks")]
        public List<BookmarkViewModel> GetBookmarks(string type = null) {
            var accountId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(type))
                return _bookmarkService.GetBookmarks(accountId);
            if (type.Equals(BookmarkTypes.Attendee))
                return _bookmarkService.GetBookmarkAccounts(accountId);
            if (type.Equals(BookmarkTypes.Company))
                return _bookmarkService.GetBookmarkCompanies(accountId);
            if (type.Equals(BookmarkTypes.Exhibition)) 
                return _bookmarkService.GetBookmarkExhibitions(accountId);

            return new List<BookmarkViewModel>();
        }
    }
}
