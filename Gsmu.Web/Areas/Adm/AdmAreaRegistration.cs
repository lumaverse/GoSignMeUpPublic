using System.Web.Mvc;

namespace Gsmu.Web.Areas.Adm
{
    public class AdmAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Adm";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Adm_default",
                "Adm/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}