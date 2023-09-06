using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Authorization
{
    public enum LoggedInUserType
    {
        Guest,
        Student,
        Supervisor,
        Instructor,
        // sub-admin, manager
        SubAdmin,
        Admin
    }
}
