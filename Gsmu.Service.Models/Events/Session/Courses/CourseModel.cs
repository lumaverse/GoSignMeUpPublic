using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Events.Session.Courses
{
   
        public class CourseModel
        {
            public int CourseId { get; set; }
            public string CourseNumber { get; set; }
            public string CourseName { get; set; }
            public int StayinPublicDays { get; set; }
            public int MaxWait { get; set; }
            public int MaxEnroll { get; set; }
            public string LocationFullInfo { get; set; }
            public string StatisticInfo { get; set; }
            public List<EventSessionCourseDateTimeModel> DateTime { get; set; }
            public DateTime? bb_last_integration_date { get; set; }
            public int? haiku_course_id { get; set; }
            public int? canvas_course_id { get; set; }

            public DateTime? heliuslms_last_integration { get; set; }
            public int? disablehaikuintegration { get; set; }
            public int? disable_canvas_integration { get; set; }

        }


    
}
