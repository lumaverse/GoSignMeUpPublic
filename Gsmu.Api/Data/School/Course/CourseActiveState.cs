using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public enum CourseActiveState
    {
        /// <summary>
        /// Represents all courses
        /// </summary>
        All = 0,

        /// <summary>
        /// Represents courses that are open right now
        /// </summary>
        Current = 1,

        /// <summary>
        /// Represents closed courses
        /// </summary>
        Past = 2
    }
}
