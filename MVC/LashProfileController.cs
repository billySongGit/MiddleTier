using Sabio.Web.Domain.Account;
using Sabio.Web.Models.ViewModels;
using Sabio.Web.Services;
using Sabio.Web.Services.Interfaces;
using Sabio.Web.Services.MetaTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sabio.Web.Domain.MetaTags;


namespace Sabio.Web.Controllers.UserProfile
{

    [AllowAnonymous]
    //[RoutePrefix("lashgirls")]
    public class LashProfileController : SiteController
    {
        IAccountsService _accountService = null;
        IUserRoleService _userRoleService = null;
        private Account _acc;

        public LashProfileController(IAccountsService accountService, IUserRoleService userRoleService)
        {
            _accountService = accountService;
            _userRoleService = userRoleService;
        }

        public override string BodyPageCss
        {
            get
            {
                return "profile-page";
            }
        }

        public override OwnerType OwnerType
        {
            get
            {
                return OwnerType.Site;
            }
        }

        public override string EntityId
        {
            get
            {
                return _entityId ?? "1";
            }
        }

        [AllowAnonymous]
        [Route("lashgirls/{handle}")]
        // GET: LashProfile
        public ActionResult LashProfile(string handle)
        {
            _acc = _accountService.GetAccountByHandle(handle);

            Domain.UserProfile user = _userRoleService.GetUserByHandle(handle);

            if ((user  == null || user.LockoutEnabled) || (user.AccountModifier != 1 && user.AccountModifier != 2))
            {
                return RedirectToAction("InvalidUser");
            }

            ItemViewModel<string> model = GetViewModel<ItemViewModel<string>>();
            if (_acc != null)
            {
                _entityId = handle;
            }

            model.Item = handle;

            return View(model);
        }

       
        protected override List<PageMetaTagWName> GetMetaTags()
        {
            List<PageMetaTagWName> tags = base.GetMetaTags();

            if (tags != null && _acc != null)
            {
                for (var i = 0; i < tags.Count; i++)
                {
                    if (tags[i].Name == "og:description" || tags[i].Name == "twitter:description")
                    {
                        tags[i].Value = _acc.Bio;
                    }
                    if (tags[i].Name == "og:image" || tags[i].Name == "twitter:image")
                    {
                        tags[i].Value = _acc.AvatarUrl;

                    }

                }
                
            }

            return tags;
        }
        [AllowAnonymous]
        [Route("lashgirls/profile/invalid")]
        // GET: LashProfile
        public ActionResult InvalidUser()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("lashgirls")]
        // GET: LashProfile
        public ActionResult Lashgirls()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("lifestylecoaches")]
        //GET: LashProfile
        public ActionResult HealthyCoaches()
        {
            return View();
        }
    }
}
