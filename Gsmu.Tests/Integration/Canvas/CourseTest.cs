using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gsmu.Api.Integration.Canvas;
using Gsmu.Api.Integration.Canvas.Clients;
using Gsmu.Api.Integration.Canvas.Entities;

using Newtonsoft.Json;

namespace Gsmu.Tests.Integration.Canvas
{
    [TestClass]
    public class CourseTest
    {

        public CourseTest()
        {
            var httpContext = HttpHelper.FakeHttpContext("http://localhost/");
            System.Web.HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void ListCourses()
        {

            NameValueCollection query = new NameValueCollection() {
                {"with_enrollments", "true"}
            };
            var response = CourseClient.ListCourses(query);
            var courses = response.Courses;

//            foreach (var course in courses)
//            {
            var enrollmentResponse = EnrollmentClient.ListCourseEnrollments(courses[0].Id, new NameValueCollection()
            {
                {"include[]", "enrollments"}
            });
                var enrollments = enrollmentResponse.Enrollments;
//            }

            var result = JsonConvert.SerializeObject(courses, Formatting.Indented);

            Console.WriteLine(result);
        }

        [TestMethod]
        public void CourseImportTest()
        {
            var result = CanvasImport.SynchronizeCourse(38331);
            Assert.IsNotNull(result.GsmuCourse.canvas_course_id);
            var result2 = CanvasImport.SynchronizeCourse(result.GsmuCourse.canvas_course_id.Value);
            Assert.AreEqual(result.GsmuCourse.canvas_course_id, result2.GsmuCourse.canvas_course_id);
            Assert.AreEqual(result.GsmuCourse.COURSEID, result2.GsmuCourse.COURSEID);
        }

        [TestMethod]
        public void CourseAndEnrollmentImportTest()
        {
            var result = CanvasImport.SyncronizeCourseAndEnrollment(54033);
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
