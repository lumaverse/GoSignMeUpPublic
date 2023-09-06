using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using entities = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Integration.Canvas
{
    public class UserSynchronizationResponse
    {
        public UserSynchronizationResult SynchRonizationResult { get; internal set; }

        public entities.Student Student
        {
            get;
            internal set;
        }

        public entities.Instructor Instructor
        {
            get;
            internal set;
        }

        public entities.Supervisor Supervisor
        {
            get;
            internal set;
        }
    }
}
