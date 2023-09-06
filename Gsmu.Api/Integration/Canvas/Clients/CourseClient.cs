using Gsmu.Api.Authorization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;

namespace Gsmu.Api.Integration.Canvas.Clients
{
    public class CourseClient
    {
        public static Response ListCourses(NameValueCollection query = null, bool allowedCoursesOnly = true)
        {

            var config = Configuration.Instance;
            if (allowedCoursesOnly)
            {
                query = query ?? new NameValueCollection();
                //query.Remove("type[]");
                query.Remove("state[]");
                foreach (var state in Configuration.Instance.IntegratedCourseWorkflowStates)
                {
                    //query["type[]"] = state.ToString();
                   // query["state[]"] = state.ToString();
                  // query["published"]="true";
                };
                query.Add("per_page", "100");
                
                //query.Add("gsmu-max-pages", "2");
            }


            var url = string.Format("/api/v1/accounts/{0}/courses", config.CanvasAccountId.Value);            
            var response = CanvasClient.GetResponse("ListCourses", http.HttpMethod.Get, url, query);
            return response;
        }
        public static Response GetCourseSectionsList(int courseId, NameValueCollection query = null)
        {
            var url = string.Format("/api/v1/courses/{0}/sections", courseId);
            var response = CanvasClient.GetResponse("GetCourseSectionsList", http.HttpMethod.Get, url, query);
            return response;
        }
        public static Response ListCourseUsers(int courseId, NameValueCollection query = null)
        {
            var url = string.Format("/api/v1/courses/{0}/users", courseId);
            var response = CanvasClient.GetResponse("ListCourseUsers", http.HttpMethod.Get, url, query);
            return response;
        }

        public static Response GetCourse(int courseId, NameValueCollection query = null)
        {
            var config = Configuration.Instance;
            //var url = string.Format("/api/v1/accounts/{0}/courses/{1}", config.CanvasAccountId.Value, courseId);
            var url = string.Format("/api/v1/courses/{0}", courseId);
            var response = CanvasClient.GetResponse("GetCourse", http.HttpMethod.Get, url, query);
            return response;
        }

        public static Response InsertCourse(Entities.Course course, string paramAccountID)
        {
            var query = GetCourseQuery(course);
            //var url = string.Format("/api/v1/accounts/{0}/courses", course.AccountId.ToString());
            var url = string.Format("/api/v1/accounts/{0}/courses", paramAccountID.ToString());
            var response = CanvasClient.GetResponse("InsertCourse_" + course.SisCourseId + "_0", http.HttpMethod.Post, url, query);
            return response;
        }

        public static Response UpdateCourse(Entities.Course course)
        {
            var query = GetCourseQuery(course);
            var url = string.Format("/api/v1/courses/{0}", course.Id);
            var response = CanvasClient.GetResponse("UpdateCourse_" + course.Id.ToString(), http.HttpMethod.Put, url, query);
            return response;
        }

        public static NameValueCollection GetCourseQuery(Entities.Course course) {
            NameValueCollection query = new NameValueCollection();
            query.Add("account_id", course.AccountId.ToString());
            query.Add("course[name]", course.Name);
            query.Add("course[public_description]", course.PublicDescription);
            query.Add("course[course_code]", course.CourseCode);
            query.Add("course[sis_course_id]", course.SisCourseId);
            //convert to new format due to API change
            if (!String.IsNullOrEmpty(course.StartAt.Value.ToString()))
            {
                query.Add("course[start_at]", course.StartAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            else
            {
                query.Add("course[start_at]", course.StartAt.Value.ToString());
            }
            if (!String.IsNullOrEmpty(course.EndAt.Value.ToString()))
            {
                query.Add("course[end_at]", course.EndAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            else
            {
                query.Add("course[end_at]", course.StartAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            query.Add("course[is_public]", course.IsPublic.Value.ToString().ToLower());
            query.Add("course[workflow_state]", course.WorkflowState.ToString());
            return query;
        }

        public static Response ListCourseEnrollments(int courseId, NameValueCollection query)
        {
            var url = string.Format("/api/v1/courses/:{0}/enrollments",courseId);
            var response = CanvasClient.GetResponse("ListCourseEnrollments_" + courseId.ToString(), http.HttpMethod.Get, url, query);
            return response;
        }

        public static Response PublishCourse(int canvasCourseId)
        {
            var url = string.Format(" /api/v1/accounts/{0}/courses", Configuration.Instance.CanvasAccountId);
            var response = CanvasClient.GetResponse("PublishCourse_" + canvasCourseId.ToString(), http.HttpMethod.Put, url, new NameValueCollection()
            {
                {"event", "offer"},
                {"course_ids[]", canvasCourseId.ToString()}
            });
            return response;
        }
        public static Response InsertCourseSection(Entities.CourseSection coursesection, string paramCanvasCourseID)
        {
            var query = GetSectionQuery(coursesection);
            var url = string.Format("/api/v1/courses/{0}/sections", paramCanvasCourseID.ToString());
            var response = CanvasClient.GetResponse("InsertCourseSection", http.HttpMethod.Post, url, query);
            return response;
        }
        public static Response DeleteCourseSection(string paramCanvasCourseSectionID)
        {
            var query = new NameValueCollection() {
                { "task", "delete"}
            };
            var url = string.Format("/api/v1/sections/{0}", paramCanvasCourseSectionID.ToString());
            var response = CanvasClient.GetResponse("DeleteCourseSection", http.HttpMethod.Delete, url, query);
            return response;
        }
        public static NameValueCollection GetSectionQuery(Entities.CourseSection coursesection)
        {
            NameValueCollection query = new NameValueCollection();
            //query.Add("account_id", coursesection.AccountId.ToString());
            query.Add("course_section[name]", coursesection.Name);
            query.Add("course_section[sis_section_id]", coursesection.sis_section_id);
            //convert to new format due to API change
            if (!String.IsNullOrEmpty(coursesection.StartAt.Value.ToString()))
            {
                query.Add("course_section[start_at]", coursesection.StartAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            else
            {
                query.Add("course_section[start_at]", coursesection.StartAt.Value.ToString());
            }
            if (!String.IsNullOrEmpty(coursesection.EndAt.Value.ToString()))
            {
                query.Add("course_section[end_at]", coursesection.EndAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            else
            {
                query.Add("course_section[end_at]", coursesection.StartAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            query.Add("course_section[restrict_enrollments_to_section_dates]", coursesection.restrict_enrollments_to_section_dates.Value.ToString().ToLower());
            query.Add("enable_sis_reactivation", "false");
            return query;
        }
    }
}
