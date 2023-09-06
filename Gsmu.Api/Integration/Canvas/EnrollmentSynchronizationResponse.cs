using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Canvas
{
    public class EnrollmentSynchronizationResponse : CourseSynchronizationResponse
    {
        public EnrollmentSynchronizationResponse() : base()
        { 
        }

        public EnrollmentSynchronizationResponse(CourseSynchronizationResponse courseSynchResponse) : base()
        {
            GsmuCourse = courseSynchResponse.GsmuCourse;
            CanvasCourse = courseSynchResponse.CanvasCourse;
        }

        public Entities.Enrollment[] Enrollments { get; set; }
    }
}
