using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Data;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class SharerController : Controller
    {
        //
        // GET: /Public/Sharer/
        public ActionResult Index()
        {
            int courseId = int.Parse(Request["cid"]);
            var model = new CourseSharerModel();
            using (SchoolEntities db = new SchoolEntities())
            {
                var course = db.Courses.Where(c => c.COURSEID == courseId).SingleOrDefault();
                model.CourseId = course.COURSEID;
                model.CourseTitle = course.COURSENUM + " - " + course.COURSENAME;
                if (course.DESCRIPTION.Length < 50)
                {
                    model.Description = course.DESCRIPTION;
                }
                else
                {
                    model.Description = course.DESCRIPTION.Substring(0, 75);
                }
                model.TileImage = !string.IsNullOrEmpty(course.TileImageUrl) ? course.TileImageUrl : "";
                model.CourseLink = Request.Url.AbsoluteUri;
            };
            return View(model);
        }

        [HttpGet]
        public JsonResult CourseShareDataByCourseId(int courseId) {
            var model = new CourseSharerModel();
            using (SchoolEntities db = new SchoolEntities())
            {
                var course = db.Courses.Where(c => c.COURSEID == courseId).SingleOrDefault();
                model.CourseId = course.COURSEID;
                model.CourseTitle = course.COURSENUM + " - " + course.COURSENAME;
                model.Description = course.DESCRIPTION;
                model.TileImage = !string.IsNullOrEmpty(course.TileImageUrl) ? course.TileImageUrl : "";
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public class CourseSharerModel {
            public int CourseId { get; set; }
            public string CourseLink { get; set; }
            public string CourseTitle { get; set; }
            public string Description { get; set; }
            public string TileImage { get; set; }
        }
	}
}