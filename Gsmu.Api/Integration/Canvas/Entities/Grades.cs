using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class Grades
    {

        [json.JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [json.JsonProperty("current_score")]
        public string CurrentScore { get; set; }

        [json.JsonProperty("final_score")]
        public string FinalScore { get; set; }

        [json.JsonProperty("current_grade")]
        public string CurrentGrade { get; set; }

        [json.JsonProperty("final_grade")]
        public string FinalGrade { get; set; }

        [json.JsonProperty("unposted_current_score")]
        public string UnPostedCurrentScore { get; set; }

        [json.JsonProperty("unposted_current_grade")]
        public string UnPostedCurrentGrade { get; set; }

        [json.JsonProperty("unposted_final_score")]
        public string UnpostedFinalScore { get; set; }

        [json.JsonProperty("unposted_final_grade")]
        public string UnpostedFinalGrade { get; set; }
    }
}
