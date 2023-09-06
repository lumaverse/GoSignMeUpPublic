using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;

using web = System.Web;
using http = System.Net.Http;
using json = Newtonsoft.Json.Linq;

using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Terminology;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Gsmu.Api.Integration.Canvas
{
    public class CanvasClient
    {
        public static Response GetResponse(string ReqType, http.HttpMethod httpMethod, string relativeUrl, NameValueCollection query, bool addSystemAccessToken = true) {
            string  query2 = null;
            int selectedpage = 0;
            if (query != null) {
                query2 = Gsmu.Api.Language.StringHelper.NameValueCollectionToQueryString(query, Language.NameValueCollectionToQueryStringBehavior.SameKeyRepeat);                
                if (query.AllKeys.Contains("page")) 
                {
                    selectedpage = Int16.Parse(query.GetValues("page").First());
                }                
            }
            return GetResponse(ReqType, httpMethod, relativeUrl, query2, addSystemAccessToken,null,null, null, selectedpage);
        }

        public static Response GetResponse(string requestType, http.HttpMethod httpMethod, string relativeUrl, string query = null, bool addSystemAccessToken = true, Response responseResult = null, string completeUrl = null, http.HttpClient client = null,int selectedpage=0)
        {
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            }
            else
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
            }
            client = client ?? new http.HttpClient();
            var config = Configuration.Instance;
            PaginationHelper pages = null;
            http.HttpResponseMessage response = null;
            bool run = true;
            var gsmuMaxPages = 0;
            if (!string.IsNullOrWhiteSpace(query))
            {
                var queryData = System.Web.HttpUtility.ParseQueryString(query);
                int.TryParse(queryData["gsmu-max-pages"], out gsmuMaxPages);
            }

            string lastRequestUrl = null;
            Action action = delegate()
            {
                UriBuilder buildUri;
                if (completeUrl == null)
                {
                    var baseUri = new Uri(config.CanvaseBaseUri, relativeUrl);
                    buildUri = new UriBuilder(baseUri);
                    buildUri.Query = query;
                }
                else
                {
                    buildUri = new UriBuilder(completeUrl);
                }

                if (addSystemAccessToken)
                {
                    buildUri.AppendQueryArgument("access_token", config.AccessToken);
                }

                string sendResult = null;
                var request = new http.HttpRequestMessage();
                request.Method = httpMethod;
                request.RequestUri = buildUri.Uri;
                lastRequestUrl = buildUri.ToString();

                response = client.SendAsync(request).Result;
                sendResult = response.Content.ReadAsStringAsync().Result;

                var TurnOnDebugTracingMode = System.Configuration.ConfigurationManager.AppSettings["TurnOnDebugTracingMode"];
                if (TurnOnDebugTracingMode != null)
                {
                    if (TurnOnDebugTracingMode.ToLower() == "on")
                    {
                        Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                        Audittrail.TableName = "Canvas";
                        Audittrail.AuditDate = DateTime.Now;
                        Audittrail.RoutineName = requestType; //"Canvas Process";
                        try
                        {
                            Audittrail.AuditAction = "Request Detail:" + buildUri.ToString() + " --- Response Detail: " + sendResult;
                        }
                        catch { }
                        Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                        LogManager.LogSiteActivity(Audittrail);
                    }
                }

                Regex re = new Regex(":[0-9]{7,},");
                MatchCollection m = re.Matches(sendResult);
                foreach (var val in m)
                {

                    sendResult = sendResult.Replace(val.ToString().Replace("}", "").Replace("{", ""), "" + val.ToString().Replace("}", "").Replace("{", "").Replace(":", ":\"").Replace(",","\",") + "");
                }
                var raw = new RawResponse(sendResult, response);
                if (responseResult == null)
                {
                    responseResult = new Response(raw);
                }
                else
                {
                    responseResult.RawResponseList.Add(raw);
                }
                pages = new PaginationHelper(response);
                if (pages.HasNext)
                {
                    completeUrl = pages.NextUrl;
                    run = pages.HasNext && (gsmuMaxPages < 1 || responseResult.RawResponseList.Count < gsmuMaxPages);
                    if (selectedpage > 0)
                    {
                        string lastPage="";
                        string param1 = "";
                        string param2 = "";
                        bool hasLastPage = pages.Pages.TryGetValue(PaginationType.Last, out lastPage);
                        if (hasLastPage == true)
                        {
                            Uri myUri = new Uri(lastPage);
                            param1 = HttpUtility.ParseQueryString(myUri.Query).Get("page");
                            param2 = HttpUtility.ParseQueryString(myUri.Query).Get("per_page");
                        }                       

                        try
                        {
                            responseResult.CourseNoOfpages = Int32.Parse(param1) * Int32.Parse(param2);
                        }
                        catch
                        {
                            responseResult.CourseNoOfpages = 1;
                        }
                        run = false;
                    }
                }
                else
                {
                    run = false;
                }
            };

            while (run)
            {
                action();
            }

            client.Dispose();

            if (responseResult.Error != null && responseResult.Error.ErrorDetails != null && responseResult.Error.ErrorDetails.Length > 0)
            {
                var TurnOnDebugTracingMode = System.Configuration.ConfigurationManager.AppSettings["TurnOnDebugTracingMode"];
                if (TurnOnDebugTracingMode != null)
                {
                    if (TurnOnDebugTracingMode.ToLower() == "on")
                    {
                        Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                        Audittrail.TableName = "Canvas";
                        Audittrail.AuditDate = DateTime.Now;
                        Audittrail.RoutineName = "Canvas Request Error";
                        try
                        {
                            Audittrail.AuditAction = "Request URL:" + lastRequestUrl.Remove(lastRequestUrl.LastIndexOf("=") + 1) + " --- Error Response Detail: " + responseResult.Error.ErrorDetails[0].Message;
                        }
                        catch { }
                        Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                        LogManager.LogSiteActivity(Audittrail);
                    }
                }
                throw new Exception(
                    string.Format("Canvas error! Message: {1}, Request: {0}", lastRequestUrl.Remove(lastRequestUrl.LastIndexOf("=") + 1), responseResult.Error.ErrorDetails[0].Message)
                );
            }

            return responseResult;

        }

        public static Response GetAcccessCode(string code)
        {
            var config = Configuration.Instance;

            var baseUri = new Uri(Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().AspSiteRootUrl);
            //var redirectUri = new Uri(baseUri, "/sso/CanvasOAuthRedirect?reason=system");

            var response = GetResponse("GetAcccessCode", http.HttpMethod.Post, "/login/oauth2/token", new NameValueCollection{
                { "client_id", config.CanvasId },
//                { "redirect_uri", redirectUri.ToString() },
                { "client_secret", config.CanvasKey },
                { "code", code }
            }, false);
            return response;
        }

        public static System.Web.Mvc.ActionResult HandleOAuth(System.Web.Mvc.ControllerBase controller, string reason = null)
        {
            var config = Canvas.Configuration.Instance;
            var context = controller.ControllerContext.HttpContext;
            var request = context.Request;
            var relativeUrl = "/sso/CanvasOAuthRedirect";
            if (reason != null)
            {
                relativeUrl += "?reason=" + System.Web.HttpUtility.UrlPathEncode(reason);
            }
            var result = new Uri(request.Url, relativeUrl);


            var redirect = new Uri(config.CanvaseBaseUri, "/login/oauth2/auth");
            var buildUri = new UriBuilder(redirect);
            buildUri.AppendQueryArgument("client_id", config.CanvasId);
            buildUri.AppendQueryArgument("response_type", "code");
            /*
            if (reason == null)
            {
                buildUri.AppendQueryArgument("scopes", "/auth/userinfo");
            }
             */
            buildUri.AppendQueryArgument("purpose", "login/canvas");
            buildUri.AppendQueryArgument("redirect_uri", result.ToString());
            return new System.Web.Mvc.RedirectResult(buildUri.ToString());
        }

        public static System.Web.Mvc.ActionResult HandleOAuthRedirect(System.Web.Mvc.ControllerBase controller, string reason = null)
        {
            var context = controller.ControllerContext.HttpContext;
            var code = context.Request.QueryString["code"];

            Entities.Authentication authentication = null;
            var error = context.Request.QueryString["error"];
            if (error == null)
            {
                var response = GetAcccessCode(code);
                authentication = response.Authentication;
            }

            var config = Configuration.Instance;

            // system authentication handling for the Canvas config page
            if (reason == "system" || reason == "system-iframe")
            {
                string iframeFix = string.Empty;
                string message = "The Canvas system integration has been established.";

                if (error != null)
                {
                    message = "The Canvas login was denied.";
                } else if (authentication != null && authentication.AcccessToken != null)
                {
                    config.AccessToken = authentication.AcccessToken;
                    config.CanvasAccountId = null;
                    config.Save();
                    message = "Your Canvas access is setup now... To properly enable integration please provide us the account under which the integration will manage the courses, user etc... The configuration is giving you a selection option right now to select from. If your user is not managing accounts in Canvas, please request an account from your Canvas administrator.";
                }
                else
                {
                    message = "GSMU known error: Access token not received. Contact customer support for more details.";
                }

                Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, message);
                System.Web.Mvc.RedirectResult result = null;
                if (reason == "system-iframe")
                {
                    result = new System.Web.Mvc.RedirectResult(Settings.Instance.GetMasterInfo4().AspSiteRootUrl + "portal.asp#{\"navigation\":\"ruby-admin-canvas\",\"param\":null,\"navigationPanel\":\"settings\"}");
                }
                else
                {
                    result = new System.Web.Mvc.RedirectResult("~/adm/canvas/settings");
                }
                return result;
            }

            // oauth authentication handling for user
            if (error != null)
            {
                Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "The Canvas login was denied.");
                var result = new System.Web.Mvc.RedirectResult("~/landing");
                return result;
            }

            // execute user synchronization here
            var userId = authentication.User.Id;
            var synchronizationResult = CanvasImport.SynchronizeUser(Convert.ToInt64(userId.ToString()));

            return CanvasImport.UserSynchornizationResultHandling(controller, synchronizationResult, reason);
        }

    }

    public class Canvas_Object_result
    {
        public Canvas.Entities.Course[] Courses { get; set; }
        public string status { get; set; }
        public int totalCount { get; set; }
        public int resultcount { get; set; }
    }
}

