using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Net;
using json = Newtonsoft.Json;
using Gsmu.Api.Web;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtension
    {
        public static System.Web.IHtmlString Json(this HtmlHelper helper, object data, json.JsonSerializerSettings settings)
        {
            var result = json.JsonConvert.SerializeObject(data, settings);
            return helper.Raw(result);
        }

        public static System.Web.IHtmlString JsonEntity(this HtmlHelper helper, object data)
        {
            var result = SerializationHelper.SerializeEntity(data);
            return helper.Raw(result);
        }

        public static System.Web.IHtmlString Json(this HtmlHelper helper, object data)
        {
            var result = SerializationHelper.SerializeObject(data);
            return helper.Raw(result);
        }

    }
}
