using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    /*
        "id": 16,
        "auth_type": "canvas"
        "position": 1,
    */
    public class AuthenticationProvider
    {
        [json.JsonProperty(PropertyName = "id")]
        public int APId { get; set; }

        [json.JsonProperty("auth_type")]
        public string APauth_type { get; set; }

        [json.JsonProperty("position")]
        public int? APposition { get; set; }
    }
}
