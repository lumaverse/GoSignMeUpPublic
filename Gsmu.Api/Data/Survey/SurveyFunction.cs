using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.Survey.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.Survey
{
    public class SurveyFunction
    {
        public string GetSurveyComments(int qid,int cid)
        {
            using (var dbSchool = new SchoolEntities())
            {
                using (var db = new SurveyEntities())
                {
                    string result = "<div  style='padding: 6px; border: thin solid #999999; background: white; margin-right:10%;  margin-left:10%;'>";

                    var showresult = 0;
                    var instid = 0;
                    instid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                    var usercourses = (from user in db.Users where user.CourseID == cid select user).ToList();
                    foreach (var u in usercourses)
                    {
                        var comments = (from e in db.AnswerComments where e.QuestionID == qid && e.UserID == u.UserID select e).ToList();
                        foreach (var com in comments)
                        {

                            if (usercourses != null)
                            {
                                if (instid != 0)
                                {
                                    var instructorcheck = (from instcourse in dbSchool.Courses where (instcourse.INSTRUCTORID == instid || instcourse.INSTRUCTORID2 == instid || instcourse.INSTRUCTORID3 == instid) && instcourse.COURSEID == u.CourseID select instcourse).FirstOrDefault();
                                    if (instructorcheck != null)
                                    {
                                        showresult = 1;
                                    }
                                }
                                else
                                {
                                    showresult = 0;
                                }
                            }
                            else
                            {
                                showresult = 0;
                            }
                            if (showresult == 1)
                            {
                                if (com.Comments != "")
                                {
                                    result = result + com.Comments + "<hr>";
                                }
                                showresult = 0;
                            }
                        }


                        var textareas = (from e in db.AnswerTextAreas where e.QuestionID == qid && e.UserID == u.UserID select e).ToList();
                        foreach (var com in textareas)
                        {
                            if (usercourses != null)
                            {
                                if (instid != 0)
                                {
                                    var instructorcheck = (from instcourse in dbSchool.Courses where (instcourse.INSTRUCTORID == instid || instcourse.INSTRUCTORID2 == instid || instcourse.INSTRUCTORID3 == instid) && instcourse.COURSEID == u.CourseID select instcourse).FirstOrDefault();
                                    if (instructorcheck != null)
                                    {
                                        showresult = 1;
                                    }
                                }
                                else
                                {
                                    showresult = 0;
                                }
                            }
                            else
                            {
                                showresult = 0;
                            }
                            if (showresult == 1)
                            {
                                if (com.TextAnswer != "")
                                {
                                    result = result + com.TextAnswer + "<hr>";
                                }
                                showresult = 0;
                            }
                        }
                        result = result;
                    }
                    return result + "</div>";
                    }
                
            }

        }

        public List<SurveyResultChoices> GetChoicesResult(int qid,int cid)
        {
            using (var dbSchool = new SchoolEntities())
            {
                List<SurveyResultChoices> Result = new List<SurveyResultChoices>();
                using (var db = new SurveyEntities())
                {
                    var instid = 0;
                    var answercount = 0;
                    var totalanswers = 0;
                    instid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var QuestionsChoices = (from s in db.QuestionAnswerChoices where s.QuestionID == qid orderby s.AnswerRank ascending select s).ToList();
                    SurveyResultChoices SurveyResultChoices = new SurveyResultChoices();
                    foreach (var q in QuestionsChoices)
                    {
                        var instructorcheck = (from instcourse in dbSchool.Courses where (instcourse.INSTRUCTORID == instid || instcourse.INSTRUCTORID2 == instid || instcourse.INSTRUCTORID3 == instid) && instcourse.COURSEID==cid select instcourse).ToList();
                        foreach (var inst in instructorcheck)
                        {
                            var usercourses = (from user in db.Users where user.CourseID == inst.COURSEID select user).ToList();
                            foreach (var user in usercourses)
                            {
                               var answermultiple= (from s in db.AnswerMultiples where s.QuestionID == qid && s.AnswerID == q.AnswerRank && s.UserID == user.UserID select s).FirstOrDefault();
                               if (answermultiple != null)
                               {
                                   answercount = answercount + 1;
                               }
                             var total_query =   (from s in db.AnswerMultiples where s.QuestionID == qid && s.UserID == user.UserID  select s).FirstOrDefault();
                             if (total_query != null)
                             {
                                 totalanswers = totalanswers + 1;
                             }
                             
                             
                            }
                        }


                        SurveyResultChoices.Choice = q.AnswerText;
                        SurveyResultChoices.No_of_Respondent = answercount;
                        if (totalanswers != 0)
                        {
                            SurveyResultChoices.Ratio = Math.Round(decimal.Parse(((SurveyResultChoices.No_of_Respondent / totalanswers) * 100).ToString()));
                        }
                        else
                        {
                            SurveyResultChoices.Ratio = 0;
                        }
                        Result.Add(SurveyResultChoices);
                        SurveyResultChoices = new SurveyResultChoices();
                        answercount = 0;
                        totalanswers = 0;
                    }
                }
                return Result;
            }
        }

        public List<SurveyResultChoices> GetChoicesResult(int qid,string type,int cid)
        {
            using (var dbSchool = new SchoolEntities())
            {
                List<SurveyResultChoices> Result = new List<SurveyResultChoices>();
                using (var db = new SurveyEntities())
                {
                    var instid = 0;
                    var answercount = 0;
                    var totalanswers = 0;
                    instid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var QuestionsChoices = Enumerable.Range(1, 5).ToList();
                    SurveyResultChoices SurveyResultChoices = new SurveyResultChoices();
                    foreach (var q in QuestionsChoices)
                    {                        var instructorcheck = (from instcourse in dbSchool.Courses where (instcourse.INSTRUCTORID == instid || instcourse.INSTRUCTORID2 == instid || instcourse.INSTRUCTORID3 == instid) && instcourse.COURSEID==cid select instcourse).ToList();
                        foreach (var inst in instructorcheck)
                        {
                            var usercourses = (from user in db.Users where user.CourseID == inst.COURSEID select user).ToList();
                            foreach (var user in usercourses)
                            {
                               var answermultiple= (from s in db.AnswerMultiples where s.QuestionID == qid && s.AnswerID == q && s.UserID == user.UserID select s).FirstOrDefault();
                               if (answermultiple != null)
                               {
                                   answercount = answercount + 1;
                               }
                             var total_query =   (from s in db.AnswerMultiples where s.QuestionID == qid && s.UserID == user.UserID  select s).FirstOrDefault();
                             if (total_query != null)
                             {
                                 totalanswers = totalanswers + 1;
                             }
                             
                             
                            }
                        }
                        SurveyResultChoices.Choice = q.ToString();
                        SurveyResultChoices.No_of_Respondent = answercount;
                        if (totalanswers != 0)
                        {
                            SurveyResultChoices.Ratio = Math.Round(decimal.Parse(((SurveyResultChoices.No_of_Respondent / totalanswers) * 100).ToString()));
                        }
                        else
                        {
                            SurveyResultChoices.Ratio = 0;
                        }
                        Result.Add(SurveyResultChoices);
                        SurveyResultChoices = new SurveyResultChoices();
                        answercount = 0;
                        totalanswers = 0;
                    }
                }
                return Result;
            }
        }
        public int GetSurveyUserId(int studid, int sid, int cid)
        {
            using (var db = new SurveyEntities())
            {
                var user = (from e in db.Users where e.GSMU_UserID == studid && e.SurveyID == sid && e.CourseID == cid select e.UserID).FirstOrDefault();
                return user;
            }
        }
        public int GetSurveyIdbyUser(int studid, int cid)
        {
            using (var db = new SurveyEntities())
            {
                var SurveyID = (from e in db.Users where e.GSMU_UserID == studid && e.CourseID == cid select e.SurveyID).FirstOrDefault();
                return SurveyID;
            }
        }
        public int GetSurveyId(int cid)
        {
            using (var db = new SurveyEntities())
            {
                var SurveyID = (from c in db.CourseSurveys where c.CourseID == cid select c.SurveyID).FirstOrDefault(); ;
                return SurveyID;
            }
        }
        public void SaveSurveyAnswers(AnswerTextArea answers)
        {
            using (var db = new SurveyEntities())
            {
                db.AnswerTextAreas.Add(answers);
                db.SaveChanges();
            }
        }
        public void SaveSurveyAnswers(AnswerMultiple answers)
        {
            using (var db = new SurveyEntities())
            {
                db.AnswerMultiples.Add(answers);
                db.SaveChanges();
            }
        }

        public void SaveSurveyComments(AnswerComment answers)
        {
            using (var db = new SurveyEntities())
            {
                db.AnswerComments.Add(answers);
                db.SaveChanges();
            }
        }

        public void SaveSurveyUser(User user)
        {
            using (var db = new SurveyEntities())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public int CreateSurveyUser(int studid, int cid, int intSurveyId)
        {

            using (var db = new SurveyEntities())
            {
                Gsmu.Api.Data.Survey.Entities.User user = new Api.Data.Survey.Entities.User();
                user.CourseID = cid;
                user.GSMU_UserID = studid;
                user.SurveyID = intSurveyId;
                user.DateInserted = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();

                return user.UserID;
            }
        }

        public List<SurveyObject> GetPostSurveyForStudent(int studentid)
        {
            List<SurveyObject> Surveys = new List<SurveyObject>();
            SurveyObject obj = new SurveyObject();
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var transciptedCourse = (from tran in db.Transcripts
                                         where tran.STUDENTID== studentid
                                         select tran.CourseId).ToList();
                if(transciptedCourse!=null){
                    using (var sb = new SurveyEntities())
                    {
                        sb.Configuration.LazyLoadingEnabled = false;
                        foreach (var c in transciptedCourse)
                        {
                            var postsurveycourses = (from b in sb.CourseSurveys
                                                     join s in sb.Surveys on b.SurveyID equals s.SurveyID
                                                     where b.CourseID == c
                                                     select s).FirstOrDefault();


                            if (postsurveycourses != null)
                            {
                                Course course = (from cr in db.Courses where cr.COURSEID == c select cr).FirstOrDefault();
                                Course_Time coursetime = (from ct in db.Course_Times where ct.COURSEID == c select ct).FirstOrDefault();
                                var checkUser = (from u in sb.Users
                                                 where u.GSMU_UserID == studentid && u.CourseID==c && 
                                                 u.SurveyID == postsurveycourses.SurveyID
                                                 select u).ToList();
                                if (checkUser != null)
                                {
                                    if (checkUser.Count == 0)
                                    {
                                        obj.SurveyTitle = postsurveycourses.Name;
                                        obj.SurveyId = postsurveycourses.SurveyID;
                                        obj.Courseid = c.Value;
                                        obj.CourseNum = course.COURSENUM;
                                        obj.CourseName = course.COURSENAME;
                                        obj.CourseDate = coursetime.COURSEDATE.Value.ToShortDateString();
                                        obj.Studentid = studentid;
                                        Surveys.Add(obj);
                                        obj = new SurveyObject();
                                    }
                                }
                                else
                                {
                                   
                                    obj.SurveyTitle = postsurveycourses.Name;
                                    obj.SurveyId = postsurveycourses.SurveyID;
                                    obj.Courseid = c.Value;
                                    obj.CourseNum = course.COURSENUM;
                                    obj.CourseName = course.COURSENAME;
                                    obj.CourseDate = coursetime.COURSEDATE.Value.ToShortDateString();
                                    obj.Studentid = studentid;
                                    Surveys.Add(obj);
                                    obj = new SurveyObject();
                                }
                            }
                        }
                    }
                }

            }
            return Surveys;
        }

        public List<SurveyObject> GetPreSurveyForStudent(int studentid)
        {
            List<SurveyObject> Surveys = new List<SurveyObject>();
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var rostercourses = (from ros in db.Course_Rosters where ros.STUDENTID == studentid
                                     join c in db.Courses on ros.COURSEID equals c.COURSEID
                                     where ros.Cancel == 0
                                     select ros.COURSEID).ToList();
                

                if (rostercourses != null)
                {
                    using (var sb = new SurveyEntities())
                    {
                        sb.Configuration.LazyLoadingEnabled = false;
                        SurveyObject obj = new SurveyObject();
                        foreach (var r in rostercourses)
                        {
                            var presurveycourses = (from b in sb.CourseSurveys
                                                    join s in sb.Surveys on b.BeforeCourseSurveyId equals s.SurveyID
                                                    where b.CourseID == r
                                                    select s).FirstOrDefault();



                            if (presurveycourses != null)
                            {
                                Course course = (from c in db.Courses where c.COURSEID == r select c).FirstOrDefault();
                                Course_Time coursetime = (from ct in db.Course_Times where ct.COURSEID == r select ct).FirstOrDefault();
                                var checkUser = (from u in sb.Users
                                                 where u.GSMU_UserID == studentid && u.CourseID == r && u.SurveyID == presurveycourses.SurveyID 
                                                 select u.GSMU_UserID).ToList();
                                if (checkUser != null)
                                {
                                    if (checkUser.Count <= 0)
                                    {
                                        
                                        obj.SurveyTitle = presurveycourses.Name +"(Pre)";
                                        obj.SurveyId = presurveycourses.SurveyID;
                                        obj.Courseid = r.Value;
                                        obj.CourseNum = course.COURSENUM;
                                        obj.CourseName = course.COURSENAME;
                                        obj.CourseDate = coursetime.COURSEDATE.Value.ToShortDateString();
                                        obj.Studentid = studentid;
                                        Surveys.Add(obj);
                                        obj = new SurveyObject();
                                    }
                                }
                            }

                        }
                    }
                }

            }
            return Surveys;
        }

        public void SendPurchaseHoursEmailAfterSurvey(int cid)
        {
            if (AuthorizationHelper.CurrentStudentUser != null)
            {
                if (AuthorizationHelper.CurrentStudentUser.EMAIL != "")
                {
                    Gsmu.Api.Data.School.Course.CourseModel courseModel = new School.Course.CourseModel(cid);
                    if (courseModel.Course.CourseConfiguration != "" && courseModel.Course.CourseConfiguration != null)
                    {
                        try
                        {
                            string purchasecredit = "";
                            string emailtopurchase = "";
                            System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                            dynamic courseconfiguration = JSSerializeObj.Deserialize(courseModel.Course.CourseConfiguration, typeof(object));
                            purchasecredit = courseconfiguration["purchasecredit"];
                            if (purchasecredit == "1")
                            {
                                if (Settings.Instance.GetMasterInfo4().customcreditsettings != "" && Settings.Instance.GetMasterInfo4().customcreditsettings != null)
                                {
                                    dynamic globalconfiguration = JSSerializeObj.Deserialize(Settings.Instance.GetMasterInfo4().customcreditsettings, typeof(object));
                                    emailtopurchase = globalconfiguration["emailtopurchase"];
                                    if (emailtopurchase == "0")
                                    {
                                        EmailAuditTrail emailentity = new EmailAuditTrail();
                                        emailentity.EmailSubject = Settings.Instance.GetMasterInfo4().HourPurchaseEmailSubject.Replace("{CourseNumber}",courseModel.Course.COURSENUM).Replace("{CourseName}",courseModel.Course.COURSENAME);
                                        emailentity.EmailBody = Settings.Instance.GetMasterInfo4().HourPurchaseEmailBody.Replace("{CourseNumber}", courseModel.Course.COURSENUM).Replace("{CourseName}", courseModel.Course.COURSENAME);
                                        emailentity.EmailBody = emailentity.EmailBody.Replace("{StartDate}", courseModel.CourseStartAsDate.ToString());
                                        emailentity.EmailBody = emailentity.EmailBody.Replace("{StartDate}", courseModel.CourseEndAsDate.ToString());
                                        emailentity.EmailBody = emailentity.EmailBody.Replace("{CreditHours}", courseModel.Course.CustomCreditHours.ToString());
                                        emailentity.EmailBody = emailentity.EmailBody.Replace("{PurchaseURL}", Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl+"/public/user/dashboard");
                                        emailentity.EmailTo = AuthorizationHelper.CurrentStudentUser.EMAIL;
                                        emailentity.AuditProcess = "Survey Send Email Credit Purchase";
                                        emailentity.AuditDate = DateTime.Now;
                                        Gsmu.Api.Networking.Mail.EmailFunction.SendEmail(emailentity, AuthorizationHelper.CurrentStudentUser.STUDENTID, cid.ToString());
                                    }
                                }
                            }

                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public class SurveyObject
        {
            public int SurveyId { get; set; }
            public string SurveyTitle { get; set; }
            public string CourseNum { get; set; }
            public string CourseName { get; set; }
            public int Courseid { get; set; }
            public int Studentid { get; set; }
            public string CourseDate { get; set; }
        }

        public class SurveyResultChoices
        {
            public string Choice {get;set;}
            public decimal No_of_Respondent{get;set;}
            public decimal Ratio {get; set;}
        }
    }
}
