using Sabio.Web.Models.Requests.Followers;
using Sabio.Web.Services.Interfaces;
using Sabio.Web.Models.Responses;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sabio.Web.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using Sabio.Web.Domain.Account;
using System;
using Sabio.Web.Models;
using System.Collections.Generic;
using Sabio.Web.Domain.Follower;
using Sabio.Web.Models.Requests;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/followers")]
    public class FollowerApiController : ApiController
    {

        private IFollowerService _followerSevice = null;
        private IUserService _userService = null;
        private IAccountsService _accountSrv = null;

        public FollowerApiController(IFollowerService followerService, IUserService userService, IAccountsService accountInject)
        {
            _followerSevice = followerService;
            _userService = userService;
            _accountSrv = accountInject;
        }

        [Route("current/{handle}"), HttpPost]
        public HttpResponseMessage InsertCurrentByHandle(string handle, FollowerRequest data)
        {
            BaseResponse response = null;
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser user = _userService.GetCurrentUser();
                    Account followedLG = _accountSrv.GetAccountByHandle(handle);
                    if (followedLG != null)
                    {
                        FollowNewsletter nwsInfo = new FollowNewsletter();
                        nwsInfo.Email = user.Email;
                        nwsInfo.Handle = handle;
                        data.FollowerID = user.Id;
                        data.ProfileUID = followedLG.UserId;


                        _followerSevice.Insert(data, nwsInfo);
                        response = new SuccessResponse();
                        return Request.CreateResponse(HttpStatusCode.OK, response);
                    }
                    else
                    {
                        throw new Exception("Account does not exist");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ModelState);
                }
            }
            catch (Exception ex)
            {
                response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("current/{handle}"), HttpDelete]
        public HttpResponseMessage DeleteCurrentByHandle(string handle)
        {
            BaseResponse response = null;
            try
            {
                IdentityUser user = _userService.GetCurrentUser();
                if (user != null)
                {
                    FollowerRequest requestData = new FollowerRequest();
                    FollowNewsletter nwsInfo = new FollowNewsletter();
                    Account followedLG = _accountSrv.GetAccountByHandle(handle);
                    requestData.FollowerID = user.Id;
                    requestData.ProfileUID = followedLG.UserId;
                    //requestData.FollowNewsletter = (delete.Contains("newsletter")) ? true : false;
                    nwsInfo.Email = user.Email;
                    nwsInfo.Handle = handle;

                    _followerSevice.Delete(requestData, nwsInfo);
                    response = new SuccessResponse();
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response = new ErrorResponse("User is not logged in");
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                }
            }
            catch (Exception ex)
            {
                response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        /* For pagination */
        [Route("{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Domain.Follower.Follower>> response = new ItemResponse<PagedList<Domain.Follower.Follower>>();
            string userId = _userService.GetCurrentUserId();
            response.Item = _followerSevice.GetFollowerPaging(userId, pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /* For pagination of visible followers*/
        [Route("{handle}/{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage GetVisible(string handle, int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Domain.Follower.Follower>> response = new ItemResponse<PagedList<Domain.Follower.Follower>>();
            response.Item = _followerSevice.GetVisibleFollowers(handle, pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("link/{handle}"), HttpGet]
        public HttpResponseMessage IsFollower(string handle)
        {
            try
            {
                IdentityUser user = _userService.GetCurrentUser();
                Account acc = _accountSrv.GetAccountByHandle(handle);
                if (acc != null)
                {
                    FollowerRequest request = new FollowerRequest();
                    FollowNewsletter nwsInfo = new FollowNewsletter();
                    request.FollowerID = user.Id;
                    request.ProfileUID = acc.UserId;
                    nwsInfo.Email = user.Email;
                    nwsInfo.Handle = handle;

                    ItemResponse<FollowerCheck> response = new ItemResponse<FollowerCheck>();
                    response.Item = _followerSevice.IsFollower(request, nwsInfo);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    throw new Exception("That Account does not exist");
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route(), HttpDelete]
        public HttpResponseMessage DeleteFollowerByAccount(FollowerRequest model)
        {
            SuccessResponse response = new SuccessResponse();
            _followerSevice.DeleteFollowerByAccount(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{currentUserId}"), HttpDelete]
        public HttpResponseMessage DeleteFollower(string currentUserId)
        {
            SuccessResponse response = new SuccessResponse();
            _followerSevice.DeleteFollower(currentUserId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("map")]
        public HttpResponseMessage SelectByBounds(BoundSearchRequest bounds)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            BaseResponse rsp;
            HttpStatusCode httpStatus;
            string userId = _userService.GetCurrentUserId();

            if (userId != null)
            {
                rsp = new ItemsResponse<Domain.Follower.Follower>(_followerSevice.GetFollowers(userId, bounds));
                httpStatus = HttpStatusCode.OK;
            }
            else
            {
                rsp = new ErrorResponse("Forbidden. User not found");
                httpStatus = HttpStatusCode.Forbidden;
            }
            return Request.CreateResponse(httpStatus, rsp);
        }

        [Route("follow"), HttpPost]
        public HttpResponseMessage Follow(string profileId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse response = new SuccessResponse();
            string userId = _userService.GetCurrentUserId();
            _followerSevice.Follow(userId, profileId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("get/profilesids"), HttpGet]
        public HttpResponseMessage GetByFollowerId()
        {
            ItemsResponse<BaseFollower> response = new ItemsResponse<BaseFollower>();
            string userId = _userService.GetCurrentUserId();
            response.Items = _followerSevice.Get(userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("unfollow/{profileId}"), HttpDelete]
        public HttpResponseMessage Unfollow(string profileId)
        {
            SuccessResponse response = new SuccessResponse();
            string userId = _userService.GetCurrentUserId();
            _followerSevice.Unfollow(userId, profileId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}