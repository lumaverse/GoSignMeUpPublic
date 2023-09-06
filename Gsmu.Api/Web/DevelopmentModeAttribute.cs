using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Gsmu.Api.Data;

namespace Gsmu.Api.Web
{
    public class DevelopmentModeAttribute : ActionFilterAttribute
    {
        private bool RequireDevelopmentMode
        {
            get;
            set;
        }

        public DevelopmentModeAttribute(bool requireDevelopmentMode)
        {
            RequireDevelopmentMode = requireDevelopmentMode;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (RequireDevelopmentMode && WebConfiguration.DevelopmentMode == false)
            {
                throw new System.Web.HttpException(500, "This feature requires development mode.");            
            }
        }
    }
}
