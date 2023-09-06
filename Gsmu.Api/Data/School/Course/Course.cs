using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Course;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class Course
    {
        private List<Course_Time> courseTimes = null;
        private CourseEnrollmentStatistics statistics = null;

        public static IEnumerable<Course_Time> FixCourseTimesForOnlineCourse(Course course, IEnumerable<Course_Time> times)
        {
            if ((course.IsOnlineCourse && times.Count() == 2) || (course.CoursesType==1))

            {
                var timeList = times.OrderBy(time => time.ID).ToList();
                var first = timeList[0];
                var second = new Course_Time();
                if (timeList.Count > 1)
                {
                    second = timeList[1];
                }
                else
                {
                    second = timeList[0];
                }
                if (first.COURSEDATE.Value > second.COURSEDATE.Value)
                {
                    return times.Reverse();
                }
                if (first.COURSEDATE.Value == second.COURSEDATE.Value)
                {
                    IEnumerable<Course_Time> sortedtime = times.OrderBy(time => time.ID);
                    times = sortedtime;
                    if (first.STARTTIME.Value > second.FINISHTIME.Value)
                    {
                        return times.Reverse();
                    }
                }
            }
            return times;
        }
        public static List<Course_Time> GetCourTimesList(int CourseId)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                return (from ct in db.Course_Times.AsNoTracking() where ct.COURSEID == CourseId orderby ct.COURSEDATE, ct.STARTTIME select ct).ToList();
            }
        }

        public static Course GetCourseDetails(int CourseId)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                return (from ct in db.Courses.AsNoTracking() where ct.COURSEID == CourseId select ct).FirstOrDefault();
            }
        }


        private List<Course_Roster> allRosters = null;

        public bool ShowPrerequisiteAsBoolean
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.ShowPrerequisite) && !string.IsNullOrWhiteSpace(this.PrerequisiteInfo);
            }
        }

        public bool MaterialsRequiredAsBoolean
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.MaterialsRequired);
            }
        }

        public bool AccessCodeRequired
        {
            get
            {
                return !string.IsNullOrEmpty(this.courseinternalaccesscode);
            }
        }

        public bool IsAccessCodeValid(string accesscode)
        {
            if (AccessCodeRequired)
            {
                return accesscode == this.courseinternalaccesscode;
            }
            return true;
        }

        public bool HideCreditInPublic
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.showcreditinpublic);
            }
        }

        public Gsmu.Api.Data.School.Course.CourseType CourseType
        {
            get
            {
                return this.coursetype.HasValue ? (Gsmu.Api.Data.School.Course.CourseType)this.coursetype.Value : Gsmu.Api.Data.School.Course.CourseType.Course;
            }
        }

        public bool IsOnlineCourse
        {
            get
            {
                return this.OnlineCourse.HasValue && Settings.GetVbScriptBoolValue(this.OnlineCourse.Value);
            }
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<Course_Time> CourseTimes
        {
            get
            {
                if (courseTimes == null)
                {
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;

                        var times = (from ct in db.Course_Times where ct.COURSEID == this.COURSEID orderby ct.COURSEDATE, ct.STARTTIME select ct).ToList();

                        courseTimes = Course.FixCourseTimesForOnlineCourse(this, times).ToList();
                    }
                }
                return courseTimes;
            }
            set
            {
                courseTimes = value;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.CANCELCOURSE);
            }
        }

        public CourseEnrollmentStatistics EnrollmentStatistics
        {
            get
            {
                if (this.statistics == null)
                {
                    this.statistics = new CourseEnrollmentStatistics(this);
                }
                return this.statistics;
            }
        }

        /// <summary>
        /// All rosters including cancelled, waiting, expired.
        /// </summary>
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<Course_Roster> AllRosters
        {
            get
            {
                if (allRosters == null)
                {
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;

                        allRosters = (from cr in db.Course_Rosters where cr.COURSEID == this.COURSEID select cr).ToList();
                    }
                }
                return allRosters;
            }
        }

        /// <summary>
        /// All rosters not cancelled or expired
        /// </summary>
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<Course_Roster> AvailableRosters
        {
            get
            {

                return CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.Available, AllRosters);
            }
        }

        /// <summary>
        /// Valid rosters not on waiting list.
        /// </summary>
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<Course_Roster> ReservedRosters
        {
            get
            {
                return CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.Reserved, AllRosters);
            }
        }

        /// <summary>
        /// Valid rosters that's paidinfull.
        /// </summary>
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<Course_Roster> PaidInFullRosters
        {
            get
            {
                return CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.PaidInFull, AllRosters);
            }
        }

        /// <summary>
        /// Valid rosters on waiting list.
        /// </summary>
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<Course_Roster> WaitingRosters
        {
            get
            {
                return CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.Waiting, AllRosters);
            }
        }

        public bool PrereqiusiteRequired
        {
            get
            {
                return this.ShowPrerequisite.HasValue && Settings.GetVbScriptBoolValue(this.ShowPrerequisite.Value);
            }
        }

        public bool CheckStudentPreRequsiteMet
        {
            get
            {
                if (Authorization.AuthorizationHelper.CurrentStudentUser != null)
                {
                    return Course.IsStudentValidForCourseByPreReq(this.COURSEID, Authorization.AuthorizationHelper.CurrentStudentUser.STUDENTID);
                }
                else
                {
                    return true;
                }
            }
        }

        private Entities.RoomDirection roomDirections = null;
        private bool roomDirectionsChecked = false;

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public Entities.RoomDirection RoomDirection
        {
            get
            { 
                if (this.roomDirectionsChecked) {
                    return this.roomDirections;
                }
                this.roomDirectionsChecked = true;

                if (this.RoomDirectionsId.HasValue && this.RoomDirectionsId.Value > 0) {
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;

                        this.roomDirections = (from d in db.RoomDirections where d.RoomDirectionsId == this.RoomDirectionsId select d).FirstOrDefault();                        
                    }
                }
                return this.roomDirections;
            }
        }

        public Course(bool setDefaults) : this()
        {
            if (setDefaults)
            {
                SetDefaults(this);
            }
        }

        public static void SetDefaults(Course gsmuCourse)
        {
            gsmuCourse.MAXENROLL = 1000;
            gsmuCourse.MAXWAIT = 1000;
            gsmuCourse.DAYS = 0;
            gsmuCourse.CourseCloseDays = 0;
            gsmuCourse.viewpastcoursesdays = 0;
            gsmuCourse.CoursesType = 0;
            gsmuCourse.disablehaikuintegration = 0;
            gsmuCourse.ShowCreditRequirementLink = 0;
            gsmuCourse.Percent4MultiEnrollDiscount = 0;
            gsmuCourse.MinMultiEnrolDiscountAll = 0;
            gsmuCourse.RoomDirectionsId = 0;
            gsmuCourse.CoursesType = 0;
            gsmuCourse.CollectExtraParticipant = 0;
            gsmuCourse.CourseColorGrouping = 0;
            gsmuCourse.CEUCredit = 0;
            gsmuCourse.GraduateCredit = 0;
            gsmuCourse.HalfDayCourse = 0;
            gsmuCourse.SubSiteId = 0;
            gsmuCourse.CREDITHOURS = 0;
            gsmuCourse.EmailReminderType = -1;
            gsmuCourse.MaterialsRequired = 0;
            //gsmuCourse.Icons='~|~~|~~|~';
            gsmuCourse.INSTRUCTORID2 = 0;
            gsmuCourse.INSTRUCTORID3 = 0;
            gsmuCourse.Membership = 0;
            gsmuCourse.BBCourseCloned = 0;
            gsmuCourse.CoursesHideFromCatalog = 0;
            gsmuCourse.Optionalcredithours1 = 0;
            gsmuCourse.Optionalcredithours2 = 0;
            gsmuCourse.Optionalcredithours3 = 0;
            gsmuCourse.Optionalcredithours4 = 0;
            gsmuCourse.Optionalcredithours5 = 0;
            gsmuCourse.Optionalcredithours6 = 0;
            gsmuCourse.Optionalcredithours7 = 0;
            gsmuCourse.Optionalcredithours8 = 0;
            gsmuCourse.Optionalcredithours1 = 0;
            gsmuCourse.ShowPrerequisite = 0;
            gsmuCourse.RemindInstructor = 0;
            gsmuCourse.CourseSpecificEnrollmentCheck = 0;
            gsmuCourse.SpecialCourseType = 0;
            gsmuCourse.mandatory = 0;
            gsmuCourse.freeclass = 0;
            gsmuCourse.NoRegEmail = 0;
            gsmuCourse.coursetype = 0;
            gsmuCourse.eventid = 0;
            gsmuCourse.sessionid = 0;
            gsmuCourse.BBServer = 0;
            gsmuCourse.bbautoenroll = 0;
            gsmuCourse.heliuslmscloned = 0;
            gsmuCourse.archived_course = 0;
            gsmuCourse.google_calendar_import_enable = 0;
            gsmuCourse.StudentChoiceCourse = 0;
            gsmuCourse.CourseCertificationsId = 0;
            gsmuCourse.SendConfirmationEmailtoInstructor = 0;
            gsmuCourse.MinMultiEnrolDiscountAll = 0;
            gsmuCourse.showcreditinpublic = 0;
            gsmuCourse.AllowSendSurvey = 0;

        }
        public static List<CoursePrerequisiteRegModel> CoursePreRequisites(int courseId)
        {
            using (var db = new SchoolEntities())
            {
                string query = "SELECT * FROM CoursePreReg WHERE CourseId = " + courseId + ";";
                var data = db.Database.SqlQuery<CoursePrerequisiteRegModel>(query).ToList();
                return data;
            }
        }

        public static List<CoursePrerequisiteRegModel> GetCoursePrerequisites(int courseId)
        {
            using (var db = new SchoolEntities())
            {
                string query = "SELECT DISTINCT cp.CourseId, cp.CoursePreReqId, c.COURSEID, c.COURSENAME as CourseName, c.COURSENUM as CourseNumber, 0 as IsStudentLoggedIn, 0 as Attended FROM CoursePreReg cp INNER JOIN Courses c ON c.courseid = cp.courseprereqid WHERE cp.CourseId = " + courseId + " ORDER BY CoursePreReqId;";
                var data = db.Database.SqlQuery<CoursePrerequisiteRegModel>(query).ToList();
                return data;
            }
        }

        public static List<CoursePrerequisiteRegModel> GetCoursePrerequisites(int courseId, int studentId)
        {
            using (var db = new SchoolEntities())
            {
                /*string query = @"SELECT DISTINCT cp.CoursePreReqId, c.COURSENAME as CourseName, c.COURSENUM as CourseNumber, 1 as IsStudentLoggedIn, ISNULL(t.studentid, 0) as StudentId,
                    (
                        CASE WHEN t.STUDENTID IS NULL THEN 0
                        ELSE
                        (
                            CASE WHEN t.ATTENDED <> 0 THEN 1 ELSE 0 END
                        )
                        END
                    ) as Attended
                FROM CoursePreReg cp
                INNER JOIN Courses c ON c.courseid = cp.courseprereqid
                LEFT JOIN Transcripts t ON t.CourseId = cp.CoursePreReqId AND t.studentid = " + studentId + " WHERE cp.CourseId = " + courseId + ";";
                */
                string query = @"SELECT DISTINCT cp.CoursePreReqId, c.COURSENAME as CourseName, c.COURSENUM as CourseNumber, 1 as IsStudentLoggedIn, ISNULL(t.studentid, 0) as StudentId,
                    (
                        CASE WHEN t.STUDENTID IS NULL THEN 0
                        ELSE
                        (
                            CASE WHEN t.ATTENDED <> 0 THEN 1 ELSE 0 END
                        )
                        END
                    ) as Attended
                FROM CoursePreReg cp
                INNER JOIN Courses c ON c.courseid = cp.courseprereqid
                LEFT JOIN Transcripts t ON t.coursenum = cp.CoursePreReqNumber AND t.studentid = " + studentId + " WHERE cp.CourseId = " + courseId + ";";

                var data = db.Database.SqlQuery<CoursePrerequisiteRegModel>(query).ToList();
                return data;
            }
        }

        public static bool SaveCoursePreRequisites(int courseId, string preReqIds)
        {
            try
            {
                using (var db = new SchoolEntities())
                {
                    foreach (var preReqId in preReqIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(preReqId))
                        {
                            string insertQuery = "INSERT INTO CoursePreReg (CourseId, CoursePreReqId) VALUES (" + courseId + ", " + preReqId + ");";
                            db.Database.ExecuteSqlCommand(insertQuery);
                            string updateQuery = "Update T1 set t1.CoursePreReqNumber=t2.coursenum from CoursePreReg t1 left join courses t2 on t1.CoursePreReqId=t2.courseid where t1.courseid=" + preReqId;
                            db.Database.ExecuteSqlCommand(updateQuery);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static bool DeleteCoursePreRequisites(int courseId, string preReqIds)
        {
            try
            {
                using (var db = new SchoolEntities())
                {
                    foreach (var preReqId in preReqIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(preReqId))
                        {
                            string deleteQuery = "Delete  CoursePreReg where CourseId=" + courseId + " and CoursePreReqId=" + preReqIds + "";
                            db.Database.ExecuteSqlCommand(deleteQuery);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool IsStudentValidForCourseByPreReq(int courseId, int studentId)
        {
            bool isValid = true;
            var coursePreRequisites = CoursePreRequisites(courseId);
            var preReqs = GetCoursePrerequisites(courseId, studentId);
            using (var db = new SchoolEntities())
            {
                foreach (var coursePreReq in coursePreRequisites)
                {
                    if(Gsmu.Api.Data.WebConfiguration.PreReqsPubView == "CourseNumOnly")
                    {
                        //SEARCH BY COURSE NUMBER
                        var _course = db.Courses.Where(t => t.COURSEID == coursePreReq.CoursePreReqId).FirstOrDefault();
                        var _transcripts = db.Transcripts.Where(t => t.CourseNum == _course.COURSENUM && t.STUDENTID == studentId).ToList();

                        //var _transcripts = (from transcript in db.Transcripts
                        //         join courses in db.Courses on transcript.CourseId equals courses.COURSEID
                        //         where courses.COURSENUM == _course.COURSENUM && transcript.STUDENTID == studentId select transcript).ToList();
                                 
                        if (_transcripts.Count() == 0)
                        {
                            return false; // if course and student id is not transcripted then consider invalid
                        }

                        bool Attended = false;
                        foreach (var transcript in _transcripts)
                        {
                            if (transcript.ATTENDED != 0)
                            {
                                Attended = true; // if finds even 1 course in with same course number that was attended, consider valid
                            }
                        }
                        if (Attended == false) { return false; } // if cant find addended consider invalid
                        }
                    else
                    {
                        //SEARCH BY COURSE ID
                        var transcripts = db.Transcripts.Where(t => t.CourseId == coursePreReq.CoursePreReqId && t.STUDENTID == studentId).ToList();
                        if (transcripts.Count() == 0)
                        {
                            return false; // if course and student id is not transcripted then consider invalid
                        }


                        foreach (var transcript in preReqs)
                        {
                            if (transcript.Attended == 0)
                            {
                                return false; // if the query finds even 1 course that was not attended, consider invalid
                            }
                        }
                    }

                }
            }
            return isValid;
        }

        public static List<CoursePrerequisiteRegModel> UnattendedCoursesFromPreReq(int courseId, int studentId)
        {
            List<CoursePrerequisiteRegModel> unattendedCourses = new List<CoursePrerequisiteRegModel>();
            var coursePreRequisites = CoursePreRequisites(courseId);
            var preReqs = GetCoursePrerequisites(courseId, studentId);

            using (var db = new SchoolEntities())
            {
                foreach (var coursePreReq in coursePreRequisites)
                {
                    var transcripts = db.Transcripts.Where(t => t.CourseId == coursePreReq.CoursePreReqId && t.STUDENTID == studentId).ToList();
                    if (transcripts.Count() == 0)
                    {
                        unattendedCourses.Add(new CoursePrerequisiteRegModel()
                        {
                            CourseId = coursePreReq.CourseId,
                            CourseName = coursePreReq.CourseName,
                            CourseNumber = coursePreReq.CourseNumber
                        });
                    }
                }

                foreach (var transcript in preReqs)
                {
                    if (transcript.Attended == 0)
                    {
                        unattendedCourses.Add(new CoursePrerequisiteRegModel()
                        {
                            CourseId = transcript.CourseId,
                            CourseName = transcript.CourseName,
                            CourseNumber = transcript.CourseNumber
                        });
                    }
                }
            }

            return unattendedCourses;
        }
    }
}
