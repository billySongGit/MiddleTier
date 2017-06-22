using Sabio.Web.Models.Requests.Account;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Sabio.Web.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sabio.Web.Domain.Account;
using Sabio.Web.Models.Requests;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sabio.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/accounts")]
    public class AccountsApiController : ApiController
    {
        private IAccountsService _accountsService = null;
        private IProductService _productService = null;
        private IUserService _userService = null;

        public AccountsApiController(IAccountsService accountsService, IProductService productService, IUserService userService)
        {
            _accountsService = accountsService;
            _productService = productService;
            _userService = userService;
        }

        /* For pagination */
        [Route("page/{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Account>> response = new ItemResponse<PagedList<Account>>();
            response.Item = _accountsService.GetAccountsPaging(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("pagev2/{pageIndex:int}/{pageSize:int}/{accountModifier:int}"), HttpGet]
        public HttpResponseMessage Get_V2(int pageIndex, int pageSize, int accountModifier)
        {
            ItemResponse<PagedList<IndexAccount>> response = new ItemResponse<PagedList<IndexAccount>>();
            response.Item = _accountsService.GetPagingByModifier_V2(pageIndex, pageSize, accountModifier);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("page/{pageIndex:int}/{pageSize:int}/{accountModifier:int}"), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize, int accountModifier)
        {
            ItemResponse<PagedList<Account>> response = new ItemResponse<PagedList<Account>>();
            response.Item = _accountsService.GetPagingByModifier(pageIndex, pageSize, accountModifier);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{search}/{pageIndex:int}/{pageSize:int}/{accountModifier:int}"), HttpGet]
        public HttpResponseMessage GetSearchPage(string search, int pageIndex, int pageSize, int accountModifier)
        {
            ItemResponse<PagedList<Account>> response = new ItemResponse<PagedList<Account>>();
            response.Item = _accountsService.Search(search, pageIndex, pageSize, accountModifier);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("influencers/{productId:int}/{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage GetAccountsByLGProduct(int productId, int pageIndex, int pageSize)
        {
            PairResponse<PagedList<Account>, PagedList<Account>> response = new PairResponse<PagedList<Account>, PagedList<Account>>();
            response.Item = _accountsService.GetInfluencersByLGProduct(productId, pageIndex, pageSize);
            response.Item2 = _productService.GetAccountsPag(productId, pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("random/{pageSize:int}"), HttpGet]
        public HttpResponseMessage GetRandom(int pageSize)
        {
            ItemResponse<PagedList<Account>> response = new ItemResponse<PagedList<Account>>();
            response.Item = _accountsService.GetRandom(pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpPut]
        public HttpResponseMessage Update(AccountsRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                SuccessResponse response = new SuccessResponse();
                string userId = _userService.GetCurrentUserId();
                _accountsService.Update(model, userId);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [Route("Background"), HttpPut]
        public HttpResponseMessage UpdateBackground(AccountsRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse r = new SuccessResponse();
            string userId = _userService.GetCurrentUserId();
            _accountsService.UpdateBackground(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, r);
        }

        [Route("video"), HttpPut]
        public HttpResponseMessage UpdateVideo(AccountsRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse r = new SuccessResponse();
            string userId = _userService.GetCurrentUserId();
            _accountsService.UpdateVideo(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, r);
        }


        [Route("Avatar"), HttpPut]
        public HttpResponseMessage UpdateAvatar(AccountsAvatarRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse r = new SuccessResponse();
            string userId = _userService.GetCurrentUserId();
            _accountsService.UpdateAvatar(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, r);
        }

        [Route("basic"), HttpPost]
        public HttpResponseMessage BasicInsert(AccountBasicAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string userId = _userService.GetCurrentUserId();
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = _accountsService.BasicInsert(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("basic/update"), HttpPut]
        public HttpResponseMessage BasicUpdate(AccountBasicAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse response = new SuccessResponse();
            _accountsService.BasicUpdate(model);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route(), HttpPost]
        public HttpResponseMessage Insert(AccountsRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();
            string userId = _userService.GetCurrentUserId();
            response.Item = _accountsService.Insert(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpGet]
        public HttpResponseMessage Get()
        {
            ItemResponse<Account> response = new ItemResponse<Account>();
            string userId = _userService.GetCurrentUserId();
            response.Item = _accountsService.Get(userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{handle}"), HttpGet]
        public HttpResponseMessage GetHandle(string handle)
        {
            ItemResponse<Account> response = new ItemResponse<Account>();
            response.Item = _accountsService.GetAccountByHandle(handle);

            if (response.Item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, response);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("email"), HttpGet]
        public HttpResponseMessage GetEmail()
        {
            ItemResponse<string> resp = null;
            HttpStatusCode status = HttpStatusCode.OK;
            IdentityUser user = _userService.GetCurrentUser();

            if (user != null)
            {
                resp = new ItemResponse<string>();
                resp.Item = user.Email;
            }
            else
            {
                status = HttpStatusCode.NotFound;
            }

            return Request.CreateResponse(status, resp);

        }

        [Route("modifier/{modifier:int}"), HttpGet]
        public HttpResponseMessage GetModifier(int modifier)
        {
            ItemsResponse<Account> response = new ItemsResponse<Account>();
            response.Items = _accountsService.GetModifier(modifier);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("highlighted"), HttpGet]
        public HttpResponseMessage GetHighlighted()
        {
            ItemsResponse<BaseAccount> response = new ItemsResponse<BaseAccount>();
            response.Items = _accountsService.GetHighlighted(true);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("handleExists"), HttpGet]
        public HttpResponseMessage GetHandleExists(string handle)
        {
            ItemResponse<bool> response = new ItemResponse<bool>();
            response.Item = _accountsService.GetHandleExists(handle);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}
