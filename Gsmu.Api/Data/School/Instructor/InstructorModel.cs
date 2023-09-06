using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.InstructorAccountModel
{

    public class CourseInstructorModel
    {
        public int CourseId
        {
            get;
            set;
        }

        public DateTime? Coursedate
        {
            get;
            set;
        }
        public string CourseName
        {
            get;
            set;
        }
        public string CourseNum
        {
            get;
            set;
        }
        public string Enrolled
        {
            get;
            set;
        }
        public int MaxEnrolled
        {
            get;
            set;
        }
        public int Waiting
        {
            get;
            set;
        }
        public int MaxWaiting
        {
            get;
            set;
        }

        public string CDates
        {
            get;
            set;
        }
        public string Room
        {
            get;
            set;
        }

        public int attendancecount
        {
            get;
            set;
        }
        public int transcriptedcount
        {
            get;
            set;
        }
        public int Cancelled
        {
            get;
            set;
        }
        public int NoWaiting
        {
            get;
            set;
        }
        public int Unpaid
        {
            get;
            set;
        }
        public int IsOnline
        {
            get;
            set;
        }
        public string StartEndTimeDisplay
        {
            get;
            set;
        }
        public string CourseDatesandTime
        {
            get;
            set;
        }
        public string Location
        {
            get;
            set;
        }

        public DateTime? MaxDate { get; set; }

        public DateTime? MinDate { get; set; }

        public short CancelCourse { get; set; }

        public DateTime? CancelCourseDate { get; set; }

        public string CancelCourseDateTime { get; set; }

        public int EnrolledInt { get; set; }
    }
}
