using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class MasterInfo3
    {
        public bool AllowViewPastCourseDaysAsBoolean
        {
            get
            {
                return this.allowviewpastcoursesdays != null && this.allowviewpastcoursesdays.Value != 0;
            }
            set
            {
                this.allowviewpastcoursesdays = value ? 0 : 1;
            }
        }

    }
    
}
