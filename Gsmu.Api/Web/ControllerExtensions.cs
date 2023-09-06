using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using json = Newtonsoft.Json;
using System.IO;

namespace System.Web.Mvc
{
    public static class ControllerExtensions
    {
        public static string GetRequestPayload(this Controller controller)
        {
            controller.Request.InputStream.Position = 0;
            var requestPayload = new StreamReader(controller.Request.InputStream).ReadToEnd();
            return requestPayload;
        }

        public static System.Web.Mvc.ActionResult JsonEntity(this Controller controller, object data)
        {
            var result = Gsmu.Api.Web.SerializationHelper.SerializeEntity(data);
            return new ContentResult()
            {
                Content = result,
                ContentType = "application/json"
            };
        }

        public static System.Web.Mvc.ActionResult JsonEntityWithoutRelationships(this Controller controller, object data)
        {
            var result = Gsmu.Api.Web.SerializationHelper.SerializeEntityWithoutRelationships(data);
            return new ContentResult()
            {
                Content = result,
                ContentType = "application/json"
            };
        }


    }
}
