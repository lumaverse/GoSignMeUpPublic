using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public static class InternalCourseSettings
    {
        public static CourseInternalState InternalCourseResultTypes
        {
            get
            {
                CourseInternalState result = CourseInternalState.Internal;

                var setting = Settings.Instance.GetMasterInfo().InternalCourses;

                if (setting == 1)
                {
                    result = CourseInternalState.Internal;
                }
                else if (setting == 2)
                {
                    result = CourseInternalState.InternalAndPublic;
                }

                return result;
            }
        }
    }
}
