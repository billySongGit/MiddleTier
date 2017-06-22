using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Sabio.Web.Models;
using Sabio.Web.Controllers.BaseControllers;

// ********** FYI: System Generated File


namespace Sabio.Web.Controllers
{   
    public class AccountController : SiteController
    {
        public ActionResult AccountWizard()
        {
            return View();
        }
        [Route("~/Account")]
        public ActionResult AccountWizardNg()
        {
            return View();
        }
    }
}