using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public class CourseColorGroupingHelper
    {
        public CourseColorGroupingsResult GetColorGroupingLimitRegistration(int? groupid)
        {
            CourseColorGroupingsResult CourseColorGroupingsResult = new CourseColorGroupingsResult();
            using (var db = new SchoolEntities())
            {
                var colorgroup = (from colors in db.CourseCategories where colors.CourseCategoryID == groupid select colors).FirstOrDefault();
                if (colorgroup != null)
                {
                    CourseColorGroupingsResult.IsLimitedtoOneRegistration = ((colorgroup.registrationlimit == 1) || (colorgroup.registrationlimit == -1));

                    var coursesingroup = (from courses in db.Courses where courses.CourseColorGrouping == groupid select courses.COURSEID).ToList();
                    CourseColorGroupingsResult.Courses = coursesingroup;
                    CourseColorGroupingsResult.GroupName = colorgroup.CourseCategoryName;
                }
                else
                {
                    return null;
                }
            }
            return CourseColorGroupingsResult;
        }

        public bool IsEnrolledCoursesinSameGroupings(int? groupid,int studentid)
        {
            if(studentid==0)
            {
                return false;
            }
            using (var db = new SchoolEntities())
            {
                var checkgroupings = (from roster in db.Course_Rosters join course in db.Courses on roster.COURSEID equals course.COURSEID where roster.STUDENTID == studentid && course.CourseColorGrouping == groupid && (from trans in db.Transcripts where trans.CourseId==course.COURSEID select trans.CourseId).Count()<=0 && roster.Cancel==0 select roster).FirstOrDefault();
                if(checkgroupings==null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public class CourseColorGroupingsResult{
        public bool IsLimitedtoOneRegistration{get;set;}
        public List<int> Courses {get;set;}
        public string GroupName { get; set; }
    }

}
