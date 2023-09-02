using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Gsmu.Api.Web;

namespace Gsmu.Web.Areas.Membership.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           // return View();

            return RedirectToAction("Browse", "../public/Course", new { showMembership = 1 });


        }

    }
}
