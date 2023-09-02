using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Adm/Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminCourseDashboard(int courseId = 0)
        {
            if (courseId > 0)
                return View();
            else
                return View("AdminCourseDashboardLanding");
        }
    }
}