using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class SupportController : Controller
    {
        //
        // GET: /Public/Support/

        public ActionResult Browser()
        {
            if ((Request.Browser.Browser != "IE" && Request.Browser.Browser != "InternetExplorer") || Request.Browser.MajorVersion > 7)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

    }
}
