using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Courses
{
    public interface ICourseDetails
    {
        CourseDetailsResultModel GetCourseDetailsById(int courseId);
        EventDetailsModel GetEventCourseFullDetailsById(int eventId);
    }
}
