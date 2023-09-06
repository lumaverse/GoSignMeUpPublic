using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gsmu.Api.Authorization
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class GsmuAuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter 
    {
        private LoggedInUserType[] RequiredUserTypes
        {
            get;
            set;
        }

        public string[] AllowedActions {
            get;
            set;
        }

        public GsmuAuthorizationAttribute(params LoggedInUserType[] requiredUserTypes)
        {
            this.RequiredUserTypes = requiredUserTypes;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            {
            }

            Action badResult = delegate()
            {
                Gsmu.Api.Web.ObjectHelper.SetShowLoginInUi(filterContext.Controller, true);
                RouteValueDictionary dic = new RouteValueDictionary();
                dic.Add("controller", "Home");
                dic.Add("action", "Index");
                dic.Add("loginRedirectUrl", filterContext.RequestContext.HttpContext.Request.Url);
                filterContext.Result = new RedirectToRouteResult(dic);
            };

            if (AllowedActions != null)
            {
                var actionAllowedCount = (from a in (from a in AllowedActions select a.ToLower()) where a == filterContext.ActionDescriptor.ActionName.ToLower() select a).Count();

                if (actionAllowedCount > 0)
                {
                    return;
                }
            }

            if (RequiredUserTypes != null )
            {
                var currentUser = AuthorizationHelper.CurrentUser;
                if (currentUser == null)
                {
                    badResult();
                }

                var requiresGuest = from ut in RequiredUserTypes where ut == LoggedInUserType.Guest select ut;
                if (requiresGuest.Count() != 0 && currentUser.LoggedInUserType == LoggedInUserType.Guest)
                {
                    return;
                }
                var exist = from ut in RequiredUserTypes where ut == currentUser.LoggedInUserType select ut;

                if (RequiredUserTypes == null || RequiredUserTypes.Length == 0)
                {
                    return;
                }
                if (exist.Count() == 0)
                {
                    badResult();
                }
            }
        }

    }
}
