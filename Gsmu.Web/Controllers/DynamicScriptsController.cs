using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Gsmu.Api.Data.School.Terminology;

namespace Gsmu.Web.Controllers
{
    public class DynamicScriptsController : Controller
    {
        public ActionResult Terminology()
        {
            Response.ContentType = "application/javascript";
            return View();
        }

    }
}
