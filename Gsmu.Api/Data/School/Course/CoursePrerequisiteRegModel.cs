using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public class CoursePrerequisiteRegModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int CoursePreReqId { get; set; }

        public string CourseName { get; set; }
        public string CourseNumber { get; set; }

        public int IsStudentLoggedIn { get; set; }
        public int StudentId { get; set; }
        public int Attended { get; set; }
    }

    public class CoursePrequisiteModel : CoursePrerequisiteRegModel
    {
        public CourseModel MainCourse { get; set; }
        public List<CourseModel> PreRequisiteCourse { get; set; }
    }

}
