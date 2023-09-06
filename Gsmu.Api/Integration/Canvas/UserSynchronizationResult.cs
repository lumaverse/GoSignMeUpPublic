using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Canvas
{
    public enum UserSynchronizationResult
    {
        DoesntHaveRole,
        SynchronizedStudent,
        SynchronizedInstructor,
        SynchronizedAsStudentAndInstructor,
        Error
    }
}
