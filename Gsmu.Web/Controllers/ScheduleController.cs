using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule HaikuSftp
        public ActionResult HaikuSftp()
        {
            var haiku = new Gsmu.Web.Areas.Adm.Controllers.HaikuController();
            return haiku.HaikuSftp();
        }
    }
}