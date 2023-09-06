using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.Calendar
{
    public class CalendarModel
    {
        public int? id { get; set; }
        public int? cid { get; set; }
        public string title { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string loc { get; set; }
        public string note { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public string rem { get; set; }
        public int EventInternalClass { get; set; }
        public string stringstartdate {get;set;}
        public string stringenddate { get; set; }
        public string OnlineDateStartEnd { get; set; }
        public string StartEndTimeDisplay { get; set; }
        public int? OnlineCourse { get; set; }
        public int ct_id { get; set; }
        public int? ctype { get; set; }
    }
}
