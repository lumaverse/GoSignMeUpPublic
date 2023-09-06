using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.School
{
    public static class Queries
    {

        public static List<entities.School> GetSchoolByName(string schoolname = null)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = new List<entities.School>();
                if (string.IsNullOrWhiteSpace(schoolname) == false)
                {
                    query = (from d in db.Schools where d.LOCATION.Contains(schoolname) select d).ToList();
                }
                else
                {
                    query = (from d in db.Schools select d).ToList();
                }
                return query;
            }
        }


        public static entities.School GetSchoolById(int? locationid)
        {
            if (locationid == null)
            {
                return null;
            }
            using (var db = new entities.SchoolEntities())
            {
                var query = (from d in db.Schools where d.locationid == locationid select d).FirstOrDefault();
                return query;
            }
        }

        public static string GetSchoolNameById(int? locationid)
        {
            var result = GetSchoolById(locationid);
            if (result == null)
            {
                return null;
            }
            return result.LOCATION;
        }

        public static int AddSchool(Gsmu.Api.Data.School.Entities.School school)
        {
            using (var db = new entities.SchoolEntities())
            {
                db.Schools.Add(school);
                db.SaveChanges();
                //return school.ID; //using location ID instead
                return Convert.ToInt32(school.locationid);
            }

        }

        public static int GetSchoolIdWithAddIfNonExistent(string schoolName)
        {
            using (var db = new entities.SchoolEntities())
            {
                var id = (from s in db.Schools where s.LOCATION.Contains(schoolName) select s.ID).FirstOrDefault();
                if (id < 1) {
                    id = AddSchool(new entities.School()
                    {
                        LOCATION = schoolName,
                        locationid = 0
                    });
                }
                return id;
            }
        }
    }
}
