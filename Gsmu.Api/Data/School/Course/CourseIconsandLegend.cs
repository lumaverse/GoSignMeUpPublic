using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.Course
{
    public class CourseIconsandLegend
    {

        public IEnumerable<Icon> GetIcons()
        {
            try
            {
                using (var db = new SchoolEntities())
                {
                    IEnumerable<Icon> item = (from m in db.Icons  select m).ToList();
                    return item;
                }
            }
            catch (Exception)
            {
                using (var db = new SchoolEntities())
                {
                    IEnumerable<Icon> item = (from m in db.Icons where m.IconsID == 0 select m).ToList();
                    return item;
                }
            }          
        }

        public IEnumerable<CourseCategory> GetGroupingsColor()
        {

            try
            {
                using (var db = new SchoolEntities())
                {
                    IEnumerable<CourseCategory> item = (from m in db.CourseCategories select m).ToList();
                    
                    return item;
                }
            }
            catch (Exception)
            {
                IEnumerable<CourseCategory> color = null;
                return color;
            }
        }
    }
}
