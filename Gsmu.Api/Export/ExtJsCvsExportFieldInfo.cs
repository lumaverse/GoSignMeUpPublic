using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Export
{
    public class ExtJsCvsExportFieldInfo
    {
        [json.JsonProperty(PropertyName = "column")]
        public string Column { get; set; }

        [json.JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}
