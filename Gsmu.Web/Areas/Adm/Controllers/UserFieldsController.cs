using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using student = Gsmu.Api.Data.School.Student;
using web = Gsmu.Api.Web;
using json = Newtonsoft.Json;
using models = Gsmu.Api.Data.ViewModels.UserFields;
using school = Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class UserFieldsController : Controller
    {
        //
        // GET: /Admin/UserFields/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DistrictAdditionalInfo(int? distid)
        {
            if (distid == null)
            {
                return PartialView(null);
            }
            using (var db = new school.Entities.SchoolEntities())
            {
                var model = (from i in db.DistrictExtraInfoes where i.districtID == distid select i).FirstOrDefault();
                return PartialView(model);
            }
        }

        public ActionResult SchoolAdditionalInfo(int? locationid)
        {
            if (locationid == null)
            {
                return PartialView(null);
            }
            using (var db = new school.Entities.SchoolEntities())
            {
                var model = (from i in db.SchoolExtraInfoes where i.schoolid == locationid select i).FirstOrDefault();
                return PartialView(model);
            }
        }

        public ActionResult GradeAdditionalInfo(int? gradeId)
        {
            if (gradeId == null)
            {
                return PartialView(null);
            }
            using (var db = new school.Entities.SchoolEntities())
            {
                var model = (from i in db.GradeExtraInfoes where i.gradeid == gradeId select i).FirstOrDefault();
                return PartialView(model);
            }
        }

        public ActionResult SaveSetting(string mifield, short vlu)
        {
            using (var db = new SchoolEntities())
            {

                var items = (from mi in db.MasterInfo2 select mi);

                foreach (var mi in items)
                {
                    switch (mifield)
                    {
                        case "DistrictEdit":
                            mi.DisallowDistrictEdit = vlu;
                            break;

                        case "SchoolEdit":
                            mi.DisallowSchoolEdit = vlu;
                            break;

                        case "GradeEdit":
                            mi.DisallowGradeEdit = vlu;
                            break;

                    }
                }
                db.SaveChanges();
            }

            var result = new JsonResult();
            result.Data = new
            {
                success = true
            };
            return result;

        }


    }
}