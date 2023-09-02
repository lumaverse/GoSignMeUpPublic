using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new Gsmu.Api.Web.RequireAdminModeAttribute("Adm", true));
        }
    }
}