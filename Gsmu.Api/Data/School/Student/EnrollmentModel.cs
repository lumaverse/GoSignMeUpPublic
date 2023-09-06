using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.Student
{
    public class EnrollmentModel
    {
        public Course_Roster Roster
        {
            get;
            set;
        }

        public Entities.Course Course
        {
            get;
            set;
        }


        public DateTime? MaxDate { get; set; }
    }
}
