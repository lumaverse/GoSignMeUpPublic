using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Integration.Blackboard;
using Gsmu.Api.Integration.Blackboard.Connector;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class BlackboardController : Controller
    {
        // GET: Adm/Blackboard
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveSettings(string blackboard_students_dsk = null, string blackboard_instructors_dsk = null, string blackboard_courses_dsk = null, string blackboard_course_roster_dsk = null, string blackboard_students_node_id = null, string blackboard_courses_node_id = null, string blackboard_instructors_node_id = null)
        {
            bool success = true;
            string message = string.Empty;

            var config = Configuration.Instance;
            config.StudentDsk = blackboard_students_dsk;
            config.InstructorsDsk = blackboard_instructors_dsk;
            config.CoursesDsk = blackboard_courses_dsk;
            config.CourseRosterDsk = blackboard_course_roster_dsk;

            config.CourseInstitutionalHierarchyNodeId = blackboard_courses_node_id;
            config.StudentInstitutionalHierarchyNodeId = blackboard_students_node_id;
            config.InstructorInstitutionalHierarchyNodeId = blackboard_instructors_node_id;
            
            config.Save();

            message = "Settings saved successfully.";
            var result = new
            {
                config = config,
                success = success,
                message = message
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        public ActionResult InstitutionalHierarchy()
        {
            var nodes = "{ \"expanded\": true, \"children\": [" + NodeConnector.NodeListJson + "]}";
            return Content(nodes, "application/json");
        }

        public ActionResult DataSourceStore()
        {
            var dsks = Gsmu.Api.Integration.Blackboard.Connector.DataSourceKeyConnector.DataSourceKeys;

            object[] json = new object[dsks.Length ];
            for (var index = 0; index < dsks.Length; index++)
            {
                json[index] = new
                {
                    display = dsks[index],
                    value = dsks[index]
                };
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

    }
}