using Sabio.Web.Domain;
using Sabio.Web.Models.Requests.Images;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/images")]
    public class ImagesApiController : ApiController
    {
        private IImagesService _imagesService = null;
        private IUserService _userService = null;
        public ImagesApiController(IImagesService imagesService, IUserService userService)
        {
            _imagesService = imagesService;
            _userService = userService;
        }

        [Route, HttpPost]
        public HttpResponseMessage AddImage(ImagesAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string userId = _userService.GetCurrentUserId();
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = _imagesService.Insert(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("product/{pid:int}"), HttpGet]
        public HttpResponseMessage SelectByProductId(int pId)
        {
            ItemsResponse<Image> response = new ItemsResponse<Image>();
            response.Items = _imagesService.GetByProductId(pId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpGet]
        public HttpResponseMessage SelectAll()
        {
            ItemsResponse<Image> response = new ItemsResponse<Image>();
            response.Items = _imagesService.Get();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("user"), HttpGet]
        public HttpResponseMessage SelectUserImages()
        {
            string userId = _userService.GetCurrentUserId();
            ItemsResponse<Image> response = new ItemsResponse<Image>();
            response.Items = _imagesService.getUserImages(userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage SelectById(int Id)
        {
            ItemResponse<Image> response = new ItemResponse<Image>();
            response.Item = _imagesService.Get(Id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(ImagesUpdateRequest model, int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                SuccessResponse response = new SuccessResponse();
                string currentUserId = _userService.GetCurrentUserId();
                _imagesService.Update(model, currentUserId, id);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [Route("Blogs/{id:int}"), HttpPut]
        public HttpResponseMessage UpdateBlog(Image model, int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                SuccessResponse response = new SuccessResponse();
                string currentUserId = _userService.GetCurrentUserId();
                _imagesService.UpdateBlogImages(model, id);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int Id)
        {
            SuccessResponse response = new SuccessResponse();
            _imagesService.Delete(Id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
