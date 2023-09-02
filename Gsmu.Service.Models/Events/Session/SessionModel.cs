using Gsmu.Service.Models;
using Gsmu.Service.Models.Events.Session.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Events.Session
{
    public class SessionModel
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string SessionNumber { get; set; }
        public string DisplayCommentEndDateStartDate { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public int MandatoryClass { get; set; }
        public string LocationFullInfo { get; set; }

        public List<CourseModel> Courses { get; set; }
        public List<EventSessionCourseDateTimeModel> DateTime { get; set; }

    }
}
