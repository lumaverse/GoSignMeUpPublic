using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;
using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas
{
    public class RawResponse
    {
        public RawResponse(string jsonString, http.HttpResponseMessage response)
        {
            StringResult = jsonString;
            Errors = new List<Exception>();
            try
            {
                JsonResult = json.Linq.JToken.Parse(jsonString);
            }
            catch (Exception e)
            {
                Errors.Add(e);
            }
            HttpResponse = response;
        }

        public Newtonsoft.Json.Linq.JToken JsonResult
        {
            get;
            private set;
        }

        public string StringResult { 
            get; 
            private set; 
        }

        public List<Exception> Errors
        {
            get;
            set;
        }

        public http.HttpResponseMessage HttpResponse { get; set; }

    }
}
