using System.Web.Mvc;

namespace Gsmu.Web.Areas.Membership
{
    public class MembershipAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Membership";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Membership_default",
                "Membership/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}