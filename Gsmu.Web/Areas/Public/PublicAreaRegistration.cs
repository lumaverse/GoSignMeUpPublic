using System.Web.Mvc;
using Gsmu.Api.Data.ViewModels;

namespace Gsmu.Web.Areas.Public
{
    public class PublicAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            var routeValues = new
            {
                controller = "Course",
                action = "Browse",
                view = Gsmu.Api.Data.WebConfiguration.DefaultCourseSearchView,
                displayMode = DisplayMode.Normal
            };
            context.MapRoute("CourseBrowser", "Public/Course/Browse/{view}/{displayMode}", routeValues);
            context.MapRoute("CourseBrowserInternal", "Public/Course/BrowseInternal/{view}/{displayMode}", routeValues);

            context.MapRoute("CourseBrowserByLayout", "Public/Course/Layout/{layoutName}", new
            {
                controller = "Course",
                action = "Layout",
                layoutName = UrlParameter.Optional
            });

            // this has to be the last one for the custom ones to work otherwise it overtakes all routes if in the start
            var route = context.MapRoute(
                "Public_default",
                "Public/{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional }
            );

        }
    }
}
