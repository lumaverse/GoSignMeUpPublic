using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using json = Newtonsoft.Json;
using Gsmu.Api.Data;

namespace Gsmu.Api.Web
{
    public static class SerializationHelper
    {
        public static string SerializeEntity(object data)
        {
            var result = json.JsonConvert.SerializeObject(data, new json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = json.ReferenceLoopHandling.Ignore,
                Formatting = WebConfiguration.DevelopmentMode ? json.Formatting.Indented : json.Formatting.None
            });
            return result;
        }

        public static string SerializeEntityWithoutRelationships(object data)
        {
            var result = json.JsonConvert.SerializeObject(data, new json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = json.ReferenceLoopHandling.Ignore,
                Formatting = WebConfiguration.DevelopmentMode ? json.Formatting.Indented : json.Formatting.None,
                ContractResolver = new NoVirtualPropertySerializationContractResolver()
            });
            return result;
        }



        public static string SerializeObject(object data)
        {
            var result = json.JsonConvert.SerializeObject(data, new json.JsonSerializerSettings()
            {
                Formatting = WebConfiguration.DevelopmentMode ? json.Formatting.Indented : json.Formatting.None
            });
            return result;
        }
    }
}
