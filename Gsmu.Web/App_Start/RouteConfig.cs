using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gsmu.Web.Areas.Public;

namespace Gsmu.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.sso/{*pathInfo}");
            var internalRoute = routes.MapRoute(
                name: "InternalRoute",
                url: "internal",
                defaults: new { 
                    controller = "Landing", 
                    action = "Internal"
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Landing", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}