using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.Interface.Instructors;
using Gsmu.Service.Models.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.Instructors
{
    public class InstructorDetailsManager : IInstructorDetails
    {
        public InstructorBasicDetailsModel GetInstructorDetailsById(int instructorId)
        {
            InstructorBasicDetailsModel InstructorModel = new InstructorBasicDetailsModel();
            using (var db = new SchoolEntities())
            {
                InstructorModel = (from _instructor in db.Instructors
                                         where _instructor.INSTRUCTORID == instructorId
                                   select new InstructorBasicDetailsModel
                                         {
                                             InstructorId = _instructor.INSTRUCTORID,
                                             InstructorFirstName = _instructor.FIRST,
                                             InstructorLastName = _instructor.LAST
                                         }).FirstOrDefault();
            }
            return InstructorModel;
        }
    }
}
