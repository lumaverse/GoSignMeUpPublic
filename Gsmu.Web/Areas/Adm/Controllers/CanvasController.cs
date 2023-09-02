using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using canvas = Gsmu.Api.Integration.Canvas;
using web = Gsmu.Api.Web;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class CanvasController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("settings");
        }

        public ActionResult Settings(bool iframe = false)
        {
            return View();
        }

        public ActionResult InitializeTestSettings()
        {
            // https://mccb.test.instructure.com/
            /*
             {"AccessToken":"1613~h4qRxWQSOxr0FrBuHHRFdDMBbkGAAFqCroyJtoR53Llgbg5FYlSXFKwqCBy8nGhN","CanvasServerUrl":"https://mccb.test.instructure.com/","CanvaseBaseUri":"https://mccb.test.instructure.com/","EnableOAuth2Authentication":true,"enableTeacherLoginAsStudent":false,"DisableGSMUAuthIfUserInCanvas":false,"HideLoginFormIfUserInCanvas":false,"DisableGSMUAuthIfUserInCanvas":false,"HideLoginFormIfUserInCanvas":false,"OAuth2AuthenticationClientId":null,"CanvasId":"170000000000065","CanvasKey":"HGfQgx1VdJM7HYFBGXX566fnPwsq8kh9Vcofq0dRo7NnyXIQCJJXYm0EWiJwyT9t","CanvasAccountId":16,"Account":{"Id":16,"Name":"MVCC Board (new)","ParentAccountId":null,"RootAccountId":null,"DefaultStorageQuotaMb":500,"DefaultUserStorageQuotaMb":50,"DefaultGroupStorageQuotaMb":50,"DefaultTimeZone":"America/Chicago"},"IntegratedCourseWorkflowStates":[1,0]}
             */
            var config = canvas.Configuration.Instance;
            config.CanvasServerUrl = "https://mccb.test.instructure.com/";
            config.CanvasId = "170000000000065";
            config.CanvasKey = "HGfQgx1VdJM7HYFBGXX566fnPwsq8kh9Vcofq0dRo7NnyXIQCJJXYm0EWiJwyT9t";
            config.EnableOAuth2Authentication = true;
            config.enableGSMUMasterAuthentication = true;
            config.enableTeacherLoginAsStudent = true;
            config.DisableGSMUAuthIfUserInCanvas = true;
            config.HideLoginFormIfUserInCanvas = true;
            config.AccessToken = null;
            config.CanvasAccountId = null;
            config.canvasAuthProdiverId = null;
            config.EnableCanvasLtiAuhentication = true;
            config.UserSynchronizationInDashboard = true;
            config.ExportUserAfterRegistration = true;
            config.ExportEnrollmentAfterCheckout = true;
            config.ExportEnrollmentCancellation = true;
            config.ExportEnrollmentNotUpdateCourse = true;
            config.DisableRosterNormalizationOnExport = true;
            config.allowCanvasCourseSubAccountIntegration = true;
            config.allowCanvasCourseSectionIntegration = true;
            config.allowSupervisorIntegration = true;
            config.allowSyncSupStudRelationIndEnrollment = true;
            config.allowCourseSectionPerRegistration = true;
            config.allowCanvasCustomCourseSISID = "random";
            config.defaultInstructorCourseRole = "TeacherEnrollment";
            config.autoMapCanvasAccount = true;
            config.UserCanvasUniqueIdentifierField = "unique_id";
            config.UserGSMUCanvasUniqueIdentifierField = null;
            config.CourseGradeBookFinalField = "final_score";
            config.GradeBookPassValue = null;
            config.GradeBookPassPercentValue = null;
            config.CanvasGradeUpdateAttendance = null;
            config.CanvasGradeFinalizeAttendance = null;
            config.CanvasUpdateNewGrade = null;
            config.CanvasTranscribeRegistration = null;
            config.CourseGradeSendCertificate = null;
            config.enableCourseGradeIntegration = true;
            config.Save();
            return new ContentResult()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(config),
                ContentType = "application/json"
            };
        }

        [HttpPost]
        public ActionResult SaveSettings(string accessToken = null, string canvasServerUrl = null, string canvasId = null, string canvasKey = null, bool enableOAuth2Authentication = false, bool enableGSMUMasterAuthentication = false, bool enableTeacherLoginAsStudent = false, bool DisableGSMUAuthIfUserInCanvas = false, bool HideLoginFormIfUserInCanvas = false, int? canvasAccountId = null, int? canvasAuthProdiverId = null, string UserCanvasUniqueIdentifierField = null, string UserGSMUCanvasUniqueIdentifierField = null, bool enableCanvasLtiAuhentication = false, bool userSynchronizationInDashboard = false, bool exportUserAfterRegistration = false, bool exportEnrollmentAfterCheckout = false, bool exportEnrollmentCancellation = false, bool ExportEnrollmentNotUpdateCourse = false, bool DisableRosterNormalizationOnExport = false, bool allowCanvasCourseSubAccountIntegration = false, bool allowCanvasCourseSectionIntegration = false, bool allowSupervisorIntegration = false, bool allowSyncSupStudRelationIndEnrollment = false, bool allowCourseSectionPerRegistration = false, bool autoMapCanvasAccount = false, string allowCanvasCustomCourseSISID = null, string defaultInstructorCourseRole = null, bool enableCourseGradeIntegration = false, string CourseGradeBookFinalField = "final_grade", string GradeBookPassValue = null, string GradeBookPassPercentValue = null, string CanvasGradeUpdateAttendance = null, string CanvasGradeFinalizeAttendance = null, string CanvasUpdateNewGrade = null, string CanvasTranscribeRegistration = null, string CourseGradeSendCertificate= null)
        {
            bool success = true;
            string message = string.Empty;
            canvas.Entities.Account[] accounts = null;
            try
            {
                var config = canvas.Configuration.Instance;
                config.CanvasServerUrl = canvasServerUrl;
                config.EnableOAuth2Authentication = enableOAuth2Authentication;
                config.enableGSMUMasterAuthentication = enableGSMUMasterAuthentication;
                config.enableTeacherLoginAsStudent = enableTeacherLoginAsStudent;
                config.DisableGSMUAuthIfUserInCanvas = DisableGSMUAuthIfUserInCanvas;
                config.HideLoginFormIfUserInCanvas = HideLoginFormIfUserInCanvas;
                config.EnableCanvasLtiAuhentication = enableCanvasLtiAuhentication;
                config.UserSynchronizationInDashboard = userSynchronizationInDashboard;
                config.ExportUserAfterRegistration = exportUserAfterRegistration;
                config.ExportEnrollmentAfterCheckout = exportEnrollmentAfterCheckout;
                config.ExportEnrollmentCancellation = exportEnrollmentCancellation;
                config.ExportEnrollmentNotUpdateCourse = ExportEnrollmentNotUpdateCourse;
                config.allowCanvasCourseSubAccountIntegration = allowCanvasCourseSubAccountIntegration;
                config.allowCanvasCourseSectionIntegration = allowCanvasCourseSectionIntegration;
                config.allowSupervisorIntegration = allowSupervisorIntegration;
                config.allowSyncSupStudRelationIndEnrollment = allowSyncSupStudRelationIndEnrollment;
                config.allowCourseSectionPerRegistration = allowCourseSectionPerRegistration;
                config.allowCanvasCustomCourseSISID = allowCanvasCustomCourseSISID;
                config.defaultInstructorCourseRole = defaultInstructorCourseRole;
                config.DisableRosterNormalizationOnExport = DisableRosterNormalizationOnExport;
                config.autoMapCanvasAccount = autoMapCanvasAccount;
                config.CanvasId = canvasId;
                config.CanvasKey = canvasKey;
                config.AccessToken = accessToken;
                config.CanvasAccountId = canvasAccountId;
                config.canvasAuthProdiverId = canvasAuthProdiverId;
                config.UserCanvasUniqueIdentifierField = UserCanvasUniqueIdentifierField;
                config.enableCourseGradeIntegration = enableCourseGradeIntegration;
                config.CourseGradeBookFinalField = CourseGradeBookFinalField;
                config.GradeBookPassValue = GradeBookPassValue;
                config.GradeBookPassPercentValue = GradeBookPassPercentValue;
                config.CanvasGradeUpdateAttendance = CanvasGradeUpdateAttendance;
                config.CanvasGradeFinalizeAttendance = CanvasGradeFinalizeAttendance;
                config.CanvasUpdateNewGrade = CanvasUpdateNewGrade;
                config.CanvasTranscribeRegistration = CanvasTranscribeRegistration;
                config.CourseGradeSendCertificate = CourseGradeSendCertificate;
                config.UserGSMUCanvasUniqueIdentifierField = UserGSMUCanvasUniqueIdentifierField;

                message = "Settings saved successfully.";
                config.Save();

                if (!string.IsNullOrWhiteSpace(config.AccessToken))
                {
                    var accountsResponse = canvas.Clients.AccountClient.GetListMainAccounts;
                    if (accountsResponse.Error != null && accountsResponse.Error.ErrorDetails != null)
                    {
                        throw new Exception(accountsResponse.Error.ErrorDetails.First().Message);
                    }
                    accounts = accountsResponse.Accounts;

                }
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }



            var result = new {
                config = canvas.Configuration.Instance,
                success = success,
                message = message,
                accounts = accounts
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        public ActionResult CanvasRequest(string function = "raw", string method = "get", string url = null, string query = null)
        {
            string json = string.Empty;
            object result = null;

            var httpMethod = new System.Net.Http.HttpMethod(method);

            switch (function.ToLower())
            {
                case "raw":
                    var response = canvas.CanvasClient.GetResponse("RawCanvasRequest", httpMethod, url, query);
                    result = response.RawResponseList;
                    break;
                default:
                    throw new Exception(
                        string.Format("Invalid Canvas service request: {0}", method)
                    ); 
            }

            return new ContentResult()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(result),
                ContentType = "application/json"
            };
        }


    }
}
