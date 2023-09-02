using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Integration.Blackboard;
using Gsmu.Api.Integration.Google;
using Gsmu.Api.Integration.Lti;
using Gsmu.Api.Integration.Canvas;
using Gsmu.Api.Web;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.User;
using System.IO;
using Gsmu.Api.Authorization;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using System.Threading;
using Google.Apis.Util.Store;
using System.Web.Script.Serialization;
using RestSharp;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;

namespace Gsmu.Web.Controllers
{
    public class SSOController : Controller
    {
        //
        // GET: /SSO/

        public ActionResult Blackboard(string hash)
        {
            //var config = Gsmu.Api.Integration.Blackboard.Configuration.Instance;
            //var url = config.BlackboardConnectionUrl + "?hash=" + hash;
            //using (StreamWriter _testData = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest.txt"), true))
            //{
            //    _testData.WriteLine(Environment.NewLine + ":::::::Requested From:::"+url + " On " + DateTime.Now); // Write the file.
            //} 
            BlackboardSso.HandleSso(hash, this);
            return RedirectToAction("Index", "Landing");
        }


        //[HttpPost]
        public ActionResult GoogleCal()
        {
			string _curDomain = Request.Url.GetLeftPart(UriPartial.Authority);
			var _sso = new ClientSecrets
			{
				ClientId = GoogleHelper.SSoClientId,
				ClientSecret = GoogleHelper.SSoClientSecret,
			};

            var googleplus_redirect_url = _curDomain+ "/SSO/GoogleCalSync";
            //var googleplus_redirect_url = "https://dev252.gosignmeup.com/SSO/GoogleCalSync";
            var Googleurl = "https://accounts.google.com/o/oauth2/auth?response_type=code&redirect_uri=" + googleplus_redirect_url + "&scope=https://www.googleapis.com/auth/calendar&client_id=" + _sso.ClientId;
            Response.Redirect(Googleurl);
            return null;
        }

			public ActionResult GoogleCalSync(string code)
		{
			string _curDomain = Request.Url.GetLeftPart(UriPartial.Authority);
			string tokenFile = Server.MapPath("~") + @"tokens.json";
			var _sso = new ClientSecrets
			{
				ClientId = GoogleHelper.SSoClientId,
				ClientSecret = GoogleHelper.SSoClientSecret,
			};

			RestClient restClient = new RestClient();
			RestRequest request = new RestRequest();

			request.AddQueryParameter("client_id", _sso.ClientId);
			request.AddQueryParameter("client_secret", _sso.ClientSecret);
			request.AddQueryParameter("code", code);
			request.AddQueryParameter("grant_type", "authorization_code");
			//request.AddQueryParameter("redirect_uri", "https://localhost:44332/SSO/GoogleCalSync");
			request.AddQueryParameter("redirect_uri", _curDomain+"/SSO/GoogleCalSync");

			restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
			var response = restClient.Post(request);

			if(response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				System.IO.File.WriteAllText(tokenFile, response.Content);
			}

			ViewBag.resp = response.StatusCode;

			return View();
		}



        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Google Calendar";


		public ActionResult Google(string begin, string end, string courseId, string command)
		{
			HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
			string url = HttpContext.Request.Url.AbsoluteUri;
			dynamic result = null;
			string request = string.Empty;

			if (!string.IsNullOrEmpty(courseId))
			{
				HttpContext.Session["courseid"] = courseId;
				HttpContext.Session["request"] = "synch";
			}

			if (!string.IsNullOrEmpty(begin))
			{
				HttpContext.Session["begin"] = begin;
				HttpContext.Session["request"] = "synch";
			}
			if (!string.IsNullOrEmpty(end))
			{
				HttpContext.Session["end"] = end;
				HttpContext.Session["request"] = "synch";
			}


			if (HttpContext.Session["request"] != null)
			{
				request = "synch";
				string tokenFile = Server.MapPath("~") + @"tokens.json";
				var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));

				ClientSecrets secrets = new ClientSecrets()
				{
					ClientId = GoogleHelper.SSoClientId,
					ClientSecret = GoogleHelper.SSoClientSecret,
				};

				var token = new TokenResponse { RefreshToken = tokens["access_token"].ToString() };
				var credential = new UserCredential(new GoogleAuthorizationCodeFlow(
					new GoogleAuthorizationCodeFlow.Initializer
					{
						ClientSecrets = secrets
					}),
					"user",
					token);

				var service = new CalendarService(new BaseClientService.Initializer
				{
					HttpClientInitializer = credential,
					ApplicationName = ApplicationName
				});


				DateTime Begin = Convert.ToDateTime(HttpContext.Session["begin"]);
				DateTime End = Convert.ToDateTime(HttpContext.Session["end"]);
				if (HttpContext.Session["courseid"] != null)
				{
					int CourseID = Convert.ToInt32(HttpContext.Session["courseid"].ToString().Split('_')[0].ToString());
					GoogleHelper.GoogleCalV3Sync(credential, 0, CourseID, Begin, End);
				}
				else
				{
					GoogleHelper.GoogleCalV3Sync(credential, 1, 0, Begin, End);
				}
				//HttpContext.Response.Write("<script type='text/javascript'> setTimeout(function(){ window.close(); }, 3000);</script>");


				GoogleUtility("idcleanup", true);
			}

			return result;

		}



        public ActionResult GoogleORG(string begin, string end, string courseId, string command)
        {

            HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            string url = HttpContext.Request.Url.AbsoluteUri;
            dynamic result = null;
            string request = string.Empty;

            if (!string.IsNullOrEmpty(courseId))
            {
                HttpContext.Session["courseid"] = courseId;
                HttpContext.Session["request"] = "synch";
            }

            if (!string.IsNullOrEmpty(begin))
            {
                HttpContext.Session["begin"] = begin;
                HttpContext.Session["request"] = "synch";
            }
            if (!string.IsNullOrEmpty(end))
            {
                HttpContext.Session["end"] = end;
                HttpContext.Session["request"] = "synch";
            }
            if (!string.IsNullOrEmpty(command))
            {
                HttpContext.Session["command"] = command;
                HttpContext.Session["request"] = "synch";
            }


            if (HttpContext.Session["request"] != null)
            {
                request = "synch";
                result = GoogleHelper.HandleGoogleOAuthService(this, request);
                GoogleUtility("idcleanup", true);
            }
            else
            {
                request = "auth";
                result = GoogleHelper.HandleGoogle(this, request);
            }

            if (result == null)
            {
                    if (request == "synch")
                    {
                        HttpContext.Session.Remove("request");
                        HttpContext.Session.Remove("AuthState");
                        if (Request.Cookies["AuthState"] != null)
                            Response.Cookies["AuthState"].Expires = DateTime.Now.AddDays(-1);
                        return View("GoogleCalendarSynch");
                    }
                    else
                    {
                        HttpContext.Session.Remove("request");
                        HttpContext.Session.Remove("AuthState");
                        if (Request.Cookies["AuthState"] != null)
                            Response.Cookies["AuthState"].Expires = DateTime.Now.AddDays(-1);
                        return RedirectToAction("Index", "Landing");
                    }
            }
            return result; 
        }


        //public ActionResult GoogleCalendarSynch(string begin, string end) 
        //{
        //    var jsonresult = new JsonResult();
        //    jsonresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    if (begin != null)
        //        HttpContext.Session["begin"] = begin;
        //    if (end != null)
        //        HttpContext.Session["end"] = end;
        //    //var result = GoogleHelper.GoogleService(this, begin, end);
        //    //var result = GoogleHelper.HandleGoogle(this);
        //    try
        //    {
        //        //if (result == null) 
        //        //{
        //        //    jsonresult.Data = new
        //        //    {
        //        //        Response = "Successfully Synched!.",
        //        //        error = "None",
        //        //        CoursesSynched = "",
        //        //        success = false
        //        //    };
        //        //}

        //    }catch(Exception ex)
        //    {
        //        jsonresult.Data = new
        //        {
        //            Response = "An error Occured.",
        //            error = ex.Message,
        //            CoursesSynched = "",
        //            success = false
        //        };
        //    }
        //    return jsonresult;
        //}

        /// <summary>
        /// Reason can be : system, student, instructor
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public ActionResult Canvas(string reason = "student")
        {
            if (!Gsmu.Api.Integration.Canvas.Configuration.Instance.EnableOAuth2Authentication)
            {
                throw new Exception("We are sorry but this funciton is disabled in the Canvas Admin settings (Canvas OAuth authentication via GSMU login).");
            }
            var result = CanvasClient.HandleOAuth(this, reason);
            return result;
        }

        public ActionResult SetCanvasUser()
        {
            if (Request["instructor"] != "")
                Session["instructorusername"] = Request["instructor"];
            else
                Session["instructorusername"] = null;
            if (Request["student"] != "")
                Session["studentusername"] = Request["student"];
            else
                Session["studentusername"] = null;
            if (Request["supervisor"] != "")
                Session["supervisorusername"] = Request["supervisor"];
            else
                Session["supervisorusername"] = null;

            return  RedirectToAction("SelectCanvasUserType","SSO");
        }

        public ActionResult SelectCanvasUserType()
        {
            ViewBag.ShowInstructorIcon = "none";
            ViewBag.ShowStudentIcon = "none";
            ViewBag.ShowSupervisorIcon = "none";
            if (Session["studentusername"] != null)
            {
                ViewBag.ShowStudentIcon = "inline";
                if (Api.Commerce.ShoppingCart.CourseShoppingCart.Instance.Items.Count()>0)
                         AuthorizationHelper.LoginStudent(Session["studentusername"].ToString());
            }
            if (Session["supervisorusername"] != null)
            {
                ViewBag.ShowSupervisorIcon = "inline";
            }
            if (Session["instructorusername"] != null)
            {
                ViewBag.ShowInstructorIcon = "inline";
            }


            return View();
        }

        public string LoginSelectedCanvasUser(string type)
        {
          
            if(type == "ST")
            {
                string username = (string) Session["studentusername"];
                var messages = AuthorizationHelper.LoginStudent(username);
            }
            else if (type == "IN")
            {
                string username = (string)Session["instructorusername"];
                AuthorizationHelper.LoginInstructor(username);
                //document.location = Settings self.AspSiteRootUrl + '';
                var url = "/public/instructor?action=login&canvas-id=" + this.ControllerContext.HttpContext.Session.SessionID;
                return url;
            }

            else if (type == "SP")
            {
                string username = (string)Session["supervisorusername"];
                AuthorizationHelper.LoginSupervisor(username);
                //document.location = Settings self.AspSiteRootUrl + '';
                var url = "/public/supervisor?action=login&canvas-id=" + username;
                return url;
            }

            return "Public/Course/Browse";
            
        }

        public ActionResult CanvasOAuthRedirect(string reason = "student")
        {
            if (!Gsmu.Api.Integration.Canvas.Configuration.Instance.EnableOAuth2Authentication)
            {
                throw new Exception("We are sorry but this funciton is disabled in the Canvas Admin settings (Canvas OAuth authentication via GSMU login).");
            }
            var result = CanvasClient.HandleOAuthRedirect(this, reason);
            if (result == null)
            {

                //Added To check Required fields that is missing upon Student Login on Canvas
                Gsmu.Web.Areas.Public.Controllers.UserController usercheck = new Areas.Public.Controllers.UserController();
                string userLoginStatus = usercheck.CheckReqMissingFields();

                if (userLoginStatus == "NoMissingReqFields")
                {
                    return RedirectToAction("Index", "Landing");
                }
                else
                {
                    return RedirectToAction("dashboard", "user", new
                    {
                        area = "Public",
                        MissingReqFields = "1"
                    });
                }
                
            }
            return result;
        }

        public ActionResult Lti()
        {
            var result = LtiHelper.HandleSso(this);

#if DEBUG
            foreach (var key in Request.Form.AllKeys)
            {
                Gsmu.Api.Web.ObjectHelper.AddRequestDebugMessage(this, string.Format("{0}: {1}\r\n", key, Request.Form[key]));
            }
#endif
            if (result == null)
            {
                return RedirectToAction("Index", "Landing");
            }
            return result;
        }

        public ActionResult GoogleUtility(string command, bool print) 
        {
            if (command == "idcleanup") 
            {
                GoogleHelper.CourseGoogleCalIDCleanUp(print);
            }
            return null;
        }

        [HttpGet]
        public JsonResult ValidateGoogleCalendarForSync() {
            string message = "";
            bool valid = true;
            //string certloc = System.Configuration.ConfigurationManager.AppSettings["p12CertificateUrl"];
            //string serviceAccountEmail = System.Configuration.ConfigurationManager.AppSettings["serviceAccountId"];
            //if (string.IsNullOrEmpty(certloc))
            //{
            //    message = "Missing Certificate setting";
            //    valid = false;
            //}
            //if (string.IsNullOrEmpty(certloc))
            //{
            //    message += "\nMissing Service Account setting";
            //    valid = false;
            //}
            ////probably not needed because this is a relative
            //string loc = System.Web.HttpContext.Current.Server.MapPath("~") + certloc;
            //if (!System.IO.File.Exists(loc))
            //{
            //    message += "\nNo Certificate Found for Service Account";
            //    valid = false;
            //}

            if(string.IsNullOrEmpty(GoogleHelper.SSoClientId) || string.IsNullOrEmpty(GoogleHelper.SSoClientSecret))
            {
                message += "Missing Google Calendar Integration Parameters";
                valid = false;
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    ResponseStatus = "ok",
                    Valid = valid,
                    Message = message == "" ? "Success" : message
                }
            };
        }
    }
}
