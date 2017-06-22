using Sabio.Web.Domain;
using Sabio.Web.Domain.Comments;
using Sabio.Web.Models.Requests.Comments;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using Sabio.Web.Services.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api.Comments
{
    [AllowAnonymous]
    [RoutePrefix("api/comments")]
    public class CommentsApiController : ApiController
    {
        ICommentsService _commentService = null;
        IUserService _userService = null;

        public CommentsApiController(ICommentsService commentService, IUserService userService)
        {
            _commentService = commentService;
            _userService = userService;
        }

        // Add a new comment
        [Route, HttpPost]
        public HttpResponseMessage Insert(CommentsAddRequest model)
        {
            string userId = _userService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                userId = "18c5d19c - 998a - 4c5f - be05 - 1a3fa116d278";
            }
            if (!ModelState.IsValid || model == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = _commentService.Insert(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("remove/{id:int}"), HttpPut]
        public HttpResponseMessage RemoveComment(int id)
        {
            ItemResponse<Comment> response = new ItemResponse<Comment>();
            _commentService.RemovedComment(id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
       
        // Update comment with Id
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(CommentsUpdateRequest model, int id)
        {
            string userId = _userService.GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
            {
                userId = "18c5d19c - 998a - 4c5f - be05 - 1a3fa116d278";
            }
            if (!ModelState.IsValid || model == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<Comment> response = new ItemResponse<Comment>();
            _commentService.Update(model, id, userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // Get Comment by Id
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage Get(int id)
        {
            ItemResponse<Comment> response = new ItemResponse<Comment>();
            response.Item = _commentService.Get(id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /* For pagination */
        [Route("page/{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Comment>> response = new ItemResponse<PagedList<Comment>>();
            response.Item = _commentService.GetCommentsPaging(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // Get all Comments with Ratings
        [Route("ratings"), HttpGet]
        public HttpResponseMessage GetRatings()
        {
            ItemsResponse<Comment> response = new ItemsResponse<Comment>();
            response.Items = _commentService.GetwithRatings();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // Get all Comments for current entity and type with Ratings
        [Route("ratings/{entityId}/{type:int}"), HttpGet]
        public HttpResponseMessage GetRatingsEntityType(string entityId, int type)
        {
            ItemsResponse<Comment> response = new ItemsResponse<Comment>();
            response.Items = _commentService.GetRatingsforEntityType(entityId, type);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // Get all Comments for current entity and type
        [Route("{entityId}/{type:int}"), HttpGet]
        public HttpResponseMessage GetforEntityType(string entityId, int type)
        {
            ItemsResponse<CommentComplete> response = new ItemsResponse<CommentComplete>();
            response.Items = _commentService.GetbyEntityType(entityId, type);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("user/{entityId}/{type:int}"), HttpGet]
        public HttpResponseMessage GetUserCommentByEntityType(string entityId, int type)
        {
            ItemResponse<CommentsAndUserResponse> response = new ItemResponse<CommentsAndUserResponse>();
            response.Item = _commentService.AllCommentsForEntityIdAndType(entityId, type, _userService.GetCurrentUserId());

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // Get all Commments
        [Route, HttpGet]
        public HttpResponseMessage GetAll()
        {
            ItemsResponse<CommentComplete> response = new ItemsResponse<Domain.Comments.CommentComplete>();
            response.Items = _commentService.Get();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // Delete a comment
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            ItemResponse<Comment> response = new ItemResponse<Comment>();
            _commentService.Delete(id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}
