using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public static class CustomCourseField5_MasterCourseIDList
    {
        public static MasterList_Result GetAllMasterCourseId(string keyword)
        {
            MasterList_Result MasterList_Result = new MasterList_Result();
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                if (keyword != null && keyword != "")
                {
                    MasterList_Result.MasterCourseIds = (from c in db.Courses where c.CustomCourseField5 == keyword select new MasterCourseId { StringMasterCourseId = c.CustomCourseField5, StringMasterCourseId_value = c.CustomCourseField5 }).Distinct().ToList();
                }
                else
                {
                    MasterList_Result.MasterCourseIds = (from c in db.Courses where c.CustomCourseField5 != "" && c.CustomCourseField5 != null orderby c.CustomCourseField5 select new MasterCourseId { StringMasterCourseId_value = c.CustomCourseField5, StringMasterCourseId = c.CustomCourseField5 }).Distinct().ToList();
                }
            }
            return MasterList_Result;
        }
        public static string UpdateMasterCourseId(string oldval, string newval)
        {
            using (var db = new SchoolEntities())
            {
                db.Courses.Where(x => oldval == x.CustomCourseField5).ToList().ForEach(a => a.CustomCourseField5 = newval);
                db.SaveChanges();
            }
            return "updated";


        }
    }
    public class MasterCourseId
    {
        public string StringMasterCourseId { get; set; }
        public string StringMasterCourseId_value { get; set; }
    }
    public class MasterList_Result
    {
        public List<MasterCourseId> MasterCourseIds { get; set; }
    }
}
