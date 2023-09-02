using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Adm/Students
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StudentNotes() {
            return PartialView("StudentNotes");
        }
    }
}