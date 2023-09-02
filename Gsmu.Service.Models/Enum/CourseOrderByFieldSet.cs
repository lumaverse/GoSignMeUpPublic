using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Enum
{
    public enum CourseOrderByFieldSet
    {
        /// <summary>
        /// This was set in V3 systemconfig_coursesortorder.asp
        /// </summary>
        [Description("Default")]
        SystemDefault,

        [Description("Course number")]
        CourseNum,

        [Description("Course name")]
        CourseName,

        [Description("Course start")]
        CourseStart,

        [Description("Location")]
        Location,

        [Description("ID")]
        CourseId,

        [Description("Course time")]
        CourseTime,

        [Description("Course date")]
        CourseDate

    }
}
