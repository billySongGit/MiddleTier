using Sabio.Web.Domain;
using Sabio.Web.Domain.Account;
using Sabio.Web.Domain.Ratings;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/stats")]
    public class StatTrackerApiController : ApiController
    {
        private IStatTrackerService _statTrackerService = null;
        private IUserService _userService = null;
        private IAccountsService _accountService = null;

        public StatTrackerApiController(IStatTrackerService statTrackerService, IUserService userService, IAccountsService accountService)
        {
            _statTrackerService = statTrackerService;
            _userService = userService;
            _accountService = accountService;
        }

        [Route(), HttpGet]
        public HttpResponseMessage GetTotalCount()
        {
            ItemResponse<StatTracker> response = new ItemResponse<StatTracker>();
            string userId = _userService.GetCurrentUserId();
            response.Item = _statTrackerService.GetFollowerCount(userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route(), HttpGet]
        public HttpResponseMessage GetPublicProfileTotalCount(string handle)
        {
            ItemResponse<StatTracker> response = new ItemResponse<StatTracker>();
            Account acc = _accountService.GetAccountByHandle(handle);

            if (acc != null)
            {
                response.Item = _statTrackerService.GetFollowerCount(acc.UserId);

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("ratings"), HttpGet]
        public HttpResponseMessage GetTotalCountAndAvg(string entity, int type)
        {
            ItemResponse<RatingStatTracker> response = new ItemResponse<RatingStatTracker>();
            response.Item = _statTrackerService.GetRatingCountAndAvg(entity, type);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("public"), HttpGet]
        public HttpResponseMessage GetTotalVisibleCount()
        {
            ItemResponse<StatTracker> response = new ItemResponse<StatTracker>();
            string userId = _userService.GetCurrentUserId();
            response.Item = _statTrackerService.GetVisibleFollowerCount(userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
