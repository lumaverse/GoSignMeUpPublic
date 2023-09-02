using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Web;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.CourseSettings;

namespace Gsmu.Web.Areas.Public.Controllers
{
    
    [DevelopmentMode(true)]
    public class TestController : Controller
    {
        public ActionResult Embed()
        {
            return View();
        }

        public ActionResult Layout()
        {
            return View();
        }

        public ActionResult Html()
        {
            return View();
        }

        public ActionResult ConfirmationAttachment(string orderNumber = "CT7S5GNR7056841")
        {
            var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
            mail.SendConfirmationEmail("50", orderNumber, "gsmu.softasware.com", "");

            return new ContentResult()
            {
                Content = "OK"
            };
        }
        public ActionResult TestMakeAttachment(int courseId) 
        {
            //for testing api
            try 
            {
                Gsmu.Api.Networking.Mail.EmailFunction emailFunction = new Gsmu.Api.Networking.Mail.EmailFunction();
                //args : email from, routine (if .vbs put .vbs), courseId,method (2 if cancel, else request)
                List<string> fileNamesCreated = emailFunction.MakeICalendarAttachment("vincentb@gosignmeup.com", "", courseId, 1);
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
            catch(Exception ex)
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
        public ActionResult TestGetConfirmationFileAttachments(int courseId) 
        {
            //for testing api
            try
            {
                Gsmu.Api.Networking.Mail.EmailFunction emailFunction = new Gsmu.Api.Networking.Mail.EmailFunction();
                List<string> fileNamesCreated = emailFunction.GetConfirmationEmailAttachments(courseId);
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
        public ActionResult TestSendConfirmationEmail(string ordernumber = "CT7S5GNR7056841")
        {
            try
            {
                var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
                var emailResponse = mail.SendConfirmationEmail("0", ordernumber, "gsmu.softasware.com", "");
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
                        EmailBCC = emailResponse.emailAuditTrailInfo.EmailBCC
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
        //params rosterid
        public ActionResult TestSendCancellationEmail(int rosterId = 0)
        {
            try
            {
                var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
                var emailResponse = mail.SendCancellationEmail(rosterId);
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
                        EmailBCC = emailResponse.emailAuditTrailInfo.EmailBCC
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
        public ActionResult TestMovedFromWaitListToEnrolled(int rosterId = 0) 
        {
            try
            {
                var mail = new Gsmu.Api.Networking.Mail.EmailFunction();
                var emailResponse = mail.SendWaitListToEnrollEMail(rosterId);
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
                        EmailBCC = emailResponse.emailAuditTrailInfo.EmailBCC
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

        public ActionResult TestGenerateCoupons()
        {
            object callResult = null;

            callResult = CouponsModel.GenerateCouponReport(Request, false);

            var result = new JavaScriptResult();
            string arguments = SerializationHelper.SerializeEntity(callResult);
            result.Script = Request["callback"] + "(" + arguments + ");";
            return result;
        }
    }
}
