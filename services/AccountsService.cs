using Sabio.Web.Models.Requests.Account;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Sabio.Web.Domain.Account;
using Sabio.Data;
using Sabio.Web.Services.Interfaces;
using Sabio.Web.Models.Requests;
using Sabio.Web.Core;

namespace Sabio.Web.Services
{
    public class AccountsService : BaseService, IAccountsService
    {
        private ICacheService _cacheService = null;
        private string globalKey = "account_global_key";


        public AccountsService(ICacheService cacheService)
        {
            _cacheService = cacheService;
            _cacheService.Add(globalKey, new object());
        }

        // UPDATE BACKGROUND IMAGE
        public void UpdateBackground(AccountsRequest model, string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_UpdateBackgroundUrl"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", userId);
                  paramCollection.AddWithValue("@BackgroundUrl", model.BackgroundPicture);
              });

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }
        }

        // UPDATE VideoUrl
        public void UpdateVideo(AccountsRequest model, string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_UpdateVideoUrl"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", userId);
                  paramCollection.AddWithValue("@VideoUrl", model.VideoUrl);
              });

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }
        }

        // UPDATE Avatar IMAGE
        public void UpdateAvatar(AccountsAvatarRequest model, string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_UpdateAvatarUrl"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", userId);
                  paramCollection.AddWithValue("@AvatarUrl", model.AvatarUrl);
              });

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }
        }

        // UPDATE ACCOUNT
        public void Update(AccountsRequest model, string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_Update"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", userId);
                  paramCollection.AddWithValue("@AvatarUrl", model.AvatarUrl);
                  paramCollection.AddWithValue("@Handle", model.Handle);
                  paramCollection.AddWithValue("@FirstName", model.FirstName);
                  paramCollection.AddWithValue("@MiddleInitial", model.MiddleInitial);
                  paramCollection.AddWithValue("@LastName", model.LastName);
                  paramCollection.AddWithValue("@GenderId", model.GenderId);
                  paramCollection.AddWithValue("@DOB", model.DOB);
                  paramCollection.AddWithValue("@Highlight", model.Highlight);
                  paramCollection.AddWithValue("@Bio", model.Bio);
                  paramCollection.AddWithValue("@CreatedBy", userId);
                  paramCollection.AddWithValue("@ModifiedBy", userId);

              }
              );

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }
        }

        public int RegisterInsert(string handle, string userId)
        {
            int id = 0;
            //this stored proc auto sets to Lash girl need to change back after demo
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_InsertRegisteredUser_V2"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Handle", handle);


                    SqlParameter p = new SqlParameter("@UserId", SqlDbType.NVarChar);
                    p.Direction = ParameterDirection.InputOutput;
                    p.Value = userId;
                    paramCollection.Add(p);
                }
                , returnParameters: delegate (SqlParameterCollection param)
                {
                    int.TryParse(param["@UserId"].Value.ToString(), out id);
                });

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }

            return id;
        }

        public int BasicInsert(AccountBasicAddRequest model, string userId)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_Insert"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@FirstName", model.FirstName);
                    paramCollection.AddWithValue("@LastName", model.LastName);
                    paramCollection.AddWithValue("@CreatedBy", userId);

                    SqlParameter p = new SqlParameter("@UserId", SqlDbType.NVarChar);
                    p.Direction = ParameterDirection.InputOutput;
                    p.Value = userId;
                    paramCollection.Add(p);
                }
                , returnParameters: delegate (SqlParameterCollection param)
                {
                    int.TryParse(param["@UserId"].Value.ToString(), out id);
                });
            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }

            return id;
        }

        public void BasicUpdate(AccountBasicAddRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_Update_V2"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@FirstName", model.FirstName);
                    paramCollection.AddWithValue("@LastName", model.LastName);
                    paramCollection.AddWithValue("@Handle", model.Handle);

                });

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }
        }
        // CREATE ACCOUNT
        public int Insert(AccountsRequest model, string userId)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_Insert"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@AvatarUrl", model.AvatarUrl);
                  paramCollection.AddWithValue("@Handle", model.Handle);
                  paramCollection.AddWithValue("@FirstName", model.FirstName);
                  paramCollection.AddWithValue("@MiddleInitial", model.MiddleInitial);
                  paramCollection.AddWithValue("@LastName", model.LastName);
                  paramCollection.AddWithValue("@GenderId", model.GenderId);
                  paramCollection.AddWithValue("@DOB", model.DOB);
                  paramCollection.AddWithValue("@Highlight", model.Highlight);
                  paramCollection.AddWithValue("@Bio", model.Bio);
                  paramCollection.AddWithValue("@CreatedBy", userId);
                  paramCollection.AddWithValue("@CreatedDate", null);


                  SqlParameter p = new SqlParameter("@UserId", SqlDbType.NVarChar);
                  p.Direction = ParameterDirection.InputOutput;
                  p.Value = userId;
                  paramCollection.Add(p);

              }, returnParameters: delegate (SqlParameterCollection param)
              {
                  int.TryParse(param["@UserId"].Value.ToString(), out id);
              }
              );

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }

            return id;
        }

        // Get All Comments with pagination
        public PagedList<Account> GetAccountsPaging(int pageIndex, int pageSize)
        {
            PagedList<Account> pagedList = null;
            int totalCount = 0;

            List<Account> list = null;

            DataProvider.ExecuteCmd(GetConnection
                , "dbo.Accounts_SelectPaginate"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                }
                , map: delegate (IDataReader reader, short set)
                 {
                     Account p = MapAccount(reader);

                     if (list == null)
                     {
                         list = new List<Account>();
                     }
                     list.Add(p);

                     if (totalCount == 0)
                         totalCount = reader.GetSafeInt32(12);
                 });

            if (list != null)
            {
                pagedList = new PagedList<Account>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }

        // GET HIGHLIGHTED!!!
        public List<BaseAccount> GetHighlighted(bool high)
        {
            List<BaseAccount> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SelectByHighlight"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Highlight", high);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    Account accountWizard = null;
                    accountWizard = MapAccount(reader);
                    BaseAccount acc = new BaseAccount();
                    acc.AvatarUrl = accountWizard.AvatarUrl;
                    acc.Handle = accountWizard.Handle;
                    if (list == null)
                    {
                        list = new List<BaseAccount>();
                    }

                    list.Add(acc);
                });

            return list;
        }

        // GET BY ACCOUNT MODIFIER!!!
        public List<Account> GetModifier(int modifier)
        {
            Account accountWizard = null;
            List<Account> list = new List<Account>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SelectByAccountModifier"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@AccountModifier", modifier);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    accountWizard = MapAccount(reader);
                    if (list == null)
                    {
                        list = new List<Account>();
                    }

                    list.Add(accountWizard);
                });

            return list;
        }

        // GET BY USER ID
        public Account Get(string userId)
        {
            string key = "account_" + userId;

            Account accountWizard = null;
            if (_cacheService.Contains(key))
            {
                accountWizard = _cacheService.Get<Account>(key);
            }
            else
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SelectById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserId", userId);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   accountWizard = MapAccount(reader);
               }
               );
                _cacheService.Add(key, accountWizard, globalKey);
            }

            return accountWizard;
        }

        // GET BY HANDLE
        public Account GetAccountByHandle(string handle)
        {
            string key = "account_" + handle;
            Account accountWizard = null;

            if (_cacheService.Contains(key))
            {
                accountWizard = _cacheService.Get<Account>(key);
            }
            else
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SelectByHandle"
                    , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                    {
                        paramCollection.AddWithValue("@Handle", handle);
                    }
                        , map: delegate (IDataReader reader, short set)
                        {
                            accountWizard = MapAccount(reader);
                        });
                _cacheService.Add(key, accountWizard, globalKey);
            }

            return accountWizard;
        }

        public bool GetHandleExists(string handle)
        {
            bool handleExists = false;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_HandleExists"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Handle", handle);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    handleExists = reader.GetSafeBool(0);

                });

            return handleExists;
        }

        public void Delete(string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Accounts_Delete"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", userId);
              }
           );

            if (_cacheService.Contains(globalKey))
            {
                _cacheService.Remove(globalKey);
                _cacheService.Add(globalKey, new object());
            }
        }

        public PagedList<IndexAccount> GetPagingByModifier_V2(int pageIndex, int pageSize, int accountModifier)
        {
            PagedList<IndexAccount> pagedList = null;
            int totalCount = 0;
            List<IndexAccount> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SelectPagByModifier_V2"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@PageIndex", pageIndex);
                   paramCollection.AddWithValue("@PageSize", pageSize);
                   paramCollection.AddWithValue("@AccountModifier", accountModifier);
               }
                , map: delegate (IDataReader reader, short set)
                {
                    IndexAccount p = MapAccountPagination_V2(reader);

                    if (list == null)
                    {
                        list = new List<IndexAccount>();
                    }
                    list.Add(p);

                    if (totalCount == 0)
                    {
                        totalCount = p.TotalCount;
                    }

                });
            if (list != null)
            {
                pagedList = new PagedList<IndexAccount>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }
        public PagedList<Account> GetPagingByModifier(int pageIndex, int pageSize, int accountModifier)
        {
            PagedList<Account> pagedList = null;
            int totalCount = 0;

            List<Account> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SelectPagByModifier"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                    paramCollection.AddWithValue("@AccountModifier", accountModifier);
                }
                 , map: delegate (IDataReader reader, short set)
                 {
                     Account p = MapAccountPagination(reader);

                     if (list == null)
                     {
                         list = new List<Account>();
                     }
                     list.Add(p);

                     if (totalCount == 0)
                         totalCount = p.TotalCount;
                 });
            if (list != null)
            {
                pagedList = new PagedList<Account>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }

        public PagedList<Account> Search(string searchText, int pageIndex, int pageSize, int accountModifier)
        {
            PagedList<Account> pagedList = null;
            int totalCount = 0;

            List<Account> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Accounts_SearchModifierByName"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@SearchText", searchText);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                    paramCollection.AddWithValue("@AccountModifier", accountModifier);
                }
                 , map: delegate (IDataReader reader, short set)
                 {
                     Account p = MapAccountPagination(reader);

                     if (list == null)
                     {
                         list = new List<Account>();
                     }
                     list.Add(p);

                     if (totalCount == 0)
                         totalCount = p.TotalCount;
                 });
            if (list != null)
            {
                pagedList = new PagedList<Account>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }
        // Mappers
        internal Account MapAccount(IDataReader reader)
        {
            Account account = new Account();
            int startingIndex = 0;

            account.UserId = reader.GetSafeString(startingIndex++);
            account.AvatarUrl = reader.GetSafeString(startingIndex++);
            account.Handle = reader.GetSafeString(startingIndex++);
            account.FirstName = reader.GetSafeString(startingIndex++);
            account.MiddleInitial = reader.GetSafeString(startingIndex++);
            account.LastName = reader.GetSafeString(startingIndex++);
            account.GenderID = reader.GetSafeInt16Nullable(startingIndex++);
            account.DOB = reader.GetSafeDateTimeNullable(startingIndex++);
            account.Highlight = reader.GetSafeBool(startingIndex++);
            account.Bio = reader.GetSafeString(startingIndex++);
            account.AccountModifier = reader.GetSafeInt32(startingIndex++);
            account.BackgroundPicture = reader.GetSafeString(startingIndex++) ?? "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRSK35OwX9nVNTcL4Mwn6Wrf0EowLS4SXvCNvNsbY81RWCzr0OZcw";
            account.VideoUrl = reader.GetSafeString(startingIndex++);

            return account;
        }

        public PagedList<Account> GetInfluencersByLGProduct(int productId, int pageIndex, int pageSize)
        {
            PagedList<Account> pagedList = null;
            int totalCount = 0;

            List<Account> list = null;

            DataProvider.ExecuteCmd(GetConnection
                , "dbo.ProductInfluencer_SelectByLGProductId_Pagination"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@ProductId", productId);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    Account p = MapAccountPagination(reader);

                    if (list == null)
                    {
                        list = new List<Account>();
                    }
                    list.Add(p);

                    if (totalCount == 0)
                        totalCount = reader.GetSafeInt32(11);
                });

            if (list != null)
            {
                pagedList = new PagedList<Account>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }


        public PagedList<Account> GetRandom(int pageSize)
        {
            PagedList<Account> pagedList = null;
            int totalCount = 0;

            List<Account> list = null;

            DataProvider.ExecuteCmd(GetConnection
                , "dbo.ProductInfluencer_SelectByRandom"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {

                    paramCollection.AddWithValue("@PageSize", pageSize);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    Account p = MapAccountPagination(reader);

                    if (list == null)
                    {
                        list = new List<Account>();
                    }
                    list.Add(p);

                    if (totalCount == 0)
                        totalCount = reader.GetSafeInt32(11);
                });

            if (list != null)
            {
                pagedList = new PagedList<Account>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }

        private Account MapAccountPagination(IDataReader reader)
        {
            Account account = new Account();
            int startingIndex = 0;

            account.UserId = reader.GetSafeString(startingIndex++);
            account.AvatarUrl = reader.GetSafeString(startingIndex++);
            account.Handle = reader.GetSafeString(startingIndex++);
            account.FirstName = reader.GetSafeString(startingIndex++);
            account.MiddleInitial = reader.GetSafeString(startingIndex++);
            account.LastName = reader.GetSafeString(startingIndex++);
            account.GenderID = reader.GetSafeInt16Nullable(startingIndex++);
            account.DOB = reader.GetSafeDateTimeNullable(startingIndex++);
            account.Highlight = reader.GetSafeBool(startingIndex++);
            account.Bio = reader.GetSafeString(startingIndex++);
            account.AccountModifier = reader.GetSafeInt32(startingIndex++);
            account.TotalCount = reader.GetSafeInt32(startingIndex++);

            return account;
        }
        private IndexAccount MapAccountPagination_V2(IDataReader reader)
        {
            IndexAccount account = new IndexAccount();
            int startingIndex = 0;

            account.UserId = reader.GetSafeString(startingIndex++);
            account.AvatarUrl = reader.GetSafeString(startingIndex++);
            account.Handle = reader.GetSafeString(startingIndex++);
            account.FirstName = reader.GetSafeString(startingIndex++);
            account.MiddleInitial = reader.GetSafeString(startingIndex++);
            account.LastName = reader.GetSafeString(startingIndex++);
            account.GenderID = reader.GetSafeInt16Nullable(startingIndex++);
            account.DOB = reader.GetSafeDateTimeNullable(startingIndex++);
            account.Highlight = reader.GetSafeBool(startingIndex++);
            account.AccountModifier = reader.GetSafeInt32(startingIndex++);
            account.TotalCount = reader.GetSafeInt32(startingIndex++);

            return account;
        }
    }
}
