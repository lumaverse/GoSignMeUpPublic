using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gsmu.Api.Web
{
    public static class EmbedHelper
    {
        public static bool IsSiteEmbedded
        {
            set {
                // Cookie issue in IE and Safari, Cross-domain iframe doesn't preserve cookie
                //var cookie = new HttpCookie(WebContextObject.SiteIsEmbedded.ToString(), value.ToString());
                //HttpContext.Current.Response.SetCookie(cookie);
                Gsmu.Api.Data.WebConfiguration.SiteIsEmbedded = value;
            }
            get
            {
                // Cookie issue in IE and Safari, Cross-domain iframe doesn't preserve cookie
                //var cookie = HttpContext.Current.Request.Cookies[WebContextObject.SiteIsEmbedded.ToString()];
                //return cookie != null && cookie.Value == Boolean.TrueString;
                return Gsmu.Api.Data.WebConfiguration.SiteIsEmbedded;
            }
        }
    }
}
