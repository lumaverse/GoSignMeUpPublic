using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.District
{
    public static class Queries
    {
        //will need to add order by on the list return.
        public static List<entities.District> GetDistrictByName(string districtname = null)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = new List<entities.District>();
                if (!string.IsNullOrWhiteSpace(districtname))
                {
                    query = (from d in db.Districts where d.DISTRICT1.Contains(districtname) select d).ToList();
                }

                else
                {
                    query = (from d in db.Districts select d).ToList();
                }
                return query;
            }
        }

        public static entities.District GetDistrictById(int? districtid)
        {
            if (districtid == null)
            {
                return null;
            }
            using (var db = new entities.SchoolEntities())
            {
                var query = (from d in db.Districts where d.DISTID == districtid select d).FirstOrDefault();
                return query;
            }
        }

        public static string GetDistrictNameById(int? districtid)
        {
            var result = GetDistrictById(districtid);
            if (result == null)
            {
                return null;
            }
            return result.DISTRICT1;
        }


        public static int AddDistrict(Gsmu.Api.Data.School.Entities.District district)
        {
            using (var db = new entities.SchoolEntities())
            {

                db.Districts.Add(district);
                db.SaveChanges();
                return district.DISTID;
            }

        }

        public static int GetDistrictIdWithAddIfNonExistent(string districtName)
        {
            using (var db = new entities.SchoolEntities())
            {
                var id = (from d in db.Districts where d.DISTRICT1.Contains(districtName) select d.DISTID).FirstOrDefault();
                if (id < 1)
                {
                    id = AddDistrict(new entities.District()
                    {
                        SortOrder = 0,
                        DISTRICT1 = districtName
                    });
                }
                return id;
            }
        }
    }
}
