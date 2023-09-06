using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class masterinfo4
    {
        public bool PublicCourseListingFastLoadAsBoolean
        {
            get
            {
                if (this.public_course_listing_fast_load == null || this.public_course_listing_fast_load.Value != 1)
                {
                    return false;
                }
                return true;
            }
            set
            {
                this.public_course_listing_fast_load = value ? 1 : 0;
            }
        }

        //public bool EnrollToWaitList {
        //    get {
        //        using (SchoolEntities db = new SchoolEntities())
        //        {
        //            var value = db.Database.SqlQuery<int>("SELECT ISNULL(EnrollToWaitList, 0) FROM Masterinfo4").SingleOrDefault();
        //            if (value == 1)
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

    }
}
