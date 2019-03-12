﻿using System.Collections.Generic;
using GamexApiService.Interface;
using System.Web.Http;
using GamexApiService.ViewModel;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;

namespace GamexApi.Controllers {

    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class ActivityHistoryController : ApiController {
        private readonly IActivityHistoryService _activityHistoryService;

        public ActivityHistoryController(IActivityHistoryService activityHistoryService) {
            _activityHistoryService = activityHistoryService;
        }

        [HttpGet]
        [Route("activity")]
        public List<ActivityHistoryViewModel> GetActivityHistories(int take = 5, int skip = 0) {
            var accountId = User.Identity.GetUserId();
            return _activityHistoryService.GetActivities(accountId, take, skip);
        }
    }
}
