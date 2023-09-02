using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Adm/Reports
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReportsPaymentClassList(int courseId, string type = "modal") {
            if (type == "modal")
                return PartialView("ReportsPaymentClassList");
            else
                return View();
        }

        public ActionResult ReportsOrderDetail(string orderNumber, string type = "modal")
        {
            if(type == "modal")
                return PartialView("ReportsOrderDetail");
            else
                return View();
        }
    }
}