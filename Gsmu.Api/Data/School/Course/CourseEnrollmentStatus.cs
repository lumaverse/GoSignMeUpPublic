using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Gsmu.Api.Data.School.Course
{
    public enum CourseEnrollmentStatus
    {
        [Description("Space available")]
        SpaceAvailable,

        [Description("Wait space available")]
        WaitSpaceAvailable,

        [Description("Class full")]
        Full,

        [Description("Expired")]
        Expired
    }
}
