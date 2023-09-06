using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public class CourseEnrollmentStatistics
    {
        public static List<Entities.Course_Roster> FilterRosterListByStatus(CourseEnrollmentStatisticsType type, List<Entities.Course_Roster> rosters)
        {
            switch (type)
            {
                case CourseEnrollmentStatisticsType.Available:
                    return (from cr in rosters where cr.IsValidForClass select cr).ToList();

                case CourseEnrollmentStatisticsType.Reserved:
                    return (from cr in rosters where cr.IsValidForClass && !cr.IsWaiting select cr).ToList();


                case CourseEnrollmentStatisticsType.Waiting:
                    return (from cr in rosters where cr.IsValidForClass && cr.IsWaiting select cr).ToList();

                case CourseEnrollmentStatisticsType.PaidInFull: // for some reason IsPaidInfull doesnt work when it's 1
                    return (from cr in rosters where cr.IsValidForClass && !cr.IsWaiting && cr.PaidInFull != 0 select cr).ToList();
                
                default:
                    throw new NotImplementedException();
            }
        }
        public static int GetTotalEnrolledinRoster(int cid)
        {
            using (var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
            {
                return (from roster in db.Course_Rosters where roster.COURSEID == cid && roster.Cancel == 0 select roster).ToList().Count();
            }
        }

        public static int GetTotalEnrolledinRosterNoWaiting(int cid)
        {
            using (var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
            {
                return (from roster in db.Course_Rosters where roster.COURSEID == cid && roster.Cancel == 0 && roster.WAITING==0 select roster).ToList().Count();
            }
        }
        public static int GetAutoReserveCourseCount(int cid)
        {
            try
            {

                using (var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
                {
                    return (from autoreservecourse in db.AutoReserveCourses where autoreservecourse.CourseId == cid && autoreservecourse.ExpiredTime > DateTime.Now && autoreservecourse.ReservationId != Commerce.ShoppingCart.CourseShoppingCart.Instance.ReservationId select autoreservecourse).ToList().Count;

                }
            }
            catch
            {
                return 0;
            }
        }
        private Entities.Course course = null;

        public CourseEnrollmentStatistics(Entities.Course course) {
            this.course = course;

            MaxEnrolledRosterCount = course.MAXENROLL.HasValue ? course.MAXENROLL.Value : 0;
            MaxWaitingRosterCount = course.MAXWAIT.HasValue ? course.MAXWAIT.Value : 0;
            var extraParticipants = new List<Entities.Course_Roster>();
            if (course.CollectExtraParticipant <= 0)
            {
                TotalRosterCount = 0;
            }
            else { 
                var rosters = this.course.AllRosters;
                 extraParticipants = Gsmu.Api.Data.School.Entities.Course_Roster.GetRosterExtraParticipantsAsRosterList(rosters);
                TotalRosterCount = rosters.Count + extraParticipants.Count;

            }

            AvailableRosterCount = this.course.AvailableRosters.Count + CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.Available, extraParticipants).Count;

            EnrolledRosterCount = GetAutoReserveCourseCount(course.COURSEID) + course.ReservedRosters.Count + CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.Reserved, extraParticipants).Count;

            WaitingRosterCount = course.WaitingRosters.Count + CourseEnrollmentStatistics.FilterRosterListByStatus(CourseEnrollmentStatisticsType.Waiting, extraParticipants).Count;

            if (course.CourseType == CourseType.Event)
            {
                if (MaxEnrolledRosterCount == 0) //if the event has 0 value for enrollment limit, it is set to No Limit. It will base on individual course statistic.
                {
                    MaxEnrolledRosterCount = EnrolledRosterCount + 10000;
                }
                using(var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
                {
                    TotalRosterCount = (from cr in db.Course_Rosters
                                          where cr.eventid == course.COURSEID
                                          select cr.OrderNumber).Distinct().Count();                    
                }                

            }

            EnrollmentStatus = CalculateEnrollmentStatus();
        }

        public CourseEnrollmentStatus EnrollmentStatus
        {
            get; private set;
        }

        private CourseEnrollmentStatus CalculateEnrollmentStatus()
        {
            if (course.CourseTimes.Count == 0)
            {
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentSubAdminUser != null)
                {
                 //   return CourseEnrollmentStatus.SpaceAvailable;
                }
                else
                {
                return CourseEnrollmentStatus.Expired;
                }
            }
            int courseCloseDays = 0;
            if (this.course.CourseCloseDays == null || this.course.CourseCloseDays == 0)
            {
                if (Settings.Instance.GetMasterInfo().ShowPastOnlineCoursesAsBoolean && this.course.IsOnlineCourse)
                {
                    courseCloseDays = 0;
                }
                else
                {
                    courseCloseDays = ((int?)Settings.Instance.GetMasterInfo().CourseCloseDays) ?? 0;
                }
            }
            else
            {
                courseCloseDays = this.course.CourseCloseDays.Value;
            }
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            int viewPastCourseDays = 0;
           // if (Settings.Instance.GetMasterInfo3().AllowViewPastCourseDaysAsBoolean)
           // {
                viewPastCourseDays = course.viewpastcoursesdays ?? 0;
           // }
            var courseTime = course.CourseTimes.First();
            DateTime? startDate = courseTime == null ? null : courseTime.COURSEDATE;
            courseTime = course.CourseTimes.Last();
            DateTime? endDate = courseTime == null ? null : courseTime.COURSEDATE;
            if ((
               (
                    (viewPastCourseDays < 1 && courseCloseDays > 0 && startDate.HasValue && startDate.Value.AddDays(-courseCloseDays) < now)
                   ||
                    (startDate.HasValue && startDate.Value.AddDays(viewPastCourseDays) <= now) 
                )
                &&
                !(
                   viewPastCourseDays<1  && this.course.IsOnlineCourse && startDate.HasValue && startDate <= now && endDate >= now
               )
            )||
            ((startDate.HasValue && startDate.Value.AddDays(-courseCloseDays) <= now) && (courseCloseDays>0))
            )
            {
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentSubAdminUser != null)
                {
                   // return CourseEnrollmentStatus.SpaceAvailable;
                }
                else
                {
                    return CourseEnrollmentStatus.Expired;
                }
            }

            // IF WAITLIST SPACE IS FULL AND ENROLL TO WAITLIST IS TRUE
            //RETURN FULL IMMEDIATELY
            if (WebConfiguration.EnrollToWaitList)
            {
                if (this.WaitSpaceAvailable <= 0)
                {
                    return CourseEnrollmentStatus.Full;
                }
            }
            
            if (this.EnrolledRosterCount >= this.MaxEnrolledRosterCount)
            {
                if (this.WaitingRosterCount >= this.MaxWaitingRosterCount)
                {
                    return CourseEnrollmentStatus.Full;
                }
                else
                {
                    return CourseEnrollmentStatus.WaitSpaceAvailable;
                }
            }
            else
            {
                if ((this.EnrolledRosterCount + this.WaitingRosterCount) >= (this.MaxEnrolledRosterCount + this.MaxWaitingRosterCount))
                {
                    return CourseEnrollmentStatus.Full;
                }
                else
                {
                    return CourseEnrollmentStatus.SpaceAvailable;
                }
            }
        }

        /// <summary>
        /// Number of total rosters in the course including cancelled and expired ones.
        /// </summary>
        public int TotalRosterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of valid rosters - excluding cancelled and expired ones.
        /// </summary>
        public int AvailableRosterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Maximum number of rosters that can be reserved for the class.
        /// </summary>
        public int MaxEnrolledRosterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Actual reserved rosters for the class/course (not expired/not cancelled).
        /// </summary>
        public int EnrolledRosterCount {
            get;
            private set;
        }

        /// <summary>
        /// Maximum number of waiting rosters for the classs.
        /// </summary>
        public int MaxWaitingRosterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Actual number of rosters in waiting status in the class (not expired, not cancelled)
        /// </summary>
        public int WaitingRosterCount {
            get;
            private set;
        }

        public int SpaceAvailable {
            get
            {
                return MaxEnrolledRosterCount - EnrolledRosterCount;
            }
        }

        public int WaitSpaceAvailable
        {
            get
            {
                return MaxWaitingRosterCount - WaitingRosterCount;
            }
        }

        public string Status
        {
            get
            {
                return Gsmu.Api.Language.EnumHelper.GetEnumDescription(this.EnrollmentStatus);
            }
        }
    }
}
