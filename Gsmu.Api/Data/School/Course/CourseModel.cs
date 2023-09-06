using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Terminology;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Data.School.Student;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Data.School.Course
{
    /// <summary>
    /// Whatever you put in the init method must be replicated in the queries class becuase many places uses the 
    /// same features and at the moment i developed this, this was the option i had, i am sure there is better one
    /// but for now we have to stick with this
    /// </summary>
    public class CourseModel
    {
        List<Material> materials = null;
        List<CourseCredit> credits = null;
        List<CourseChoice> coursechoices = null;
        List<Entities.Course> childbundledcourses = null;
        List<int> parentbundledcourseids = null;
        List<Instructor> instructor = null;
        List<Icon> icons = null;
        List<int> courseids = null;

        IEnumerable<Course_Time> courseTimes = null;

        public CourseModel()
        {
        }

        public CourseModel(int courseId, bool generateModelForAdmin = false)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                var course = (from c in db.Courses.AsNoTracking() where c.COURSEID == courseId select c).FirstOrDefault();
                Init(db, course, generateModelForAdmin);
            }
        }

        public CourseModel(SchoolEntities db, int courseId, bool generateModelForAdmin = false)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;

            var course = (from c in db.Courses.AsNoTracking() where c.COURSEID == courseId select c).FirstOrDefault();
            Init(db, course, generateModelForAdmin);
        }

        public CourseModel(SchoolEntities db, Entities.Course c, bool generateModelForAdmin = false)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;

            Init(db, c, generateModelForAdmin);
        }


        /// <summary>
        /// All queries here must be the same in <see cref="Queries">queries search method</see>.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="c"></param>
        private void Init(SchoolEntities db, Entities.Course c, bool generateModelForAdmin = false)
        {
            if (c != null)
            {
                GenerateModelFormAdmin = generateModelForAdmin;

                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;
                bool disabled_prices = true;
                if (Settings.Instance.GetMasterInfo4().DisplayNonDefaultMembership == 1)
                {
                     if (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null)
                    {

                    }

                    disabled_prices = AuthorizationHelper.CurrentUser.MembershipType == Student.MembershipType.NonMember;
                }
                if (!Student.MembershipHelper.MembershipEnabled)
                {
                    disabled_prices = false;
                }
                Course = c;
                CourseTimes = (from ct in db.Course_Times.AsNoTracking() where ct.COURSEID == c.COURSEID orderby ct.COURSEDATE, ct.STARTTIME select ct).ToList();
                PricingOptionsMember = (from cpo in db.CoursePricingOptions
                                        join po in db.PricingOptions on cpo.PricingOptionId equals po.PricingOptionID
                                        where cpo.CourseId == c.COURSEID && cpo.Type == (int)PricingOptionType.Member
                                        select new PricingModel()
                                        {
                                            CoursePricingOption = cpo,
                                            PricingOption = po,
                                            MembershipType = Student.MembershipType.Member,
                                            Disabled = (disabled_prices)
                                        }).ToList();
                PricingOptionsNonMember = (from cpo in db.CoursePricingOptions
                                           join po in db.PricingOptions on cpo.PricingOptionId equals po.PricingOptionID
                                           where cpo.CourseId == c.COURSEID && cpo.Type == (int)PricingOptionType.NonMember
                                           select new PricingModel()
                                           {
                                               CoursePricingOption = cpo,
                                               PricingOption = po,
                                               MembershipType = Student.MembershipType.NonMember,
                                               Disabled = (disabled_prices)
                                           }).ToList();
                PricingOptionsSpecial1 = (from cpo in db.CoursePricingOptions
                                          join po in db.PricingOptions on cpo.PricingOptionId equals po.PricingOptionID
                                          where cpo.CourseId == c.COURSEID && cpo.Type == (int)PricingOptionType.Special1
                                          select new PricingModel()
                                          {
                                              CoursePricingOption = cpo,
                                              PricingOption = po,
                                              MembershipType = Student.MembershipType.Special1
                                          }).ToList();
                MainCategory = null;
                SubCategory = null;
                SubSubCategory = null;
                if (this.BundledCourses.Count == 0)
                {
                    this.BundleCourseStatus = this.Course.EnrollmentStatistics.EnrollmentStatus;
                }
                else
                {
                    foreach (var bundle in this.BundledCourses)
                    {
                        if (this.BundleCourseStatus != CourseEnrollmentStatus.Full)
                        {
                            if (bundle.EnrollmentStatistics.EnrollmentStatus == CourseEnrollmentStatus.Full || bundle.EnrollmentStatistics.EnrollmentStatus == CourseEnrollmentStatus.WaitSpaceAvailable)
                            {
                                this.BundleCourseStatus = CourseEnrollmentStatus.Full;
                            }
                        }
                    }
                }
            }
        }

        public bool GenerateModelFormAdmin
        {
            private set;
            get;
        }

        public int CourseId
        {
            get
            {
                return Course.COURSEID;
            }
        }

        public Gsmu.Api.Data.School.Entities.Course Course
        {
            get;
            set;
        }

        //public bool AllowPartialPayment
        //{
        //    get
        //    { 
        //        return (Settings.Instance.GetMasterInfo3().AllowPartialPayment == 1) ? true : false;
        //    }
        //}

        public int PartialPaymentNon
        {
            get
            {
                int val = 0;
                int.TryParse(Course.PartialPaymentNon, out val);
                return val;
            }
        }

        public int PartialPaymentAmount
        {
            get
            {
                int val = 0;
                int.TryParse(Course.PartialPaymentAmount, out val);
                return val;
            }
        }

        public int PartialPaymentSP
        {
            get
            {
                int val = 0;
                int.TryParse(Course.PartialPaymentSP, out val);
                return val;
            }
        }

        public IEnumerable<PricingModel> PricingOptionsMember
        {
            get;
            set;
        }

        public IEnumerable<PricingModel> PricingOptionsNonMember
        {
            get;
            set;
        }

        public IEnumerable<PricingModel> PricingOptionsSpecial1
        {
            get;
            set;
        }

        public IEnumerable<Course_Time> CourseTimes
        {
            get
            {
                return courseTimes;
            }
            set
            {
                courseTimes = Gsmu.Api.Data.School.Entities.Course.FixCourseTimesForOnlineCourse(this.Course, value);
            }
        }

        public Course_Time CourseStart
        {
            get
            {
                if (CourseTimes == null || CourseTimes.Count() == 0)
                {
                    return null;
                }
                return CourseTimes.First();
            }
        }

        public DateTime? CourseStartAsDate
        {
            get
            {
                if (CourseTimes == null || CourseTimes.Count() == 0)
                {
                    return null;
                }
                var start = CourseTimes.First();
                try
                {
                    DateTime date = new DateTime(start.COURSEDATE.Value.Year, start.COURSEDATE.Value.Month, start.COURSEDATE.Value.Day, start.STARTTIME.Value.Hour, start.STARTTIME.Value.Minute, start.STARTTIME.Value.Second);
                    return date;
                }
                catch {
                    return null;
                }
            }
        }

        public DateTime? CourseEndAsDate
        {
            get
            {
                if (CourseTimes == null || CourseTimes.Count() == 0)
                {
                    return null;
                }

                Course_Time end;
                if (CourseTimes.Count() == 1)
                {
                    end = CourseTimes.First();
                }
                else
                {
                    end = CourseTimes.Last();
                }
                try
                {
                    DateTime date = new DateTime(end.COURSEDATE.Value.Year, end.COURSEDATE.Value.Month, end.COURSEDATE.Value.Day, end.FINISHTIME.Value.Hour, end.FINISHTIME.Value.Minute, end.FINISHTIME.Value.Second);
                    return date;
                }
                catch {
                    return null;
                }
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                if (CourseStartAsDate.HasValue && CourseEndAsDate.HasValue)
                {
                    return CourseEndAsDate - CourseStartAsDate;
                }
                return null;
            }
        }


        public MainCategory MainCategory
        {
            get;
            set;
        }

        public SubCategory SubCategory
        {
            get;
            set;
        }

        public short EventInternalClass
        {
            get;
            set;
        }

        public string InstructorName
        {
            get;
            set;
        }

        public SubSubCategory SubSubCategory
        {
            get;
            set;
        }
        public CourseEnrollmentStatus BundleCourseStatus
        {
            get;
            set;
        }


        /// <summary>
        /// This needs to check for the logged in user.
        /// Calculate the price based on the membership status.
        /// Patrik,
        /// User should be able to add courses to cart prior to login.
        /// When they have not login, we should default them with NoDistPrice. If the course has pricing options, it should use 
        /// [PricingOptions].NonPrice. Once they login, the membership will dictate the price. If they are a member,
        /// it will use DistPrice or 
        /// [PricingOptions].Price or [PricingOptions].SpecialMemberPrice1 depend on the settings.
        /// Currently, the course detail or hitlist are displaying both price Member and Non-Member. 
        /// So by that, we don’t have to worry about the
        /// users do not know how many prices available.
        /// When they login, we will need to recheck the existing cart, 
        /// alert the user and auto remove the duplicate course out of the cart.
        /// Let me know if you need further explaination.
        /// </summary>
        public List<PricingModel> GetCoursePrices(Student.MembershipType membershipType)
        {
            // if membership is turned off, default to member price - this default will need to 
            // reflect the system config on file "systemconfig_membership.asp" 
            if (!Student.MembershipHelper.MembershipEnabled && Settings.Instance.GetMasterInfo2().DefaultPublicPricingType == 0)
            {
                membershipType = Student.MembershipType.Member;
            }
            else if (!Student.MembershipHelper.MembershipEnabled && Settings.Instance.GetMasterInfo2().DefaultPublicPricingType == 1)
            {
                membershipType = Student.MembershipType.NonMember;
            }
            if (AuthorizationHelper.CurrentSupervisorUser != null || AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null)
            {
                var tempStudObj = Api.Data.School.Entities.Student.GetStudent(CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent);
                if (tempStudObj != null)
                {
                    membershipType = tempStudObj.MembershipType;
                }                    
            }

            var list = new List<PricingModel>();

            if (PricingOptionsHelper.PricingVisible)
            {
                if (Settings.GetVbScriptBoolValue(Course.DisplayPrice))
                {
                    PricingModel defltprice = GetDefaultCoursePrice(membershipType);
                    if (((defltprice.NonOptionPrice != 0) || ((Student.MembershipHelper.MembershipEnabled) && (Settings.Instance.GetMasterInfo4().DisplayNonDefaultMembership == 1))) || (membershipType == Student.MembershipType.Special1))
                    {
                        list.Add(GetDefaultCoursePrice(membershipType));
                    }
                }

                List<PricingModel> effective;
                List<PricingModel> disabledprice;
                List<PricingModel> disabledspecialprice;
                if (Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper.GetConfig().useClubPilates == 1 && Course.clubready_package_id > 0)
                {
                    Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper ClubPilatesHelper = new Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper();
                    PricingModel clubpilateprices = new PricingModel();
                    foreach (var installment in ClubPilatesHelper.GetPackageInstallmentDetails(Course.clubready_package_id.Value))
                    {
                        clubpilateprices.ClubReadyPriceDescription = ClubPilatesHelper.GetPackageDetails(Course.clubready_package_id.Value).Description;
                        clubpilateprices.NonOptionPrice = decimal.Parse(installment.DueAmountPerPayment);
                        clubpilateprices.MembershipType = MembershipType.Member;
                        clubpilateprices.Disabled = false;
                        clubpilateprices.ClubReadyInstallmentId = installment.installmentId;
                        clubpilateprices.CoursePricingOption = new CoursePricingOption();
                        clubpilateprices.CoursePricingOption.Price = decimal.Parse(installment.DueAmountPerPayment);
                        clubpilateprices.CoursePricingOption.PriceTypedesc = ClubPilatesHelper.GetPackageDetails(Course.clubready_package_id.Value).Description;
                        clubpilateprices.CoursePricingOption.CoursePricingOptionId = installment.installmentId;


                        clubpilateprices.PricingOption = new PricingOption();
                        clubpilateprices.PricingOption.Price = decimal.Parse(installment.DueAmountPerPayment);
                        clubpilateprices.PricingOption.PriceTypedesc = ClubPilatesHelper.GetPackageDetails(Course.clubready_package_id.Value).Description;
                        clubpilateprices.PricingOption.PricingOptionID = installment.installmentId;
                        clubpilateprices.PricingOption.NonPrice = decimal.Parse(installment.DueAmountPerPayment);
                        list.Add(clubpilateprices);
                        clubpilateprices = new PricingModel();


                    }
                }
                else
                {
                    switch (membershipType)
                    {
                        case Student.MembershipType.Member:
                            if (!Student.MembershipHelper.MembershipEnabled)
                            {
                                effective = (from pm in this.PricingOptionsMember where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();
                            } else
                            {
                                effective = (from pm in this.PricingOptionsMember where pm.EffectivePrice >= 0 && pm.IsAvailable(this) && (pm.CoursePricingOption.Type ==0) orderby pm.Label select pm).ToList();
                            }
                            
                            if (Settings.Instance.GetMasterInfo4().DisplayNonDefaultMembership == 1)
                            {
                                disabledprice = (from pm in this.PricingOptionsNonMember where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();
                                disabledspecialprice = (from pm in this.PricingOptionsSpecial1 where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();
                                if (Settings.GetVbScriptBoolValue(Course.DisplayPrice))
                                {
                                    PricingModel defltprice = GetDefaultCoursePrice(Student.MembershipType.NonMember);
                                    if (defltprice.NonOptionPrice != 0)
                                    {
                                        defltprice.Disabled = true;
                                        list.Add(defltprice);
                                    }

                                    PricingModel defltspecialprice = GetDefaultCoursePrice(Student.MembershipType.Special1);
                                    if (defltspecialprice.NonOptionPrice != 0)
                                    {
                                        defltspecialprice.Disabled = true;
                                        list.Add(defltspecialprice);
                                    }


                                }
                            }
                            else
                            {
                                disabledprice = null;
                                disabledspecialprice = null;
                            }

                            break;

                        case Student.MembershipType.NonMember:
                            if (!Student.MembershipHelper.MembershipEnabled)
                            {
                                effective = (from pm in this.PricingOptionsNonMember where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();
                            } else
                            {
                                effective = (from pm in this.PricingOptionsNonMember where pm.EffectivePrice >= 0 && pm.IsAvailable(this) && pm.CoursePricingOption.Type == 1 orderby pm.Label select pm).ToList();

                            }                                

                            if (Settings.Instance.GetMasterInfo4().DisplayNonDefaultMembership == 1)
                            {
                                disabledprice = (from pm in this.PricingOptionsMember where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();
                                disabledspecialprice = (from pm in this.PricingOptionsSpecial1 where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();

                                if (Settings.GetVbScriptBoolValue(Course.DisplayPrice))
                                {
                                    PricingModel defltprice = GetDefaultCoursePrice(Student.MembershipType.NonMember);
                                    defltprice = GetDefaultCoursePrice(Student.MembershipType.Member);
                                    if (defltprice.NonOptionPrice != 0)
                                    {
                                        defltprice.Disabled = true;
                                        list.Add(defltprice);
                                    }


                                    PricingModel defltspecialprice = GetDefaultCoursePrice(Student.MembershipType.Special1);
                                    if (defltspecialprice.NonOptionPrice != 0)
                                    {
                                        defltspecialprice.Disabled = true;
                                        list.Add(defltspecialprice);
                                    }
                                }
                            }
                            else
                            {
                                disabledprice = null;
                                disabledspecialprice = null;
                            }
                            break;

                        case Student.MembershipType.Special1:
                            effective = (from pm in this.PricingOptionsSpecial1 where pm.EffectivePrice >= 0 && pm.IsAvailable(this) orderby pm.Label select pm).ToList();
                            disabledprice = null;
                            disabledspecialprice = null;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    foreach (var price in effective)
                    {
                        price.Disabled = false;

                    }
                    list.AddRange(effective);
                    if (disabledprice != null)
                    {
                        disabledprice.Select(c => { c.Disabled = true; return c; }).ToList();
                        list.AddRange(disabledprice);
                    }
                    if (disabledspecialprice != null)
                    {
                        disabledspecialprice.Select(c => { c.Disabled = true; return c; }).ToList();
                        list.AddRange(disabledspecialprice);
                      
                    }

                }
            }
            // if PricingVisible is false must have no display
            //else
            //{
            //    list.Add(GetDefaultCoursePrice(membershipType));
            //}
            list = list.Distinct().ToList();
            return list;
        }


        /// <summary>
        /// Returns the price of the course for the current user logged in or the guest user.
        /// </summary>
        public List<PricingModel> EffectivePrices
        {
            get
            {
                if (SelectedStudentMembershipType == null)
                {
                    return GetCoursePrices(AuthorizationHelper.CurrentUser.MembershipType);
                }
                else
                {
                    if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
                    {
                        return GetCoursePrices(SelectedStudentMembershipType);
                    }
                    else
                    {
                        return GetCoursePrices(AuthorizationHelper.CurrentUser.MembershipType);
                    }
                }

            }
            set
            {

                GetCoursePrices(SelectedStudentMembershipType);

            }


        }
        public MembershipType SelectedStudentMembershipType
        {
            get
            {

                if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
                {
                    try
                    {
                        var user = Gsmu.Api.Data.School.Entities.Student.GetStudent(CourseShoppingCart.Instance.MultipleOrder_SelectedStudent);
                        return user.MembershipType;

                    }
                    catch
                    {
                        try
                        {
                            var user = Gsmu.Api.Data.School.Entities.Student.GetStudent(CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent);
                            return user.MembershipType;
                        }
                        catch
                        {
                            return MembershipType.Member;
                        }
                    }

                }
                else
                {
                    return AuthorizationHelper.CurrentUser.MembershipType;
                }


            }
            set
            {
                if (AuthorizationHelper.CurrentStudentUser != null)
                {
                    MembershipHelper.GetMembershipType(AuthorizationHelper.CurrentStudentUser.DISTEMPLOYEE);
                }
            }
        }
        public String CertificateProgram
        {
            get
            {
                Certificate.UserCertificate certificateinfo = new Certificate.UserCertificate();
                if (Settings.Instance.GetMasterInfo2().CertificationsOnOff == 2)
                {
                    return certificateinfo.GetCertificateName(Course.CustomCourseField5);
                }
                else
                {
                    return certificateinfo.GetCertificateName(Course.COURSENUM);
                }
            }
        }

        public bool IsCourseHasPricingOptionInRange
        {
            get
            {
                return GetCoursePricingOptionInRange(AuthorizationHelper.CurrentUser.MembershipType);
            }
        }
        private bool GetCoursePricingOptionInRange(Student.MembershipType membershipType)
        {
            if (!Student.MembershipHelper.MembershipEnabled)
            {
                membershipType = Student.MembershipType.Member;
            }
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                int coursepricingoptions = 0;
                switch (membershipType)
                {
                    case Student.MembershipType.Member:
                        coursepricingoptions = (from c in db.CoursePricingOptions where c.CourseId == this.CourseId && c.Type == 0 select c).Count();
                        break;
                    case Student.MembershipType.NonMember:
                        coursepricingoptions = (from c in db.CoursePricingOptions where c.CourseId == this.CourseId && c.Type == 1 select c).Count();
                        break;
                    case Student.MembershipType.Special1:
                        coursepricingoptions = (from c in db.CoursePricingOptions where c.CourseId == this.CourseId && c.Type == 3 select c).Count();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (coursepricingoptions > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private PricingModel GetDefaultCoursePrice(Student.MembershipType membershipType)
        {
            // since this method is called internally we dont have to pay attention to the membership type
            // if we start using this from outside, uncomment this:
            // if membership is turned off, default to member price
            //if (!Student.MembershipHelper.MembershipEnabled)
            //{
            //    membershipType = Student.MembershipType.Member;
            //}
            if (CourseShoppingCart.Instance.CountMembership>0)
            {
                membershipType = MembershipType.Member;
            }

            switch (membershipType)
            {
                case Student.MembershipType.Member:
                    return new PricingModel()
                    {
                        NonOptionPrice = Course.DISTPRICE ?? 0,
                        MembershipType = membershipType,
                        Disabled = false
                    };

                case Student.MembershipType.NonMember:
                    return new PricingModel()
                    {
                        NonOptionPrice = Course.NODISTPRICE ?? 0,
                        MembershipType = membershipType,
                        Disabled = false
                    };

                case Student.MembershipType.Special1:
                    return new PricingModel()
                    {
                        NonOptionPrice = Course.SpecialDistPrice1 ?? 0,
                        MembershipType = membershipType
                    };

                default:
                    throw new NotImplementedException();
            }
        }

        public List<Material> Materials
        {
            get
            {
                if (this.materials != null)
                {
                    return this.materials;
                }

                this.materials = new List<Material>();
                var list = string.Empty;
                if (this.Course != null)
                {
                    list = this.Course.MATERIALS ?? string.Empty;
                }
                list = list.Trim();

                if (string.IsNullOrEmpty(list))
                {
                    return this.materials;
                }

                var listSplit = list.Split(',');

                List<int> materialIdList = new List<int>();
                foreach (var id in listSplit)
                {
                    var validIdString = id.Replace("~", "");
                    int validId;
                    if (int.TryParse(validIdString, out validId))
                    {
                        materialIdList.Add(validId);
                    }
                }
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;

                    this.materials = (from m in db.Materials where materialIdList.Contains(m.productID) orderby m.product_num select m).ToList();

                    for (int x = 0; x < this.materials.Count(); x++)
                    {
                        if (this.materials[x].quantity > 0)
                        {
                            this.materials[x].quantity = (this.materials[x].quantity - getMaterialQtyPurchased(this.materials[x].productID));
                        }
                    }
                }

                return this.materials;
            }
        }
        public int getMaterialQtyPurchased(int material_id)
        {
            using (var db = new SchoolEntities())
            {
                if (db.rostermaterials.Where(m => m.productID == material_id).Count() > 0)
                {
                    if (db.rostermaterials.Where(m => m.productID == material_id).Sum(m => m.qty_purchased).HasValue)
                        return (int)db.rostermaterials.Where(m => m.productID == material_id).Sum(m => m.qty_purchased).Value;
                    else
                        return 0;
                }
                return 0;
            }
        }
        public List<Instructor> Instructors
        {
            get
            {
                if (this.instructor != null)
                {
                    return this.instructor;
                }
                List<int> instructorIdList = new List<int>();
                instructorIdList.Add(this.Course.INSTRUCTORID ?? 0);
                instructorIdList.Add(this.Course.INSTRUCTORID2 ?? 0);
                instructorIdList.Add(this.Course.INSTRUCTORID3 ?? 0);

                string instructor_string = string.Join(",", instructorIdList.ToString());

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;

                    //******* Get List of Instructors from DB base on the selected course, this is not yet sorted. CHarIndex is interpreted differently on LINQ and Entities Query.
                    //Maximum of 3 will be on the list.
                    var instructors_raw = (from m in db.Instructors
                                           where instructorIdList.Contains(m.INSTRUCTORID)
                                           select m).ToList();

                    //*** Creat new Instructor List Instance.
                    this.instructor = new List<Instructor>();
                    foreach (var instid in instructorIdList) // Loop on all selected instructor id.
                    {
                        if (instid != 0)
                        {
                            //Insert Instructor sorted base on how it was listed on InstructorID List.
                            //This is query is no longer communicating on DB. It only check values on List of selected instructor.
                            this.instructor.Add(
                                (from a in instructors_raw where a.INSTRUCTORID == instid select a).FirstOrDefault()
                                );
                        }
                    }
                }

                return this.instructor;
            }
        }

        public List<Icon> Icons
        {
            get
            {
                try
                {
                    if (this.icons != null)
                    {
                        return this.icons;
                    }
                    string Iconarray = "";
                    try
                    {
                        Iconarray = this.Course.Icons;
                    }
                    catch (Exception)
                    {
                    }
                    List<int> IconIdList = new List<int>();
                    foreach (string ic in Iconarray.Replace("~", "").Split('|'))
                    {
                        int validId;
                        if (int.TryParse(ic, out validId))
                        {
                            IconIdList.Add(validId);
                        }
                    }
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        this.icons = (from m in db.Icons where IconIdList.Contains(m.IconsID) select m).ToList();
                    }

                }
                catch (Exception)
                {
                }
                return this.icons;
            }
        }

        public List<int> Courses
        {
            get
            {
                if (this.courseids != null)
                {
                    return this.courseids;
                }
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;

                    var ShowPastOnlineCourses = Settings.Instance.GetMasterInfo().ShowPastOnlineCoursesAsBoolean;
                    var showCourseNum = ((Settings.Instance.GetMasterInfo2().HideCourseNumber == 0) ? true : false);
                    if ((!WebConfiguration.HideAdditionalCourseOffering) && showCourseNum)
                    {
                        var courses = (from m in db.Courses
                                       //join ct in db.Course_Times on m.COURSEID equals ct.COURSEID
                                       where m.COURSENUM == this.Course.COURSENUM && m.COURSEID != this.Course.COURSEID
                                       && m.CANCELCOURSE == 0 && m.InternalClass == 0
                                       //&& ct.COURSEDATE.Value >= DateTime.Today
                                       //orderby ct.COURSEDATE ascending
                                       select new
                                       {
                                           m.COURSEID,
                                           MinDate = (from ct2 in db.Course_Times where ct2.COURSEID == m.COURSEID select ct2.COURSEDATE).Min(),
                                           MaxDate = (from ct2 in db.Course_Times where ct2.COURSEID == m.COURSEID select ct2.COURSEDATE).Max(),
                                           OnlineCourse = m.OnlineCourse
                                       })
                                       .Distinct()
                                       .ToList();


                        courseids = new List<int>();
                        foreach (var crs in courses.OrderBy(x => x.MinDate))
                        {
                            var addthiscourse = false;
                            if ((crs.MinDate != null) && (crs.MaxDate != null))
                            {
                                if (crs.MinDate.Value >= DateTime.Today && crs.MaxDate.Value >= DateTime.Today)
                                {
                                    addthiscourse = true;
                                }

                                if (ShowPastOnlineCourses && crs.OnlineCourse == 1 && crs.MinDate.Value < DateTime.Today && crs.MaxDate.Value >= DateTime.Today)
                                {
                                    addthiscourse = true;
                                }
                            }
                            if (addthiscourse)
                            {
                                courseids.Add(crs.COURSEID);
                            }
                        }

                    }            //if (courseids.Count > 0)
                    //{
                    //    var courseid = courses.Select(p => p.COURSEID).Distinct().ToList();
                    //    this.courseids = courseid;
                    //}
                }
                return this.courseids;
            }
        }
        public CourseSettings CourseSettings
        {
            get
            {
                var Setting = new CourseSettings();
                DateTime now = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);

                //TIME ZONE
                if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 1)
                {
                    Setting.TimezoneAbv = "MST"; // Mountain (1)
                }
                else if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 2)
                {
                    Setting.TimezoneAbv = "CST"; // Central (2)
                }
                else if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 3)
                {
                    Setting.TimezoneAbv = "EST"; // Eastern (3)
                }
                else if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 0)
                {
                    Setting.TimezoneAbv = "PST"; // Pacific (0)
                }
                else
                {
                    Setting.TimezoneAbv = ""; // other value means timezone is not in used. (99). If need more timezone then just add the number accordingly.
                }
                if (Settings.Instance.GetMasterInfo3().system_timezone_hour != 999)
                {
                    Setting.TimezoneAddHour = Settings.Instance.GetMasterInfo3().system_timezone_hour.Value;
                }
                else
                {
                    Setting.TimezoneAddHour = 0;
                }



                String vOnlineClasslabel = "";
                String vmi3OnlineClasslabel = Settings.Instance.GetMasterInfo3().OnlineClassLabel.ToString();
                if (this.Course.OnlineCourse == 1 && this.Course.coursetype == 0)
                {
                    if (!string.IsNullOrEmpty(vmi3OnlineClasslabel))
                    {
                        vOnlineClasslabel = vmi3OnlineClasslabel;
                    }
                    else
                    {
                        vOnlineClasslabel = " Online Course ";
                    }
                    if (this.Course.coursetype == 1) { vOnlineClasslabel = " Conference/Event "; }
                }
                Setting.OnlineClasslabel = vOnlineClasslabel;


                String vCourseDateTimeHeader1st = "Start Date(s)";
                String vCourseDateTimeHeader2nd = "End Date(s)";
                Boolean vShowCourseDateTime = false;
                if (this.Course.OnlineCourse == 0)
                {
                    vCourseDateTimeHeader1st = "Date(s)";
                    vCourseDateTimeHeader2nd = "Time(s)";
                    vShowCourseDateTime = true;
                }
                if (this.Course.coursetype == 1)
                {
                    vCourseDateTimeHeader1st = "Start Date";
                    vCourseDateTimeHeader2nd = "End Date";
                    vShowCourseDateTime = true;
                }

                Setting.CourseDateTimeHeader1st = vCourseDateTimeHeader1st;
                Setting.CourseDateTimeHeader2nd = vCourseDateTimeHeader2nd;
                Setting.ShowCourseDateTime = vShowCourseDateTime;



                var db = new SchoolEntities();
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                var enrollmentStatistics = this.Course.EnrollmentStatistics;
                int MAXENROLL = enrollmentStatistics.MaxEnrolledRosterCount;
                int ENROLLED = enrollmentStatistics.EnrolledRosterCount;
                int MAXWAIT = enrollmentStatistics.MaxWaitingRosterCount;
                int WAITING = enrollmentStatistics.WaitingRosterCount;

                int ENROLLEDAVAIL = enrollmentStatistics.SpaceAvailable;
                int WAITINGAVAIL = enrollmentStatistics.WaitSpaceAvailable;





                Setting.MAXENROLL = MAXENROLL;
                Setting.ENROLLED = ENROLLED;
                Setting.ENROLLEXTRA = 0;
                Setting.MAXWAIT = MAXWAIT;
                Setting.WAITING = WAITING;
                Setting.ENROLLEDAVAIL = ENROLLEDAVAIL;
                Setting.ENROLLEDEXTRA = 0;

                String vstatus = enrollmentStatistics.Status;


                // ******************************************************
                // ******************************************************
                // ******************************************************
                // ******************************************************
                // these values are not properly working
                // the correct status is in courseenrollmnet statistic 
                // ******************************************************
                // ******************************************************
                // ******************************************************
                // ******************************************************

                //CLOSE REGISTRATION / ENROLLMENT
                Boolean bEnrollmentClosed = false;
                String strCourseCloseDateTime = "";
                DateTime dtCourseCloseDateTime = new DateTime();
                DateTime dtCourseStartTime = new DateTime();
                String strStartDate = "";

                int CourseCloseDaysGlobal = Settings.Instance.GetMasterInfo().CourseCloseDays.HasValue ? Settings.Instance.GetMasterInfo().CourseCloseDays.Value : 0;
                int CourseCloseDaysCourse = this.Course.CourseCloseDays.HasValue ? this.Course.CourseCloseDays.Value : 0;
                int CourseCloseDays = CourseCloseDaysCourse > 0 ? CourseCloseDaysCourse : CourseCloseDaysGlobal;
                int CourseViewPastCoursesDays = this.Course.viewpastcoursesdays.HasValue ? this.Course.viewpastcoursesdays.Value : 0;

                if (this.CourseTimes.Count() > 0)
                {
                    dtCourseCloseDateTime = this.CourseTimes.First().COURSEDATE.Value;
                    DateTime dtCourseStartDate = this.CourseTimes.First().COURSEDATE.Value;
                    strStartDate = dtCourseCloseDateTime.ToShortDateString();
                    TimeSpan SpanDifferent = (dtCourseCloseDateTime - DateTime.Now);
                    try
                    {
                        dtCourseStartTime = this.CourseTimes.First().STARTTIME.Value;
                        dtCourseStartTime = dtCourseStartTime.AddHours(Setting.TimezoneAddHour);
                    }
                    catch { }
                    dtCourseCloseDateTime = dtCourseCloseDateTime.AddHours(dtCourseStartTime.Hour);

                    if (CourseCloseDays > 0)
                    {
                        dtCourseCloseDateTime = dtCourseCloseDateTime.AddDays(CourseCloseDays * -1);
                    }

                    strCourseCloseDateTime = dtCourseCloseDateTime.ToString("ddd.  MMM d, yyyy") + " " + dtCourseCloseDateTime.ToShortTimeString();

                    if (now > dtCourseCloseDateTime || SpanDifferent.TotalDays <= CourseViewPastCoursesDays * -1)
                    {

                        // added the second condition to make all past courses to be open for enrollment. An urgent fix for ticekt:10506
                        if ((this.Course.OnlineCourse != 1) && (dtCourseStartDate >= now))
                        {
                            bEnrollmentClosed = true;
                            vstatus = TerminologyHelper.Instance.GetTermCapital(TermsEnum.Enrollment) + " Closed";
                        }
                        else
                        {
                            bEnrollmentClosed = false;
                        }
                    }

                }

                Setting.CourseCloseDate = strCourseCloseDateTime;
                Setting.EnrollmentClosed = bEnrollmentClosed;
                Setting.StartDate = strStartDate;





                if (ENROLLEDAVAIL < 1 && this.Course.coursetype == 0)
                {
                    vstatus = "Class Full";
                }
                else if (this.Course.coursetype == 0)
                {

                    vstatus = "Not Sure";
                    if (ENROLLEDAVAIL > 0) { vstatus = "Space Available"; }
                    else if (WAITINGAVAIL > 0) { vstatus = "Wait Space Available"; } else { vstatus = "Class Full"; }
                    if (this.Course.CANCELCOURSE == 1 || this.Course.CANCELCOURSE == -1) { vstatus = "Cancelled"; }

                    if (vstatus == TerminologyHelper.Instance.GetTermCapital(TermsEnum.Enrollment) + " reached the limit" || vstatus == "Class Full" || bEnrollmentClosed)
                    {

                        String CourseFullMessageLong = Settings.Instance.GetMasterInfo2().CourseFullMessageLong;
                        String CourseFullMessage = Settings.Instance.GetMasterInfo2().CourseFullMessage;

                        if (this.Course.OnlineCourse != 1)
                        {
                            if (!string.IsNullOrEmpty(CourseFullMessageLong))
                            {
                                vstatus = CourseFullMessageLong;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(CourseFullMessage))
                                {
                                    vstatus = CourseFullMessage;
                                }
                                else
                                {
                                    vstatus = "This class is no longer available for online registration.";
                                }
                            }
                        }
                    }
                    //else
                    //optional verbiage?

                }
                else if (this.Course.coursetype == 1)
                {

                    vstatus = "Space Not Available";
                    if (MAXENROLL > ENROLLED) { vstatus = "Space Available"; }
                    else if (ENROLLEDAVAIL < 0 && (WAITINGAVAIL > 0)) { vstatus = "Wait Space Available"; }
                    else { vstatus = "Class Full"; }

                }
                else
                {
                    vstatus = "Unidentified Status";
                }


                int HideSeatsAvailable = Settings.Instance.GetMasterInfo2().HideSeatsAvailable;
                string COURSENAME = this.Course.COURSENAME;
                Boolean HideTextSeatsAvailable = true;
                if (this.Course.coursetype != 1 && HideSeatsAvailable != 0)
                {
                    if (vstatus != TerminologyHelper.Instance.GetTermCapital(TermsEnum.Enrollment) + " reached the limit" && vstatus != "Class Full" && bEnrollmentClosed != true)
                    {

                        if (COURSENAME.IndexOf("materials only") > 0)
                        {
                            HideTextSeatsAvailable = true;
                        }
                        else
                        {
                            HideTextSeatsAvailable = false;
                        }
                    }
                }

                //if(bEnrollmentClosed){vstatus = TerminologyHelper.Instance.GetTermCapital(TermsEnum.Enrollment) + " Closed";}

                string TextSeatsAvailable = "";
                if (ENROLLED <= 1)
                {
                    TextSeatsAvailable = "There is " + ENROLLED + " student " + TerminologyHelper.Instance.GetTermCapital(TermsEnum.Enrolled) + " in this class.";
                }
                else
                {
                    TextSeatsAvailable = "There are " + ENROLLED + " student(s)  + TerminologyHelper.Instance.GetTermCapital(TermsEnum.Enrolled) +  in this class.";
                }
                if (ENROLLEDAVAIL <= 1)
                {
                    TextSeatsAvailable += "There is " + ENROLLEDAVAIL + " seat available at this time.";
                }
                else
                {
                    TextSeatsAvailable += "There are " + ENROLLEDAVAIL + " seat(s) available at this time.";
                }

                Setting.HideTextSeatsAvailable = HideTextSeatsAvailable;
                Setting.TextSeatsAvailable = TextSeatsAvailable;

                Setting.CLASSSTATUS = vstatus;

                Boolean vShowInstructor = false;
                if (Settings.Instance.GetMasterInfo2().ShowInstructor == 1) { vShowInstructor = true; }
                Setting.ShowInstructor = vShowInstructor;

                Boolean ShowOnlineClassDate = false;
                if (Settings.Instance.GetMasterInfo3().ShowOnlineClassDate == 1) { ShowOnlineClassDate = true; }
                Setting.ShowOnlineClassDate = ShowOnlineClassDate;

                Boolean IsOnlineCourse = false;
                if (this.Course.OnlineCourse == 1) { IsOnlineCourse = true; }
                Setting.IsOnlineCourse = IsOnlineCourse;







                return Setting;
            }
        }

        public List<CourseCredit> Credits
        {
            get
            {
                if (this.credits != null)
                {
                    return credits;
                }

                this.credits = new List<CourseCredit>();

                var availableCreditTypes = CourseCreditHelper.GetAvailableCreditTypesForCourse(this.Course, GenerateModelFormAdmin);

                foreach (var credit in availableCreditTypes)
                {
                    var creditAmount = GetCreditAmount(credit);
                    if (GenerateModelFormAdmin || creditAmount > 0)
                    {
                        var creditLabel = CourseCreditHelper.GetCreditLabel(credit);
                        var creditObject = new CourseCredit(credit, creditAmount, creditLabel);
                        credits.Add(creditObject);
                    }
                }

                return this.credits;
            }
        }


        public double GetCreditAmount(CourseCreditType credit)
        {
            switch (credit)
            {
                case CourseCreditType.Credit:
                    return Course.CREDITHOURS.HasValue ? Course.CREDITHOURS.Value : 0;

                case CourseCreditType.Ceu:
                    return Course.CEUCredit.HasValue ? Course.CEUCredit.Value : 0;

                case CourseCreditType.Graduate:
                    return Course.GraduateCredit.HasValue ? Course.GraduateCredit.Value : 0;

                case CourseCreditType.Custom:
                    return Course.CustomCreditHours.HasValue ? (double)Course.CustomCreditHours.Value : 0; ;

                case CourseCreditType.InService:
                    return Course.InserviceHours;

                case CourseCreditType.Optional1:
                    return Course.Optionalcredithours1.HasValue ? (double)Course.Optionalcredithours1.Value : 0;

                case CourseCreditType.Optional2:
                    return Course.Optionalcredithours2.HasValue ? (double)Course.Optionalcredithours2.Value : 0;

                case CourseCreditType.Optional3:
                    return Course.Optionalcredithours3.HasValue ? (double)Course.Optionalcredithours3.Value : 0;

                case CourseCreditType.Optional4:
                    return Course.Optionalcredithours4.HasValue ? (double)Course.Optionalcredithours4.Value : 0;

                case CourseCreditType.Optional5:
                    return Course.Optionalcredithours5.HasValue ? (double)Course.Optionalcredithours5.Value : 0;

                case CourseCreditType.Optional6:
                    return Course.Optionalcredithours6.HasValue ? (double)Course.Optionalcredithours6.Value : 0;

                case CourseCreditType.Optional7:
                    return Course.Optionalcredithours7.HasValue ? (double)Course.Optionalcredithours7.Value : 0;

                case CourseCreditType.Optional8:
                    return Course.Optionalcredithours8.HasValue ? (double)Course.Optionalcredithours8.Value : 0;

                default:
                    throw new NotImplementedException(credit.ToString() + " is not implemented for GetCreditLabel.");
            }
        }
        public int CreditOption
        {
            get
            {
                if ((this.Course.CourseConfiguration != "") && (this.Course.CourseConfiguration != null))
                {
                    try
                    {
                        string creditoption = "";
                        JavaScriptSerializer j = new JavaScriptSerializer();
                        dynamic configuration = j.Deserialize(this.Course.CourseConfiguration, typeof(object));
                        creditoption = configuration["creditoption"];
                        return int.Parse(creditoption);
                    }
                    catch
                    {
                        return 0;
                    }

                }
                return 0;

            }
        }

        public List<CourseChoice> CourseChoices
        {
            get
            {
                if (this.coursechoices != null)
                {
                    return this.coursechoices;
                }

                this.coursechoices = new List<CourseChoice>();

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;

                    //this.coursechoices = (from dbc in db.Courses.Include("CourseChoices") 
                    //                      where dbc.COURSEID == this.Course.COURSEID select dbc).First().CourseChoices.ToList();

                    var data = (from c in db.Courses.Include("CourseChoices")
                                where c.COURSEID == this.Course.COURSEID
                                select c).FirstOrDefault()
                                .CourseChoices.Select(cc => new CourseChoice()
                                {
                                    CourseChoice1 = cc.CourseChoice1,
                                    CourseChoiceId = cc.CourseChoiceId
                                }).ToList<CourseChoice>().OrderBy(c => c.CourseChoice1);
                    this.coursechoices = data.ToList<CourseChoice>();
                }
                return this.coursechoices.ToList();
            }
        }

        public List<Entities.Course> BundledCourses
        {
            get
            {
                if (this.childbundledcourses != null)
                {
                    return this.childbundledcourses;
                }

                this.childbundledcourses = new List<Entities.Course>();

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;

                    //add only child bundled courses that are in the future
                    this.childbundledcourses = (from c1 in db.Courses
                                                join ft in db.FastTrackCourses on c1.COURSEID equals ft.FTMainCourseId
                                                join c2 in db.Courses on ft.FTCourseId equals c2.COURSEID
                                                join ct2 in
                                                    (from t in db.Course_Times
                                                     group t by t.COURSEID into g
                                                     select new
                                                     {
                                                         CourseId = g.Key,
                                                         StartDate = g.Min(t => t.COURSEDATE),
                                                         StartTime = g.Min(t => t.STARTTIME)
                                                     })
                                                on c2.COURSEID equals ct2.CourseId
                                                where c1.COURSEID == CourseId //&& ct2.StartDate >= DateTime.Now
                                                orderby ct2.StartDate, ct2.StartTime
                                                select c2).ToList();

                    return this.childbundledcourses;
                }
            }
        }

        public List<int> ParentBundledCourseIds
        {
            get
            {
                if (this.parentbundledcourseids != null)
                {
                    return this.parentbundledcourseids;
                }

                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;

                    //add only parent bundled courses that are in the future
                    this.parentbundledcourseids = (from c1 in db.Courses
                                                   join ft in db.FastTrackCourses on c1.COURSEID equals ft.FTMainCourseId
                                                   join c2 in db.Courses on ft.FTCourseId equals c2.COURSEID
                                                   join ct1 in
                                                       (from t in db.Course_Times
                                                        group t by t.COURSEID into g
                                                        select new
                                                        {
                                                            CourseId = g.Key,
                                                            StartDate = g.Min(t => t.COURSEDATE)
                                                        })
                                                   on c1.COURSEID equals ct1.CourseId
                                                   where c2.COURSEID == CourseId && ct1.StartDate >= DateTime.Now
                                                   orderby c1.COURSEID
                                                   select c1.COURSEID).ToList();

                    return this.parentbundledcourseids;
                }
            }
        }
    }
    public class CourseBundleViewModel {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseNumber { get; set; }
    }
}
