using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.Survey.Entities;
using Gsmu.Api.Networking.Mail;
using Gsmu.Api.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.Survey
{
    public class Survey
    {
        public Survey() { }
        public Survey(int intSurveyId)
        {
            using (var db = new SurveyEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                var Survey = (from c in db.Surveys where c.SurveyID == intSurveyId select c).FirstOrDefault();
                var Questions = (from s in db.Questions where s.SurveyID == intSurveyId orderby s.Position ascending select s).ToList();
                var SurveyCourses = (from surveycourse in db.CourseSurveys where surveycourse.SurveyID == intSurveyId select surveycourse).ToList();
                Init(db, Survey, Questions, SurveyCourses);
            }
        }


        private void Init(SurveyEntities db, Entities.Survey i, List<Gsmu.Api.Data.Survey.Entities.Question> q, List<Gsmu.Api.Data.Survey.Entities.CourseSurvey> surveycourses)
        {
            SurveyModel = i;
            SurveyQuestions = q;
            CourseSurvey = surveycourses;
        }

        public List<Gsmu.Api.Data.Survey.Entities.QuestionAnswerChoice> QuestionsChoices(int questionid)
        {
            using (var db = new SurveyEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                var QuestionsChoices = (from s in db.QuestionAnswerChoices where s.QuestionID == questionid orderby s.AnswerRank ascending select s).ToList();
                return QuestionsChoices;
            
            } 
        }

        public Gsmu.Api.Data.Survey.Entities.Question QuestionDetails(int questionid)
        {
            using (var db = new SurveyEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                var Question = (from s in db.Questions where s.QuestionID == questionid  select s).FirstOrDefault();
                return Question;

            }
        }

        public Gsmu.Api.Data.Survey.Entities.masterinfo GetMasterInfo()
        {
            using (var db = new SurveyEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                var masterinfo = (from s in db.masterinfoes select s).FirstOrDefault();
                return masterinfo;

            }
        }
        public Gsmu.Api.Data.Survey.Entities.Survey SurveyModel
        {
            get;
            set;
        }

        public List<Gsmu.Api.Data.Survey.Entities.Question> SurveyQuestions
        {
            get;
            set;
        }
        public List<Gsmu.Api.Data.Survey.Entities.CourseSurvey> CourseSurvey
        {
            get;
            set;
        }
    }
    public static class SurveyEmail
    {
        public static string SendSurveyEmail(int survey_type, int courseId, string courseDetails)
        {
            using (var db = new SurveyEntities())
            {
                var masterinfo = (from s in db.masterinfoes select s).FirstOrDefault();
                var survey = (from s in db.CourseSurveys where s.CourseID==courseId select s).FirstOrDefault();
                if(survey==null)
                {
                    return "Survey not available";
                }
                int? surveyId = 0;
                if (survey_type == 0)
                {
                    surveyId = survey.BeforeCourseSurveyId;
                }
                else
                {
                    surveyId = survey.SurveyID;
                }

                if((surveyId== 0) || (surveyId == null))
                {
                    return "Survey not available";
                }
                string surveyName = (from s in db.Surveys where s.SurveyID == surveyId.Value select s.Name).FirstOrDefault();
                string emailSubject = masterinfo.AfterCourseReminderSubject;
                string emailBody = masterinfo.AfterCourseReminderBody;
                string surveyLink = "/public/Survey/ShowSurvey?studid={studeid}&sid=" + surveyId.ToString() + "&cid="+courseId.ToString();
                emailBody = emailBody.Replace("{Details}", "{studentdetail}" + courseDetails);
                 EmailFunction EmailFunction = new EmailFunction();
                using (var schoolDb = new SchoolEntities())
                {
                    var emailRoster = (from s in schoolDb.Course_Rosters join stud in schoolDb.Students on s.STUDENTID equals stud.STUDENTID where s.Cancel == 0 && s.COURSEID == courseId select stud).ToList();
                    if (emailRoster.Count == 0)
                    {
                        return "No Student enrolled is this course.";
                    }
                    foreach (var roster in emailRoster)
                    {
                        surveyLink = surveyLink.Replace("{studeid}",roster.STUDENTID.ToString());
                        emailBody=emailBody.Replace("{studentdetail}","Student:  " +roster.LAST+ ", " + roster.FIRST +"("+roster.STUDENTID+")");
                        emailBody = emailBody.Replace("{SurveyLink}", "<a href='" + surveyLink + "'>" + surveyName+"</a>");

                        if(roster.EMAIL!="")
                        {
                            EmailFunction.SendEmail(roster.EMAIL,emailSubject,emailBody,"");
                        }

                    }
                }

            }
            return "Survey sent to students";
        }
    }

    public class SurveyInfo
    {
        #region Static

        public static SurveyInfo Instance
        {
            get
            {
                var scSurvey = ObjectHelper.GetSessionObject<SurveyInfo>(WebContextObject.SurveyInfo);
                if (scSurvey == null)
                {
                    scSurvey = new SurveyInfo();
                    ObjectHelper.SetSessionObject<SurveyInfo>(WebContextObject.SurveyInfo, scSurvey);
                }
                return scSurvey;
            }


        }

        #endregion
        public int studentid
        {
            get;
            set;

        }
        public int surveyid
        {
            get;
            set;
        }

        public int courseid
        {
            get;
            set;
        }

        public void SetSurvey(int sid, int studid, int cid)
        {
            studentid = studid;
            surveyid = sid;
            courseid = cid;
        }
    }
}
