using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Data;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.CourseRoster;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.Events.Entities;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class CalendarController : Controller 
    {
        //
        // GET: /Public/Calendar/
        [CustomSecurityforCrossSiteForgeryAttributes]
        public ActionResult Index()
        {

                if (Settings.Instance.GetMasterInfo3().calendarredirectoutsidelink == 99)
                {
                    return Content("Module is not available.");
                }
                ViewBag.GoogleAnalyticsSetUpScript = Settings.Instance.GetMasterInfo3().GoogleTracker_code;
                string maincategory = Request["main"];
                ViewBag.MainCategory = GetAllMainCategory();
                ViewBag.SubCategory = GetAllSubCategory(maincategory);
                ViewBag.MaincategoryCounter = ViewBag.MainCategory.Count;
                ViewBag.SubcategoryCounter = ViewBag.SubCategory.Count;
                return View();

        }

        public ActionResult CalendarIndex()
        {
            ViewBag.MainCategory = GetAllMainCategory();
            return View();
        }


        public ActionResult GetCalendarByCategory(string maincategory)
        {

            return View();
        }

        public List<string> GetAllMainCategory()
        {
            List<string> mylist = Gsmu.Api.Data.School.CourseRoster.Queries.GetAllMainCategories();
            return mylist;
 
        }


        public List<string> GetAllSubCategory(string maincategory)
        {
            List<string> mylist = Gsmu.Api.Data.School.CourseRoster.Queries.GetAllSubCategories(maincategory);
            return mylist;

        }
        public ActionResult GetCalendarCourses(string maincategory,string subcategory)
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                try
                {
                    if ((maincategory == "") || (maincategory == null) || (maincategory == "null"))
                    {
                        result.Data = Gsmu.Api.Data.School.CourseRoster.Queries.GetCalendarEntries();
                    }
                    else
                    {
                        if ((subcategory != "") && (subcategory != null) && (subcategory != "null"))
                        {
                            result.Data = Gsmu.Api.Data.School.CourseRoster.Queries.GetCalendarEntries(maincategory,subcategory);
                        }
                        else
                        {
                            result.Data = Gsmu.Api.Data.School.CourseRoster.Queries.GetCalendarEntries(maincategory,"");
                        }
                    }


                }
                catch (Exception y)
                {
                    result.Data = new
                    {
                        error = y.Message,
                        success = false
                    };
                }

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        public ActionResult CourseCalendarDetails(int intCourseID = 0)
        {
            var result = new JsonResult();
            CourseModel course = new CourseModel(intCourseID);
            result.Data = new
            {
                title = course.Course.COURSENAME,
                description = course.Course.DESCRIPTION,
                success = true
            };
            return result;
        }


        public ActionResult CourseEventDetails(int intEventId = 0)
        {
            var result = new JsonResult();
            @event eventdetail = Gsmu.Api.Data.Events.Queries.GetEventDetails(intEventId);
            if (eventdetail != null)
            {
                result.Data = new
                {
                    title = eventdetail.title,
                    description = eventdetail.description,
                    startdatevalue = eventdetail.DateStart,
                    enddatevalue = eventdetail.DateEnd,
                    timevalue = eventdetail.Eventtime,
                    locationvalue = eventdetail.Location,
                    speakervalue = eventdetail.featuredspeaker,
                    contactvalue = eventdetail.ContactInfo,
                    emailvalue = eventdetail.ContactEmail,
                    feevalue = eventdetail.Fees,
                    websitevalue = eventdetail.WebsiteLink,
                    success = true
                };
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

    }
}
