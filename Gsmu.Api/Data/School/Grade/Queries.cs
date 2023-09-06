using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.Grade
{
    public static class Queries
    {

        public static List<Grade_Level> GetGradeByName(string gradename = null)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = (from d in db.Grade_Levels where d.GRADE.Contains(gradename) select d).ToList();
                return query;
            }
        }

        public static Grade_Level GetGradeById(int? gradeid)
        {
            if (gradeid == null)
            {
                return null;
            }
            using (var db = new entities.SchoolEntities())
            {
                var query = (from d in db.Grade_Levels where d.GRADEID == gradeid select d).FirstOrDefault();
                return query;
            }
        }

        public static string GetGradeNameById(int? gradeid)
        {
            var result = GetGradeById(gradeid);
            if (result == null)
            {
                return null;
            }
            return result.GRADE;
        }


        public static int AddGrade(Gsmu.Api.Data.School.Entities.Grade_Level grade)
        {
            using (var db = new entities.SchoolEntities())
            {
                db.Grade_Levels.Add(grade);
                db.SaveChanges();
                return grade.GRADEID;
            }

        }


        public static int GetGradeIdWithAddIfNonExistent(string gradeName)
        {
            using (var db = new entities.SchoolEntities())
            {
                var id = (from g in db.Grade_Levels where g.GRADE.Contains(gradeName) select g.GRADEID).FirstOrDefault();
                if (id < 1)
                {
                    id = AddGrade(new entities.Grade_Level()
                    {
                        SortOrder = 0,
                        SchoolId = 0,
                        GRADE = gradeName
                    });
                }
                return id;
            }

        }
    }
}
