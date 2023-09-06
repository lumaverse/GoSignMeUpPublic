using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.CourseRoster
{
    public class LayoutOrderModel
    {
        public DateTime? OrderDate {get; set;}
        public string OrderNumber { get; set; }
        public string StudentFirst {get; set;}
        public string StudentLast {get; set;}
    }
}
