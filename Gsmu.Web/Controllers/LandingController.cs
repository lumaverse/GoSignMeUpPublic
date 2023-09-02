using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Web;
using Gsmu.Api.Data;
using System.Web.Script.Serialization;
using System.IO;
using Gsmu.Api.Authorization;

namespace Gsmu.Web.Controllers
{
    public class LandingController : Controller
    {
        public ActionResult Index()
        {

            try
            {
                if (WebConfiguration.AuditSiteAcitivityLevel.ToLower() != "off")
                {
                    Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                    Audittrail.TableName = Request.UserHostName;
                    Audittrail.AuditDate = DateTime.Now;
                    Audittrail.RoutineName = "Public Visit";

                    Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                    LogManager.LogSiteActivity(Audittrail);
                }
            }
            catch(Exception e)
            {
                Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                Audittrail.TableName = Request.UserHostName;
                Audittrail.AuditDate = DateTime.Now;
                Audittrail.RoutineName = "Public Visit";
                Audittrail.ATErrorMsg = e.Message;

                Gsmu.Api.Logging.LogManagerDispossable LogManager= new Api.Logging.LogManagerDispossable();
               LogManager.LogSiteActivity(Audittrail);
            }
            if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.LoggedInUserType == Gsmu.Api.Authorization.LoggedInUserType.Admin || Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.LoggedInUserType == Gsmu.Api.Authorization.LoggedInUserType.SubAdmin)
            {
                if (Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent == 0)
                {
                    return Redirect("/admin/homeportal.asp?");
                }
                if ( (Request.UrlReferrer == null) || Request.UrlReferrer.ToString()=="")
                {
                  //  AuthorizationHelper.Logout();
                }
            }
            string CASrequiredlogin = "0";
            string casconfig = Settings.Instance.GetMasterInfo4().CASAuthConfiguration;
            string WelcomePage = System.Configuration.ConfigurationManager.AppSettings["WelcomePage"];
            if ((casconfig != "") && (casconfig != null))
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic settingsconfig = j.Deserialize(casconfig, typeof(object));
                CASrequiredlogin = settingsconfig["requiredlogin"];
            }
            string session_identifier = Request["session_identifier"];
            if ((session_identifier != null) && (session_identifier != ""))
            {
                string amount = Request["pmt_amt"];
                return RedirectToAction("TouchNetTLinkConfirmation", "Cart", new { area = "Public", session_identifier = session_identifier, pmt_amt = amount }
                );
            }

            else if (Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled == 1 || Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled == 2)
            //if (1 == 1)
            {
              //  var headers = String.Empty;
               //foreach (var key in Request.Headers.AllKeys)
                 //   headers += key + "=" + Request.Headers[key] + Environment.NewLine;



             //   ViewBag.Headertxt = headers;
              // return RedirectToAction("Browse", "Course", new
               //{
                 //  area = "Public",
                  // message = headers +"qwertyu"
             //  });

                string curReferer = Request.Headers["Referer"];
                if (string.IsNullOrEmpty(curReferer)) { curReferer = ""; }
                if ((!string.IsNullOrEmpty(curReferer)) && curReferer.ToLower().IndexOf("saml2") != -1 || curReferer.ToLower().IndexOf("samlrequest") != -1 || curReferer.ToLower().IndexOf("authfed.fiu.edu") != -1)
                {
                    return RedirectToAction("ShibbolethAuthentication", "AuthMe");
                } else {
                    if (!string.IsNullOrEmpty(WelcomePage))
                    {
                        return Redirect(WelcomePage);
                    }
                    else
                    {
                        return RedirectToAction("ShibbolethAuthentication", "AuthMe");
                    }
                }
            }

            else if (((Settings.Instance.GetMasterInfo2().useCASAuth == 1) || (Settings.Instance.GetMasterInfo2().useCASAuth == -1)) && (Request.QueryString["ticket"]!=null) )
            {
                TimeSpan mTS = DateTime.Now - DateTime.Parse("1970-01-01");
                string issueInstant_s = DateTime.Now.ToString("yy") + "-" +  DateTime.Now.ToString("MM") + "-" +  DateTime.Now.ToString("dd") + "T" +  DateTime.Now.ToString("hh") + ":" +  DateTime.Now.ToString("mm") + ":" + DateTime.Now.ToString("ss") + "Z";
                return RedirectToAction("CASServiceValidation", "AuthMe", new
                {
                    ticket = Request.QueryString["ticket"],
                    requestID = "_" + Request.QueryString["REMOTE_HOST"] + "." + mTS.TotalSeconds,
                    issueInstant = issueInstant_s
                });
            }
            else
            {

                if (((Settings.Instance.GetMasterInfo2().useCASAuth == 1) || (Settings.Instance.GetMasterInfo2().useCASAuth == -1)) && ((CASrequiredlogin.Trim() == "-1") || (CASrequiredlogin.Trim() == "1")))
                {

                    return Redirect(Settings.Instance.GetMasterInfo2().CASAuthURL+"?service="+Request.Url.Scheme+"://" + Request.Url.Host);
                }
                else
                {
                    if (!string.IsNullOrEmpty(WelcomePage))
                    {
                        if (AuthorizationHelper.CurrentSupervisorUser != null)
                        {
                            return Redirect("public/supervisor");
                        }
                        else
                        {
                            return Redirect(WelcomePage);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Browse", "Course", new
                        {
                            area = "Public",
                            message = Gsmu.Web.Areas.Public.StripHtmlTags.Strip(Request.QueryString["message"])
                        });
                    }
                }
            }
        }

        public ActionResult Internal()
        {
            return RedirectToAction("BrowseInternal", "Course", new
            {
                area = "Public"
            });
        }

        public ActionResult Embedded()
        {
            Uri myReferrer = Request.UrlReferrer;

            //using (StreamWriter _testData = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest.txt"), true))
            //{
            //    if (myReferrer != null)
            //    {
            //        _testData.WriteLine(Environment.NewLine + ":::::::Requested From:::" + myReferrer.ToString() + " On " + DateTime.Now); // Write the file.
            //    }
            //    else
            //    {
            //        _testData.WriteLine(Environment.NewLine + ":::::::Requested From::: Direct Accessing the site." + " On " + DateTime.Now); // Write the file.
            //    }
            //} 
            //EmbedHelper.IsSiteEmbedded = true;
            return RedirectToAction("Index", new
            {
                embed = true
            });
        }

        public ActionResult IFrameFix(string redirect)
        {

            //Uri myReferrer = Request.UrlReferrer;

            //using (StreamWriter _testData = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest.txt"), true))
           // {
            //    if (myReferrer != null)
            //    {
            //        _testData.WriteLine(Environment.NewLine + ":::::::Requested From:::" + myReferrer.ToString() + " On " + DateTime.Now); // Write the file.
            //    }
            //    else
            //    {
            //        _testData.WriteLine(Environment.NewLine + ":::::::Requested From::: Direct Accessing the site." + " On " + DateTime.Now); // Write the file.
            //    }
            //} 
            //EmbedHelper.IsSiteEmbedded = true;
            ViewBag.redirect = redirect;
            return View();
        }

        public ActionResult ErrorPage(string err)
        {
            ViewBag.ErrMsgOrCode = err;
            return View();
        }

        public ActionResult AdminPortalSessionTerminal(string username, string userid, string adminusersessionid)
        {
            //try
            //{
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                {
                    Gsmu.Api.Authorization.AuthorizationHelper.Logout();
                }
                var callResult = AuthorizationHelper.ConfigurePortalAdministratorLogin(username, adminusersessionid);
                Gsmu.Web.Areas.Adm.Controllers.AdministratorEnrollmentController.SetPrincipalStudent(int.Parse(Request.QueryString["studentid"].Replace("ST", "")));
                if ((AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null) && (!adminusersessionid.Contains("00000000-0000-0000-0000-000000000000")))
                {
                    return RedirectToAction("Browse", "Course", new
                    {
                        area = "Public"
                    });
                }
                else
                {
                string _errstring = "";
                _errstring = (AuthorizationHelper.CurrentAdminUser == null ? "AUTH001" : _errstring);
                _errstring = (AuthorizationHelper.CurrentSubAdminUser == null ? "AUTH002" : _errstring);
                _errstring = (adminusersessionid.Contains("00000000-0000-0000-0000-000000000000") ? "AUTH003" : _errstring);
                    return RedirectToAction("ErrorPage", "Landing", new { err = _errstring });
                }
            //}
            //catch
            //{
            //}
            //return RedirectToAction("ErrorPage", "Landing");
        }

        public ActionResult AdminPortalSession(string username, string userid, string adminusersessionid)
        {
            var result = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            try
            {
                var callResult = AuthorizationHelper.ConfigurePortalAdministratorLogin(username, adminusersessionid);
                Gsmu.Web.Areas.Adm.Controllers.AdministratorEnrollmentController.SetPrincipalStudent(int.Parse(Request.QueryString["studentid"].Replace("ST", "")));
                if ((AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null) && (!adminusersessionid.Contains("00000000-0000-0000-0000-000000000000")))
                {
                    result.Data = new
                    {
                        status = "success",
                        success = true
                    };
                }
                else
                {
                    result.Data = new
                    {
                        status = "Err501",
                        success = false
                    };
                }
                return result;

            }
            catch
            {
                result.Data = new
                {
                    status = "Err502",
                    success = false
                };
                return result;
            }
        }
    }
}
