using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    class InstructorModel
    {
        public InstructorModel() { }
        public InstructorModel(int instructorid)
        {
            using (var db = new SchoolEntities())
            {
                var instructor = (from c in db.Instructors where c.INSTRUCTORID == instructorid select c).FirstOrDefault();
                Init(db, instructor);
            }
        }
        private void Init(SchoolEntities db, Entities.Instructor i)
        {
            Instructor = i;
        }
        public Gsmu.Api.Data.School.Entities.Instructor Instructor
        {
            get;
            set;
        }
    }


}
