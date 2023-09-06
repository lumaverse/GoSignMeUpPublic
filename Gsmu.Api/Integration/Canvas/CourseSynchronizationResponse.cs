using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Canvas
{
    public class CourseSynchronizationResponse
    {
        public CourseSynchronizationResponse()
        {
            Errors = new List<Exception>();
        }

        public List<Exception> Errors { get; internal set; }

        public Gsmu.Api.Data.School.Entities.Course GsmuCourse
        {
            get;
            set;
        }

        public Entities.Course CanvasCourse
        {
            get;
            set;
        }


    }
}
