using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Gsmu.Api.Data;
using Gsmu.Api.Update;

namespace Gsmu.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        //better todo this way.
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (Request.QueryString["load-session"] != null)
            {
                Response.SetCookie(new HttpCookie(WebConfiguration.SessionCookieName, Request.QueryString["load-session"]));
            }
            //if (Request.QueryString["embed"] != null)
            //{
            //    Gsmu.Api.Web.EmbedHelper.IsSiteEmbedded = true;
            //}

            var asp = Settings.Instance.GetMasterInfo4().AspSiteRootUrl ?? string.Empty;
            asp = asp.ToLower();
            if (!string.IsNullOrWhiteSpace(asp))
            {
                try
                {
               //    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
                catch { }
            }

            if (Settings.Instance.GetMasterInfo().RequireSSL == -1 && Request.ServerVariables["SCRIPT_NAME"].ToLower() != "silentformpost.asp")
            {
                if (HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
                {
                    Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
                }
            }
        }
        protected void Application_Start()
        {
            // this must be the very first call, to execute updates on the system before anything happens
            Updater.Execute();

            //if (Settings.Instance.GetMasterInfo().RequireSSL == -1)
            //{
                //GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            //}
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                Gsmu.Api.Web.RequireAdminModeAttribute.InitializeAdminMode();
            }
            System.Web.Optimization.BundleTable.EnableOptimizations = !WebConfiguration.DevelopmentMode || WebConfiguration.ForceCssJssCompression;
        }
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Request.IsSecureConnection == true)
            {
                foreach (string s in Response.Cookies.AllKeys)
                {
                    Response.Cookies[s].Secure = true;
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            HttpContext.Current.Session["session-start"] = DateTime.Now;
            HttpContext.Current.Session.Timeout = WebConfiguration.LoggedOutSessionTimeout;
            
            try
            {
                
            /*    if (Request.UrlReferrer.AbsoluteUri.Contains("admin"))
                {
                   // using (System.IO.StreamWriter _testData = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest2.txt"), true))
                   // {
                       // _testData.WriteLine(Environment.NewLine + ":::::::Requested From:::" + "admin" + " On " + DateTime.Now); // Write the file.
                   // } 
                }
                else
                { */

                    if (Request.IsSecureConnection == true)
                    {
                        Response.Cookies["ASP.NET_SessionId"].Secure = true;
                        Response.Cookies["ASP.NET_SessionId"].HttpOnly = true;
                        Response.Cookies["WEUFHSAJDFJQW"].Secure = true;
                        Response.Cookies["WEUFHSAJDFJQW"].HttpOnly = true;
                        foreach (string s in Response.Cookies)
                        {
                            if (!Response.Cookies[s].Secure)
                                Response.Cookies[s].Secure = true;
                        }
                        foreach (string s in Response.Cookies.AllKeys)
                        {
                            if (!Response.Cookies[s].Secure)
                                Response.Cookies[s].Secure = true;
                                Response.Cookies[s].HttpOnly = true;
                        }
                        /*
                        if (!Response.Cookies["GSMUusername"].Secure)
                            Response.Cookies["GSMUusername"].Secure = true;
                        if (!Response.Cookies["ASP.NET_SessionId"].Secure)
                            Response.Cookies["ASP.NET_SessionId"].Secure = true;
                        if (!Response.Cookies["GSMUaccess"].Secure)
                            Response.Cookies["GSMUaccess"].Secure = true;
                        if (!Response.Cookies["membership-username"].Secure)
                            Response.Cookies["membership-username"].Secure = true;
                        */
                    }
                //}
                    
            }

            catch (Exception)
            {

            }
             
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (!WebConfiguration.DevelopmentMode)
            {
                var error = Server.GetLastError().GetBaseException();
                var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;
                if (code != 404)
                {
                    string PageUrl = Request.Url.ToString();
                    Gsmu.Api.Logging.LogManager.LogException(PageUrl, "Unhandled application exception", error);
                }
            }
        }        

    }
}