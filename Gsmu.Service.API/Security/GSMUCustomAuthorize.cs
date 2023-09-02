using Gsmu.Service.BusinessLogic.Security.Authentication;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.School;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Linq;
using Gsmu.Service.Models.Students;

namespace Gsmu.Service.API.Security
{
    public class GSMUCustomAuthorize : AuthorizeAttribute
    {
        Gsmu.Service.Interface.Security.Authentication.IAuthenticationManager _authenticationManager;
        Gsmu.Service.Interface.Students.IStudentManager _studentManager;
        Gsmu.Service.Interface.Admin.IAdminManager _adminManager;
        HttpError error = null;
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext == null)
                {
                    throw new Exception("Missing HttpContext");
                }

                string token = string.Empty;
                string role = string.Empty;
                IEnumerable<string> headerValues;
                actionContext.Request.Headers.TryGetValues("token", out headerValues);

                if (headerValues == null || headerValues.FirstOrDefault() == null)
                {
                    throw new Exception(ConfigSettingConstant.MISSING_TOKEN);
                }

                token = actionContext.Request.Headers.GetValues("token").FirstOrDefault();
                role = actionContext.Request.Headers.GetValues("role").FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    error = new HttpError(ConfigSettingConstant.MISSING_TOKEN_VALUE);
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, error);
                }
                else
                {
                    var base64EncodedTokenBytes = System.Convert.FromBase64String(token);
                    var decodedToken = System.Text.Encoding.UTF8.GetString(base64EncodedTokenBytes).Split(':');

                    string username = string.Empty;
                    string sessionId = string.Empty;
                    string usertype = role;
                    if (usertype == "student")
                    {
                        string _username = actionContext.Request.Headers.GetValues("username").FirstOrDefault();
                        string _sessionId = actionContext.Request.Headers.GetValues("sessionId").FirstOrDefault();
                        base64EncodedTokenBytes = System.Convert.FromBase64String(_username);
                        var _decodeduserName = System.Text.Encoding.UTF8.GetString(base64EncodedTokenBytes);
                        base64EncodedTokenBytes = System.Convert.FromBase64String(_sessionId);
                        var _decodedSessionId = System.Text.Encoding.UTF8.GetString(base64EncodedTokenBytes);
                        username = _decodeduserName;
                        sessionId = _decodedSessionId;
                    }
                    else
                    {
                        username = decodedToken[0];
                        sessionId = decodedToken[1];
                    }
                    if (decodedToken.Length >= 3)
                    {
                        usertype = decodedToken[2];
                    }

                    _authenticationManager = new AuthenticationManager();
                    if (usertype == "student")
                    {
                        var studentCreds = _studentManager.GetStudentAuthBySessionId(sessionId);
                        if (studentCreds != null) {
                            StudentAuthenticationResponseModel StudentAuthenticationResponseModel = _authenticationManager.LoginStudent(username, studentCreds.Password);
                            if (StudentAuthenticationResponseModel.Studentid == 0)
                            {
                                error = new HttpError(ConfigSettingConstant.REQUEST_NOT_AUTHORIZED);
                                actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, error);
                            }
                        }
                        
                    }
                    else
                    {
                        var adminCreds = _adminManager.GetAdminAuthBySessionId(sessionId);
                        if (adminCreds != null){
                            AdminModel admin = _authenticationManager.GetAdminByCredential(username, adminCreds.Password);
                            if (admin == null)
                            {
                                error = new HttpError(ConfigSettingConstant.REQUEST_NOT_AUTHORIZED);
                                actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, error);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                error = new HttpError(ex.Message);
                actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, error);
            }

        }
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}