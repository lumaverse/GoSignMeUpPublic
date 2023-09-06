using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Entities;
using System.Web.Script.Serialization;
using Gsmu.Api.Data;

namespace Gsmu.Api.Commerce.ShoppingCart
{

    public class MembershipShoppingCartItem
    {

        public Gsmu.Api.Data.School.Entities.Membership Membership
        {
            get
            {

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;

                    var membrsp = (from m in db.Memberships where m.MembershipID == MembershipID select m).FirstOrDefault();
                    return membrsp;
                }
            }

        }

       
        public int MembershipID
        {
            get;
            set;
        }


    }

}
