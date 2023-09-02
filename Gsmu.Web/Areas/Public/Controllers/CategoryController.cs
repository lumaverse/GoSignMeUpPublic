using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class CategoryController : Controller
    {

        /// <summary>
        /// Returns the category tree for public usage.
        /// </summary>
        public ActionResult LeftCategories(int subsiteid = 0, bool courseInternal = false, CourseCancelState cancelState = CourseCancelState.NotCancelled, bool ShowPastCourses = false, bool ShowMembershipCourses=false)
        {
            var model = Gsmu.Api.Data.School.Category.Queries.CategoryTree(subsiteid, courseInternal, cancelState, ShowPastCourses, ShowMembershipCourses);
            return PartialView(model);
        }

        public ActionResult LeftLegends()
        {
            CourseIconsandLegend CourseIconsandLegend = new CourseIconsandLegend();
            var colorcode = CourseIconsandLegend.GetGroupingsColor();
            var icons = CourseIconsandLegend.GetIcons();
            ViewBag.ColorCode = colorcode;
            ViewBag.Icons = icons;
            ViewBag.EnabledColor = Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo2().EnableCourseColors);
            return PartialView();
        }

    }
}
