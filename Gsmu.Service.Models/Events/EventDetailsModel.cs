
using Gsmu.Service.Models.Events.Session;
using Gsmu.Service.Models.Events.Session.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Events
{
    public class EventDetailsModel
    {
        public int EventId { get; set; }
        public List<Gsmu.Service.Models.Courses.MaterialModel> materials { get; set; }
        public string EventNumber { get; set; }
        public string EventName { get; set; }
        
        public string DisplayCommentEndDateStartDate { get; set; }
        public int MaxEnrollment { get; set; }
        public int CloseEnrollmentDays { get; set; }
        public string AccessCode { get; set; }
        public string RoomNum { get; set; }
        public string RoomDirection { get; set; }
        public string Location { get; set; }
        public string LocationAdditionalInfo { get; set; }
        public string State{get;set;}
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public List<SessionModel> Sessions { get; set; }
        public List<EventSessionCourseDateTimeModel> DateTime { get; set; }
        public int? CourseEventID { get; set; }
    }
}
