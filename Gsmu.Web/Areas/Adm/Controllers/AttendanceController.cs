using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web = Gsmu.Api.Web;
using attendance = Gsmu.Api.Data.School.Attendance;
using json = Newtonsoft.Json;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class AttendanceController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Attendance");
        }

        public ActionResult PortalAttendance(int courseId)
        {
            var model = new attendance.AttendanceModel(courseId);
            return View("Attendance", model);
        }

        public ActionResult Attendance(int? courseId = null)
        {
            attendance.AttendanceModel model = null;
            if (courseId.HasValue)
            {
                model = new attendance.AttendanceModel(courseId.Value);
            }
            return View(model);
        }

        public ActionResult AttendanceInfo(int courseId)
        {
            var model = new attendance.AttendanceModel(courseId);
            return this.JsonEntityWithoutRelationships(model);
        }
	}
}