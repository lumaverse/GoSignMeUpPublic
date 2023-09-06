using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

using Gsmu.Api.Data;

namespace Gsmu.Api.Web
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RequireAdminModeAttribute : ActionFilterAttribute
    {
        private static readonly string VARIABLE_NAME = "adminmode";

        public bool RequireAdminMode
        {
            get;
            set;
        }

        public string Area
        {
            get;
            private set;
        }

        public RequireAdminModeAttribute(bool requireAdminMode)
        {
            RequireAdminMode = requireAdminMode;
        }

        public RequireAdminModeAttribute()
        {
            RequireAdminMode = true;
        }

        public RequireAdminModeAttribute(string area, bool requireAdminMode)
        {
            this.RequireAdminMode = requireAdminMode;
            this.Area = area;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!RequireAdminMode) {
                return;
            }

            // check if the current check is for the current area
            if (this.Area != null)
            {
                var routeArea = filterContext.RouteData.DataTokens["area"] as string;
                if (this.Area != routeArea)
                {
                    return;
                }
            }

            if (!IsAdminMode)
            {
                throw new System.Web.HttpException(500, "This feature requires administrative login.");
            }
        }

        public static bool IsAdminMode
        {
            get
            {
                string adminmode = System.Web.HttpContext.Current.Session[VARIABLE_NAME] as string;
                return (adminmode != null && adminmode == "true") || WebConfiguration.DevelopmentMode;
            }
        }

        public static void InitializeAdminMode()
        {
            var context = System.Web.HttpContext.Current;
            var adminmode = context.Request[VARIABLE_NAME];
            if (adminmode != null && adminmode != "true")
            {
                context.Session.Remove(VARIABLE_NAME);
            }
            if (!IsAdminMode)
            {
                SetAdminMode();
            }
        }

        public static void SetAdminMode()
        {
            var context = System.Web.HttpContext.Current;
            
            var asp = Settings.Instance.GetMasterInfo4().AspSiteRootUrl ?? string.Empty;
            asp = asp.ToLower();

            if (!string.IsNullOrWhiteSpace(asp) && (context.Request.UrlReferrer == null || !context.Request.UrlReferrer.ToString().ToLower().StartsWith(asp)))
            {
                context.Session.Remove(VARIABLE_NAME);
            }
            else
            {
                context.Session[VARIABLE_NAME] = "true";
            }
        }
    }
}
