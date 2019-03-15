using System.Collections.Generic;
using System.Web.Http;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity.Constant;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class RewardController : ApiController {
        private IRewardService _rewardService;
        private IActivityHistoryService _activityHistoryService;
        private IAccountService _accountService;

        public RewardController(
            IRewardService rewardService,
            IActivityHistoryService activityHistoryService,
            IAccountService accountService) {
            _rewardService = rewardService;
            _activityHistoryService = activityHistoryService;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("rewards")]
        public List<RewardShortViewModel> GetRewards() {
            return _rewardService.GetRewards();
        }

        [HttpGet]
        [Route("reward")]
        public RewardDetailViewModel GetReward(int id) {
            return null;
        }

        [HttpPost]
        [Route("reward")]
        public IHttpActionResult ExchangeReward() {
            return Ok();
        }
    }
}
