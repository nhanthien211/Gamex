using System.Collections.Generic;
using System.Web.Http;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class RewardController : ApiController {
        private IRewardService _rewardService;
        private IActivityHistoryService _activityHistoryService;
        private IRewardHistoryService _rewardHistoryService;
        private IAccountService _accountService;

        public RewardController(
            IRewardService rewardService,
            IActivityHistoryService activityHistoryService,
            IRewardHistoryService rewardHistoryService,
            IAccountService accountService) {
            _rewardService = rewardService;
            _activityHistoryService = activityHistoryService;
            _rewardHistoryService = rewardHistoryService;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("rewards")]
        public List<RewardDetailViewModel> GetRewards() {
            return _rewardService.GetRewards();
        }

        //[HttpGet]
        //[Route("reward")]
        //public RewardDetailViewModel GetReward(int id) {
        //    return _rewardService.GetReward(id);
        //}

        [HttpPost]
        [Route("reward")]
        public IHttpActionResult ExchangeReward(RewardBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var accountId = User.Identity.GetUserId();
            var result = _rewardService.ExchangeReward(accountId, model.Id);
            if (!result.Ok) {
                return BadRequest(result.Message);
            }

            var reward = _rewardService.GetRewardContent(model.Id);
            RecordActivity(accountId, "Exchanged reward " + reward.Content);
            RecordExchangeActivity(accountId, reward.RewardId);
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        [Route("reward/history")]
        public List<RewardHistoryViewModel> GetRewardHistory() {
            var accountId = User.Identity.GetUserId();
            return _rewardHistoryService.GetExchangeHistory(accountId);
        }

        private bool RecordActivity(string accountId, string activity) {
            return _activityHistoryService.AddActivity(accountId, activity);
        }

        private bool RecordExchangeActivity(string accountId, int rewardId) {
            return _rewardHistoryService.RecordExchangeHistory(accountId, rewardId);
        }

        [HttpGet]
        [Route("reward/point")]
        public RewardPointViewModel GetAccountRewardPoint() {
            var accountId = User.Identity.GetUserId();
            var point = _accountService.GetPoint(accountId);
            return point;
        }

        [HttpGet]
        [Route("reward/leaderboards")]
        public LeaderBoardViewModel GetLeaderBoard() {
            var accountId = User.Identity.GetUserId();
            return _accountService.GetLeaderBoardAccounts(accountId);
        }
    }
}
