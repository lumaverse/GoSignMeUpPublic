using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class MasterInfo
    {
        public bool ShowPastOnlineCoursesAsBoolean
        {
            get
            {
                if (this.ShowPastOnlineCourses == null || this.ShowPastOnlineCourses.Value != 1)
                {
                    return false;
                }
                return true;
            }
            set
            {
                this.ShowPastOnlineCourses = value ? 1 : 0;
            }
        }

    }
}
