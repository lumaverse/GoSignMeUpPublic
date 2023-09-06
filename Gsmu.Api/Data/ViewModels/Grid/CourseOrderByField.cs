using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Gsmu.Api.Data.ViewModels.Grid
{
    public enum CourseOrderByField
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
