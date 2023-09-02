using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Data;
using Gsmu.Api.Data.ViewModels.Grid;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.Memberships;
using Gsmu.Api.Data.ViewModels.Layout;
using System.Web.Script.Serialization;
using Gsmu.Api.Networking.Mail;
using System.IO;
using Gsmu.Api.Data.School.Entities;
using System.Text;
using System.Configuration;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Data.School.Transcripts;

using Gsmu.Api.Export.StudentTranscript;
using Gsmu.Api.Authorization;
using Gsmu.Api.Commerce;
using System.Dynamic;
using BlackBoardAPI;
using static BlackBoardAPI.BlackBoardAPIModel;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class CourseController : Controller
    {
        public ActionResult Index()
        {

            return RedirectToAction("Browse");
        }

        public ActionResult BrowseInternal(ViewTemplateType view, DisplayMode displayMode = DisplayMode.Normal)
        {
            return Browse(view, displayMode);
        }
        public ActionResult Layout(string layoutName = null)
        {
            if (Gsmu.Api.Data.WebConfiguration.CourseSearchSingleView)
            {
                return new HttpNotFoundResult();
            }

            if (string.IsNullOrEmpty(layoutName))
            {
                throw new Exception("Layout not specified as the last item in route: /course/layout/{layoutName}");
            }

            var layout = LayoutManager.GetLayoutById(layoutName);

            if (string.IsNullOrEmpty(layout))
            {
                throw new Exception(string.Format(
                    "Invalid layout: {0}", layoutName
                    ));
            }

            ViewBag.LayoutSettings = layout;
            return View("BrowseComposite");
        }

        /// <summary>
        /// Left category template.
        /// </summary>
        /// <returns></returns>
        [ValidateInput(true)]
        public ActionResult Browse(ViewTemplateType view, DisplayMode displayMode)
        {

            var a = Request.QueryString;
            try
            {
                CourseShoppingCart.Instance.SelectedCourseID = int.Parse(Request.QueryString["courseid"]);
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn == false)
                {
                    if (Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled > 0)
                    {
                        if (Settings.Instance.GetMasterInfo4().shibboleth_required_login > 0)
                        {
                            return Redirect("~");
                        }
                    }
                }
                else
                {
                    CourseShoppingCart.Instance.SelectedCourseID = null;
                }
            }
            catch {
            }
            if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn == false)
            {
                if (Settings.Instance.GetPDFHeaderFooterInfo().Forcelogin == 1 || Settings.Instance.GetPDFHeaderFooterInfo().Forcelogin == 2)
                {
                    string WelcomePage = System.Configuration.ConfigurationManager.AppSettings["WelcomePage"];
                    string curReferer = Request.Headers["Referer"];
                    if (!string.IsNullOrEmpty(WelcomePage) && !curReferer.ToLower().Contains(WelcomePage.ToLower()))
                    {
                        return Redirect("/" + WelcomePage);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Membership", new { returnUrl = Request.Url.AbsoluteUri });
                    }
                }
            }
            else
            {
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                {
                    if (CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent <= 0)
                    {
                        return RedirectToAction("Index", "Supervisor", new { returnUrl = Request.Url.AbsoluteUri });
                    }
                }

                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser != null)
                {
                    if (CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent <= 0)
                    {
                        return RedirectToAction("Index", "instructor", new { returnUrl = Request.Url.AbsoluteUri });
                    }
                }
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.LoggedInUserType == Gsmu.Api.Authorization.LoggedInUserType.SubAdmin)
                {
                    if ((Request.UrlReferrer == null) || Request.UrlReferrer.ToString() == "")
                    {
                        if (!Request.Url.AbsoluteUri.Contains("localhost")) // for debugging purposes
                        {
                            AuthorizationHelper.Logout();
                        }
                    }
                }
            }
            if (Gsmu.Api.Data.WebConfiguration.CourseSearchSingleView && view != Gsmu.Api.Data.WebConfiguration.DefaultCourseSearchView)
            {
                return new HttpNotFoundResult();
            }
            ViewBag.GoogleAnalyticsSetUpScript = Settings.Instance.GetMasterInfo3().GoogleTracker_code;
            ViewBag.BGColorInfolist = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.BGColorInfos();
            ViewBag.DisplayMode = displayMode;
            string UserLoginAutoPopup = System.Configuration.ConfigurationManager.AppSettings["UserLoginAutoPopup"];
            if (!string.IsNullOrEmpty(UserLoginAutoPopup))
            {
                if (UserLoginAutoPopup.ToLower() == "true")
                {
                    ViewBag.AutoPopUplogin = "true";
                }
                else
                {
                    ViewBag.AutoPopUplogin = "false";
                }
            }
            else
            {
                ViewBag.AutoPopUplogin = "false";
            }
            switch (view)
            {
                case ViewTemplateType.LeftCategory:
                    return View("BrowseLeftCategory");

                case ViewTemplateType.AllLeft:
                    return View("BrowseAllLeft");

                case ViewTemplateType.Composite:
                    return View("BrowseComposite");

                default:
                    throw new Exception(
                        string.Format("Invalid view template: {0}", view)
                    );
            }
        }


        /// <summary>
        /// Grid list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderByField"></param>
        /// <param name="orderByDirection"></param>
        /// <param name="text"></param>
        /// <param name="mainCategory"></param>
        /// <param name="subCategory"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult List(
            int page = 1,
            int? pageSize = null,
            CourseOrderByField orderByField = CourseOrderByField.SystemDefault,
            OrderByDirection orderByDirection = OrderByDirection.Ascending,
            string text = null,
            string mainCategory = null,
            string subCategory = null,
            string subsubcattext = null,
            bool subCategoryIsSubSub = false,
            CourseActiveState state = CourseActiveState.Current,
            ListingType? view = null,
            bool courseInternal = false,
            DateTime? from = null,
            DateTime? until = null,
            int CoursePopout = 0,
            CourseCancelState cancelState = CourseCancelState.NotCancelled,
            bool showinternal = false,
            bool showclosedpast = false,
            bool showmembership = false

            )
        {

            if (view == null)
            {
                view = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.PublicLayoutConfiguration.SearchColumns.DefaultView;
            }

            string path = Request.RawUrl;
            if (path.ToLower().Contains("browseinternal"))
            {
                courseInternal = true;
            }
            if (!Request.IsAjaxRequest() && Request["viewstate"] != null && Request["viewstate"].Length > 5 && (Request["AllowDirectLoad"] != null || WebConfiguration.DevelopmentMode))
            {
                // {"ViewListType":"TileJuly","Page":3,"PageSize":25,"OrderByField":"SystemDefault","OrderByDirection":"Ascending","CourseActiveState":"All","MainCategory":"Biology","SubCategory":"Zoology","SubCategoryIsSubSub":false,"Text":"","DateFrom":null,"DateUntil":null,"CancelState":"NotCancelled","CoursePopout":0,"OpenMainCategories":{"0":true},"ActiveCourseCategoryIndex":1,"ActiveCourseCategoryIsSubSub":false}

                string decodedString;
                try
                {
                    byte[] data = Convert.FromBase64String(Request["viewstate"]);
                    decodedString = Encoding.UTF8.GetString(data);

                }
                catch (FormatException)
                {
                    decodedString = Request["viewstate"];
                }

                dynamic viewstate = Newtonsoft.Json.JsonConvert.DeserializeObject(decodedString);
                if (string.IsNullOrEmpty((string)viewstate.DateFrom))
                {
                    viewstate.DateFrom = null;
                }
                if (string.IsNullOrEmpty((string)viewstate.DateUntil))
                {
                    viewstate.DateUntil = null;
                }
                page = viewstate.Page == null ? page : (int)viewstate.Page;
                pageSize = viewstate.PageSize == null ? pageSize : (int)viewstate.PageSize;
                orderByField = viewstate.OrderByField == null ? orderByField : (CourseOrderByField)Enum.Parse(typeof(CourseOrderByField), (string)viewstate.OrderByField);
                orderByDirection = viewstate.OrderByDirection == null ? orderByDirection : (OrderByDirection)Enum.Parse(typeof(OrderByDirection), (string)viewstate.OrderByDirection);
                text = viewstate.Text == null ? text : (string)viewstate.Text;
                mainCategory = viewstate.MainCategory == null ? mainCategory : (string)viewstate.MainCategory;
                subCategory = viewstate.SubCategory == null ? subCategory : (string)viewstate.SubCategory;
                subsubcattext = viewstate.subsubcattext == null ? subCategory : (string)viewstate.subsubcattext;
                try
                {
                    subCategoryIsSubSub = viewstate.SubCategoryIsSubSub == null ? subCategoryIsSubSub : (bool)viewstate.SubCategoryIsSubSub;
                }
                catch
                {
                }
                state = viewstate.CourseActiveState == null ? state : (CourseActiveState)Enum.Parse(typeof(CourseActiveState), (string)viewstate.CourseActiveState);
                view = viewstate.ViewListType == null ? view : (ListingType)Enum.Parse(typeof(ListingType), (string)viewstate.ViewListType);
                courseInternal = viewstate.CourseInternal == null ? courseInternal : (bool)viewstate.CourseInternal;
                from = viewstate.DateFrom == null ? from : DateTime.Parse((string)viewstate.DateFrom);
                until = viewstate.DateUntil == null ? until : DateTime.Parse((string)viewstate.DateUntil);
                CoursePopout = viewstate.CoursePopout == null ? CoursePopout : (int)viewstate.CoursePopout;
                cancelState = viewstate.CancelState == null ? cancelState : (CourseCancelState)Enum.Parse(typeof(CourseCancelState), (string)viewstate.CancelState);

            } else if (
                ((!Request.IsAjaxRequest() || ControllerContext.IsChildAction) && !string.IsNullOrWhiteSpace(Settings.Instance.GetMasterInfo2().PublicAnnouncement2)) || view == ListingType.Anncmnt)
            {
                return PartialView("PublicAnnouncement");
            }

            if (pageSize == null)
            {
                pageSize = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().DefaultPaginationCourses.HasValue ? Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().DefaultPaginationCourses : 12;
            }

            var queryState = new QueryState()
            {
                Page = page,
                PageSize = pageSize.Value,
                OrderField = orderByField,
                OrderByDirection = orderByDirection
            };

            CourseInternalState internalState = CourseInternalState.Public;
            if (courseInternal)
            {
                internalState = InternalCourseSettings.InternalCourseResultTypes;
                InternalCourseSetUp();
            }
            if (!WebConfiguration.DevelopmentMode)
            {
                state = CourseActiveState.Current;
                cancelState = CourseCancelState.NotCancelled;
            }
            var courses = new GridModel<CourseModel>(0, queryState);
            string UseNewPublicDatabaseView = System.Configuration.ConfigurationManager.AppSettings["UseNewPublicDatabaseView"];
            if (from != null && until == null) { var nw = DateTime.Now; until = nw.AddYears(10); } // until date cannot be empty if from date has value
            if (!string.IsNullOrEmpty(UseNewPublicDatabaseView))
            {
                if (bool.Parse(UseNewPublicDatabaseView) == true)
                {
                    courses = Queries.NewSearchFromView(queryState, text, mainCategory, subCategory, subsubcattext, subCategoryIsSubSub, state, from, until, internalState, cancelState, showinternal, showclosedpast, showmembership);
                }
                else
                {
                    courses = Queries.Search(queryState, text, mainCategory, subCategory, subsubcattext, subCategoryIsSubSub, state, from, until, internalState, cancelState);
                }
            }
            else
            {
                courses = Queries.Search(queryState, text, mainCategory, subCategory, subsubcattext, subCategoryIsSubSub, state, from, until, internalState, cancelState);
            }


            courses.PagerCallbackTemplate = "window.CourseSearchInstance.Paginate({0},{1});";
            courses.SorterCallbackTemplate = "window.CourseSearchInstance.Sort('{0}');";
            ViewBag.CoursePopout = CoursePopout;
            ViewBag.HideSeatsAvailable = Settings.Instance.GetMasterInfo2().HideSeatsAvailable;
            //Start for Color coding and Icons
            CourseIconsandLegend CourseIconsandLegend = new CourseIconsandLegend();
            var colorcode = CourseIconsandLegend.GetGroupingsColor();
            var icons = CourseIconsandLegend.GetIcons();
            ViewBag.ColorCode = colorcode;
            ViewBag.Icons = icons;
            ViewBag.EnabledColor = Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo2().EnableCourseColors);

            //end for icons
            ActionResult result = null;

            if (view == ListingType.Grid && !LayoutManager.PublicLayoutConfiguration.SearchColumns.GridViewEnabled)
            {
                view = ListingType.TileJuly;
            }
            else if (view == ListingType.TileJuly && !LayoutManager.PublicLayoutConfiguration.SearchColumns.TileJulyViewEnabled)
            {
                view = ListingType.Grid;
            }

            switch (view)
            {
                case ListingType.Grid:
                    result = PartialView("GridList", courses);
                    break;

                case ListingType.TileJuly:
                    result = PartialView("TileListJuly", courses);
                    break;

                default:
                    throw new NotImplementedException(view.ToString());

            }

            return result;
        }


        [System.Web.Http.HttpPost]
        public JsonResult GetCourseList(int courseId, bool showPast = false, string keyword = "")
        {
            string HideCourseNameOnPreReqList = System.Configuration.ConfigurationManager.AppSettings["HideCourseNameOnPreReqList"];

            //DEFAULT querystate
            var queryState = new QueryState()
            {
                Page = 1,
                PageSize = 100,
                OrderField = CourseOrderByField.SystemDefault,
                OrderByDirection = OrderByDirection.Ascending
            };

            var courses = new GridModel<CourseModel>(0, queryState);
            string UseNewPublicDatabaseView = System.Configuration.ConfigurationManager.AppSettings["UseNewPublicDatabaseView"];
            if (!string.IsNullOrEmpty(UseNewPublicDatabaseView))
            {
                if (bool.Parse(UseNewPublicDatabaseView) == true)
                {
                    if (showPast) {
                        courses = Queries.NewSearchFromView(queryState, keyword, "", "", "", false, CourseActiveState.Past, null, null,
                            CourseInternalState.InternalAndPublic, CourseCancelState.NotCancelled, false, true); // current state is past and show closed passed is true
                    }
                    else {
                        courses = Queries.NewSearchFromView(queryState, keyword, "", "", "", false, CourseActiveState.Current, DateTime.Now, null);
                    }

                }
                else
                {
                    courses = Queries.Search(queryState, keyword, "", "", "", false, showPast ? CourseActiveState.Past : CourseActiveState.Current, DateTime.Now, null);
                }
            }
            else
            {
                courses = Queries.Search(queryState, keyword, "", "", "", false, showPast ? CourseActiveState.Past : CourseActiveState.Current, DateTime.Now, null);
            }

            var result = new JsonResult();
            var coursePreReqData = Course.CoursePreRequisites(courseId).ToList();
            var currentCourses = (from c in courses.Result.ToList()
                                  where c.CourseId != courseId
                                  select new
                                  {
                                      CourseId = c.CourseId,
                                      CourseNumber = c.Course.COURSENUM,
                                      CourseName = c.Course.COURSENAME
                                  })
                             .ToList();

            HashSet<int> diffCourseIds = new HashSet<int>(coursePreReqData.Select(c => c.CoursePreReqId));
            var finalData = currentCourses.Where(c => !diffCourseIds.Contains(c.CourseId)).ToList();

            if (HideCourseNameOnPreReqList == "true")
            {
                if (keyword.Length < 3)
                {
                    result.Data = new { data = new List<newCourseModel>() };
                    return result;
                }
                finalData = finalData.Where(c => c.CourseNumber.ToLower().Contains(keyword.ToLower())).ToList();
                var grps = finalData.GroupBy(x => x.CourseNumber); // group by state
                var nwdta = new List<newCourseModel>();
                foreach (var grp in grps)
                {
                    var fdta = finalData.FirstOrDefault(c => c.CourseNumber == grp.Key);
                    nwdta.Add(new newCourseModel()
                    {
                        CourseId = fdta.CourseId,
                        CourseNumber = grp.Key,
                        CourseName = ""
                    });
                }
                result.Data = new { data = nwdta };
            }
            else
            {
                result.Data = new { data = finalData };
            }
            return result;
        }

        public class newCourseModel
        {
            public int CourseId { get; set; }
            public string CourseNumber { get; set; }
            public string CourseName { get; set; }
    }

        [System.Web.Http.HttpPost]
        public JsonResult SaveCoursePreReq(int courseId, string preReqIds, string process = "")
        {
            if (process == "delete")
            {
                bool success = Course.DeleteCoursePreRequisites(courseId, preReqIds);
                return new JsonResult()
                {
                    Data = new { Success = success }
                };
            }
            else
            {
                bool success = Course.SaveCoursePreRequisites(courseId, preReqIds);
                return new JsonResult()
                {
                    Data = new { Success = success }
                };
            }
        }

        [System.Web.Http.HttpPost]
        public JsonResult GetCoursePrerequisites(int courseId)
        {
            var result = new JsonResult();

            if (AuthorizationHelper.CurrentUser.IsLoggedIn && AuthorizationHelper.CurrentStudentUser != null)
            {
                int studentId = AuthorizationHelper.CurrentStudentUser.STUDENTID;
                var coursePreReqData = Course.GetCoursePrerequisites(courseId, studentId).ToList();
                result.Data = new { data = coursePreReqData, isValid = Course.IsStudentValidForCourseByPreReq(courseId, studentId) };
            }
            else if (AuthorizationHelper.CurrentUser.IsLoggedIn && AuthorizationHelper.CurrentSupervisorUser != null)
            {
                int studentId = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                if (studentId > 0)
                {
                    var coursePreReqData = Course.GetCoursePrerequisites(courseId, studentId).ToList();
                    result.Data = new { data = coursePreReqData, isValid = Course.IsStudentValidForCourseByPreReq(courseId, studentId) };
                }
                else
                {
                    var coursePreReqData = Course.GetCoursePrerequisites(courseId).ToList();
                    result.Data = new { data = coursePreReqData };
                }
            }
            else if (AuthorizationHelper.CurrentUser.IsLoggedIn && (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
            {
                int studentId = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                if (studentId > 0)
                {
                    var coursePreReqData = Course.GetCoursePrerequisites(courseId, studentId).ToList();
                    result.Data = new { data = coursePreReqData, isValid = Course.IsStudentValidForCourseByPreReq(courseId, studentId), displayPrompt = true };
                }
                else
                {
                    var coursePreReqData = Course.GetCoursePrerequisites(courseId).ToList();
                    result.Data = new { data = coursePreReqData, isValid = true };
                }
            }
            else
            {
                var coursePreReqData = Course.GetCoursePrerequisites(courseId).ToList();
                result.Data = new { data = coursePreReqData };
            };
            return result;
        }

        [System.Web.Http.HttpGet]
        public JsonResult IsStudentValidForCourseByPreReq(int courseId, int studentId)
        {
            var result = new JsonResult();
            bool isValid = Course.IsStudentValidForCourseByPreReq(courseId, studentId);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = new { isValid = isValid };
            return result;
        }

        [System.Web.Http.HttpGet]
        public JsonResult UnattendedCoursesForCourseByPreReq(int courseId, int studentId)
        {
            var result = new JsonResult();
            var data = Course.UnattendedCoursesFromPreReq(courseId, studentId);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = new { isValid = data.Count() == 0, data = data, responseMessage = data.Count() > 0 ? Gsmu.Api.Data.WebConfiguration.PreReqNotMetMsg : "All prerequisite passed." };
            return result;
        }

        public ActionResult CourseList()
        {
            return View();
        }

        public ActionResult CoursePrerequisites()
        {

            return View();
        }

        public ActionResult CoursePrerequisitesContainer()
        {
            return View();
        }

        public ActionResult MembershipDetails()
        {
            ViewBag.memheader = Settings.Instance.GetMasterInfo().MembershipHeader;
            ViewBag.memfooter = Settings.Instance.GetMasterInfo().MembershipFooter;

            Memberships membr = new Memberships();
            var memberships = membr.StudentMemberships();
            ViewBag.memberships = memberships;
            return PartialView();
        }


        public ActionResult CourseDetails(int intCourseID = 0)
        {
            //Start for Color coding and Icons
            CourseIconsandLegend CourseIconsandLegend = new CourseIconsandLegend();
            ViewBag.V3InstructorImage = ConfigurationManager.AppSettings["V3InstructorImage"];
            var colorcode = CourseIconsandLegend.GetGroupingsColor();
            var icons = CourseIconsandLegend.GetIcons();
            ViewBag.ColorCode = colorcode;
            ViewBag.Icons = icons;
            ViewBag.EnabledColor = Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo2().EnableCourseColors);
            //end for icons
            CourseModel course = new CourseModel(intCourseID);
            return PartialView(course);
        }


        public ActionResult CourseLocation(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseNameDesc(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseEvent(CourseModel course)
        {

            return PartialView(course);
        }

        public ActionResult CourseInstructors(CourseModel course)
        {
      
            return PartialView(course);
        }

        public ActionResult CourseMaterials(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseAccessCode(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseRequisite(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseDateTime(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CoursePhoto(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CoursePrice(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseContact(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseCredits(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult SocialMediaLogo(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseDateTimeStart(int intCourseId)
        {
            EnrollmentFunction EnrollmentFunction = new EnrollmentFunction();
            CourseModel cmodel = new CourseModel(intCourseId);
            ViewBag.EnrollmentStatus = cmodel.Course.EnrollmentStatistics.EnrollmentStatus.ToString();
            ViewBag.COURSENAME = cmodel.Course.COURSENAME;
            ViewBag.COURSEID = cmodel.Course.COURSEID;
            ViewBag.courseStartEndTimeDisplay = (cmodel.Course.StartEndTimeDisplay != null ? cmodel.Course.StartEndTimeDisplay.Trim() : string.Empty);
            ViewBag.CourseEndAsDate = cmodel.CourseEndAsDate;
            ViewBag.CourseStartDate = cmodel.CourseStart.COURSEDATE;
            ViewBag.CourseStartTime = cmodel.CourseStart.STARTTIME;
            return PartialView();
        }

        public ActionResult GetCourseDetails(int cid)
        {
            if (cid != 0)
            {
                CourseModel cmodel = new CourseModel(cid);
                return Json(new { coursenumber = cmodel.Course.COURSENUM, coursename = cmodel.Course.COURSENAME }); ;
            }
            else
            {
                return Json(new { result = "No Result", coursenumber ="", coursename = ""});
            }
        }

        public ActionResult CourseSimilarCourseNumber(CourseModel course)
        {
            return PartialView(course);
        }

        public ActionResult CourseLinkAction(int intCourseId, string cmd, string urllnk)
        {
            CourseModel course = new CourseModel(intCourseId);
            ViewBag.cmd = cmd;
            ViewBag.urllnk = urllnk;
            return PartialView(course);
        }

        public String EmailCourse()
        {
            String emailTo = Request["emailTo"];
            String emailCC = Request["emailCC"];
            String emailSubject = Request["emailSubject"];
            String firstBody = Request["firstBody"];
            String courseBody = Request["courseBody"];

            courseBody = courseBody.Replace("@@@", "<");
            courseBody = courseBody.Replace("~~~", ">");

            Gsmu.Api.Data.School.Entities.EmailAuditTrail EmailEntity = new Gsmu.Api.Data.School.Entities.EmailAuditTrail();
            EmailFunction EmailFunction = new EmailFunction();
            EmailEntity.EmailBody = firstBody + "<br>" + courseBody;
            EmailEntity.EmailTo = emailTo;
            EmailEntity.EmailCC = emailCC;
            EmailEntity.EmailSubject = emailSubject;
            EmailEntity.AuditDate = DateTime.Now;
            EmailFunction.SendEmail(EmailEntity);

            string msg = "Done Sending";
            string reply = "{success:true, msg:" + msg + "}";
            return reply;
        }

        public ActionResult ValidateCaptcha(string captchaChallenge, string captchaResponse)
        {
            var cientIP = Request.ServerVariables["REMOTE_ADDR"];
            //var privateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];
            var privateKey = "6LcPeLEUAAAAAEgOEIHqkTBppuGMeNaHuj5D5imT";

            string data = string.Format("secret={0}&remoteip={1}&challenge={2}&response={3}", privateKey, cientIP, captchaChallenge, captchaResponse);
            byte[] byteArray = new ASCIIEncoding().GetBytes(data);

            WebRequest request = WebRequest.Create("https://www.google.com/recaptcha/api/siteverify");
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            var status = (((HttpWebResponse)response).StatusCode);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            var responseLines = responseFromServer.Split(new string[] { "\n" }, StringSplitOptions.None);
            var success = responseLines[0].Equals("true");

            return Json(new { Success = success });
        }

        public ActionResult CourseWork(int courseid)
        {
            ViewBag.files = Gsmu.Api.Data.School.Course.CourseFilesHelper.GetCourseFileList(courseid);
            CourseModel course = new CourseModel(courseid);
            string datetimevalues = "";
            foreach (Course_Time datetime in course.CourseTimes)
            {
                datetimevalues = datetimevalues + "-" + DateTime.Parse(datetime.COURSEDATE.ToString()).ToShortDateString();
            }
            ViewBag.courseid = courseid;
            ViewBag.datetimevalues = datetimevalues;
            ViewBag.coursename = course.Course.COURSENAME;
            ViewBag.coursedate = course.CourseTimes;
            

            return PartialView();
        }
        public string EnrollRoster(string rosterid)
        {
            return MoveToEnrollRoster(rosterid);
        }
        public string CancelRoster(string rosterid, int sendcancellationemail = 1)
        {
            SchoolEntities db = new SchoolEntities();
            EnrollmentFunction enroll = new EnrollmentFunction();
            EmailFunction EmailFunction = new EmailFunction();

            int roster_id = Convert.ToInt32(rosterid);

            //CANCEL THE ORIGINAL ROSTER END
            enroll.CancelEnrollment(int.Parse(rosterid));

            var course = (from cr in db.Course_Rosters join c in db.Courses on cr.COURSEID equals c.COURSEID
                          where cr.RosterID == roster_id
                          select new
                          {
                              coursetype= c.coursetype,
                              eventid = c.eventid,
                              sessionid = c.sessionid,
                              course_id = cr.COURSEID,
                              student_id = cr.STUDENTID,
                              iswaitingalready = cr.WAITING
                          }).FirstOrDefault();
            if (course.sessionid != 0 && course.sessionid != null)
            {
                var sessiondata = (from session in db.Courses where session.COURSEID == course.sessionid select session).FirstOrDefault();
                if (sessiondata.mandatory == 1 || sessiondata.mandatory == -1)
                {
                    var coursinevents = (from courses in db.Courses join roster in db.Course_Rosters on courses.COURSEID equals roster.COURSEID where courses.eventid == course.eventid && roster.STUDENTID == course.student_id && roster.Cancel==0 select roster).ToList();
                    foreach (var courseinevent in coursinevents)
                    {
                        enroll.CancelEnrollment(courseinevent.RosterID);
                    }

                    var eventmainroster = (from roster in db.Course_Rosters where roster.COURSEID == course.eventid && roster.Cancel ==0 && roster.STUDENTID == course.student_id select roster.RosterID).FirstOrDefault();
                    if (eventmainroster != null)
                    {
                        enroll.CancelEnrollment(eventmainroster);
                    }
                }

            }

            if (course.coursetype == 1)
            {
                var coursinevents = (from courses in db.Courses join roster in db.Course_Rosters on courses.COURSEID equals roster.COURSEID where courses.eventid == course.course_id && roster.STUDENTID == course.student_id && roster.Cancel == 0 select roster).ToList();
                foreach (var courseinevent in coursinevents)
                {
                    enroll.CancelEnrollment(courseinevent.RosterID);
                }
            }
            if (course.iswaitingalready == 0)
            {
                enroll.PopulateWaitingPeople(course.course_id.Value,false);
            }
            if (sendcancellationemail == 1)
            {
                EmailFunction.SendCancellationEmail(roster_id);
            }

            //CHECK FOR BUNDLE/FASTTRACK
            var fastTrackCourse = (from f in db.FastTrackCourses
                                   where f.FTMainCourseId == course.course_id
                                   select f.FTCourseId).ToList();
            if (fastTrackCourse.Count() > 0)
            {
                foreach (int bundled_course_id in fastTrackCourse)
                {
                    var roster_of_bundled_courses = (from rbc in db.Course_Rosters
                                                     where rbc.COURSEID == bundled_course_id
                                                     && rbc.STUDENTID == course.student_id
                                                     select new {
                                                         roster_id = rbc.RosterID,
                                                         course_id = rbc.COURSEID,
                                                         student_id = rbc.STUDENTID,
                                                         iswaitingalready = rbc.WAITING
                                                     }).ToList();
                    if (roster_of_bundled_courses.Count() > 0)
                    {
                        foreach (var rbc_roster in roster_of_bundled_courses)
                        {
                            enroll.CancelEnrollment(rbc_roster.roster_id);
                            if (rbc_roster.iswaitingalready == 0)
                            {
                                enroll.PopulateWaitingPeople(rbc_roster.course_id.Value,false);
                            }

                            EmailFunction.SendCancellationEmail(rbc_roster.roster_id);
                        }
                    }
                }
            }

            //Check if the course is part of a bundle as child bundle- it should cancel the main course and other courses
            var fastTrackChildCourse = (from f in db.FastTrackCourses
                                   where f.FTCourseId == course.course_id
                                   select f.FTMainCourseId).FirstOrDefault();
            if ((fastTrackChildCourse != null) && (fastTrackChildCourse != 0))
            {

                var fastTrackChildMainCourse = (from f in db.FastTrackCourses
                                                where f.FTMainCourseId == fastTrackChildCourse
                                            select f.FTCourseId).ToList();
                fastTrackChildMainCourse.Add(fastTrackChildCourse);
                if (fastTrackChildMainCourse.Count() > 0)
                {
                    foreach (int bundled_course_id in fastTrackChildMainCourse)
                    {
                        var roster_of_bundled_courses = (from rbc in db.Course_Rosters
                                                         where rbc.COURSEID == bundled_course_id
                                                         && rbc.STUDENTID == course.student_id
                                                         select new
                                                         {
                                                             roster_id = rbc.RosterID,
                                                             course_id = rbc.COURSEID,
                                                             student_id = rbc.STUDENTID,
                                                             iswaitingalready = rbc.WAITING
                                                         }).ToList();
                        if (roster_of_bundled_courses.Count() > 0)
                        {
                            foreach (var rbc_roster in roster_of_bundled_courses)
                            {
                                enroll.CancelEnrollment(rbc_roster.roster_id);
                                if (rbc_roster.iswaitingalready == 0)
                                {
                                    enroll.PopulateWaitingPeople(rbc_roster.course_id.Value,false);
                                }

                                EmailFunction.SendCancellationEmail(rbc_roster.roster_id);
                            }
                        }
                    }
                }
            }

            if ((CreditCardPaymentHelper.UsePayPalAdvance || CreditCardPaymentHelper.UsePayPal) && (Settings.Instance.GetMasterInfo3().AutomaticCreditToCardAccount == 1))
            {
                CreditCardPayments payment = new CreditCardPayments();
                payment.RefundPaypal(0, 0, "", roster_id, 0.00);
            }
            //END
            return rosterid;
        }



		public string MoveToEnrollRoster(string rosterid)
        {
            SchoolEntities db = new SchoolEntities();
            EnrollmentFunction enroll = new EnrollmentFunction();
            EmailFunction EmailFunction = new EmailFunction();

            int roster_id = Convert.ToInt32(rosterid);

            enroll.STMoveToEnroll(int.Parse(rosterid));

            return rosterid;
        }

        public void InternalCourseSetUp()
        {

                int districtidspecific = Settings.Instance.GetMasterInfo2().InternalCourseShowByDisctrictId;
                if ((Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn) && (Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser!=null))
                {
                    if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser.DISTRICT == districtidspecific || districtidspecific  == 0)
                    {
                        Gsmu.Api.Authorization.AuthorizationHelper.VisibleInternalCourses = 1;
                    }
                }
                else
                {
                    Gsmu.Api.Authorization.AuthorizationHelper.VisibleInternalCourses = 1;
                }

        }

        public ActionResult GetCourseHours(int studentid)
        {
            if (studentid == 0)
            {
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser != null)
                {
                    studentid = Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser.STUDENTID;
                }
            }
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Transcripts trans = new Transcripts();
            result.Data = trans.GetStudentCourseHoursforPurchase(studentid).ToList();

            return result;
        }
        //This will return list of Transcript Hours that already purchased.
        public ActionResult GetPurchasedCourseHours(int studentid,string startdate,string enddate)
        {
            if (studentid == 0)
            {
                try
                {
                    if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser != null)
                    {
                        studentid = Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser.STUDENTID;
                    }
                }
                catch
                {
                    studentid = 0;
                }
            }
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Transcripts trans = new Transcripts();
            result.Data = trans.GetStudentCourseHoursPurchased(studentid, startdate, enddate).ToList();

            return result;
        }

        public string GetPDFTranscript(string startdate, string enddate,string caller, int? studentid=0)
        {
            int sid = studentid.Value;
            if (AuthorizationHelper.CurrentStudentUser != null)
            {
                sid = AuthorizationHelper.CurrentStudentUser.STUDENTID;
            }
            if (sid != 0)
            {
                PdfStudentTranscript trans = new PdfStudentTranscript(sid, startdate, enddate, caller);
                if (trans.DefaultPdfTranscript != null)
                {

                    trans.Execute();
                }

                return trans.PdfOutFile;
            }
            return "";
        }
        public int GetDefaultTranscript()
        {
            Transcripts transcript = new Transcripts();
            return transcript.GetDefaultTranscript();
        }
        
    }
}
