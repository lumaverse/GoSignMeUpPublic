using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Web.SessionState;

namespace Gsmu.Tests
{
    class HttpHelper
    {
        public static HttpContext FakeHttpContext(string url)
        {
            var uri = new Uri(url);
            var httpRequest = new HttpRequest(string.Empty, uri.ToString(),
                                                uri.Query.TrimStart('?'));
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id",
                                            new SessionStateItemCollection(),
                                            new HttpStaticObjectsCollection(),
                                            10, true, HttpCookieMode.AutoDetect,
                                            SessionStateMode.InProc, false);

            SessionStateUtility.AddHttpSessionStateToContext(
                                                 httpContext, sessionContainer);

            return httpContext;
        }
    }
}
