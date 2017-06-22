using Sabio.Data;
using Sabio.Web.Core;
using Sabio.Web.Domain;
using Sabio.Web.Domain.Comments;
using Sabio.Web.Domain.Ratings;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.Comments;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services.Comments
{
    public class CommentsService : BaseService, ICommentsService
    {
        IAccountsService _accountService = null;
        private ICacheService _cacheService = null;

        public CommentsService(IAccountsService accountService, ICacheService cacheService)
        {
            _accountService = accountService;
            _cacheService = cacheService;
        }

        // Insert new Comment
        public int Insert(CommentsAddRequest model, string userId)
        {
            int Id = 0;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@EntityId", model.EntityId);
                    paramCollection.AddWithValue("@ParentId", model.ParentId);
                    paramCollection.AddWithValue("@TypeId", model.TypeId);
                    paramCollection.AddWithValue("@Title", model.Title);
                    paramCollection.AddWithValue("@Description", model.Description);
                    paramCollection.AddWithValue("@CreatedBy", userId);
                    paramCollection.AddWithValue("@CreatedDate", model.CreatedDate);
                    paramCollection.AddWithValue("@ModifiedDate", model.ModifiedDate);

                    SqlParameter com = new SqlParameter("@Id", SqlDbType.Int);
                    com.Direction = ParameterDirection.Output;
                    paramCollection.Add(com);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    int.TryParse(param["@Id"].Value.ToString(), out Id);
                }
               )

            return Id;
        }

        // Update with Id
        public void Update(CommentsUpdateRequest model, int id, string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_Update"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@EntityId", model.EntityId);
                paramCollection.AddWithValue("@ParentId", model.ParentId);
                paramCollection.AddWithValue("@TypeId", model.TypeId);
                paramCollection.AddWithValue("@Title", model.Title);
                paramCollection.AddWithValue("@Description", model.Description);
                paramCollection.AddWithValue("@CreatedBy", userId);
                paramCollection.AddWithValue("@CreatedDate", model.CreatedDate);
                paramCollection.AddWithValue("@ModifiedBy", userId);
                paramCollection.AddWithValue("@Id", id);
            });
        }

        public void RemovedComment(int id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_RemovedUpdate"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            });
        }

        // Get by Id
        public CommentComplete Get(int id)
        {
            CommentComplete com = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_Select"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", id);
               }
                   , map: delegate (IDataReader reader, short set)
                   {
                       com = MapComment<CommentComplete>(reader);

                   });

            return com;
        }

        // Get All
        public List<CommentComplete> Get()
        {
            CommentComplete com = null;
            List<CommentComplete> list = new List<CommentComplete>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_SelectAll"
               , inputParamMapper: null, map: delegate (IDataReader reader, short set)
               {
                   com = MapComment<CommentComplete>(reader);

                   if (list == null)
                   {
                       list = new List<CommentComplete>();
                   }

                   list.Add(com);
               }
            );

            return list;
        }

        // Delete by Id
        public void Delete(int id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_Delete"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", id);
              }
           );
        }

        // Get All Comments with pagination
        public PagedList<Comment> GetCommentsPaging(int pageIndex, int pageSize)
        {
            PagedList<Comment> pagedList = null;
            int totalCount = 0;

            List<Comment> list = null;

            DataProvider.ExecuteCmd(GetConnection
                , "dbo.Comments_SelectPaginate"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    Comment p;
                    MapWithTotalCount(reader, out p);

                    if (list == null)
                    {
                        list = new List<Comment>();
                    }
                    list.Add(p);
                    if (totalCount == 0)
                        totalCount = p.TotalCount;
                });

            if (list != null)
            {
                pagedList = new PagedList<Comment>(list, 0, pageSize, totalCount);

            }

            return pagedList;
        }

        // Get All Comments with Ratings
        public List<Comment> GetwithRatings()
        {
            Comment com = null;
            List<Comment> list = new List<Comment>();
            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_SelectAll_withRatings"
              , inputParamMapper: null, map: delegate (IDataReader reader, short set)
                {
                    com = MapCommentRating(reader, out com);
                    if (list == null)
                    {
                        list = new List<Comment>();
                    }

                    list.Add(com);
                });

            return list;
        }

        // Get Comments with Ratings for current Entity and Type
        public List<Comment> GetRatingsforEntityType(string entityId, int type)
        {
            Comment com = null;
            List<Comment> list = new List<Comment>();
            DataProvider.ExecuteCmd(GetConnection, "Comments_SelectbyEntityType_withRatings"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@EntityId", entityId);
                  paramCollection.AddWithValue("@TypeId", type);
              }
                  , map: delegate (IDataReader reader, short set)
                  {
                      com = MapCommentRating(reader, out com);
                      if (list == null)
                      {
                          list = new List<Comment>();
                      }

                      list.Add(com);
                  });

            return list;
        }

        // Get Comments for current Entity and Type
        public List<CommentComplete> GetbyEntityType(string entityId, int type)
        {

            // these are where the parents go
            //List<CommentComplete> topLevelComments = null;
            List<CommentComplete> allComments = null;
            Dictionary<int, List<CommentComplete>> children = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_SelectbyEntityType"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@EntityId", entityId);
                   paramCollection.AddWithValue("@TypeId", type);
               }
                   , map: delegate (IDataReader reader, short set)
                   {
                       CommentComplete com = MapComment<CommentComplete>(reader);

                       int commentId = com.Id;
                       int parentId = com.ParentId;

                       if (children == null)
                       {
                           children = new Dictionary<int, List<CommentComplete>>();
                       }

                       if (allComments == null)
                       {
                           allComments = new List<CommentComplete>();
                       }

                       if (!children.ContainsKey(parentId))
                       {
                           children.Add(parentId, new List<CommentComplete>());
                       }

                       children[parentId].Add(com);

                       allComments.Add(com);
                   });

            if (allComments != null)
            {
                foreach (CommentComplete comment in allComments)
                {
                    if (!children.ContainsKey(comment.Id))
                    {
                        continue;
                    }

                    List<CommentComplete> newreplies = children[comment.Id];

                    comment.Replies = newreplies;
                }

                return children[0];

            }

            return allComments;
        }

        public List<Comment> GetUsersCommentsByEntityType(string entityId, int type)
        {
            Comment com = null;
            List<Comment> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_AccountsSelectByEntityId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@EntityId", entityId);
                   paramCollection.AddWithValue("@TypeId", type);
               }
                   , map: delegate (IDataReader reader, short set)
                   {
                       com = MapCommentUser(reader);
                       if (list == null)
                       {
                           list = new List<Comment>();
                       }

                       list.Add(com);
                   });

            return list;

        }

        public CommentsAndUserResponse AllCommentsForEntityIdAndType(string entityId, int type, string userId)
        {
            List<Comment> comments = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_AccountsSelectByEntityId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@EntityId", entityId);
                   paramCollection.AddWithValue("@TypeId", type);
               }
                   , map: delegate (IDataReader reader, short set)
                   {
                       Comment com = MapCommentUser(reader);
                       if (comments == null)
                       {
                           comments = new List<Comment>();
                       }

                       comments.Add(com);
                   });

            return new CommentsAndUserResponse
            {
                CurrentUser = _accountService.Get(userId),
                Comments = comments
            };
        }

        // Mappers
        private static T MapComment<T>(IDataReader reader) where T : Comment, new()
        {
            T com = new T();
            //   RatingDomain rate = new RatingDomain();
            int startingIndex = 0;
            com.Id = reader.GetSafeInt32(startingIndex++);
            com.EntityId = reader.GetSafeString(startingIndex++);
            com.ParentId = reader.GetSafeInt32(startingIndex++);
            com.TypeId = reader.GetSafeInt32(startingIndex++);
            com.Title = reader.GetSafeString(startingIndex++);
            com.Description = reader.GetSafeString(startingIndex++);
            //com.CreatedBy = reader.GetSafeString(startingIndex++);
            com.CreatedDate = reader.GetSafeUtcDateTime(startingIndex++);
            //com.ModifiedBy = reader.GetSafeString(startingIndex++);
            com.ModifiedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.Handle = reader.GetSafeString(startingIndex++);
            com.FirstName = reader.GetSafeString(startingIndex++);
            com.LastName = reader.GetSafeString(startingIndex++);
            com.AvatarUrl = reader.GetSafeString(startingIndex++);

            return com;
        }

        private static Comment MapCommentRating(IDataReader reader, out Comment com)
        {
            com = new Comment();
            com.Rating = new Rating();
            int startingIndex = 0;
            com.Id = reader.GetSafeInt32(startingIndex++);
            com.EntityId = reader.GetSafeString(startingIndex++);
            com.ParentId = reader.GetSafeInt32(startingIndex++);
            com.TypeId = reader.GetSafeInt32(startingIndex++);
            com.Title = reader.GetSafeString(startingIndex++);
            com.Description = reader.GetSafeString(startingIndex++);
            com.CreatedBy = reader.GetSafeString(startingIndex++);
            com.CreatedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.ModifiedBy = reader.GetSafeString(startingIndex++);
            com.ModifiedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.Rating.Id = reader.GetSafeInt32(startingIndex++);
            com.Rating.RatingAmount = reader.GetSafeByte(startingIndex++);
            com.Rating.CreatedDate = reader.GetSafeUtcDateTime(startingIndex++);

            return com;
        }

        private static Comment MapCommentUser(IDataReader reader)
        {
            Comment com = new Comment();
            //   RatingDomain rate = new RatingDomain();
            int startingIndex = 0;
            com.DiffDate = reader.GetSafeInt32(startingIndex++);
            com.Id = reader.GetSafeInt32(startingIndex++);
            com.EntityId = reader.GetSafeString(startingIndex++);
            com.ParentId = reader.GetSafeInt32(startingIndex++);
            com.TypeId = reader.GetSafeInt32(startingIndex++);
            com.Title = reader.GetSafeString(startingIndex++);
            com.Description = reader.GetSafeString(startingIndex++);
            com.CreatedBy = reader.GetSafeString(startingIndex++);
            com.CreatedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.ModifiedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.Handle = reader.GetSafeString(startingIndex++);
            com.FirstName = reader.GetSafeString(startingIndex++);
            com.LastName = reader.GetSafeString(startingIndex++);
            com.AvatarUrl = reader.GetSafeString(startingIndex++);

            return com;
        }

        private static Comment MapWithTotalCount(IDataReader reader, out Comment com)
        {
            com = new Comment();
            int startingIndex = 0;

            com.Id = reader.GetSafeInt32(startingIndex++);
            com.EntityId = reader.GetSafeString(startingIndex++);
            com.ParentId = reader.GetSafeInt32(startingIndex++);
            com.TypeId = reader.GetSafeInt32(startingIndex++);
            com.Title = reader.GetSafeString(startingIndex++);
            com.Description = reader.GetSafeString(startingIndex++);
            com.CreatedBy = reader.GetSafeString(startingIndex++);
            com.CreatedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.ModifiedBy = reader.GetSafeString(startingIndex++);
            com.ModifiedDate = reader.GetSafeUtcDateTime(startingIndex++);
            com.TotalCount = reader.GetSafeInt32(startingIndex++);

            return com;
        }
    }
}