using Gsmu.Api;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.CourseRoster;
using Gsmu.Api.Data.ViewModels.Layout;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class ConfirmationScreenController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("confirmation");
        }


        public ActionResult Confirmation(string order, string print)
        {

            if (print != null)
            {
                return View("Confirmation", "_PrintLayout");
            }
            return View("Confirmation");
        }

        public ActionResult ConfirmationPartial(string order, string print)
        {
            if (order == null || order == "")
            {
                return null;
            }
            var orderModel = new OrderModel(order);
            return PartialView("_PartialConfirmation", orderModel);
        }

        public ActionResult ConfirmationQuery(string query)
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var data = Gsmu.Api.Data.School.CourseRoster.Queries.GetLayoutOrders(query);
            result.Data = new
            {
                Result = data,
                TotalCount = data.Count()
            };
            return result;
        }

        [HttpGet]
        public ActionResult ConfirmationScreenData()
        {
            JsonResult result = new JsonResult();

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = new
            {
                Header = OrderModel.Header,
                Footer = OrderModel.Footer,
                HeaderWhenOnWaitingList = OrderModel.HeaderWhenOnWaitingList
            };
            return result;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConfirmationScreenData(string header, string footer, string headerWhenOnWaitingList)
        {
            OrderModel.UpdateConfirmationHeaders(header, headerWhenOnWaitingList, footer);
            return new ContentResult();
        }

        public ActionResult UploadConfirmationContentFile(ConfirmationArea area, HttpPostedFileBase file)
        {
            var result = new JsonResult()
            {
            };
            string fileName = Gsmu.Api.Data.School.CourseRoster.OrderModel.SaveUploadContentFile(area, file);
            result.Data = new
            {
                success = true,
                file = fileName,
                mime = file.ContentType
            };

            return result;
        }
	}
}