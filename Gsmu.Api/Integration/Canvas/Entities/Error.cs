using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class Error
    {
        [json.JsonProperty("error_report_id")]
        public int? ErrorReportId { get; set; }

        [json.JsonProperty("errors")]
        public ErrorDetails[] ErrorDetails { get; set; }
    }
}
