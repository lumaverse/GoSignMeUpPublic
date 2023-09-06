using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Data.School.Memberships
{
    public class Memberships
    {

        public List<Entities.Course> StudentMemberships()
        {

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                //var membership = (from mbr in db.Memberships
                //                  select mbr).ToList();
                var membership = (from mbr in db.Courses
                                  select mbr)
                                  .Where(c => c.COURSENUM == "~ZZZZZZ~"
                                  && c.CANCELCOURSE == 0
                                  )
                                  .ToList();

                var membershiplist = new List<Entities.Course>();
                foreach (var item in membership)
                {
                    bool showMembership = false;
                    var datestart = (from ct in db.Course_Times where ct.COURSEID == item.COURSEID select ct.COURSEDATE).Min();
                    var dateexpired = (from ct in db.Course_Times where ct.COURSEID == item.COURSEID select ct.COURSEDATE).Max();
                    var startfromdatepurchased = item.DAYS;


                    if (item.DAYS > 0)
                    {
                        if (dateexpired.Value.Date >= DateTime.Today.Date)
                        {
                            dateexpired = DateTime.Now.AddDays(startfromdatepurchased.Value);
                        }
                    }

                    if (datestart.Value.Date <= DateTime.Today.Date && dateexpired.Value.Date >= DateTime.Today.Date)
                    {
                        showMembership = true;
                    }

                    if (datestart.Value.Date == dateexpired.Value.Date)
                    {
                        showMembership = false;
                    }

                    if (item.SubStatus == 1)
                    {
                        showMembership = true;
                    }

                    if (showMembership)
                    {
                        //item.DESCRIPTION = datestart.ToString() + " - " + dateexpired.ToString() + ">>" + DateTime.Today.Date.ToString() + " ... " + startfromdatepurchased;
                        membershiplist.Add(item);
                    }
                }

                return membershiplist;
            }
        }
        public Entities.Course StudentMembership(int membershipid)
        {

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                //var membership = (from mbr in db.Memberships
                //                  where mbr.MembershipID == membershipid
                //                  select mbr).FirstOrDefault();
                var membership = (from mbr in db.Courses
                                  where mbr.COURSEID == membershipid
                                  select mbr).FirstOrDefault();
                return membership;
            }
        }
        //public List<Membership> StudentMemberships()
        //{

        //    using (var db = new SchoolEntities())
        //    {
        //        var membership = (from mbr in db.Memberships
        //                          select mbr).ToList();
        //        return membership;
        //    }
        //}


    }

}
