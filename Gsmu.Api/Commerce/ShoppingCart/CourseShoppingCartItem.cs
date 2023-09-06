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
using Gsmu.Api.Data.School.Terminology;
using Gsmu.Api.Data;

namespace Gsmu.Api.Commerce.ShoppingCart
{
    public class CourseShoppingCartItem
    {
        /// <summary>
        /// Retrieve the corresponding course in the cart.
        /// </summary>
        [ScriptIgnore]
        public Gsmu.Api.Data.School.Entities.Course Course
        {
            get;
            internal set;
        }

        public bool IsBundledCourse
        {
            get;
            set;
        }

        public int? BundleParentCourseId
        {
            get;
            set;
        }

        /// <summary>
        /// The course shopping cart item constructors are internal 
        /// because validation is done in the shopping
        /// cart in the addcourse method, so we should not call 
        /// it outside unless you know what you are doing.      
        /// </summary>
        internal CourseShoppingCartItem() { }

        /// <summary>
        /// Should find a course in the db and add it.
        /// 
        /// The course shopping cart item constructors are internal 
        /// because validation is done in the shopping
        /// cart in the addcourse method, so we should not call 
        /// it outside unless you know what you are doing.      
        /// </summary>
        /// <param name="courseId"></param>
        internal CourseShoppingCartItem(int courseId, PricingModel pricingModel, CourseChoice courseChoice, List<Material> materials = null, bool hiddenInCart = false, int eventParent=0, List<int> events = null, List<CourseCredit> selectedcredits = null, bool isBunldedCourse = false, int? bundleParentCourseId = null, List<CourseExtraParticipant> extraParticipants = null, int studentId = 0)
        {

            using (var db = new SchoolEntities())
            {
                SessionName = "";
                SessionDate = DateTime.Now;
                SessionTime = DateTime.Now;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                var course = (from c in db.Courses where c.COURSEID == courseId  select c).FirstOrDefault();

                if ((course.MaterialsRequiredAsBoolean || course.MaterialsRequired == 2) && (materials == null || materials.Count == 0))
                {
                    if (course.MATERIALS != "")
                    {
                        throw new Exception("This course requires " + TerminologyHelper.Instance.GetTermCapital(TermsEnum.Materials) + ".");
                    }
                }
                if (course != null)
                {
                    if((course.sessionid!=null) &&(course.sessionid!=0)){
                    var session = (from sessions in db.Courses where sessions.COURSEID == course.sessionid select sessions).FirstOrDefault();
                    SessionName = session.COURSENAME + " - " + session.COURSEID.ToString();
                        if (Settings.Instance.GetMasterInfo2().HideCourseNumber == 0)
                        {
                            SessionName = session.COURSENUM +"<br>" + SessionName;
                         }

                        var sessiondatetime = (from sessions in db.Course_Times where sessions.COURSEID == course.sessionid orderby sessions.COURSEDATE, sessions.STARTTIME select sessions).FirstOrDefault();
                        SessionDate = sessiondatetime.COURSEDATE.Value;
                        SessionTime = sessiondatetime.STARTTIME.Value;
                    }

                    if(course.eventid!=null && (course.eventid != 0))
                    {
                        var evnt = (from evnts in CourseShoppingCart.Instance.Items where evnts.Course.COURSEID == course.eventid select evnts.Course).FirstOrDefault();
                        EventName = evnt.COURSENAME + " - " + evnt.COURSEID.ToString();
                        if (Settings.Instance.GetMasterInfo2().HideCourseNumber == 0)
                        {
                            EventName = evnt.COURSENUM + "<br>" + EventName;
                        }
                    }

                    if (course.coursetype == 1)
                    {
                        CheckoutEventId = course.COURSEID;
                        EventName = course.COURSENAME + " - " + course.COURSEID.ToString();
                        if (Settings.Instance.GetMasterInfo2().HideCourseNumber == 0)
                        {
                            EventName = course.COURSENUM + "<br>" + EventName;
                        }
                    }
                    else
                    {
                        CheckoutEventId = course.eventid.Value;
                    }
                }

                this.Course = course;
            } 

            PricingModel = pricingModel;
            CourseChoice = courseChoice;
            Materials = materials;
            EventParent = eventParent;
            Events = events;
            BundleParentCourseId = bundleParentCourseId;
            IsBundledCourse = isBunldedCourse;
            ExtraParticipants = extraParticipants;
            StudentId = studentId;
            CourseId = courseId;
            IsPartialPayment = false;
            HiddenInCart = hiddenInCart;
            SelectedCredits = selectedcredits;
            
          
        }

        public void Refresh()
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var course = (from c in db.Courses where c.COURSEID == Course.COURSEID select c).FirstOrDefault();
                this.Course = course;
            }
        }

        public List<Material> Materials
        {
            set;
            get;
        }
        public List<int> Events
        {
            set;
            get;
        }
        public List<CourseCredit> SelectedCredits
        {
            get;
            set;
        }

        public bool HasMembership
        {
            get
            {
                int membershipCount = (from c in CourseShoppingCart.Instance.Items where c.Course.COURSENUM.ToString() == "~ZZZZZZ~" select c).Count();
                if (membershipCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HasMaterials
        {
            get
            {
                return Materials != null && Materials.Count > 0;
            }
        }

        public decimal MateriaTotal
        {
            get
            {
                decimal total = 0;
                if (Materials == null)
                {
                    return total;
                }
                total += (from m in Materials select m.ActualPriceTotal).Sum();
                return total;
            }
        }

        public decimal NonTaxableMateriaTotal
        {
            get
            {
                decimal total = 0;
                if (Materials == null)
                {
                    return total;
                }
                total += (from m in Materials where m.taxable==0 select m.ActualPriceTotal).Sum();
                return total;
            }
        }
        public decimal? AdminSetCourseTotal
        {
            get;
            set;
        }

        public decimal CourseTotal
        {
            get
            {

               var coursePrice = PricingModel == null ? 0 : PricingModel.EffectivePrice;
               if (HasMembership)
               {
                   if (this.IsBundledCourse)
                   {
                       coursePrice = 0;
                   }
                   else
                   {
                       coursePrice = Convert.ToDecimal(this.Course.DISTPRICE);
                   }

                   if (AdminSetCourseTotal.HasValue) {
                        coursePrice = AdminSetCourseTotal.Value;
                   }

                   foreach (var item in CourseShoppingCart.Instance.MultipleStudentCourses.Where(st => st.CourseId == this.CourseId))
                   {
                     
                       item.CourseTotal = coursePrice;
                       item.DISTEMPLOYEE = 1;
                       item.useAdminPricing = 1;
                   }
               }

               else
               {
                   foreach (var item in CourseShoppingCart.Instance.MultipleStudentCourses.Where(st => st.CourseId == this.CourseId))
                   {
                       if (item.useAdminPricing != 1)
                       {

                           item.CourseTotal = coursePrice;
                       }
                   }

                    if (AdminSetCourseTotal.HasValue)
                    {
                        coursePrice = AdminSetCourseTotal.Value;
                    }

                }




                return coursePrice;
            }
        }

        public decimal LineTotal
        {
            get
            {
                var total = MateriaTotal + CourseTotal;
                return total;
            }
        }

        public PricingModel PricingModel
        {
            get;
            set;
        }
        public string DiscountCouponPerCourse
        {
            get;
            set;
        }
        public string DiscountAmountPerCourse
        {
            get;
            set;
        }
        public string RoommateName
        {
            get;
            set;

        }

        public string RoommateGender
        {
            get;
            set;
        }

        public string RoommateQuestion
        {
            get;
            set;
        }
        public bool IsPartialPayment
        {
            get;
            set;
        }

        public decimal PartialPayment
        {
            get
            {
                if (!AuthorizationHelper.CurrentUser.IsLoggedIn)
                {
                    return 0;
                }
                else
                {

                    if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo().Membership_Status == 0)
                    {
                        if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo2().DefaultPublicPricingType == 0)
                        {
                            //Member
                            return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentAmount) ? "0.00" : this.Course.PartialPaymentAmount);
                        }
                        else
                        {
                            //Non - Member
                            return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentNon) ? "0.00" : this.Course.PartialPaymentNon);
                        }
                    }
                    else
                    {

                        switch (AuthorizationHelper.CurrentUser.MembershipType)
                        {
                            case Gsmu.Api.Data.School.Student.MembershipType.Member:
                                return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentAmount) ? "0.00" : this.Course.PartialPaymentAmount);

                            case Gsmu.Api.Data.School.Student.MembershipType.NonMember:
                                return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentNon) ? "0.00" : this.Course.PartialPaymentNon);

                            case Gsmu.Api.Data.School.Student.MembershipType.Special1:
                                return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentSP) ? "0.00" : this.Course.PartialPaymentSP);

                            default:
                                if (Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType == 2)
                                {
                                    return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentAmount) ? "0.00" : this.Course.PartialPaymentAmount);


                                }
                                else
                                {
                                    return Decimal.Parse(string.IsNullOrEmpty(this.Course.PartialPaymentNon) ? "0.00" : this.Course.PartialPaymentNon);


                                }
                               // throw new NotImplementedException();

                        }
                    }
                }
            }
        }

        public CourseChoice CourseChoice
        {
            get;
            set;
        }

        public List<CourseExtraParticipant> ExtraParticipants
        {
            get;
            set;
        }

        public int EnrolleeCount
        {
            get
            {
                return HasExtraParticipants ? ExtraParticipants.Count + 1 : 1;
            }
        }

        public bool HasExtraParticipants
        {
            get
            {
                return ExtraParticipants != null && ExtraParticipants.Count > 0;
            }
        }

        public string CRExtraParticipant
        {
            get;
            set;
        }

        public bool IsWaiting
        {
            get
            {
                var stats = this.Course.EnrollmentStatistics;
                var count = EnrolleeCount;
                var waitAvailable = stats.WaitSpaceAvailable;
                var available = stats.SpaceAvailable;

                if (available<=0)
                {
                    if (waitAvailable>0)
                    {
                        return true;
                    }
                    else
                    {
                        CourseShoppingCart.Instance.RemoveCourse(this.Course.COURSEID);
                        throw new System.InvalidOperationException("There's no more space on this class");
                    }
                }
                return false;
            }
        }

        public string SessionName
        {
            get;
            set;
        }

        public DateTime SessionDate
        {
            get;
            set;
        }

        public DateTime SessionTime
        {
            get;
            set;
        }

        public string EventName
        {
            get;
            set;
        }

        public int CheckoutEventId
        {
            get;
            set;
        }

        /// <summary>
         //This is to hold Student Id on Multiple Enrollment
        // Use on Supervisor Account
        /// </summary>       
        public int StudentId
        {
            get;
            set;
        }

        public int CourseId
        {
            get;
            set;
        }

        public int EventParent { get; set; }

        public bool HiddenInCart { get; set; }
    }
}
