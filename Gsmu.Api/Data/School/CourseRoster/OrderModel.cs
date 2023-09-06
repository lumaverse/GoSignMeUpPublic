using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Commerce.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core;
using Gsmu.Api.Data.School.Course;
namespace Gsmu.Api.Data.School.CourseRoster
{
    public class OrderClass
    {
       public  Course_Roster courseroster { get; set; }
       public List<rostermaterial> rosterMaterials { get; set; }
        

    }
    public class OrderModel
    {
        #region Static

        public static string Footer
        {
            get
            {
                return Settings.Instance.GetMasterInfo().CancelInfo;
            }
        }

        public static string Header
        {
            get
            {
                return Settings.Instance.GetMasterInfo().EnrollConfirm;
            }
        }

        public static string HeaderWhenOnWaitingList
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Settings.Instance.GetMasterInfo2().AltCourseConfirmation))
                {
                    return Settings.Instance.GetMasterInfo2().AltCourseConfirmation;
                }
                else
                {
                    return "At least one course in the order is wait listed. See details below.";
                }
            }
        }

        public static void UpdateConfirmationHeaders(string header, string headerWhenOnWaitingList, string footer)
        {
            var mi1 = Settings.Instance.GetMasterInfo();
            var mi2 = Settings.Instance.GetMasterInfo2();
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                mi1.EnrollConfirm = header;
                mi1.CancelInfo = footer;
                mi2.AltCourseConfirmation = headerWhenOnWaitingList;

                db.MasterInfoes.Attach(mi1);
                db.Entry(mi1).State = System.Data.Entity.EntityState.Modified;
                
                db.MasterInfo2.Attach(mi2);
                db.Entry(mi2).State = System.Data.Entity.EntityState.Modified;
                
                db.SaveChanges();
            }
        }

        #endregion

        public OrderModel(string OrderNumber, string requester)
        {
            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    CourseRostersOptimized = (from cr in db.Course_Rosters
                                     where cr.OrderNumber == OrderNumber
                                     select new OrderClass
                                     {
                                         courseroster = cr,
                                         rosterMaterials = (from m in db.rostermaterials where m.RosterID == cr.RosterID select m).ToList()

                                     }).ToList();
                    //Check for Master Order.
                    if (CourseRostersOptimized.Count == 0)
                    {
                        CourseRostersOptimized = (from cr in db.Course_Rosters
                                         where cr.MasterOrderNumber == OrderNumber
                                         select new OrderClass
                                         {
                                             courseroster = cr,
                                             rosterMaterials = (from m in db.rostermaterials where m.RosterID == cr.RosterID select m).ToList()

                        }).ToList();
                    }
                }
            }
        }
        public OrderModel(string OrderNumber,int?sid=0)
        {
            IndividualOrders = new List<IndividualOrder>();
            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    //Check for Master Order first
                    CourseRosters = (from cr in db.Course_Rosters join time in db.Course_Times on cr.COURSEID equals time.COURSEID where cr.MasterOrderNumber == OrderNumber  orderby time.COURSEDATE, time.STARTTIME select cr).DistinctBy(dis => dis.RosterID).ToList();
                    if (sid != 0)
                    {
                        CourseRosters = (from cr in db.Course_Rosters join time in db.Course_Times on cr.COURSEID equals time.COURSEID where cr.MasterOrderNumber == OrderNumber && cr.STUDENTID == sid orderby time.COURSEDATE, time.STARTTIME select cr).DistinctBy(dis => dis.RosterID).ToList();

                    }
                    if (CourseRosters.Count == 0)
                    {
                        CourseRosters = (from cr in db.Course_Rosters join time in db.Course_Times on cr.COURSEID equals time.COURSEID where cr.OrderNumber == OrderNumber orderby time.COURSEDATE, time.STARTTIME select cr).DistinctBy(dis => dis.RosterID).ToList();
                    }
                    var list = new List<Gsmu.Api.Data.School.Entities.Course_Roster>();

                    foreach (var roster in CourseRosters)
                    {
                        roster.Cours = (from c in db.Courses where c.COURSEID == roster.COURSEID select c).FirstOrDefault();
                        roster.Student = (from s in db.Students where s.STUDENTID == roster.STUDENTID select s).FirstOrDefault();
                        if (roster.Cours != null)
                        {
                            if (roster.Cours.eventid != 0 && roster.Cours.eventid != null)
                            {
                                if (roster.Cours.sessionid != null)
                                {
                                    roster.CourseSession = (from session in db.Courses where session.COURSEID == roster.Cours.sessionid select session.COURSENUM + " - " + session.COURSENAME).FirstOrDefault() + "<br />";
                                }
                                if (list.Where(rosterlist => rosterlist.COURSEID == roster.eventid.Value).Count() <= 0)
                                {
                                    try
                                    {
                                        list.Add(IncludeEventinRosterDisplay(roster.Cours.eventid.Value, roster.Student, OrderNumber, roster.TotalPaid));
                                    }
                                    catch { }
                                }
                                roster.EventId = roster.Cours.eventid.Value;
                            }
                            else
                            {
                                if (roster.Cours.coursetype == 1)
                                {

                                    roster.EventId = roster.Cours.COURSEID;
                                }
                            }
                        }
                        var materials = (from m in db.rostermaterials where m.RosterID == roster.RosterID select m).ToList();
                        roster.rostermaterials = new System.Collections.ObjectModel.Collection<rostermaterial>(materials);
                        roster.CourseExtraParticipants = (from m in db.CourseExtraParticipants where m.RosterId == roster.RosterID select m).ToList();
                        list.Add(roster);
                        IndividualOrders.Add(GetIndividualOrder(roster.OrderNumber));
                    }

                    CourseRosters = list;
                }
            }
        }
           
        public Gsmu.Api.Data.School.Entities.Course_Roster IncludeEventinRosterDisplay(int EventId,Gsmu.Api.Data.School.Entities.Student student,string OrderNumber, decimal? totalPaid)
        {
            using (var db = new SchoolEntities())
            {
                var singleRoster = new Gsmu.Api.Data.School.Entities.Course_Roster(true)
                {
                    Course = (from _event in db.Courses where _event.COURSEID ==EventId select _event).FirstOrDefault(),
                    Student = student,
                    TotalPaid = totalPaid,
                    COURSEID = EventId,
                    STUDENTID = student.STUDENTID,
                    DATEADDED = System.DateTime.Now,
                    TIMEADDED = System.DateTime.Now,
                    OrderNumber = OrderNumber,

                };
                return singleRoster;
            }
        }
        public IndividualOrder GetIndividualOrder(string OrderNumber)
        {
            float discount = 0;
            decimal cardfee = 0;
            IndividualOrder IndOrder = new IndividualOrder();
            List<Course_Roster> CourseRosters_internal = new List<Course_Roster>();
            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                var listcount = 0;
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    CourseRosters_internal = (from cr in db.Course_Rosters where cr.OrderNumber == OrderNumber select cr).ToList();
                    var list = new List<Gsmu.Api.Data.School.Entities.Course_Roster>();
                    foreach (var roster in CourseRosters_internal)
                    {
                        var materials = (from m in db.rostermaterials where m.RosterID == roster.RosterID select m).ToList();
                        roster.rostermaterials = new System.Collections.ObjectModel.Collection<rostermaterial>(materials);
                        roster.CourseExtraParticipants = (from m in db.CourseExtraParticipants where m.RosterId == roster.RosterID select m).ToList();
                        list.Add(roster);
                        if (Settings.Instance.GetMasterInfo4().AllowCouponPerOrder == 0)
                        {
                            discount = roster.CouponDiscount.Value;
                        }
                        else{
                            discount = discount +float.Parse(  roster.SingleRosterDiscountAmount.Value.ToString());

                        }
                        cardfee = roster.creditcardfee.Value;
                    }

                    CourseRosters_internal = list;

                }
               
                IndOrder.OrderNumber = OrderNumber;
                IndividualOrderNumber = OrderNumber;
                IndOrder.OrderTotal = CourseRosters_internal.Sum(cr => cr.MaterialTotalCost + cr.CourseCostDecimal + cr.CourseSalesTaxPaid) + cardfee;
                if (CourseRosters_internal.First().PaidInFull != 0)
                {
                    IndOrder.TotalPaid = CourseRosters_internal.Sum(cr => (cr.MaterialTotalCost + cr.CourseCostDecimal + cr.CourseSalesTaxPaid)) - decimal.Parse(discount.ToString()) + cardfee;
                }
                else
                {
                    try
                    {
                        IndOrder.TotalPaid = CourseRosters_internal.First().TotalPaid.Value;
                    }
                    catch { }
                }
                return IndOrder;

            }
            else
            {
                return null;
            }
        }

        public Course_Roster SingleRoster
        {
            get
            {
                return CourseRosters.FirstOrDefault();
            }
        }

        public List<Gsmu.Api.Data.School.Entities.Course_Roster> CourseRosters
        {
            get;
            set;
        }
        public List<OrderClass> CourseRostersOptimized
        {
            get;
            set;
        }
        public Entities.Student Student
        {
            get
            {
                try
                {
                    return SingleRoster.Student;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string ConfirmationHeader
        {
            get
            {
                if (this.IsWaiting)
                {
                    return OrderModel.HeaderWhenOnWaitingList;
                }
                else
                {
                    return OrderModel.Header;
                }
            }
        }

        public string ConfirmationFooter
        {
            get
            {
                return OrderModel.Footer;
            }
        }

        public bool IsWaiting
        {
            get
            {
                var waitingRoster = (from cr in CourseRosters where cr.IsValidForClass && cr.IsWaiting select cr).FirstOrDefault();
                return waitingRoster != null;
            }
        }

        public decimal OrderTotal
        {
            get
            {
                decimal total = 0;
                if (Settings.Instance.GetMasterInfo4().AllowCouponPerOrder == 1)
                {
                    total = CourseRosters.Sum(cr => (cr.MaterialTotalCost + cr.CourseCostDecimal + cr.CourseSalesTaxPaid) - (cr.CouponDiscount == null ? 0 : Convert.ToDecimal(cr.CouponDiscount))); 
                }
                else
                {
                    if (CourseRosters != null)
                    {
                        var CouponDiscount = (from cr in CourseRosters select cr.CouponDiscount).FirstOrDefault();
                        total = CourseRosters.Sum(cr => (cr.MaterialTotalCost + cr.CourseCostDecimal + cr.CourseSalesTaxPaid)) - (CouponDiscount == null ? 0 : Convert.ToDecimal(CouponDiscount));
                    }
                    else
                    {
                        total = CourseShoppingCart.Instance.SubTotal;
                    }
                }
                return total;
            }
        }


        public decimal OrderTotalOptimized
        {
            get
            {
                decimal total = 0;
                if (Settings.Instance.GetMasterInfo4().AllowCouponPerOrder == 1)
                {
                    total = CourseRostersOptimized.Where(cr => cr.courseroster.Cancel == 0).Sum(cr => (cr.courseroster.MaterialTotalCost + cr.courseroster.CourseCostDecimal + cr.courseroster.CourseSalesTaxPaid) - (cr.courseroster.CouponDiscount == null ? 0 : Convert.ToDecimal(cr.courseroster.CouponDiscount)));
                }
                else
                {
                    var CouponDiscount = (from cr in CourseRostersOptimized select cr.courseroster.CouponDiscount).FirstOrDefault();
                    total = CourseRostersOptimized.Where(cr => cr.courseroster.Cancel == 0).Sum(cr => (cr.courseroster.MaterialTotalCost + cr.courseroster.CourseCostDecimal + cr.courseroster.CourseSalesTaxPaid)) - (CouponDiscount == null ? 0 : Convert.ToDecimal(CouponDiscount));
                }
                return total;
            }
        }
        public decimal CardFee
        {
            get
            {
                decimal cardfee = 0;
                if (CourseRosters != null)
                {
                    if (CourseRosters.First().creditcardfee != null)
                    {
                        cardfee = CourseRosters.First().creditcardfee.Value;
                    }
                }
                return cardfee;
            }
        }
        public decimal OrderTotalNoDiscount
        {
            get
            {
                var total = CourseRosters.Sum(cr => (cr.MaterialTotalCost + cr.CourseCostDecimal + cr.CourseSalesTaxPaid));

                return total;
            }
        }
        public float StackDiscountTotal
        {
            get
            {
                if (Settings.Instance.GetMasterInfo4().AllowCouponPerOrder == 0)
                {
                    float total=0;
                    try
                    {
                        if (CourseRosters.Where(roster => roster.CouponDiscount > 0).FirstOrDefault().CouponDiscount != null)
                        {
                            total = CourseRosters.Where(roster => roster.CouponDiscount > 0).FirstOrDefault().CouponDiscount.Value;
                        }
                    }
                    catch { }

                    return total;
                }
                else
                {
                    var total =float.Parse( CourseRosters.Sum(cr => (cr.SingleRosterDiscountAmount)).Value.ToString());

                    return total;
                }
            }
        }
        public string OrderDetails 
        {
            get
            {
                try
                {
                    var model = new CourseModel(Convert.ToInt32(SingleRoster.COURSEID));
                    return SingleRoster.PricingOption;
                }
                catch
                {
                    return "";
                }
            }
        }

        public string UsernameField
        {
            get
            {
                if (this.Student.google_user == 1)
                {
                    return "Google Account";
                }
                return "Username";
            }
        }

        public string Username
        {
            get
            {
                if (this.Student.google_user == 1)
                {
                    return this.Student.EMAIL;
                }
                return this.Student.USERNAME;
            }
        }

        public string OrderNumber
        {
            get
            {
                try
                {
                    return SingleRoster.OrderNumber;
                }
                catch
                {
                    return "";
                }
            }
        }

        public bool HidePaymentInfo
        {
            get
            {
                if (Settings.Instance.GetMasterInfo2().HidePaymentInfo == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public static string SaveUploadContentFile(ConfirmationArea area, System.Web.HttpPostedFileBase file)
        {
            string pathName;

            switch (area)
            {
                case ConfirmationArea.Header:
                    pathName = "header";
                    break;

                case ConfirmationArea.HeaderOnWaitingList:
                    pathName = "headeronwaitinglist";
                    break;

                case ConfirmationArea.Footer:
                    pathName = "footer";
                    break;

                default:
                    throw new Exception("The confirmation area you are uploading for is not implemented");
            }

            var webPath = "Content/CourseConfirmation/" + pathName + "/" + file.FileName;
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/" + webPath);
            file.SaveAs(path);
            return webPath;
        }

        public List<IndividualOrder> IndividualOrders
        {
            get;
            set;
        }

        public string IndividualOrderNumber
        {
            get;
            set;
        }
       

    }

    public class IndividualOrder
    {
        public decimal OrderTotal
        {
            get;
            set;
        }
        public string OrderNumber
        {
            get;
            set;
        }
        public decimal TotalPaid
        {
            get;
            set;
        }
    }
}
