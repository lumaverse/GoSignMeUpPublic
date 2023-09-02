using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Gsmu.Api.Web;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SSO()
        {
            return View();
        }

    }
}
