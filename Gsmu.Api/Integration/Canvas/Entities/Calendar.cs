using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class Calendar
    {

        [json.JsonProperty("ics")]
        public string Ics { get; set; }

    }
}
