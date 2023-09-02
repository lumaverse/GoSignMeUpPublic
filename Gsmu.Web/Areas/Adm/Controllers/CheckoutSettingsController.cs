using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Data;
using System.Web.Helpers;

using student = Gsmu.Api.Data.School.Student;
using web = Gsmu.Api.Web;
using json = Newtonsoft.Json;
using models = Gsmu.Api.Data.ViewModels.UserFields;
using school = Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.User;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class CheckoutSettingsController : Controller
    {
        // REQUIRED ADMIN ACCESS
        // GET: /Admin/CheckoutSettings/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetCheckoutSettings()
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            result.Data = new
            {
                success = true,
                AllowPartialPayment = (Settings.Instance.GetMasterInfo3().AllowPartialPayment == 1) ? true : false
            };
            return result;
        }

        [HttpPost]
        public ActionResult UpdateCheckoutSettings(string PartialPayment)
        {
            int output = (PartialPayment == "1" || PartialPayment == "-1" || PartialPayment == "true") ? 1 : 0;
            Gsmu.Api.Data.Settings.Instance.SetMasterinfoValue(3, "AllowPartialPayment", output.ToString());

            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = new
            {
                success = true,
            };
            return result;
        }

    }


}