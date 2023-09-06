using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Authorization;
using Gsmu.Api.Networking.Mail;
using System.Reflection;
using haiku = Gsmu.Api.Integration.Haiku;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using System.Globalization;
// TODO(class) Reorder, this gets messy with alt+shift+F10
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Oauth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Util.Store;
using Google.Apis.Json;
//using Google.Apis.Auth.OAuth2.Mvc;

using Google.Apis.Auth.OAuth2.Flows;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace Gsmu.Api.Integration.Google
{
    public static class GoogleHelper
    {
        public static bool SsoEnabled
        {
            get
            {
                var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_enabled;
                return value.HasValue && value.Value == 1;
            }
        }

        public static string SSoClientId
        {
            get
            {
                //var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id;
                var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_calendar;
                var objects = JObject.Parse(value);
                value = (string)objects["client_id"];
                return value;
            }
        }

        public static string SSoClientSecret
        {
            get
            {
                //var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_secret;
                var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_calendar;
                var objects = JObject.Parse(value);
                value = (string)objects["client_secret"];
                return value;
            }
        }

        public static string SSoApiKey
        {
            get
            {
                var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().google_sso_api_key;
                return value;
            }
        }

        public static System.Web.Mvc.ActionResult HandleSso(System.Web.Mvc.ControllerBase controller)
        {
            System.Web.Mvc.ActionResult result = null;
            var openId = new OpenIdRelyingParty();
            IAuthenticationResponse response = openId.GetResponse();

            if (response == null)
            {
                var openid = new OpenIdRelyingParty();
                IAuthenticationRequest request = openid.CreateRequest(
                    Identifier.Parse(WellKnownProviders.Google)
                );

                var fr = new FetchRequest();
                fr.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                fr.Attributes.AddRequired(WellKnownAttributes.Name.First);
                fr.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                fr.Attributes.AddRequired("http://schemas.openid.net/ax/api/user_id");
                request.AddExtension(fr);

                // Require some additional data
                request.AddExtension(new ClaimsRequest
                {
                    Email = DemandLevel.Require,
                    FullName = DemandLevel.Require,
                    Nickname = DemandLevel.Require
                });

                result = request.RedirectingResponse.AsActionResultMvc5();
            }
            else // response != null
            {
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        var oid = response.ClaimedIdentifier.ToString();
                        var fetch = response.GetExtension<FetchResponse>();
                        var id = fetch.GetAttributeValue("http://schemas.openid.net/ax/api/user_id");
                        var userId = GoogleHelper.GetGoogleUserId(id);
                        var student = StudentHelper.GetStudent(userId);

                        if (student == null)
                        {
                            var firstName = fetch.GetAttributeValue(WellKnownAttributes.Name.First);
                            var lastName = fetch.GetAttributeValue(WellKnownAttributes.Name.Last);
                            var email = fetch.GetAttributeValue(WellKnownAttributes.Contact.Email);
                            var password = GoogleHelper.GetGooglePassword(id);

                            student = new Student();
                            student.FIRST = firstName;
                            student.LAST = lastName;
                            student.EMAIL = email;
                            student.USERNAME = userId;
                            student.STUDNUM = password;
                            student.google_user = 1;

                            StudentHelper.RegisterStudent(student);
                        }
                        Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google login successful.");
                        var messages = AuthorizationHelper.LoginStudent(userId);
                        Gsmu.Api.Web.ObjectHelper.AddRequestMessages(controller, messages);
                        break;

                    case AuthenticationStatus.Canceled:
                        Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google login cancelled.");
                        break;

                    case AuthenticationStatus.Failed:
                        Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google login failed.");
                        break;
                }
            }
            return result;
        }
        //Token
        private static TokenResponse token;
        //Temp CLIENT SECRETS 
        public static ClientSecrets secrets = new ClientSecrets()
        {
            ClientId = SSoClientId == "" ? "949209139310-55hpris0j0i5fsvq762pa738rbkpg6hk.apps.googleusercontent.com" : SSoClientId,
            ClientSecret = SSoClientSecret == "" ? "Eg_KA1lKd4Q_cb-erCYH9BZ-" : SSoClientSecret
        };
        static CalendarService calService;
        static PlusService plusService;
        //GOOGLE TESTING
        public static void GoogleTest(string loc)
        {
            string serviceAccountEmail = "504735883351-9hhq5pndpckm8fs1vm62co9br3fje5dn@developer.gserviceaccount.com";
            var certificate = new X509Certificate2(loc, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail) { Scopes = new[] { CalendarService.Scope.Calendar } }.FromCertificate(certificate));
            var initService = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = "GoogleService",
                HttpClientInitializer = credential
            });

            DateTime start_time = new DateTime(2015, 3, 1, 12, 00, 00);
            DateTime end_time = new DateTime(2015, 3, 2, 12, 00, 00);
            System.Threading.Tasks.Task<bool> test = credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None);
            CalendarSynch(start_time.ToString(), end_time.ToString(), "test Name", "2525", "test description", initService);
        }

        ///HANDLE GOOGLESSO - USED BY AUTHENTICATION
        //USES COOKIES
        public static System.Web.Mvc.ActionResult HandleGoogle(System.Web.Mvc.ControllerBase controller, string requester)
        {
            string response = string.Empty;
            System.Web.Mvc.ContentResult content = new System.Web.Mvc.ContentResult();
            string state = string.Empty;
            List<string> coursenames = new List<string>();
            //int courseCounter = 0;
            string scheme = HttpContext.Current.Request.Url.Scheme;
            string host = HttpContext.Current.Request.Url.Host;
            string port = HttpContext.Current.Request.Url.Port.ToString();
            string url = scheme + "://" + host + ":" + port;
            string fullurl = controller.ControllerContext.HttpContext.Request.Url.AbsoluteUri;

            //check if the url has been redirected
            if (HttpContext.Current.Request.Cookies.Get("AuthState") == null)
            {
                Random random = new Random((int)DateTime.Now.Ticks);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < 13; i++)
                {
                    builder.Append(Convert.ToChar(
                            Convert.ToInt32(Math.Floor(
                                    26 * random.NextDouble() + 65))));
                }

                state = builder.ToString();

                string scope = string.Empty;
                scope = "https://www.googleapis.com/auth/plus.profile.emails.read";
                string urlEndpoint = "https://accounts.google.com/o/oauth2/auth?client_id=" + secrets.ClientId
                    + "&response_type=code&scope=openid " + scope + "&redirect_uri=" + url + "/sso/google"
                    + "&state=security_token=" + state + "&approval_prompt=auto&access_type=online&include_granted_scopes=true";

                HttpContext.Current.Response.Cookies["AuthState"].Value = state;
                controller.ControllerContext.HttpContext.Response.Redirect(urlEndpoint);
            }
            else
            {
                // Get the code from the request POST body.
                string AuthState = HttpContext.Current.Request.Cookies.Get("AuthState").Value;
                if (!string.IsNullOrEmpty(AuthState))
                {
                    //make sure to clear the cookie to reset the requests
                    HttpContext.Current.Response.Cookies["AuthState"].Expires = DateTime.Now.AddDays(-1);
                }
                if (fullurl.Contains("code"))
                {
                    string code = string.Empty;
                    string return_scope = string.Empty;
                    string sessionstate = string.Empty;
                    string return_scope_selection = string.Empty;

                    //FIRST LOAD (FIRST AUTHENTICATION)
                    code = controller.ControllerContext.HttpContext.Request.QueryString["code"].ToString();
                    sessionstate = (string)controller.ControllerContext.HttpContext.Request.QueryString["state"];
                    sessionstate = sessionstate.Split('=')[1].ToString();


                    if ((!string.IsNullOrEmpty(AuthState) && sessionstate == AuthState) || HttpContext.Current.Request.Cookies.Get("OAuthRefreshToken") != null) //Compare the state code from the original to the response
                    {
                        //GET refreshtoken from previous logins OAuthRefreshToken
                        // Use the code exchange flow to get an access and refresh token.

                        IAuthorizationCodeFlow flow =
                            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                            {
                                ClientSecrets = secrets,
                                Scopes = new string[] { PlusService.Scope.UserinfoProfile, PlusService.Scope.UserinfoEmail, CalendarService.Scope.Calendar }
                            });
                        //Authenticatin Starts---
                        if (HttpContext.Current.Request.Cookies.Get("OAuthRefreshToken") == null)
                            token = flow.ExchangeCodeForTokenAsync("", code, url + "/sso/google",
                                    CancellationToken.None).Result;
                        else
                            token = flow.RefreshTokenAsync("", HttpContext.Current.Request.Cookies.Get("OAuthRefreshToken").Value, CancellationToken.None).Result;

                        Oauth2Service service = new Oauth2Service(
                            new BaseClientService.Initializer());
                        Oauth2Service.TokeninfoRequest request = service.Tokeninfo();

                        if (token.AccessToken != null)
                            request.AccessToken = token.AccessToken;

                        if (token.RefreshToken != null) //INITIALIZE 
                            HttpContext.Current.Response.Cookies["OAuthRefreshToken"].Value = token.RefreshToken;

                        Tokeninfo info = request.Execute();
                        //Authenticatin Ends---
                        string gplus_id = info.UserId;
                        //Provide the Auth for getting the user information from google +
                        var credential = new UserCredential(flow, info.UserId, token);
                        //var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("test") { Scopes = new[] { CalendarService.Scope.Calendar }, User = "my email here" }
                        if (requester == "auth")
                        {
                            var plusService = new PlusService(new BaseClientService.Initializer()
                            {
                                ApplicationName = "GSMUGOOGLE+",
                                HttpClientInitializer = credential
                            });

                            //check if blacklist // already done below.
                            //if (CheckEmailGroupListing(info.Email) == "blacklist")
                            //{
                            //    Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google login failed. Your email address domain is on blacklist.");
                            //    return null;
                            //}

                            //Authentication
                            //Execute the Profile Saving if existed
                            var profile = plusService.People.Get("me").Execute();
                            var userId = GoogleHelper.GetGoogleUserId(gplus_id);
                            var student = StudentHelper.GetStudent(userId);
                            var googleEmail = profile.Emails.Count() > 1 ? profile.Emails.First().Value.ToString() : info.Email;

                            //CheckEmailRestriction
                            string CurrentURI = Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl + "public/user/CheckEmailRestriction";
                            string reqCheck = "domain=@" + googleEmail.ToString().Split('@')[1].ToString() + "&email=" + googleEmail;

                            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(CurrentURI);
                            objRequest.Method = "POST";
                            objRequest.ContentLength = reqCheck.Length;
                            objRequest.ContentType = "application/x-www-form-urlencoded";

                            StreamWriter myWriter = null;
                            myWriter = new StreamWriter(objRequest.GetRequestStream());
                            myWriter.Write(reqCheck);
                            myWriter.Close();

                            // returned values are returned as a stream, then read into a string
                            string YayorNay = "true";
                            string YayorNayMsg = "* Failed to login. Please try again or contact Administrator.";
                            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                            using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
                            {
                                //post_response = responseStream.ReadToEnd();
                                var post_response = JObject.Parse(responseStream.ReadToEnd());
                                responseStream.Close();
                                YayorNay = (string)post_response["valid"];
                                YayorNayMsg = (string)post_response["EmailNotification"];
                            }
                            //Begin - YayorNay - always true unless check domain proved otherwise  
                            if (YayorNay == "True")
                            {
                                //allow key off by google email
                                if (student == null)
                                {
                                    student = StudentHelper.GetStudentByEmail(googleEmail);
                                }
                                if (student == null)
                                {
                                    var firstName = profile.Name.GivenName;
                                    var lastName = profile.Name.FamilyName;
                                    var password = GoogleHelper.GetGooglePassword(userId);
                                    string notes = profile.AboutMe;

                                    student = new Student();
                                    student.FIRST = firstName;
                                    student.LAST = lastName;
                                    student.EMAIL = googleEmail;
                                    student.USERNAME = userId;
                                    student.STUDNUM = "Google Assigned/Maintains";
                                    student.NOTES = notes;
                                    student.ADDRESS = "";
                                    student.CITY = "";
                                    student.STATE = "";
                                    student.ZIP = "";
                                    student.StudRegField1 = "";
                                    student.StudRegField2 = "";
                                    student.StudRegField3 = "";
                                    student.StudRegField4 = "";
                                    student.StudRegField5 = "";
                                    student.StudRegField6 = "";
                                    student.StudRegField7 = "";
                                    student.StudRegField8 = "";
                                    student.StudRegField9 = "";
                                    student.StudRegField10 = "";
                                    student.StudRegField11 = "";
                                    student.StudRegField12 = "";
                                    student.StudRegField13 = "";
                                    student.StudRegField14 = "";
                                    student.StudRegField15 = "";
                                    student.StudRegField16 = "";
                                    student.StudRegField17 = "";
                                    student.StudRegField18 = "";
                                    student.StudRegField19 = "";
                                    student.StudRegField20 = "";
                                    student.SCHOOL = 0;
                                    student.DISTRICT = 0;
                                    student.GRADE = 0;
                                    student.DateAdded = DateTime.Now;
                                    student.LastUpdateTime = DateTime.Now;
                                    student.SAPLastPendingReason = "Passing from Google";
                                    student.AuthFromLDAP = 0;
                                    student.loginTally = 0;
                                    student.google_user = 1;
                                    //create haiku account if Haiku Setting Turn on Import During Authentication is on.
                                    // it will search for existing account in Haiku, 
                                    //      if not exist create new account
                                    //      if exist, parse the importid & haiku internal ID and populate the existing account.
                                    var HaikuConfig = haiku.Configuration.Instance;
                                    if (HaikuConfig.EnableExportGoogleUser2Haiku)
                                    {
                                        //for now use this create account request work around to see the IDs.
                                        // will need to use Haiku API to check if Google user exist or not when it's available
                                        //MailAddress address = new MailAddress(email);
                                        //string host = address.Username;
                                        string[] sTempEmail = googleEmail.Split('@');
                                        string HaikuUserLookUpParam = "import_id=GSMU" + gplus_id + "&first_name=" + firstName + "&last_name=" + lastName + "&login=" + sTempEmail[0];
                                        HaikuUserLookUpParam = HaikuUserLookUpParam + "&password=maintain-by-haiku&google_email=" + googleEmail + "&user_type=S&enabled=true";
                                        object result = null;
                                        //getDebugResponse(string debugMethod = "get", string debugRequestURL = "test/ping", string debugQuery = null)
                                        result = haiku.HaikuImport.syncGoogleAct2HaikutUponAuth("post", "user?", HaikuUserLookUpParam);
                                        //need to parse the response.
                                        //1. handling the case the domain email already exist in Haiku
                                        //2. need to handle the case of random GMAIL account.
                                        //3. exist or not
                                        object ResultObject = result;
                                        Type myObjectType = ResultObject.GetType();
                                        IList<PropertyInfo> myObjectList = new List<PropertyInfo>(myObjectType.GetProperties());
                                        int myHaiKuID = 0;
                                        string tempHaikuID = "";
                                        string tempHaikuImportID = "";

                                        foreach (PropertyInfo myObjectItem in myObjectList)
                                        {
                                            var myObjectItemName = myObjectItem.Name.ToString();
                                            if (myObjectItemName == "ResponseString")
                                            {
                                                //regular expression need to be used if this is permanent. 
                                                object myObjectItemValue = myObjectItem.GetValue(ResultObject, null);
                                                string tempMyObjectItemValue = myObjectItemValue.ToString();
                                                if (tempMyObjectItemValue.IndexOf("[") != -1)
                                                {
                                                    //user exist reponse ERROR but not really an error more like a warning
                                                    tempHaikuID = ExtractBetween(tempMyObjectItemValue, "[", "]");
                                                    int tempNumber;
                                                    if (tempHaikuID != "" && int.TryParse(tempHaikuID, out tempNumber))
                                                    {
                                                        myHaiKuID = Int32.Parse(tempHaikuID);
                                                        tempHaikuImportID = ExtractBetween(tempMyObjectItemValue, "Existing Import ID: &quot;", "&quot;");
                                                    }
                                                }
                                                else if (tempMyObjectItemValue.IndexOf("A user with the import_id") != -1)
                                                {
                                                    // found user exist with same importID
                                                    // no way to verify this at the moment -  not linking
                                                }
                                                else if (tempMyObjectItemValue.IndexOf("import_id=") != -1)
                                                {
                                                    //user does not exist, response OK
                                                    tempMyObjectItemValue = tempMyObjectItemValue.Replace("_id", "_xd");
                                                    tempMyObjectItemValue = tempMyObjectItemValue.Replace("=\"", "=[");
                                                    tempMyObjectItemValue = tempMyObjectItemValue.Replace("\"", "]");
                                                    tempHaikuID = ExtractBetween(tempMyObjectItemValue, "id=[", "] google_enabled");
                                                    int tempNumber;
                                                    if (tempHaikuID != "" && int.TryParse(tempHaikuID, out tempNumber))
                                                    {
                                                        myHaiKuID = Int32.Parse(tempHaikuID);
                                                    }
                                                    tempHaikuImportID = "GSMU" + gplus_id;
                                                }
                                            }
                                        }
                                        student.haiku_import_id = tempHaikuImportID;
                                        student.haiku_user_id = myHaiKuID;
                                    }

                                    StudentHelper.RegisterStudent(student);
                                }
                                else
                                {
                                    var firstName = profile.Name.GivenName;
                                    var lastName = profile.Name.FamilyName;
                                    googleEmail = profile.Emails.Count() > 1 ? profile.Emails.First().Value.ToString() : info.Email;
                                    string notes = profile.AboutMe;
                                    string address = profile.CurrentLocation;
                                    string experience = profile.Occupation;
                                    student.FIRST = firstName;
                                    //student.USERNAME = userId; //with google keyed off by email is ok. Beware alias may result incorrect g-ID
                                    student.LAST = lastName;
                                    student.EMAIL = googleEmail;
                                    student.NOTES = notes;
                                    student.ADDRESS = address;
                                    student.EXPERIENCE = experience;
                                    StudentHelper.UpdateStudentInfo(student);
                                }

                                Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google login successful.");
                                //required email is unique with Google SSO.
                                var messages = AuthorizationHelper.LoginStudentByEmail(googleEmail);
                                Gsmu.Api.Web.ObjectHelper.AddRequestMessages(controller, messages);
                            }
                            else
                            {
                                Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, YayorNayMsg + " ErrG102.");
                            }
                            //End - YayorNay - always true unless check domain proved otherwise  
                            return null;

                        }
                        else if (requester == "synch")
                        {

                            var initService = new CalendarService(new BaseClientService.Initializer()
                            {
                                ApplicationName = "GSMUGOOGLE+",
                                HttpClientInitializer = credential
                            });
                            using (var db = new SchoolEntities())
                            {
                                string link = string.Empty;
                                string instructor = string.Empty;
                                string location = string.Empty;
                                string location_additional_info = string.Empty;
                                string final_location = string.Empty;
                                string description = string.Empty;
                                string coursename = string.Empty;
                                string start_time = string.Empty;
                                string end_time = string.Empty;


                                DateTime Begin = Convert.ToDateTime(HttpContext.Current.Session["begin"]);
                                DateTime End = Convert.ToDateTime(HttpContext.Current.Session["end"]);
                                bool delete = false;
                                int CourseID = 0;
                                if (HttpContext.Current.Session["courseid"] != null)
                                {
                                    if (HttpContext.Current.Session["courseid"].ToString().Contains("_"))
                                    {
                                        if (HttpContext.Current.Session["courseid"].ToString().Split('_')[1].ToString() == "delete")
                                        {
                                            delete = true;
                                        }
                                        CourseID = Convert.ToInt32(HttpContext.Current.Session["courseid"].ToString().Split('_')[0].ToString());
                                    }
                                    else
                                    {
                                        CourseID = Convert.ToInt32(HttpContext.Current.Session["courseid"].ToString());
                                    }

                                    var courseList = (from courses in db.Courses
                                                      join courseTime in db.Course_Times on courses.COURSEID equals courseTime.COURSEID
                                                      where (courseTime.COURSEDATE != null)
                                                      && courses.COURSEID == CourseID
                                                      select new { courses, courseTime }).Distinct().OrderBy(ct => ct.courseTime.COURSEDATE);
                                    if (courseList.Count() > 0)
                                    {
                                        if (delete)
                                        {
                                            CalendarDelete(CourseID.ToString(), initService);
                                        }
                                        else
                                        {
                                            var courseFullInfo = new CourseModel(CourseID);
                                            var courseInfo = courseFullInfo.Course;
                                            var courseInstructors = courseFullInfo.Instructors.ToList();
                                            var courseDateFrom = courseFullInfo.CourseStartAsDate;
                                            var courseDateTo = courseFullInfo.CourseEndAsDate;

                                            coursename = courseInfo.IsCancelled ? courseInfo.COURSENAME + "(Cancelled)" : courseInfo.COURSENAME;
                                            final_location = courseInfo.LOCATION + ", " + courseInfo.LocationAdditionalInfo;
                                            link = "<a href='" +
                                            Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl != "" ?
                                            Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl + "public/course/browse?courseid=" + CourseID :
                                            Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().AspSiteRootUrl
                                            + "dev_students.asp?action=coursedetail&id=" + CourseID + "'>Click Here for Details</a><br /><br />";

                                            foreach (var instructors in courseInstructors)
                                            {
                                                instructor += instructors.FIRST + " " + instructors.LAST + ", ";
                                            }

                                            start_time = courseDateFrom.Value.Date.ToString("yyyy-MM-dd") + " " + courseDateFrom.Value.Date.ToString("HH:mm:ss");
                                            end_time = courseDateTo.Value.Date.ToString("yyyy-MM-dd") + " " + courseDateTo.Value.Date.ToString("HH:mm:ss");
                                            description = link + "<br />" + instructor + "<br />" + final_location;
                                            CalendarSynch(start_time, end_time, coursename, CourseID.ToString(), description, initService);
                                        }
                                    }
                                    HttpContext.Current.Response.Write("<script type='text/javascript'> setTimeout(function(){ window.close(); }, 3000);</script>");
                                }
                                else
                                {
                                    var courseTimes = (from courses in db.Courses
                                                       join courseTime in db.Course_Times on courses.COURSEID equals courseTime.COURSEID
                                                       where (courseTime.COURSEDATE != null)
                                                       && courseTime.COURSEDATE >= Begin
                                                       && courseTime.COURSEDATE <= End
                                                       select new { courses, courseTime }).Distinct().OrderBy(ct => ct.courseTime.COURSEDATE);
                                    foreach (var course in courseTimes)
                                    {
                                        var courseFullInfo = new CourseModel(course.courses.COURSEID);
                                        var courseInfo = courseFullInfo.Course;
                                        var courseInstructors = courseFullInfo.Instructors.ToList();
                                        var courseDateFrom = courseFullInfo.CourseStartAsDate;
                                        var courseDateTo = courseFullInfo.CourseEndAsDate;

                                        coursename = courseInfo.IsCancelled ? courseInfo.COURSENAME + "(Cancelled)" : courseInfo.COURSENAME;
                                        final_location = courseInfo.LOCATION + ", " + courseInfo.LocationAdditionalInfo;
                                        link = "<a href='" +
                                        Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl != "" ?
                                        Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl + "public/course/browse?courseid=" + CourseID :
                                        Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().AspSiteRootUrl
                                        + "dev_students.asp?action=coursedetail&id=" + CourseID + "'>Click Here for Details</a><br /><br />";

                                        foreach (var instructors in courseInstructors)
                                        {
                                            instructor += instructors.FIRST + " " + instructors.LAST + ", ";
                                        }
                                        start_time = courseDateFrom.Value.Date.ToString("yyyy-MM-dd") + " " + courseDateFrom.Value.Date.ToString("HH:mm:ss");
                                        end_time = courseDateTo.Value.Date.ToString("yyyy-MM-dd") + " " + courseDateTo.Value.Date.ToString("HH:mm:ss");

                                        description = link + "<br />" + instructor + "<br />" + final_location;
                                        CalendarSynch(start_time, end_time, coursename, course.courses.COURSEID.ToString(), description, initService);

                                    }
                                    HttpContext.Current.Response.Write("<script type='text/javascript'> setTimeout(function(){ window.close(); }, 3000);</script>");
                                }
                            }
                            return null;
                        }
                    }
                    else
                    {
                        Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google failed.");
                        return null;
                    }
                }

            }
            return content;
        }



        //HANDLE OAUTH SERVICE - USED BY CALENDAR SYNCING
        //USES SSL / CERTIFICATE TO VALIDATE - IMPERSONATE THE GOOGLE USER - NO AUTHENTICATION DONE
        public static System.Web.Mvc.ActionResult HandleGoogleOAuthService(System.Web.Mvc.ControllerBase controller, string requester)
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
            string certloc = System.Configuration.ConfigurationManager.AppSettings["p12CertificateUrl"]; //@"GoogleService-bf3a67b9854d.p12"; JOSEPH's ACCOUNT
            if (string.IsNullOrEmpty(certloc))
            {
                HttpContext.Current.Response.Write("<p style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Google Authentication failed. Google service settings on webconfig not set, Please check appconfig settings.</p>");
                return null;
            }
            string loc = System.Web.HttpContext.Current.Server.MapPath("~") + certloc;
            if (!File.Exists(loc))
            {
                HttpContext.Current.Response.Write("<p style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Google Authentication failed. Google Service certificate not found, Please check appconfig settings.</p>");
                return null;
            }
            string serviceAccountEmail = System.Configuration.ConfigurationManager.AppSettings["serviceAccountId"];
            if (string.IsNullOrEmpty(serviceAccountEmail))
            {
                serviceAccountEmail = "615138420313-gaclsmf3vlam987upnkg5v5310sh617v@developer.gserviceaccount.com";
            }
            //Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id;//"615138420313-gaclsmf3vlam987upnkg5v5310sh617v@developer.gserviceaccount.com"; // CLYDE's ACCOUNT

            if (!serviceAccountEmail.Contains("gserviceaccount.com"))
            {
                HttpContext.Current.Response.Write("<p style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Google Authentication failed. Install Service Account on <a href='https://console.developers.google.com/project'>console</a>, follow the directions <a href='https://developers.google.com/console/help/new/?hl=en_US#serviceaccounts'>here</a>.</p>");
                return null;
            }

            var certificate = new X509Certificate2(loc, "notasecret", X509KeyStorageFlags.MachineKeySet |
                                     X509KeyStorageFlags.PersistKeySet |
                                     X509KeyStorageFlags.Exportable);
            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = new[]
                {
                    CalendarService.Scope.Calendar,
                    PlusService.Scope.UserinfoEmail,
                    PlusService.Scope.UserinfoProfile
                }
            }.FromCertificate(certificate));

            if (requester == "auth")
            {
                GoogleAuthenticate(credential, controller);
                return null;
            }
            else
            {
                DateTime Begin = Convert.ToDateTime(HttpContext.Current.Session["begin"]);
                DateTime End = Convert.ToDateTime(HttpContext.Current.Session["end"]);
                if (HttpContext.Current.Session["courseid"] != null)
                {
                    int CourseID = Convert.ToInt32(HttpContext.Current.Session["courseid"].ToString().Split('_')[0].ToString());
                    GoogleCalendarSyncHandler(credential, 0, CourseID, Begin, End);
                }
                else
                {
                    GoogleCalendarSyncHandler(credential, 1, 0, Begin, End);
                }
                HttpContext.Current.Response.Write("<script type='text/javascript'> setTimeout(function(){ window.close(); }, 3000);</script>");
            }
            return null;
        }

        #region MAIN ACTIONS
            /// <summary>
            /// SYNC CALENDAR
            /// </summary>
            /// <param name="start_time"></param>
            /// <param name="end_time"></param>
            /// <param name="coursename"></param>
            /// <param name="ID"></param>
            /// <param name="desc"></param>
            /// <param name="loc"></param>
            /// <param name="cal"></param>
            private static void CalendarSynch(string start_time, string end_time, string coursename, string ID, string desc, CalendarService cal, int dateCount = 0)
            {
                string additionalErrorResponse = string.Empty;
                string googleCalOwnerStatus = string.Empty;
                try
                {
                    DateTime DateChecker;
                    string calendar_email = "primary";
                    Event entry = new Event();
                    EventDateTime start = new global::Google.Apis.Calendar.v3.Data.EventDateTime();
                    EventDateTime finish = new global::Google.Apis.Calendar.v3.Data.EventDateTime();

                    if (DateTime.TryParse(start_time, out DateChecker) && DateTime.TryParse(end_time, out DateChecker))
                    {
                        if (Convert.ToDateTime(end_time) >= Convert.ToDateTime(start_time))
                        {
                            using (var db = new SchoolEntities())
                            {
                                int course_id = Convert.ToInt32(ID);
                                //check course before sending to Calendar
                                var course = (from c in db.Courses
                                              where c.COURSEID == course_id && c.InternalClass == 0
                                              select c).SingleOrDefault();

                                if (course == null)
                                {
                                    HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'>Course ID: " + ID + " <font color='orange'>Not Synced</font> on your calendar</h3>");
                                    HttpContext.Current.Response.Write("<p style='font-style:italic;font-family: monospace;'>Reason : This is because this course is internal. </p>");
                                }
                                else
                                {
                                    #region HANDLES GOOGLE REQUEST AND SETS THE CALENDAR
                                    // This will get the calendar owned by the service account, if this fails means it doesnt own one
                                    List<CalendarListEntry> calendarEntryList = cal.CalendarList.List().Execute().Items.ToList();
                                    // iF THIS IS EMPTY, USE THE PRIMARY EMAIL OR THE OWNER OF THE EMAIL
                                    if (!string.IsNullOrEmpty(Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id))
                                        calendar_email = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id;
                                    // Check if the google_sso_client_id is owned by the Service Account
                                    CalendarListEntry calendarEntry = calendarListEntryFromSettings(calendarEntryList);
                                    if (calendarEntry != null)
                                    {
                                        if (!googleAccountIsWriter(calendarEntry))
                                        {
                                            //either role is not "writer" or google_sso_client_id calendar email 
                                            googleCalOwnerStatus = "<br/> The service account used does not have 'writer' role on the calendar id : " + calendar_email + "<br/>";
                                        }
                                        else
                                        {
                                            calendar_email = calendarEntry.Id;
                                        }
                                    }
                                    else
                                    {
                                        additionalErrorResponse = "<br/> The service account used could not find calendar id : " + calendar_email + " on it's list of owned calendars.<br/>";
                                    }
                                    #endregion

                                    #region TIMEZONE / TIME HANDLING
                                    TimeZoneInfo localZoneInfo = TimeZoneInfo.Local;
                                    //SETTINGS FROM DB - [MASTERINFO3].system_timezone_hour
                                    int? system_time_zone = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().system_timezone_hour;
                                    start.DateTime = system_time_zone == 999 || system_time_zone == 0 ? Convert.ToDateTime(start_time) : GoogleBasedDate(Convert.ToDateTime(start_time).ToUniversalTime());
                                    finish.DateTime = system_time_zone == 999 || system_time_zone == 0 ? Convert.ToDateTime(end_time) : GoogleBasedDate(Convert.ToDateTime(end_time)).ToUniversalTime();
                                    start.TimeZone = AmericanTimeZone(system_time_zone); // IF THE TIMEZONE SETTING IS NONE USE THE CALENDAR TIMEZONE
                                    finish.TimeZone = AmericanTimeZone(system_time_zone); // IF THE TIMEZONE SETTING IS NONE USE THE CALENDAR TIMEZONE
                                    #endregion

                                    #region GOOGLE CALENDAR ENTRY VALUE SETTING
                                    //APPEND DATETIME START AND END TO DESCRIPTION TO INDICATE THE ORIGINAL DATETIME START AND DATE
                                    desc = "Course DATES : (" + start_time + " - " + end_time + ")" + "<br />" + Environment.NewLine + desc;
                                    entry.Location = course.STREET + ", " + course.CITY + ", " + course.STATE + ", " + course.ZIP;
                                    entry.Summary = coursename;
                                    entry.Description = desc;
                                    entry.Start = start;
                                    entry.End = finish;

                                    //ALTER IF ONLINE - SHOULD ONLY TAG THE START DATE
                                    if (course.IsOnlineCourse) {
                                        entry.End = start;
                                    }
                                    #endregion

                                    #region GOOGLE CALENDAR EVENT CREATE/UPDATE EXECUTIONG
                                    if (string.IsNullOrEmpty(course.SUBCATEGORY2c) || dateCount > 1)
                                    {
                                        Event eventCreated = cal.Events.Insert(entry, calendar_email).Execute();
                                        string eventId = eventCreated.Id;

                                        //save id in course id
                                        course.SUBCATEGORY2c = "gcalid_" + eventId;

                                        #region LOG MULTIPLE CALID ON DB
                                        if (!course.IsOnlineCourse && dateCount > 1)
                                        {
                                            //USE new column gcal_ids
                                            //Enable this one if the gcal_ids column is added on db and on edbmx
                                            //course.SUBCATEGORY2b += (!string.IsNullOrEmpty(course.SUBCATEGORY2b) ? course.SUBCATEGORY2b + "," : "") + eventId;
                                        }
                                    #endregion

                                    db.SaveChanges();
                                        HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'>Course ID: " + ID + " ( " + start.DateTime.ToString() + " - " + finish.DateTime.ToString() + " ) <font color='green'>Synched</font> on your calendar</h3>");
                                    }
                                    else
                                    {
                                        //this is already in the calendar (DB)
                                        string gcal_id = course.SUBCATEGORY2c;
                                        string gcal_id_val = gcal_id.Split('_')[1].ToString();
                                        cal.Events.Update(entry, calendar_email, gcal_id_val).Execute();
                                        HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'>Course ID: " + ID + " ( " + start.DateTime.ToString() + " - " + finish.DateTime.ToString() + " ) <font color='green'>Updated</font> on your calendar</h3>");
                                    }
                                    #endregion
                                   
                            }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    additionalErrorResponse += googleCalOwnerStatus;
                    string error_message = string.Empty;
                    if (ex.InnerException != null)
                    {
                        error_message = ex.InnerException.Message;
					HttpContext.Current.Response.Write("<h3 style='padding:5px; font-family: monospace; font-weight:bold;color:red;'>Course ID: " + ID + " - Course was not synced due to some internal issue. ERCAL002" + additionalErrorResponse + "</h3>");
					HttpContext.Current.Response.Write("<a href='/SSO/GoogleCal' style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold; font-size:18px;'>Please re-login here and authorized Google Calendar.</a>");
				}
				else
                    {
                        error_message = ex.Message;
                        HttpContext.Current.Response.Write("<h3 style='padding:5px; font-family: monospace; font-weight:bold;color:red;'>Course ID: " + ID + " - Course was not synced due to some internal issue. ERCAL003" + additionalErrorResponse + "</h3>");
                        HttpContext.Current.Response.Write("<a href='/SSO/GoogleCal' style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold; font-size:18px;'>Please re-login here and authorized Google Calendar.</a>");
                    }
                    HttpContext.Current.Response.Write("<p style='font-style:italic;font-family: monospace;'>Please note: The error also can be caused by multiple numbers of course times item </p>");
                    //@todo : create a function for this
                    string fullurl = HttpContext.Current.Request.Url.AbsoluteUri;
                    //EmailAuditTrail emailAuditrail = new EmailAuditTrail();
                    //emailAuditrail.EmailTo = "vincentb@gosignmeup.com";
                    //emailAuditrail.EmailSubject = "ERROR OCCURED";
                    //emailAuditrail.EmailBody = "Error Stack : " + error_message + "\n" + "URL : " + fullurl;
                    //emailAuditrail.AuditDate = DateTime.Now;
                    //emailAuditrail.AuditProcess = "Google Calendar Sync";
                    //EmailFunction.SendEmail(emailAuditrail);
                }

            }
            /// <summary>
            /// DELETE CALENDAR EVENT
            /// </summary>
            /// <param name="ID"></param>
            /// <param name="cal"></param>
            private static void CalendarDelete(string ID, CalendarService cal)
            {
                try
                {
                    using (var db = new SchoolEntities())
                    {
                        int course_id = Convert.ToInt32(ID);
                        var courseFullInfo = new CourseModel(course_id);

                        if (courseFullInfo != null && !string.IsNullOrEmpty(courseFullInfo.Course.SUBCATEGORY2c))
                        {
                            var course_cal = (from c in db.Courses
                                              where c.COURSEID == course_id
                                              select c).SingleOrDefault();
                            string calendar_email = "primary";
                            if (!string.IsNullOrEmpty(Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id))
                                calendar_email = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id;
                            string gcal_id = courseFullInfo.Course.SUBCATEGORY2c;
                            string gcal_id_val = gcal_id.Split('_')[1].ToString();

                            //GET THE EVENT
                            var calendarEvent = googleCalendarEventByEventId(calendar_email, gcal_id_val, cal);
                            if (calendarEvent != null)
                            {
                                #region TIMEZONE / TIME HANDLING
                                EventDateTime start = new global::Google.Apis.Calendar.v3.Data.EventDateTime();
                                EventDateTime finish = new global::Google.Apis.Calendar.v3.Data.EventDateTime();

                                TimeZoneInfo localZoneInfo = TimeZoneInfo.Local;
                                int? system_time_zone = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().system_timezone_hour;
                                start.DateTime = system_time_zone == 999 || system_time_zone == 0 ? Convert.ToDateTime(courseFullInfo.CourseStartAsDate) : GoogleBasedDate(Convert.ToDateTime(courseFullInfo.CourseStartAsDate).ToUniversalTime());
                                finish.DateTime = system_time_zone == 999 || system_time_zone == 0 ? Convert.ToDateTime(courseFullInfo.CourseEndAsDate) : GoogleBasedDate(Convert.ToDateTime(courseFullInfo.CourseEndAsDate)).ToUniversalTime();
                                start.TimeZone = AmericanTimeZone(system_time_zone); // IF THE TIMEZONE SETTING IS NONE USE THE CALENDAR TIMEZONE
                                finish.TimeZone = AmericanTimeZone(system_time_zone); // IF THE TIMEZONE SETTING IS NONE USE THE CALENDAR TIMEZONE

                                start.DateTime = calendarEvent.Start.DateTime;
                                finish.DateTime = calendarEvent.End.DateTime;
                                #endregion
                                //GET ALL THE POSSIBLE evets within the range of the date from the course event that was added
                                var listOfCalendarEventsByRange = googleCalendarEventByRange(calendar_email, ID, cal, start.DateTime.Value, finish.DateTime.Value);
                                if (listOfCalendarEventsByRange.Count() == 0)
                                {
                                    cal.Events.Delete(calendar_email, gcal_id_val).Execute();
                                }
                                else
                                {
                                    foreach (var calEvent in listOfCalendarEventsByRange)
                                    {
                                        cal.Events.Delete(calendar_email, calEvent.Id).Execute();
                                    }
                                }
                            }
                            course_cal.SUBCATEGORY2c = ""; //Empty after deleting
                            db.SaveChanges();
                            HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'>Course ID: " + ID + " <font color='red'>Removed</font> on your calendar</h3>");
                        }
                        else
                        {
                            //Force manual delete
                            HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'>Course ID: " + ID + "</h3>");
                            HttpContext.Current.Response.Write("<p style='font-family: monospace;'>This course event is either not on calendar or has used the old data/implementation and would recommend to delete manually on calendar.</p>");
                            HttpContext.Current.Response.Write("<p style='font-style:italic;font-family: monospace;'>Note : We recommend you synch your course on google calendar either by re-updating on Courses_edit page or Google Sych on systemConfig_courseComments page.</p>");
                        }

                    }
                }
                catch (Exception ex)
                {
                    string error_message = string.Empty;
                    if (ex.Source == "Google.Apis")
                    {
                        if (ex.Message.Contains("[404]"))
                        {
                            HttpContext.Current.Response.Write("<h3 style='color: red; font-family: monospace; font-weight:bold;'>Course ID: " + ID + " - Is not found in calendar </h3>");
                        }
                        else
                        {
                            if (ex.InnerException != null)
                            {
                                error_message = ex.InnerException.Message;
                                HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Course ID: " + ID + " - Course event was not deleted due to some internal issue. This error has been sent to the tech team.</h3>");
                            }
                            else
                            {
                                error_message = ex.Message;
                                HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Course ID: " + ID + " - Course event was not deleted due to some internal issue. This error has been sent to the tech team.</h3>");
                            }
                        }
                    }
                    else
                    {
                        if (ex.InnerException != null)
                        {
                            error_message = ex.InnerException.Message;
                            HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Course ID: " + ID + " - Course event was not deleted due to some internal issue. This error has been sent to the tech team.</h3>");
                        }
                        else
                        {
                            error_message = ex.Message;
                            HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;color:red;'>Course ID: " + ID + " - Course event was not deleted due to some internal issue. This error has been sent to the tech team.</h3>");
                        }
                    }
                    HttpContext.Current.Response.Write("<script type='text/javascript'> setTimeout(function(){ window.close(); }, 3000);</script>");
                    //@todo : create a function for this
                    string fullurl = HttpContext.Current.Request.Url.AbsoluteUri;
                    //EmailAuditTrail emailAuditrail = new EmailAuditTrail();
                    //emailAuditrail.EmailTo = "vincentb@gosignmeup.com";
                    //emailAuditrail.EmailSubject = "ERROR OCCURED";
                    //emailAuditrail.EmailBody = "Error Stack : " + error_message + "\n" + "URL : " + fullurl;
                    //emailAuditrail.AuditDate = DateTime.Now;
                    //emailAuditrail.AuditProcess = "Google Calendar Sync";
                    //EmailFunction.SendEmail(emailAuditrail);
                }

            }
            /// <summary>
            /// Clear All Calendar Data
            /// </summary>
            /// <param name="cal"></param>
            private static void CalendarClear(CalendarService cal)
            {
                try
                {
                    cal.Calendars.Clear("primary").Execute();
                    //UPDATE the CALENDAR ID field to empty
                    using (var db = new SchoolEntities())
                    {
                        var courses = (from c in db.Courses
                                       select c);
                        if (courses.Count() > 0)
                        {
                            courses.ToList().ForEach(c => c.SUBCATEGORY2c = "");
                            db.SaveChanges();
                        }
                    }
                    HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'><font color='red'>Deleted</font> all events in your calendar</h3>");
                }
                catch (Exception ex)
                {
                    string error_message = string.Empty;
                    error_message = ex.Message;
                    HttpContext.Current.Response.Write("<h3 style='color: red; font-family: monospace; font-weight:bold;'>Calendar clearing failed because of some internal issue. This error has been sent to the tech team.</h3>");
                    //@todo : create a function for this
                    string fullurl = HttpContext.Current.Request.Url.AbsoluteUri;
                    //EmailAuditTrail emailAuditrail = new EmailAuditTrail();
                    //emailAuditrail.EmailTo = "vincentb@gosignmeup.com";
                    //emailAuditrail.EmailSubject = "ERROR OCCURED";
                    //emailAuditrail.EmailBody = "Error Stack : " + error_message + "\n" + "URL : " + fullurl;
                    //emailAuditrail.AuditDate = DateTime.Now;
                    //emailAuditrail.AuditProcess = "Google Calendar Sync";
                    //EmailFunction.SendEmail(emailAuditrail);
                }
            }
            /// <summary>
            /// Delete All Calendar Events
            /// </summary>
            /// <param name="cal"></param>
            private static void CalendarDeleteAllEvents(CalendarService cal)
            {
                try
                {

                    Events events = cal.Events.List("primary").Execute();
                    List<Event> items = events.Items.ToList();
                    foreach (Event item in items)
                    {
                        cal.Events.Delete("primary", item.Id).Execute();
                    }
                    //UPDATE the CALENDAR ID field to empty
                    using (var db = new SchoolEntities())
                    {
                        var courses = (from c in db.Courses
                                       select c);
                        if (courses.Count() > 0)
                        {
                            courses.ToList().ForEach(c => c.SUBCATEGORY2c = "");
                            db.SaveChanges();
                        }
                    }
                    HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'><font color='red'>Deleted</font> all events in your calendar</h3>");
                }
                catch (Exception ex)
                {
                    string error_message = ex.Message;
                    HttpContext.Current.Response.Write("<h3 style='color: red; font-family: monospace; font-weight:bold;'>Deleting of all events failed because of some internal issue . This error has been sent to the tech team.</h3>");
                    //@todo : create a function for this
                    string fullurl = HttpContext.Current.Request.Url.AbsoluteUri;
                    //EmailAuditTrail emailAuditrail = new EmailAuditTrail();
                    //emailAuditrail.EmailTo = "vincentb@gosignmeup.com";
                    //emailAuditrail.EmailSubject = "ERROR OCCURED";
                    //emailAuditrail.EmailBody = "Error Stack : " + error_message + "\n" + "URL : " + fullurl;
                    //emailAuditrail.AuditDate = DateTime.Now;
                    //emailAuditrail.AuditProcess = "Google Calendar Sync";
                    //EmailFunction.SendEmail(emailAuditrail);
                }
            }
            /// <summary>
            /// executes authentication by google plus
            /// </summary>
            public static void GoogleAuthenticate(ServiceAccountCredential serviceAcccountCredential, System.Web.Mvc.ControllerBase controller)
        {
            var plusService = new PlusService(new BaseClientService.Initializer()
            {
                ApplicationName = "GSMUGOOGLE+",
                HttpClientInitializer = serviceAcccountCredential
            });
            //Authentication
            //Execute the Profile Saving if existed
            var profile = plusService.People.Get("me").Execute();
            var userId = GoogleHelper.GetGoogleUserId(profile.Id);
            var student = StudentHelper.GetStudent(userId);
            if (student == null)
            {
                var firstName = profile.Name.GivenName;
                var lastName = profile.Name.FamilyName;
                var googleEmail = profile.Emails.FirstOrDefault().Value.ToString();
                var password = GoogleHelper.GetGooglePassword(userId);

                student = new Student();
                student.FIRST = firstName;
                student.LAST = lastName;
                student.EMAIL = googleEmail;
                student.USERNAME = userId;
                student.STUDNUM = password;
                student.google_user = 1;

                StudentHelper.RegisterStudent(student);
            }
            Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "Google login successful.");
            var messages = AuthorizationHelper.LoginStudent(userId);
            Gsmu.Api.Web.ObjectHelper.AddRequestMessages(controller, messages);
        }
        #endregion

 
 
        public static void GoogleCalV3Sync(UserCredential serviceAcccountCredential, int mode, int courseId, DateTime Begin, DateTime End)
        {
            using (var db = new SchoolEntities())
            {
                var coursesWithCourseTimeRelevance = (from courses in db.Courses
                                                      join courseTime in db.Course_Times on courses.COURSEID equals courseTime.COURSEID
                                                      where (courseTime.COURSEDATE != null) &&
                                                      (
                                                        mode == 0 ? courses.COURSEID == courseId :
                                                        courseTime.COURSEDATE >= Begin && courseTime.COURSEDATE <= End
                                                      )
                                                      select new { courses, courseTime })
                                                      .Distinct()
                                                      .OrderBy(ct => ct.courseTime.COURSEDATE)
                                                      .Distinct().ToList();
                if (coursesWithCourseTimeRelevance.Count() > 0)
                {
                    //only authenticate when there's data that can be synced
                    var calendarService = new CalendarService(new BaseClientService.Initializer()
                    {
                        ApplicationName = "GoogleService",
                        HttpClientInitializer = serviceAcccountCredential
                    });

                    if (mode == 0) //single
                    {
                        //if (HttpContext.Current.Session["courseid"].ToString().Contains("_"))
                        //{
                        if (HttpContext.Current.Session["courseid"].ToString().Contains("_") && HttpContext.Current.Session["courseid"].ToString().Split('_')[1].ToString() == "delete")
                        {
                            CalendarDelete(courseId.ToString(), calendarService);
                        }
                        else
                        {
                            //IF COURSE ONLINE ONLY SYNC ONCE ELSE SYNC WITH THE MULTIPLE DATES
                            var syncCourse = coursesWithCourseTimeRelevance.FirstOrDefault();
                            bool isOnline = syncCourse.courses.IsOnlineCourse;
                            if (isOnline)
                            {
                                GoogleCalendarSyncProcessContent(calendarService, syncCourse.courses, null);
                            }
                            else
                            {
                                int dateCount = coursesWithCourseTimeRelevance.Count();
                                foreach (var course in coursesWithCourseTimeRelevance)
                                {
                                    GCalEventDateModel dateModel = new GCalEventDateModel()
                                    {
                                        StartDate = course.courseTime.COURSEDATE.Value,
                                        EndDate = course.courseTime.COURSEDATE.Value,
                                        StartTime = course.courseTime.STARTTIME.Value,
                                        EndTime = course.courseTime.FINISHTIME.Value
                                    };
                                    GoogleCalendarSyncProcessContent(calendarService, course.courses, dateModel, dateCount);
                                }
                            }
                        }
                        //}
                    }
                    else
                    {
                        foreach (var course in coursesWithCourseTimeRelevance)
                        {
                            GoogleCalendarSyncProcessContent(calendarService, course.courses, null);
                        }
                    }
                }
            }

        }


        #region SUB ACTIONS
        /// <summary>
        /// Handles the Google Calendar syncing, on what type or mode will it be
        /// whether it's in a single type or in a multiple type or deletion
        /// </summary>
        /// <param name="serviceAcccountCredential"></param>
        /// <param name="mode"></param>
        /// <param name="courseId"></param>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        public static void GoogleCalendarSyncHandler(ServiceAccountCredential serviceAcccountCredential, int mode, int courseId, DateTime Begin, DateTime End)
            {
                using (var db = new SchoolEntities())
                {
                    var coursesWithCourseTimeRelevance = (from courses in db.Courses
                                                          join courseTime in db.Course_Times on courses.COURSEID equals courseTime.COURSEID
                                                          where (courseTime.COURSEDATE != null) &&
                                                          (
                                                            mode == 0 ? courses.COURSEID == courseId : 
                                                            courseTime.COURSEDATE >= Begin && courseTime.COURSEDATE <= End
                                                          )
                                                          select new { courses, courseTime })
                                                          .Distinct()
                                                          .OrderBy(ct => ct.courseTime.COURSEDATE)
                                                          .Distinct().ToList();
                    if (coursesWithCourseTimeRelevance.Count() > 0)
                    {
                        //only authenticate when there's data that can be synced
                        var calendarService = new CalendarService(new BaseClientService.Initializer()
                        {
                            ApplicationName = "GoogleService",
                            HttpClientInitializer = serviceAcccountCredential
                        });

                        if (mode == 0) //single
                        {
                            //if (HttpContext.Current.Session["courseid"].ToString().Contains("_"))
                            //{
                                if (HttpContext.Current.Session["courseid"].ToString().Contains("_") && HttpContext.Current.Session["courseid"].ToString().Split('_')[1].ToString() == "delete")
                                {
                                    CalendarDelete(courseId.ToString(), calendarService);
                                }
                                else
                                {
                                    //IF COURSE ONLINE ONLY SYNC ONCE ELSE SYNC WITH THE MULTIPLE DATES
                                    var syncCourse = coursesWithCourseTimeRelevance.FirstOrDefault();
                                    bool isOnline = syncCourse.courses.IsOnlineCourse;
                                    if (isOnline)
                                    {
                                        GoogleCalendarSyncProcessContent(calendarService, syncCourse.courses, null);
                                    }
                                    else
                                    {
                                        int dateCount = coursesWithCourseTimeRelevance.Count();
                                        foreach (var course in coursesWithCourseTimeRelevance)
                                        {
                                            GCalEventDateModel dateModel = new GCalEventDateModel()
                                            {
                                                StartDate = course.courseTime.COURSEDATE.Value,
                                                EndDate = course.courseTime.COURSEDATE.Value,
                                                StartTime = course.courseTime.STARTTIME.Value,
                                                EndTime = course.courseTime.FINISHTIME.Value
                                            };
                                            GoogleCalendarSyncProcessContent(calendarService, course.courses, dateModel, dateCount);
                                        }
                                    }
                                }
                            //}
                        }
                        else
                        {
                            foreach (var course in coursesWithCourseTimeRelevance)
                            {
                                GoogleCalendarSyncProcessContent(calendarService, course.courses, null);
                            }
                        }
                    }
                }

            }
            /// <summary>
        /// Executes single google syncing and multiple syncing
        /// </summary>
        /// <param name="googleCalendarService"></param>
        /// <param name="courseId"></param>
            public static void GoogleCalendarSyncProcessContent(CalendarService calendarService, Course course, GCalEventDateModel dateModel, int dateCount = 0)
            {
                string courseDescription = string.Empty;
                string startTime = string.Empty;
                string endTime = string.Empty;
                string courseName = string.Empty;

                courseDescription = GoogleCalendarDetails(course.COURSEID, out startTime, out endTime, out courseName);

                if (dateModel != null)
                {
                    //Override the start/end time if the dateModel is not null
                    startTime = dateModel.StartDate.ToString("yyyy-MM-dd") + " " + dateModel.StartTime.ToString("HH:mm:ss");
                    endTime = dateModel.EndDate.ToString("yyyy-MM-dd") + " " + dateModel.EndTime.ToString("HH:mm:ss");
                }

                CalendarSynch(startTime, endTime, courseName, course.COURSEID.ToString(), courseDescription, calendarService, dateCount);
            }
        #endregion

        #region PROPERTY HANDLERS
            public static string ExtractBetween(string text, string start, string end)
            {
                int iStart = text.IndexOf(start);
                iStart = (iStart == -1) ? 0 : iStart + start.Length;
                int iEnd = text.LastIndexOf(end);
                if (iEnd == -1)
                {
                    iEnd = text.Length;
                }
                int len = iEnd - iStart;

                return text.Substring(iStart, len);
            }
            /// <summary>
            /// This is also present in V3 google/google.asp please if you change here makes sure
            /// to change there so you do not break the login functions.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            private static string GetGooglePassword(string id)
            {
                return "g-" + id + "-secret>>>931-t@@l";
            }
            /// <summary>
            /// This is also present in V3 google/google.asp please if you change here makes sure
            /// to change there so you do not break the login functions.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            private static string GetGoogleUserId(string id)
            {
                return "g-" + id;
            }
            /// <summary>
            /// Get American Timezone
            /// </summary>
            /// <param name="system_date_timezone"></param>
            /// <returns></returns>
            public static string AmericanTimeZone(int? system_date_timezone)
            {
                switch (system_date_timezone)
                {
                    case 0: return "America/Los_Angeles"; // Pacific Standard Time
                    case 1: return "America/Denver"; // Mountain Standard Time 
                    case 2: return "America/Chicago"; // Central Standard Time
                    case 3: return "America/New_York"; // Eastern Standard Time
                    default: return "America/Los_Angeles"; // Pacific Standard Time
                };
            }
            /// <summary>
            /// Get google base date - from masterinfo
            /// </summary>
            /// <param name="dateTime"></param>
            /// <returns></returns>
            public static DateTime GoogleBasedDate(DateTime dateTime)
            {
                int? system_time_zone = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().system_timezone_hour;
                int timezonesT = 0;
                switch (system_time_zone)
                {
                    case 0:
                        timezonesT = 0; // -8
                        break;
                    case 1:
                        timezonesT = -1; //-7
                        break;
                    case 2:
                        timezonesT = -2; //-6
                        break;
                    case 3:
                        timezonesT = -3; //-5
                        break;
                    default:
                        timezonesT = 0; // -8
                        break;
                };
                return dateTime.AddHours(timezonesT);
            }
            /// <summary>
            /// Returns the description of the course 
            /// </summary>
            /// <param name="couresId"></param>
            /// <returns></returns>
            public static string GoogleCalendarDetails(int couresId, out string start_time, out string end_time, out string course_name)
            {
                using (var db = new SchoolEntities())
                {
                    string link = string.Empty;
                    string instructor = string.Empty;
                    string location = string.Empty;
                    string location_additional_info = string.Empty;
                    string final_location = string.Empty;
                    string description = string.Empty;
                    string coursename = string.Empty;
                    string start_time_out = string.Empty;
                    string end_time_out = string.Empty;
                    string course_name_out = string.Empty;

                    var courseFullInfo = new CourseModel(couresId);
                    var courseInfo = courseFullInfo.Course;
                    var courseInstructors = courseFullInfo.Instructors.ToList();
                    var courseDateFrom = courseFullInfo.CourseStartAsDate;
                    var courseDateTo = courseFullInfo.CourseEndAsDate;
                    var courseDateFromTime = courseFullInfo.CourseStart.STARTTIME;
                    var courseDateToTime = courseFullInfo.CourseStart.FINISHTIME;

                    coursename = courseInfo.IsCancelled ? courseInfo.COURSENAME + "(Cancelled)" : courseInfo.COURSENAME;
                    final_location = courseInfo.LOCATION + ", " + courseInfo.LocationAdditionalInfo;
                    link = "<a href='" +
                    Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl != "" ?
                    Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl + "public/course/browse?courseid=" + couresId :
                    Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().AspSiteRootUrl
                    + "dev_students.asp?action=coursedetail&id=" + couresId + "'>Click Here for Details</a><br /><br />";

                    foreach (var instructors in courseInstructors)
                    {
                        instructor += instructors.FIRST + " " + instructors.LAST + ", ";
                    }
                    //out string parameters
                    start_time = courseDateFrom.Value.Date.ToString("yyyy-MM-dd") + " " + courseDateFromTime.Value.ToString("HH:mm:ss");
                    end_time = courseDateTo.Value.Date.ToString("yyyy-MM-dd") + " " + courseDateToTime.Value.ToString("HH:mm:ss");
                    course_name = coursename;

                    description = link + "<br />" + instructor + "<br />" + final_location;
                    return description;
                }
            }

            /// <summary>
            /// Returns the Calendar Entry from the Calendar List which was set from School's Database
            /// from masterinfo3 google_sso_client_id
            /// returns null if calendar list entry did not found the calendar id set from google_sso_client_id
            /// </summary>
            /// <param name="calendarList"></param>
            /// <returns></returns>
            public static CalendarListEntry calendarListEntryFromSettings(List<CalendarListEntry> calendarList)
            {
                foreach (CalendarListEntry calendarListEntry in calendarList)
                {
                    if (calendarListEntry.Id.ToLower() == Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().google_sso_client_id)
                    {
                        return calendarListEntry;
                    }
                }
                return null;
            }
            /// <summary>
            /// Returns boolean value if the calendar id set from Database is owned or has writer role by the Service Account
            /// </summary>
            /// <param name="calendarListEntry"></param>
            /// <returns>Boolean</returns>
            public static bool googleAccountIsWriter(CalendarListEntry calendarListEntry)
            {
                bool isWriter = false;
                isWriter = calendarListEntry.AccessRole == "writer" ? true : false;
                if (isWriter)
                {
                    return isWriter;
                }
                return isWriter;
            }
            /// <summary>
            /// Gets Google Calendar Event By Event ID
            /// </summary>
            /// <param name="calendar_email"></param>
            /// <param name="eventId"></param>
            /// <param name="cal"></param>
            /// <returns></returns>
            public static Event googleCalendarEventByEventId(string calendar_email, string eventId, CalendarService cal)
            {
                return cal.Events.Get(calendar_email, eventId).Execute();
            }
            /// <summary>
            /// Gets Google Calendar List by Date Range
            /// </summary>
            /// <param name="calendar_email"></param>
            /// <param name="eventId"></param>
            /// <param name="cal"></param>
            /// <returns></returns>
            public static List<Event> googleCalendarEventByRange(string calendar_email, string courseId, CalendarService cal, DateTime start, DateTime finish)
            {
                var calendar = cal.Events.List(calendar_email);
                calendar.TimeMin = start;
                calendar.TimeMax = finish;
                var calendarList = calendar.Execute().Items.ToList();
                return calendarList.Where(t => !string.IsNullOrEmpty(t.Description) && (t.Description.Contains(courseId) || t.Description.IndexOf(courseId) > -1)).ToList();
            }
        #endregion

        #region UTILITIES
            /// <summary>
            /// Cleans up all the repeating calendar id on the course database.
            /// </summary>
            public static void CourseGoogleCalIDCleanUp(bool logToView)
        {
            using (var db = new SchoolEntities())
            {
                string query = @";WITH cte AS
                                (
                                SELECT
                                      ROW_NUMBER() OVER(PARTITION BY SUBCATEGORY2c  ORDER BY SUBCATEGORY2c ) AS rno,
                                      * 
                                  FROM Courses
                                  WHERE SUBCATEGORY2c <> ''
                                ) SELECT * FROM cte WHERE rno <> 1";
                var resultData = db.Courses.SqlQuery(query).ToList();
                //logs the courses on webview
                if (logToView)
                {
                    foreach (var courseLog in resultData)
                    {
                        courseLog.SUBCATEGORY2c = "";
                        db.SaveChanges();
                        HttpContext.Current.Response.Write("<h3 style='padding:5px; border-bottom:1px solid #ddd; font-family: monospace; font-weight:bold;'>Course ID: " + courseLog.COURSEID + " is found to have a duplicate GCal Event ID. This is <font color='green'>Corrected</font> on your database now.</h3>");
                    }
                }
                //    }
                //}
            }
        }
        #endregion

        #region MODELS
            public class GCalEventDateModel {
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public DateTime StartTime { get; set; }
                public DateTime EndTime { get; set; }
            }
        #endregion
    }
}