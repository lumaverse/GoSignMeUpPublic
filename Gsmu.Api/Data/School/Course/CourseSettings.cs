using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.Course
{
    public class CourseSettings
    {
        public string OnlineClasslabel { get; set; }

        public string CourseCloseDate { get; set; }

        public string CourseDateTimeHeader1st { get; set; }

        public string CourseDateTimeHeader2nd { get; set; }

        public bool ShowCourseDateTime { get; set; }

        public int MAXENROLL { get; set; }

        public int ENROLLED { get; set; }

        public int ENROLLEXTRA { get; set; }

        public int MAXWAIT { get; set; }

        public int WAITING { get; set; }

        public int ENROLLEDAVAIL { get; set; }

        public int ENROLLEDEXTRA { get; set; }

        public string CLASSSTATUS { get; set; }

        public bool HideTextSeatsAvailable { get; set; }

        public bool EnrollmentClosed { get; set; }

        public string TextSeatsAvailable { get; set; }

        public bool ShowInstructor { get; set; }

        public string StartDate { get; set; }

        public bool ShowOnlineClassDate { get; set; }

        public bool IsOnlineCourse { get; set; }


        public string TimezoneAbv { get; set; }

        public int TimezoneAddHour { get; set; }
    }
}
