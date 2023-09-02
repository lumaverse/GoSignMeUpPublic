using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.Terminology;
using Gsmu.Api.Data.School.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Service.Models.Events;
using Gsmu.Service.Models.Events.Session;
using Gsmu.Service.Interface;
using Gsmu.Service.Models.Events.Session.Courses;


namespace Gsmu.Service.BusinessLogic.Events
{
    public class EventDetails : IEventDetails
    {

        public EventDetailsModel GetEventDetails(int Eventid,bool isAdmin=false)
        {

            using (var db = new SchoolEntities())
            {

                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                var eventdetails = (from events in db.Courses
                                    where events.COURSEID == Eventid
                                    select new EventDetailsModel
                                    {
                                        EventId = events.COURSEID,
                                        EventName = events.COURSENAME,
                                        EventNumber = events.COURSENUM,
                                        DisplayCommentEndDateStartDate = events.StartEndTimeDisplay

                                    }).FirstOrDefault();

                if (eventdetails != null)
                {
                    eventdetails.DateTime = GetEventSessionCourseDateTimes(eventdetails.EventId);
                    eventdetails.Sessions = GetEventSession(eventdetails.EventId, isAdmin);
                }

                return eventdetails;



            }

        }
        public List<SessionModel> GetEventSession(int eventid, bool isAdmin = false)
        {
            List<int> PastSessions = new List<int>();
            using (var db = new SchoolEntities())
            {
                var sessions= (from session in db.Courses
                        where session.eventid == eventid
                        && session.sessionid == 0 && session.CANCELCOURSE ==0
                        
                        select new SessionModel
                        {
                            SessionId = session.COURSEID,
                            SessionName = session.COURSENAME,
                            SessionNumber = session.COURSENUM,
                            DisplayCommentEndDateStartDate = session.StartEndTimeDisplay,
                            MandatoryClass = session.mandatory.Value,
                            LocationFullInfo =session.LOCATION +","+session.STREET +"," + session.CITY +"," +session.STATE +","+ session.ZIP,
                        }).ToList();


                foreach (var session in sessions)
                {
                    session.DateTime = GetEventSessionCourseDateTimes(session.SessionId);
                    session.Courses = GetSessionCourses(session.SessionId, isAdmin);
                    session.LocationFullInfo = session.LocationFullInfo.Replace(",,", " ");
                    if (!isAdmin)
                    {
                        if (session.DateTime.First().CourseDate < DateTime.Now)
                        {
                            PastSessions.Add(session.SessionId);

                        }
                    }
                }
                if (!isAdmin)
                {
                    foreach (var sid in PastSessions)
                    {
                        sessions.RemoveAll(_session => _session.SessionId == sid);
                    }
                }
                sessions = sessions.OrderBy(session => session.DateTime.FirstOrDefault().CourseDate).ThenBy(session => session.DateTime.FirstOrDefault().StartTime).ToList();
                return sessions;


            }
        }

        public List<CourseModel> GetSessionCourses(int sessionid, bool isAdmin = false)
        {
            List<int> PastCourses = new List<int>();
            int Noenrolled = 0;
            int NoWaitenrolled = 0;
            using (var db = new SchoolEntities())
            {
                var courses= (from course in db.Courses
                        where course.sessionid == sessionid && course.CANCELCOURSE ==0
                        select new CourseModel
                        {
                            CourseId = course.COURSEID,
                            CourseName = course.COURSENAME,
                            CourseNumber = course.COURSENUM,
                            StayinPublicDays = course.viewpastcoursesdays.Value,
                            LocationFullInfo =course.LOCATION +", "+course.STREET +", " + course.CITY +", " +course.STATE +", "+ course.ZIP,
                            MaxWait = course.MAXWAIT.Value,
                            MaxEnroll = course.MAXENROLL.Value,
                            bb_last_integration_date = course.bb_last_integration_date,
                            haiku_course_id = course.haiku_course_id,
                            canvas_course_id = course.canvas_course_id,
                            heliuslms_last_integration = course.heliuslms_last_integration,
                            disablehaikuintegration = course.disablehaikuintegration,
                            disable_canvas_integration = course.disable_canvas_integration

                        }).ToList();

                foreach (var course in courses)
                {
                    course.DateTime = GetEventSessionCourseDateTimes(course.CourseId);
                    try
                    {
                        if (course.DateTime.Count()== 0)
                        {
                            PastCourses.Add(course.CourseId);
                        }
                    }
                    catch
                    {

                    }

                    course.LocationFullInfo = course.LocationFullInfo.Replace(", ,", " ");

                     Noenrolled = (from roster in db.Course_Rosters where roster.COURSEID == course.CourseId && roster.Cancel==0 && roster.WAITING ==0 select roster.RosterID).Count();
                     NoWaitenrolled = (from roster in db.Course_Rosters where roster.COURSEID == course.CourseId && roster.Cancel == 0 && roster.WAITING != 0 select roster.RosterID).Count();

                     course.StatisticInfo = "Enrolled: " + Noenrolled.ToString();
                     course.StatisticInfo = course.StatisticInfo + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Enrolled Available: " + (course.MaxEnroll - Noenrolled).ToString();
                     course.StatisticInfo = course.StatisticInfo + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Waiting List: " + NoWaitenrolled.ToString();
                     course.StatisticInfo = course.StatisticInfo + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Waiting List Available: " + (course.MaxWait - NoWaitenrolled).ToString();
                     if (course.bb_last_integration_date != null)
                     {
                         if (course.bb_last_integration_date <= DateTime.Parse("1/1/1990"))
                         {
                             course.bb_last_integration_date = null;
                         }
                     }
                     if (course.heliuslms_last_integration != null)
                     {
                         if (course.heliuslms_last_integration <= DateTime.Parse("1/1/1990"))
                         {
                             course.heliuslms_last_integration = null;
                         }
                     }
 
                                       
                    if (!isAdmin)
                    {
                        if (course.DateTime.Count()>0)
                        {
                            if (course.DateTime.First().CourseDate.Value.AddDays(course.StayinPublicDays) < DateTime.Now)
                            {
                                PastCourses.Add(course.CourseId);

                            }
                        }
                        else
                        {
                            PastCourses.Add(course.CourseId);
                        }
                    }
                }

                try { 
                {
                    foreach (var cid in PastCourses)
                    {
                        courses.RemoveAll(_course => _course.CourseId == cid);
                    }
                }
                //courses = courses.OrderBy(course => course.DateTime.FirstOrDefault().CourseDate).ThenBy(course => course.DateTime.FirstOrDefault().StartTime).ToList();
                    courses = courses.OrderBy(course => course.DateTime.FirstOrDefault().StartTime).ThenBy(course => course.CourseName).ToList();
                }

                catch { }
                return courses;

            }
        }

        public List<EventSessionCourseDateTimeModel> GetEventSessionCourseDateTimes(int id)
        {
            using (var db = new SchoolEntities())
            {
                return (from datetime in db.Course_Times
                        where datetime.COURSEID == id
                        orderby datetime.COURSEDATE, datetime.STARTTIME
                        select new EventSessionCourseDateTimeModel
                        {
                            CourseDate = datetime.COURSEDATE,
                            CourseId = datetime.COURSEID,
                            StartTime = datetime.STARTTIME,
                            EndTime = datetime.FINISHTIME,
                        }).ToList();
            }
        }

    }
   
}
