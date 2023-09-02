using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Courses
{
    public class EventSessionCourseDateTimeModel
    {
        public int? CourseId { get; set; }
        public DateTime? CourseDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class CourseDateTimeFormattedModel
    {
        public string CourseDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    public class CourseDateTimeModel
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public DateTime? CourseDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        //temporarily using this since the DateTime post is not working
        public string CourseDateString { get; set; }
        public string StartTimeString { get; set; }
        public string EndTimeString { get; set; }
    }
}
