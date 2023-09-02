using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Networking.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class NetworkingController : Controller
    {
        // GET: Adm/Networking
        public ActionResult Index()
        {
            return View();
        }
        //EMAIL Sending
        public ActionResult SendConfirmationEmail(string OrderNumber = "", MailSettings.NetworkingEmailResendingModel resendingModel = null)
        {
            try
            {
                var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
                var emailResponse = mail.SendConfirmationEmail("0", OrderNumber, "gsmu.softasware.com", "", resendingModel);
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = emailResponse.status,
                        SentStatus = emailResponse.sentStatus,
                        EmailSubject = emailResponse.emailAuditTrailInfo.EmailSubject,
                        EmailBody = emailResponse.emailAuditTrailInfo.EmailBody,
                        EmailFrom = emailResponse.emailAuditTrailInfo.EmailFrom,
                        EmailTo = emailResponse.emailAuditTrailInfo.EmailTo,
                        EmailCC = emailResponse.emailAuditTrailInfo.EmailCC,
                        EmailBCC = emailResponse.emailAuditTrailInfo.EmailBCC,
                        ErrorDescription = emailResponse.errorMessage
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "Failed",
                        SentStatus = false,
                        ErrorDescription = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
        public ActionResult SendCancellationtionEmail(int RosterId, MailSettings.NetworkingEmailResendingModel resendingModel = null)
        {
            try
            {
                var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
                var emailResponse = mail.SendCancellationEmail(RosterId, resendingModel);
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = emailResponse.status,
                        SentStatus = emailResponse.sentStatus,
                        EmailSubject = emailResponse.emailAuditTrailInfo.EmailSubject,
                        EmailBody = emailResponse.emailAuditTrailInfo.EmailBody,
                        EmailFrom = emailResponse.emailAuditTrailInfo.EmailFrom,
                        EmailTo = emailResponse.emailAuditTrailInfo.EmailTo,
                        EmailCC = emailResponse.emailAuditTrailInfo.EmailCC,
                        EmailBCC = emailResponse.emailAuditTrailInfo.EmailBCC,
                        ErrorDescription = emailResponse.errorMessage
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "Failed",
                        SentStatus = false,
                        ErrorDescription = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
        public ActionResult SendWaitListEmail(int RosterId, MailSettings.NetworkingEmailResendingModel resendingModel = null)
        {
            try
            {
                var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
                var emailResponse = mail.SendWaitListToEnrollEMail(RosterId, resendingModel);
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = emailResponse.status,
                        SentStatus = emailResponse.sentStatus,
                        EmailSubject = emailResponse.emailAuditTrailInfo.EmailSubject,
                        EmailBody = emailResponse.emailAuditTrailInfo.EmailBody,
                        EmailFrom = emailResponse.emailAuditTrailInfo.EmailFrom,
                        EmailTo = emailResponse.emailAuditTrailInfo.EmailTo,
                        EmailCC = emailResponse.emailAuditTrailInfo.EmailCC,
                        EmailBCC = emailResponse.emailAuditTrailInfo.EmailBCC,
                        ErrorDescription = emailResponse.errorMessage
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "Failed",
                        SentStatus = false,
                        ErrorDescription = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
        public ActionResult SendCourseReminderEmail(int courseId, string dateFrom, string dateTo)
        {
            return View();
        }
        public ActionResult MakeEmailAttachmentsByCourse(string emailAddress, string callroutine, int courseId)
        {
            try
            {
                Gsmu.Api.Networking.Mail.EmailFunction emailFunction = new Gsmu.Api.Networking.Mail.EmailFunction();
                //args : email from, routine (if .vbs put .vbs), courseId,method (2 if cancel, else request)
                List<string> fileNamesCreated = emailFunction.MakeICalendarAttachment(emailAddress, "", courseId, 1);
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "Success",
                        FileNames = fileNamesCreated
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "Failed",
                        ErrorDescription = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        //EMAIL CONTENT PREVIEW
        public ActionResult GetEmailPreview(int mailType, bool replaceToken = false, string OrderNumber = "", int RosterId = 0)
        {
            try
            {
                EmailFunction emailFunction = new EmailFunction();
                var getEmailPreview = emailFunction.GetEmailPreview(mailType, replaceToken, OrderNumber, RosterId);
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "success",
                        Subject = getEmailPreview.subject,
                        Body = getEmailPreview.body
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "Failed",
                        ErrorDescription = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        //ORDER INFO
        public ActionResult GetOrderInfoByOrderNumber(string query = "", int mailType = 0)
        {
            using (var db = new SchoolEntities())
            {
                var rosterOrderNumber = (from r in db.Course_Rosters
                                         join s in db.Students on r.STUDENTID equals s.STUDENTID
                                         join c in db.Courses on r.COURSEID equals c.COURSEID
                                         where r.OrderNumber.StartsWith(query)
                                         select new
                                         {
                                             id = r.RosterID.ToString(),
                                             name = r.OrderNumber,
                                             label = "Roster ID : " + r.RosterID + " | Order # : " + r.OrderNumber + " | Course : " + c.COURSENAME + " ( " + c.COURSEID + " ) " + " | Student : " + s.FIRST + " " + s.LAST
                                         })
                                     .Distinct()
                                     .ToList();

                return new JsonResult()
                {
                    Data = rosterOrderNumber,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            };

        }


        //This is the function for Email Confimation//
        public ActionResult GetMultipleCourseListByOrderNum(bool replaceToken = true, string OrderNumber = "", int RosterId = 0)
        {
            EmailFunction emailFunction = new EmailFunction();
            EmailFunction.EmailContentList EmailContentList = new EmailFunction.EmailContentList();

            string Body = string.Empty;
            string subject = string.Empty;
            using (var _dbSchool = new SchoolEntities())
            {

                var result = (from _courseList in _dbSchool.Course_Rosters
                              where _courseList.OrderNumber == (OrderNumber)
                              select new EmailFunction.EmailContentList
                              {
                                  CourseId = _courseList.COURSEID,
                                  RosterId = _courseList.RosterID,

                              }).ToList();
                Body = Settings.Instance.GetMasterInfo3().EmailConfirmationContent;
                subject = Settings.Instance.GetMasterInfo3().EmailConfirmationSubject;
                int startindex = 0;
                int endindex = 0;
                startindex = Body.IndexOf("{Start Tokens}");
                startindex = ((startindex >= 0) ? startindex : 0);
                endindex = Body.IndexOf("{End Tokens}");
                endindex = ((endindex >= 0) ? endindex : Body.Length - 1);
                string sub = Body.Substring(startindex, endindex - startindex);
                string EmailBody = string.Empty;
                string sub1 = string.Empty;
                foreach (var course in result)
                {

                    var roster = _dbSchool.Course_Rosters.Where(r => course.RosterId > 0 ? r.RosterID == course.RosterId : r.OrderNumber == OrderNumber).FirstOrDefault();
                    EmailContentList.subject = replaceToken ? emailFunction.ReplaceToken("0.0", subject, roster, "") : subject;
                    EmailContentList.Body = replaceToken ? emailFunction.ReplaceToken("0.0", Body, roster, "") : Body;
                    sub1 = replaceToken ? emailFunction.ReplaceToken("0.0", sub, roster, "") : sub;
                    EmailBody += replaceToken ? emailFunction.ReplaceToken("0.0", sub, roster, "") : sub;

                }

                subject = EmailContentList.subject;
                EmailContentList.EmailBody = EmailContentList.Body.Replace(sub1, EmailBody);

                return new JsonResult()
                {
                    Data = new
                    {
                        Status = "success",
                        Subject = subject,
                        Body = EmailContentList.EmailBody
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            }
        }
    }
}
