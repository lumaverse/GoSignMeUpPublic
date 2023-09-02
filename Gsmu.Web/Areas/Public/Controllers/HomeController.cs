using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Public/Home/

        public ActionResult Index()
        {
                        string WelcomePage = System.Configuration.ConfigurationManager.AppSettings["WelcomePage"];
                        string curReferer = Request.Headers["Referer"];
                        if (!string.IsNullOrEmpty(WelcomePage) && !curReferer.ToLower().Contains(WelcomePage.ToLower()))
                        {
                            return Redirect("/"+WelcomePage);
                        }
                        else
                        {
                            return RedirectToAction("Browse", "Course", new
                            {
                                loginRedirectUrl = Request.QueryString["loginRedirectUrl"]
                            });
                        }
        }

    }
}
