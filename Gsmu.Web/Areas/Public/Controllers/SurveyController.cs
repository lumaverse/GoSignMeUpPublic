using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Data.Survey;
using Gsmu.Api.Data.School.User;
using Gsmu.Api.Web;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data;
using Newtonsoft.Json;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Transcripts;
using Gsmu.Api.Export.GradeCertificate;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Networking.Mail;


namespace Gsmu.Web.Areas.Public.Controllers
{
    public class SurveyController : Controller
    {
        public ActionResult SurveyLogin(int studid = 0, int sid = 0, int cid = 0)
        {
            ViewBag.SurveyId = sid;
            ViewBag.StudentId = studid;
            ViewBag.CourseId = cid;
            SurveyInfo.Instance.SetSurvey(sid, studid, cid);
            ViewBag.CASConfigLoginGsmu = MembershipController.GetCASConfig();
            return View();
        }
        public ActionResult ShowSurvey(int studid = 0, int sid = 0, int cid = 0)
        {
            SurveyInfo.Instance.SetSurvey(0, 0, 0); //Set it to blank after login, or failed to login. Values are set on LoginPage
            ViewBag.ShibbolethLogin = 0;
            var countinst = 0; 
            Survey surveyapi = new Survey(sid);
            try
            {
                CourseModel course = new CourseModel(cid);
                ViewBag.Course = course.Course.COURSENAME;
                if (course.Instructors != null)
                {
                    foreach(Instructor _Instructor in course.Instructors){
                        countinst = countinst + 1;
                        ViewBag.Instructor = ViewBag.Instructor + _Instructor.FIRST + " " + _Instructor.LAST + "<br />";
                    }
                }
                ViewBag.countinst = countinst;
            }
            catch (Exception)
            {
            }
            if (surveyapi.SurveyModel != null)
            {
                ViewBag.Title = surveyapi.SurveyModel.Name;
                ViewBag.SurveyDescription = surveyapi.SurveyModel.Description;
                ViewBag.Questions = surveyapi.SurveyQuestions;
                ViewBag.SurveyImage = surveyapi.SurveyModel.SurveyImage;
                if (surveyapi.SurveyModel.SurveyLoginType == 0)
                {
                    if ((AuthorizationHelper.CurrentStudentUser == null) &&  (AuthorizationHelper.CurrentInstructorUser == null))
                    {
                        if ((Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled == 1 || Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled == 2) && (Settings.Instance.GetMasterInfo4().shibboleth_required_login == 1))
                        {
                            ViewBag.ShibbolethLogin = 1;
                        }

                        ViewBag.LoginRequired = 1;
                        ViewBag.Error = 0;
                    }
                    else
                    {
                        if (AuthorizationHelper.CurrentStudentUser != null)
                        {
                            if (AuthorizationHelper.CurrentStudentUser.STUDENTID != studid)
                            {
                                ViewBag.Error = 1;
                                ViewBag.LoginRequired = 0;
                            }
                            else
                            {
                                ViewBag.Error = 0;
                                ViewBag.LoginRequired = 0;
                            }
                        }
                        else
                        {
                            if (AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID != studid)
                            {
                                ViewBag.Error = 1;
                                ViewBag.LoginRequired = 0;
                            }
                            else
                            {
                                ViewBag.Error = 0;
                                ViewBag.LoginRequired = 0;
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.LoginRequired = 0;
                    ViewBag.Error = 0;
                }

                ViewBag.SurveyId = sid;
                ViewBag.StudentId = studid;
                ViewBag.CourseId = cid;
                ViewBag.Date = DateTime.Now.Date.ToShortDateString();
            }
            else
            {
                ViewBag.Title = "Survey is Not Available";
                ViewBag.Questions = surveyapi.SurveyQuestions;
            }
            string callfromAdmin = "";
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("admin"))
            {
                callfromAdmin = "yes";
            }

            if (AuthorizationHelper.CurrentSubAdminUser != null || AuthorizationHelper.CurrentAdminUser != null || callfromAdmin == "yes")
            {
                ViewBag.LoginRequired = 0;
                ViewBag.IsAdminAccess = true;
                ViewBag.Referrer = Request.UrlReferrer.AbsoluteUri;
            }
            else
            {

                ViewBag.Referrer = "";

            }
            
            return View();
        }

        public ActionResult SurveyConfirmation(string status, string finishedsurveyid)
        {
            string file = "";
            if (status.Contains('|'))
            {
                file = status.Split('|')[1];
                status = status.Split('|')[0];
               
            }
            if (status.ToLower() == "true")
            {
                int intParsed;
                Survey surveyapi;
                if (int.TryParse(finishedsurveyid, out intParsed))
                {
                    // perform your code
                    surveyapi = new Survey(intParsed);
                }
                else
                {
                    surveyapi = new Survey();
                }
                if (!string.IsNullOrEmpty(surveyapi.SurveyModel.SurveyThankYouMessage))
                {
                    ViewBag.ThankYouMessage = surveyapi.SurveyModel.SurveyThankYouMessage;
                } 
                else 
                {
                    ViewBag.ThankYouMessage = surveyapi.GetMasterInfo().DefaultThankYouMessage;
                }

                ViewBag.CertificateLink = file;
            }

            else
            {
                ViewBag.ThankYouMessage = status;
            }
            return View();
        }
        public ActionResult CommentsResult(int intSurveyId, int questionid, int withComment,int cid)
        {
            SurveyFunction surveyfunction = new SurveyFunction();
            //surveyfunction.GetSurveyComments(questionid,cid);
            ViewBag.QuestionId = surveyfunction.GetSurveyComments(questionid,cid);
            return PartialView();
        }
        public ActionResult LikertRatingResult(int intSurveyId, int questionid, int questionnum, string label,int cid)
        {
            SurveyFunction survefunction = new SurveyFunction();
            ViewBag.ChoicesResult = survefunction.GetChoicesResult(questionid,"likertrating",cid);
            ViewBag.Label = label;
            ViewBag.No = questionnum;
            ViewBag.QuestionId = questionid;
            return PartialView();
        }
        public ActionResult LikertResult(int intSurveyId, int questionid, int questionnum, string label,int cid)
        {
            SurveyFunction survefunction = new SurveyFunction();
            ViewBag.ChoicesResult = survefunction.GetChoicesResult(questionid, "likert",cid);
            ViewBag.Label = label;
            ViewBag.No = questionnum;
            ViewBag.QuestionId = questionid;
            return PartialView();
        }
        public ActionResult CheckboxResult(int intSurveyId, int questionid, int questionnum, string label,int cid)
        {
            SurveyFunction survefunction = new SurveyFunction();
            ViewBag.ChoicesResult = survefunction.GetChoicesResult(questionid,cid);
            ViewBag.Label = label;
            ViewBag.No = questionnum;
            ViewBag.QuestionId = questionid;
            return PartialView();
        }
        public ActionResult RadioResult(int intSurveyId, int questionid, int questionnum, string label,int cid)
        {
            SurveyFunction survefunction = new SurveyFunction();
            ViewBag.ChoicesResult = survefunction.GetChoicesResult(questionid,cid);
            ViewBag.Label = label;
            ViewBag.No = questionnum;
            ViewBag.QuestionId = questionid;
            return PartialView();
        }
        public ActionResult LikertRating(int intSurveyId, int questionid, int withComment)
        {
            ViewBag.QuestionId = questionid;
            ViewBag.withComment = withComment;
            return PartialView();
        }

        public ActionResult Likert(int intSurveyId, int questionid, int withComment)
        {
            ViewBag.QuestionId = questionid;
            ViewBag.withComment = withComment;
            return PartialView();
        }
        public ActionResult SurveyCheckBox(int intSurveyId, int questionid, int withComment)
        {
            Survey surveyapi = new Survey(intSurveyId);
            ViewBag.Choices = surveyapi.QuestionsChoices(questionid);
            ViewBag.QuestionId = questionid;
            ViewBag.withComment = withComment;
            return PartialView();
        }

        public ActionResult SurveyRadioButton(int intSurveyId, int questionid, int withComment)
        {
            Survey surveyapi = new Survey(intSurveyId);
            ViewBag.Choices = surveyapi.QuestionsChoices(questionid);
            ViewBag.SurveyId = intSurveyId;
            ViewBag.QuestionId = questionid;
            ViewBag.withComment = withComment;
            return PartialView();
        }
        public ActionResult GetQuestions(int intSurveyId)
        {
            Survey surveyapi = new Survey(intSurveyId);
            var list_questions = JsonConvert.SerializeObject(surveyapi.SurveyQuestions,
                        Formatting.None,
             new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    });
            return Json(list_questions, JsonRequestBehavior.AllowGet);
        }

        public string SubmitAnswer(string answers, int studid = 0, int intSurveyId = 0, int cid = 0)
        {
            try
            {
                int studentid = studid;
                string[] answerSets = answers.Split(new string[] { "~||~" }, StringSplitOptions.None);
                Survey surveyapi = new Survey(intSurveyId);
                int logintype = surveyapi.SurveyModel.SurveyLoginType;
                SurveyFunction survefunction = new SurveyFunction();
                Gsmu.Api.Data.Survey.Entities.AnswerTextArea answertext = new Api.Data.Survey.Entities.AnswerTextArea();
                Gsmu.Api.Data.Survey.Entities.AnswerMultiple answermutiple = new Api.Data.Survey.Entities.AnswerMultiple();
                Gsmu.Api.Data.Survey.Entities.AnswerComment answerComment = new Api.Data.Survey.Entities.AnswerComment();
                if (logintype == 0)
                {
                    int survey_st_id = survefunction.GetSurveyUserId(studid, intSurveyId, cid);
                    if (survey_st_id == 0)
                    {
                        if (studid != 0)
                        {
                            survey_st_id = survefunction.CreateSurveyUser(studid, cid, intSurveyId);
                        }
                    }

                    else
                    {
                        return "You already took the survey.";
                    }

                    studid = survey_st_id;
                }
                else
                {
                   studid = survefunction.CreateSurveyUser(0, cid, intSurveyId);
                }
                foreach (string val in answerSets)
                {
                    if (val.Contains("~|~"))
                    {
                        string qid = val.Split(new string[] { "~|~" }, StringSplitOptions.None)[0];
                        string ans = val.Split(new string[] { "~|~" }, StringSplitOptions.None)[1];
                        string comment = val.Split(new string[] { "~|~" }, StringSplitOptions.None)[2];
                        Gsmu.Api.Data.Survey.Entities.Question question = surveyapi.QuestionDetails(int.Parse(qid));
                        if (question != null)
                        {
                            if ((question.Type.ToLower() == "textbox") || (question.Type.ToLower() == "textarea") || (question.Type.ToLower() == "signature"))
                            {
                                answertext.QuestionID = int.Parse(qid);
                                answertext.TextAnswer = ans;
                                answertext.UserID = studid;
                                survefunction.SaveSurveyAnswers(answertext);

                                answerComment.Comments = comment;
                                answerComment.QuestionID = int.Parse(qid);
                                answerComment.UserID = studid;
                                answerComment.Comment = 1;
                                survefunction.SaveSurveyComments(answerComment);
                            }
                            else if ((question.Type.ToLower() == "likert") || (question.Type.ToLower() == "likertrating") || (question.Type.ToLower() == "radio"))
                            {
                                int i = 0;
                                if (int.TryParse(ans, out i))
                                {
                                    answermutiple.QuestionID = int.Parse(qid);
                                    answermutiple.AnswerID = i;
                                    answermutiple.UserID = studid;
                                    
                                    survefunction.SaveSurveyAnswers(answermutiple);

                                    answerComment.Comments = comment;
                                    answerComment.QuestionID = int.Parse(qid);
                                    answerComment.UserID = studid;
                                    answerComment.Comment = 1;
                                    survefunction.SaveSurveyComments(answerComment);
                                }
                            }
                            else if (question.Type.ToLower() == "checkbox")
                            {
                                if (ans.Contains(','))
                                {
                                    int i = 0;
                                    foreach (string checkedval in ans.Split(','))
                                    {
                                        if (int.TryParse(checkedval, out i))
                                        {
                                            answermutiple.QuestionID = int.Parse(qid);
                                            answermutiple.AnswerID = i;
                                            answermutiple.UserID = studid;
                                            survefunction.SaveSurveyAnswers(answermutiple);
                                        }
                                    }

                                    answerComment.Comments = comment;
                                    answerComment.QuestionID = int.Parse(qid);
                                    answerComment.UserID = studid;
                                    answerComment.Comment = 1;
                                    survefunction.SaveSurveyComments(answerComment);
                                }
                            }
                        }

                    }
                }
                string CertificateLink = "";
                survefunction.SendPurchaseHoursEmailAfterSurvey(cid);
                if(surveyapi.SurveyModel.AfterSurveyCertificate!="")
                {
                    try
                    {
                        
                        Transcripts tran = new Transcripts();
                        if( (Settings.Instance.GetMasterInfo3().TranscribeOnSurveyCompletion == 1)|| (Settings.Instance.GetMasterInfo3().TranscribeOnSurveyCompletion == -1))
                        {
                            tran.TranscribeStudent(studentid, cid);
                        }
                        Transcript StudentTranscript = tran.StudentTranscriptedCourse(studentid, cid);
                        Course_Roster StudentRoster = tran.StudentTranscriptedRoster(studentid, cid);

                        CourseModel cmodel = new CourseModel(cid);
                        cmodel.Course.coursecertificate = int.Parse(surveyapi.SurveyModel.AfterSurveyCertificate.Replace("~", string.Empty));
                        PdfGradeCertificate certificate = new PdfGradeCertificate(cmodel.Course, StudentRoster, StudentTranscript);
                        certificate.Execute();
                        EmailAuditTrail emailentity = new EmailAuditTrail();
                        CertificateLink ="|"+ Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl+"Temp/" + certificate.PdfFileName;
                        emailentity.AttachmentNameMemo = certificate.PdfOutFile + "|";
                        emailentity.EmailBody = surveyapi.SurveyModel.AfterSurveyEmailBody.Replace("{CourseName}", cmodel.Course.COURSENAME).Replace("{First}", StudentHelper.GetStudent(studentid).FIRST).Replace("{Last}", StudentHelper.GetStudent(studentid).LAST).Replace("{Email}", StudentHelper.GetStudent(studentid).EMAIL);
                        emailentity.EmailSubject = surveyapi.SurveyModel.AfterSurveyEmailSubject.Replace("{CourseName}", cmodel.Course.COURSENAME).Replace("{First}", StudentHelper.GetStudent(studentid).FIRST).Replace("{Last}", StudentHelper.GetStudent(studentid).LAST);
                        emailentity.EmailTo = StudentHelper.GetStudent(studentid).EMAIL;
                        if (Settings.Instance.GetMasterInfo().BCCEmailAdmin == 2)
                        {
                            emailentity.EmailBCC = Settings.Instance.GetMasterInfo().PublicEmailAddress.ToString();
                        }
                        emailentity.AuditProcess = "Survey Certificate";
                        emailentity.AuditDate = DateTime.Now;
                        EmailFunction.SendEmail(emailentity, studentid,cid.ToString());
                    }
                    catch
                    {
                        return "Error on sending certificate.";
                    }
                    
                }
                return "true" + CertificateLink;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


       }
        public ActionResult ShowSurveyComments(int sid = 0, int cid = 0){
            var countinst = 0;
            var validrequest = 0;
            try
            {
                Survey surveyapi = new Survey(sid);
                CourseModel course = new CourseModel(cid);
                ViewBag.Course = course.Course.COURSENAME;
                ViewBag.Cid = cid;
                if (course.Instructors != null)
                {
                    foreach (Instructor _Instructor in course.Instructors)
                    {
                        countinst = countinst + 1;
                        ViewBag.Instructor = ViewBag.Instructor + _Instructor.FIRST + " " + _Instructor.LAST + "<br />";
                        if (AuthorizationHelper.CurrentInstructorUser != null)
                        {
                            if (AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID == _Instructor.INSTRUCTORID)
                            {
                                validrequest = 1;
                            }
                        }
                        else
                        {
                        }
                    }
                }
                ViewBag.Title = surveyapi.SurveyModel.Name;
                ViewBag.Questions = surveyapi.SurveyQuestions;
                ViewBag.countinst = countinst;
            }
            catch (Exception)
            {
            }
            ViewBag.validrequest = validrequest;
            return View();
        }

        public ActionResult ShowSurveyResults(int sid = 0, int cid = 0)
        {
            var countinst = 0;
            var validrequest = 0;
            try
            {
                Survey surveyapi = new Survey(sid);
                CourseModel course = new CourseModel(cid);
                ViewBag.Course = course.Course.COURSENAME;
                ViewBag.Cid = cid;
                if (course.Instructors != null)
                {
                    foreach (Instructor _Instructor in course.Instructors)
                    {
                        countinst = countinst + 1;
                        ViewBag.Instructor = ViewBag.Instructor + _Instructor.FIRST + " " + _Instructor.LAST + "<br />";
                        if (AuthorizationHelper.CurrentInstructorUser != null)
                        {
                            if (AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID == _Instructor.INSTRUCTORID)
                            {
                                validrequest = 1;
                            }
                        }
                        else
                        {
                        }
                    }
                }
                ViewBag.countinst = countinst;
                ViewBag.Title = surveyapi.SurveyModel.Name;
                ViewBag.Questions = surveyapi.SurveyQuestions;
            }
            catch (Exception)
            {
            }
            ViewBag.validrequest = validrequest;
            return View();
        }
        //param values are for test only
        public string TestCertificate(int surveyId = 51, int studentId = 119018, int courseId = 8546, int certId = 47)
        {
            Survey surveyapi = new Survey(surveyId);
            Transcripts tran = new Transcripts();
            if ((Settings.Instance.GetMasterInfo3().TranscribeOnSurveyCompletion == 1) || (Settings.Instance.GetMasterInfo3().TranscribeOnSurveyCompletion == -1))
            {
                tran.TranscribeStudent(studentId, courseId);
            }
            Transcript StudentTranscript = tran.StudentTranscriptedCourse(studentId, courseId);
            Course_Roster StudentRoster = tran.StudentTranscriptedRoster(studentId, courseId);

            CourseModel cmodel = new CourseModel(courseId);
            cmodel.Course.coursecertificate = certId;
            PdfGradeCertificate certificate = new PdfGradeCertificate(cmodel.Course, StudentRoster, StudentTranscript);
            certificate.Execute();
            return certificate.PdfOutFile;
        }
    }
}