using Gsmu.Api.Authorization;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.InstructorAccountModel;
using Gsmu.Api.Data.Survey;
using Gsmu.Api.Data.Survey.Entities;
using Gsmu.Api.Data.ViewModels.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.InstructorHelper
{
    public class InstructorHelper
    {


        public static GridModel<CourseInstructorModel> GetAllCourses(QueryState state, int instructorid, string folderpath,string courseStatus)
        {

            using (var db = new SchoolEntities())
            {
                var courses = (from a in db.Courses
                               where (a.INSTRUCTORID == instructorid || a.INSTRUCTORID2 == instructorid || a.INSTRUCTORID3 == instructorid)
                               select new CourseInstructorModel
                               {
                                   CancelCourse = a.CANCELCOURSE,
                                   CancelCourseDateTime = a.LastUpdateTime.ToString(),
                                   Coursedate = (from r in db.Course_Times where r.COURSEID == a.COURSEID orderby r.COURSEDATE select r.COURSEDATE).FirstOrDefault(),
                                   MaxDate = (from ct in db.Course_Times where ct.COURSEID == a.COURSEID select ct.COURSEDATE).Max(),
                                   MinDate = (from ct in db.Course_Times where ct.COURSEID == a.COURSEID select ct.COURSEDATE).Min(),
                                   CourseName = a.COURSENAME,
                                   CourseNum = a.COURSENUM,
                                   CourseId = a.COURSEID,
                                   MaxEnrolled = a.MAXENROLL.Value,
                                   MaxWaiting = a.MAXWAIT.Value,
                                   EnrolledInt = (from r in db.Course_Rosters where r.Cancel == 0 && r.COURSEID == a.COURSEID && r.WAITING == 0 select r.STUDENTID).Count(),
                                   Enrolled = (from r in db.Course_Rosters where r.Cancel == 0 && r.COURSEID == a.COURSEID && r.WAITING == 0 select r.STUDENTID).Count().ToString() + " of " + a.MAXENROLL.Value.ToString(),
                                   Cancelled = (from r in db.Course_Rosters where (r.Cancel == 1 || r.Cancel == -1) && r.COURSEID == a.COURSEID && r.WAITING == 0 select r.STUDENTID).Count(),
                                   NoWaiting = (from r in db.Course_Rosters where r.Cancel == 0 && r.COURSEID == a.COURSEID && r.WAITING == 1 select r.STUDENTID).Count(),
                                   Unpaid = (from r in db.Course_Rosters where r.Cancel == 0 && r.COURSEID == a.COURSEID && r.PaidInFull != 0 select r.STUDENTID).Count(),

                                   Room = (a.ROOM == "" || a.ROOM == null) ? a.LOCATION + "" : "<br />Room:" + a.ROOM,
                                   Location = a.LOCATION + " " + a.STREET + " " + a.CITY + " " + a.STATE + " " + a.ZIP + " " + a.Country + "<br />Room:" + a.ROOM,
                                   attendancecount = (from att in db.Attendances where att.COURSEID == a.COURSEID select att.COURSEID).Count(),
                                   transcriptedcount = (from tran in db.Transcripts where tran.CourseId == a.COURSEID select tran.CourseId).Count(),
                                   IsOnline = a.OnlineCourse.Value,
                                   StartEndTimeDisplay = a.StartEndTimeDisplay

                               });
                     if (state.Filters != null)
                     {
                         if (state.Filters.ContainsKey("keyword"))
                         {
                             var keyword = state.Filters["keyword"];
                             courses = courses.Where(e => e.CourseName.Contains(keyword) || e.CourseNum.Contains(keyword));
                         }
                     }
                    if (state.OrderFieldString != null)
                    {
                        switch (state.OrderFieldString)
                        {
                            case "CourseName":
                                if (state.OrderByDirection == OrderByDirection.Ascending)
                                {
                                    courses = courses.OrderBy(e => e.CourseName);
                                }
                                else
                                {
                                    courses = courses.OrderByDescending(e => e.CourseName);
                                }
                                break;

                            case "CourseId":
                                if (state.OrderByDirection == OrderByDirection.Ascending)
                                {
                                    courses = courses.OrderBy(e => e.CourseId);
                                }
                                else
                                {
                                    courses = courses.OrderByDescending(e => e.CourseId);
                                }
                                break;
                            case "CDates":
                                if (state.OrderByDirection == OrderByDirection.Ascending)
                                {
                                    courses = courses.OrderBy(e => e.Coursedate);
                                }
                                else
                                {
                                    courses = courses.OrderByDescending(e => e.Coursedate);
                                }
                                break;
                            case "CourseNum":
                                if (state.OrderByDirection == OrderByDirection.Ascending)
                                {
                                    courses = courses.OrderBy(e => e.CourseNum);
                                }
                                else
                                {
                                    courses = courses.OrderByDescending(e => e.CourseNum);
                                }
                                break;
                            case "Room":
                                if (state.OrderByDirection == OrderByDirection.Ascending)
                                {
                                    courses = courses.OrderBy(e => e.Room);
                                }
                                else
                                {
                                    courses = courses.OrderByDescending(e => e.Room);
                                }
                                break;
                            case "Enrolled":
                                if (state.OrderByDirection == OrderByDirection.Ascending)
                                {
                                    courses = courses.OrderBy(e => e.EnrolledInt);
                                }
                                else
                                {
                                    courses = courses.OrderByDescending(e => e.EnrolledInt);
                                }
                                break;
                            default:
                                //courses = courses.OrderBy(e => e.CourseId);
                                courses = courses.OrderBy(e => e.CourseNum);
                                break;
                        }
                    }                
                else
                {
                    courses = courses.OrderBy(e => e.CourseName);
                }
                    //DateTime dateTodayMax = DateTime.Today.AddDays(1).AddMinutes(-1);
                    DateTime datetoday = DateTime.Today;

                    if (courseStatus == "past")
                    {
                        courses = courses.Where(a => a.CancelCourse == 0 && a.MinDate < datetoday && a.MaxDate < datetoday);
                    }
                    else if (courseStatus == "cancelled")
                    {
                        courses = courses.Where(a => a.CancelCourse != 0);
                    }
                    else if (courseStatus == "needattendance")
                    {
                        courses = courses.Where(a => a.CancelCourse == 0 && a.transcriptedcount == 0 && a.attendancecount == 0 && a.Coursedate < datetoday);
                    }
                    else
                    {
                        courses = courses.Where(a => a.CancelCourse == 0 && a.MaxDate >= datetoday);
                    }

                    if (courseStatus == "past")
                    {
                        BuildExport(courses.ToList(), folderpath, "past");
                    }
                    else if (courseStatus == "needattendance")
                    {
                        BuildExport(courses.ToList(), folderpath, "need_attendance");
                    }
                    else
                    {
                        BuildExport(courses.ToList(), folderpath, "current");
                    }
                    List<CourseInstructorModel> newList = new List<CourseInstructorModel>();
                var model = new GridModel<CourseInstructorModel>(courses.Count(), state);
                courses = model.Paginate(courses);
                var DateFormatsettings = Settings.Instance.GetPubDateFormat();
                foreach (var a in courses.ToList())
                {
                        a.CDates = a.Coursedate.Value.ToString(DateFormatsettings);

                        if (Settings.Instance.GetMasterInfo3().ShowOnlineClassDate == 1 || Settings.Instance.GetMasterInfo3().ShowOnlineClassDate == -1)
                        {
                            if (a.IsOnline == 1 || a.IsOnline==-1)
                            {
                                a.CDates = "Online Course";
                                a.CourseDatesandTime = "Online Course";
                            }
                            if (a.StartEndTimeDisplay != "")
                            {
                                a.CDates = a.StartEndTimeDisplay;
                                a.CourseDatesandTime = a.StartEndTimeDisplay;
                            }
                        }
                        string tempTimeList = "";
                        var courseTimes = (from ct in db.Course_Times where ct.COURSEID == a.CourseId select ct).ToList();
                        foreach (var time in courseTimes)
                        {
                            if (a.IsOnline != 1 && a.IsOnline != -1 && a.StartEndTimeDisplay == "")
                            {
                                tempTimeList = tempTimeList + time.COURSEDATE.Value.ToString(DateFormatsettings) + "(" + time.STARTTIME.Value.ToString("hh:mm tt") + " - " + time.FINISHTIME.Value.ToString("hh:mm tt") + ")<br />";
                            }

                        }
                        if (a.IsOnline != 1 && a.IsOnline != -1 && a.StartEndTimeDisplay == "")
                        {
                            a.CourseDatesandTime = tempTimeList;
                        }
                        newList.Add(a);
                    

                }
                model.Result = newList;

                return model;
            }
        }

            public static void BuildExport(List<CourseInstructorModel> courses,string folderpath,string status)
            {
                 StringBuilder sb = new StringBuilder();
                sb.Append("Course Number").Append(",");
                sb.Append("Course Name").Append(",");
                sb.Append("Course Date").Append(",");
                sb.Append("Location and Room #").Append(",");
                sb.Append("Number Enrolled").Append(",");
                sb.AppendLine();
                foreach (var row in courses)
                {
                        sb.Append(row.CourseNum).Append(",");
                        sb.Append("\"" + row.CourseName + "\"").Append(",");
                        sb.Append(row.Coursedate).Append(",");
                        sb.Append(" "+row.Location).Append(",");
                        sb.Append(row.Enrolled).Append(",");
                    sb.AppendLine();
                }
                if (status == "current")
                {
                    string filename = "CourseList" + AuthorizationHelper.CurrentInstructorUser.USERNAME.Replace("/","") + ".csv";
                    string path = folderpath + filename;
                    File.WriteAllText(path, sb.ToString());
                }
                else if (status == "need_attendance")
                {
                    string filename = "need_attendance_CourseList" + AuthorizationHelper.CurrentInstructorUser.USERNAME.Replace("/", "") + ".csv";
                    string path = folderpath + filename;
                    File.WriteAllText(path, sb.ToString());
                }
                else
                {
                    string filename = "PastCourseList" + AuthorizationHelper.CurrentInstructorUser.USERNAME.Replace("/", "") + ".csv";
                    string path = folderpath + filename;
                    File.WriteAllText(path, sb.ToString());
                }
            }

            public static Entities.Instructor GetInstructor(string username)
            {
                using (var db = new SchoolEntities())
                {
                    var instructor = (from s in db.Instructors where s.USERNAME == username select s).FirstOrDefault();
                    return instructor;
                }
            }
            public static Entities.Instructor GetInstructor(int id)
            {
                using (var db = new SchoolEntities())
                {
                    var instructor = (from s in db.Instructors where s.INSTRUCTORID == id select s).FirstOrDefault();
                    return instructor;
                }
            }


            public static GridModel<SurveyModel> GetSurveyResultDetails(QueryState queryState, int instructorid)
            {
                SurveyModel SurveyModel = new SurveyModel();
                List<SurveyModel> SUrveys = new List<SurveyModel>();
                using (var db = new SchoolEntities())
                {
                    var courses = (from a in db.Courses
                                   where   a.CANCELCOURSE!=1 && a.CANCELCOURSE !=-1 &&(a.INSTRUCTORID == instructorid || a.INSTRUCTORID2 == instructorid || a.INSTRUCTORID3 == instructorid)
                                   select a).ToList();
                    using (var dbSurvey = new SurveyEntities())
                    {
                        foreach(var c in courses){
                        var surveys = (from survey in dbSurvey.CourseSurveys  where survey.CourseID == c.COURSEID  select survey).FirstOrDefault();
                        var coursedate = (from date in db.Course_Times where date.COURSEID == c.COURSEID select date).FirstOrDefault();
                        if (surveys != null)
                        {
                            var mainsurvey = (from s in dbSurvey.Surveys where (s.SurveyID == surveys.SurveyID || s.SurveyID == surveys.BeforeCourseSurveyId) && s.SurveyStatus!=0 && s.instviewresult ==1  select s).FirstOrDefault();
                            if (mainsurvey != null)
                            {
                                SurveyModel.CourseName = c.COURSENAME;
                                var usercourses = (from user in dbSurvey.Users where user.CourseID == c.COURSEID select user).FirstOrDefault();
                                if (usercourses != null)
                                {
                                    try
                                    {
                                        SurveyModel.SurveyName = "Survey Name: " + mainsurvey.Name + "<br />" + "Course Name: " + c.COURSENAME + "<br />Course Start Date: " + coursedate.COURSEDATE.Value.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                    }
                                    catch
                                    {
                                        SurveyModel.SurveyName = "Survey Name: " + mainsurvey.Name + "<br />" + "Course Name: " + c.COURSENAME;
                                    }
                                    SurveyModel.SurveyId = mainsurvey.SurveyID.ToString() + "-" + c.COURSEID;
                                    SUrveys.Add(SurveyModel);
                                    SurveyModel = new SurveyModel();
                                }

                            }
                        }
                        }
                    }
                }
                if(queryState.OrderField!=null){
                    if (queryState.OrderByDirection!=null)
                    {

                        if (queryState.OrderField.ToString().ToLower() == "surveyname")
                            {
                                if (queryState.OrderByDirection == OrderByDirection.Ascending)
                                    SUrveys = SUrveys.OrderBy(survey => survey.SurveyName).ToList();
                                else
                                    SUrveys = SUrveys.OrderByDescending(survey => survey.SurveyName).ToList();
                            }
                            else
                            {
                                if (queryState.OrderByDirection == OrderByDirection.Ascending)
                                    SUrveys = SUrveys.OrderBy(survey => survey.SurveyId).ToList();
                                else
                                    SUrveys = SUrveys.OrderByDescending(survey => survey.SurveyId).ToList();
                            }

                            

                        
                    }
                }

                var model = new GridModel<SurveyModel>(SUrveys.Count(), queryState);
                var ISUrveys = model.Paginate(SUrveys.AsQueryable());

                    model.Result = ISUrveys.ToList();
                
                return model;


            }


       
    }


}
