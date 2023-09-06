using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;

namespace Gsmu.Api.Integration.Canvas.Clients
{
    public class EnrollmentClient
    {
        public static Response ListUserEnrollments(long userId, NameValueCollection query = null, bool allowedEnrollmentStatesOnly = true, bool allowedEnrollmentTypesOnly = true)
        {
            query = ConfigureEnrollmentQuery(query, allowedEnrollmentStatesOnly, allowedEnrollmentTypesOnly);
            var url = string.Format("/api/v1/users/{0}/enrollments", userId);
            var response = CanvasClient.GetResponse("ListUserEnrollments", http.HttpMethod.Get, url, query);
            return response;
        }

        public static Response ListCourseEnrollments(int courseId, NameValueCollection query = null, bool allowedEnrollmentStatesOnly = true, bool allowedEnrollmentTypesOnly = true)
        {
            query = ConfigureEnrollmentQuery(query, allowedEnrollmentStatesOnly, allowedEnrollmentTypesOnly);
            var url = string.Format("/api/v1/courses/{0}/enrollments", courseId);
            var response = CanvasClient.GetResponse("ListCourseEnrollments2", http.HttpMethod.Get, url, query);
            return response;
        }

        public static Response ListSectionEnrollments(int courseId, int sectionid, NameValueCollection query = null, bool allowedEnrollmentStatesOnly = true, bool allowedEnrollmentTypesOnly = true)
        {
            query = ConfigureEnrollmentQuery(query, allowedEnrollmentStatesOnly, allowedEnrollmentTypesOnly);
            var url = string.Format("/api/v1/sections/{0}/enrollments", sectionid);
            var response = CanvasClient.GetResponse("ListCourseEnrollments3", http.HttpMethod.Get, url, query);
            return response;
        }
        
        private static NameValueCollection ConfigureEnrollmentQuery(NameValueCollection query, bool allowedEnrollmentStatesOnly, bool allowedEnrollmentTypesOnly)
        {
            if (!allowedEnrollmentStatesOnly && !allowedEnrollmentTypesOnly)
            {
                return query;
            }
            query = query ?? new NameValueCollection();
            var config = Configuration.Instance;

            if (allowedEnrollmentStatesOnly)
            {
                query.Remove("state[]");
                config.IntegratedEnrollmentStates.ForEach(delegate(Entities.EnrollmentState state)
                {
                    query.Add("state[]", state.ToString());
                });
            }

            if (allowedEnrollmentTypesOnly)
            {
                query.Remove("type[]");
                config.IntegratedEnrollmentTypes.ForEach(delegate(Entities.EnrollmenType type)
                {
                    query.Add("type[]", type.ToString());
                });
            }

            return query;
        }

        public static Response DeleteEnrollment(int courseId, long enrollmentId)
        {
            var url = string.Format("/api/v1/courses/{0}/enrollments/{1}", courseId, enrollmentId);
            var query = new NameValueCollection() {
                { "task", "delete"}
            };

            var response = CanvasClient.GetResponse("DeleteEnrollment_" + enrollmentId.ToString(), http.HttpMethod.Delete, url, query);
            return response;
        }

        public static Response InsertEnrollment(Entities.Enrollment enrollment)
        {
            var url = string.Format("/api/v1/courses/{0}/enrollments", enrollment.CourseId.ToString());
            var query = new NameValueCollection();
            if (enrollment.CourseSectionId != 0)
            {
                query.Add("enrollment[user_id]", enrollment.UserId.ToString());
                query.Add("enrollment[type]", enrollment.Type.ToString());
                query.Add("enrollment[course_section_id]", enrollment.CourseSectionId.ToString());
                query.Add("enrollment[enrollment_state]", enrollment.EnrollmentState.ToString());
            }
            else
            {
                query.Add("enrollment[user_id]", enrollment.UserId.ToString());
                query.Add("enrollment[type]", enrollment.Type.ToString());
                query.Add("enrollment[enrollment_state]", enrollment.EnrollmentState.ToString());
            }
            var response = CanvasClient.GetResponse("InsertEnrollment_" + enrollment.CourseId.ToString() + "_" + enrollment.UserId.ToString(), http.HttpMethod.Post, url, query);
            return response;
        }
    }
}
