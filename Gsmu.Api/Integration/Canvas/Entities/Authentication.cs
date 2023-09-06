using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class Authentication
    {
        [json.JsonProperty("access_token")]
        public string AcccessToken
        {
            get;
            set;
        }

        [json.JsonProperty("user")]
        public User User
        {
            get;
            set;
        }
    }
}
