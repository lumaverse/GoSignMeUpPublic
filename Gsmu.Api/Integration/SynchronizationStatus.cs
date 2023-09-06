using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration
{
    public enum SynchronizationStatus
    {
        Unset = 0,
        Inserted = 1,
        Updated = 2,
        Skipped = 3,
        Error = 4,        
    }
}
