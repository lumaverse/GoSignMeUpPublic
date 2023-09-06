using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.CourseRoster
{
    public enum RosterCancelStatus : short
    {
        Valid = 0,
        InvalidOrCancelled = 1,
        WaitingForPayment = 2,
        FailedPayment = 3,
        IncompleteRegistration = 4
    }
}
