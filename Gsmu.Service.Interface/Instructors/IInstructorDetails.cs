using Gsmu.Service.Models.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Instructors
{
    public interface IInstructorDetails
    {
        InstructorBasicDetailsModel GetInstructorDetailsById(int instructorId);
    }
}
