using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lti = Gsmu.Api.Integration.Lti;
using web = Gsmu.Api.Web;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class LtiController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("settings");
        }


        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveSettings(string oAuthServiceKey, string oAuthServiceSecret)
        {
            bool success = true;
            string message = string.Empty;
            try
            {
                var config = lti.Configuration.Instance;
                config.OAuthServiceKey = oAuthServiceKey;
                config.OAuthServiceSecret = oAuthServiceSecret;
                config.Save();
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return new JsonResult()
            {
                Data = new
                {
                    config = lti.Configuration.Instance,
                    success = success,
                    message = message
                }
            };
        }

    }
}
