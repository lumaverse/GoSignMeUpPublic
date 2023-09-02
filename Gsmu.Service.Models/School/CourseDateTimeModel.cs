using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.School
{
    public class CourseDateTimeModel
    {
        public int? CourseId { get; set; }
        public DateTime? CourseDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
