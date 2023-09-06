using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Web;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.Supervisor;

namespace Gsmu.Api.Commerce.ShoppingCart
{
    public class CourseShoppingCart
    {
        #region Static 

        public static CourseShoppingCart Instance
        {
            get
            {
                var cart = ObjectHelper.GetSessionObject<CourseShoppingCart>(WebContextObject.ShoppingCart);
                if (cart == null)
                {
                    cart = new CourseShoppingCart();
                    ObjectHelper.SetSessionObject<CourseShoppingCart>(WebContextObject.ShoppingCart, cart);
                }                
                return cart;
            }
        }

        #endregion

        internal List<CourseShoppingCartItem> items = new List<CourseShoppingCartItem>();

        /// <summary>
        /// The count of items in the cart.
        /// </summary>
        /// 
        public int? SelectedCourseID
        {
            get;
            set;
        }
        public int Count
        {
            get {
                return items.Count;
            }
        }
        public Guid ReservationId
        {
            get;
            set;
        }
        public string ReservationExpired
        {
            get;
            set;
        }
        public int CountMembership
        {
            get
            {
                return items.Where(c => c.Course.COURSENUM == "~ZZZZZZ~").Count();                  
            }
        }

        public int CountNonBundleCourses
        {
            get {
                var count = (from c in items where !c.IsBundledCourse select c).Count();
                return count;
            }
        }

        /// <summary>
        /// The read only items , because the list for now is managed by the cart.
        /// </summary>
        public IEnumerable<CourseShoppingCartItem> Items
        {
            get
            {
                return items.AsEnumerable();
            }

        }

        public List<CourseMultipleStudentItem> MultipleStudentCourses
        {
            get;
            set;
        }

        public int MultipleOrder_PrincipalStudent
        {
            get;
            set;
        }

        public string MultipleOrder_PrincipalStudentName
        {
            get;
            set;
        }

        public string CurrentOrderPaymntMode
        {
            get;
            set;
        }
        public int MultipleOrder_SelectedStudent
        {
            get;
            set;
        }
        public int MultipleOrder_PricingOptionId
        {
            get;
            set;
        }
        public double SalesTaxPercentage
        {
            get;
            set;
        }

        public bool AdminCheckAccessCode
        {
            get;
            set;
        }

        public bool AdminCheckCourseRequirements
        {
            get;
            set;
        }

        public string CurrentOrderNumber
        {
            get;
            set;
        }


        /// <summary>
        /// Simple string status of the shopping cart.
        /// </summary>
        public string Status
        {
            get
            {
                int membrspCount = MembershipShoppingCart.Instance.Count;
                int courseCount = items.Where(course => course.Course.CourseType == CourseType.Course).Count();
                int totalCount = membrspCount + courseCount;

                if (totalCount == 0)
                {
                    return "Empty";
                }
                else if (totalCount == 1)
                {
                    return "1 item";
                }
                else
                {
                    return totalCount + " items";
                }

            }
        }

        public void SetSalesTaxPercentage(string State)
        {
            this.SalesTaxPercentage = 0;
            using (var db = new SchoolEntities())
            {
                var salestax = (from a in db.SalesTaxes where a.State == State select a.SalesTax1).FirstOrDefault();
                if (salestax != null)
                {
                    this.SalesTaxPercentage = double.Parse(salestax.ToString()) ;
                }
            }

        }

        /// <summary>
        /// Adds a course to the cart if not already there.
        /// Should check existing course in cart.
        /// Need to check enrollments!
        /// </summary>
        public void AddCourse(int courseId, int? coursePricingOptionId = null, int? courseChoiceId = 0, List<int> materialIds = null, int eventParent = 0, List<int> eventIds = null, List<string> selectedcredits = null, string accessCode = null, bool freeCourseLikePartOfBundle = false, bool isBundledCourse = false, int? bundleParentCourseId = 0, List<CourseExtraParticipant> extraParticipants = null, int studentId = 0, List<int> qty = null, bool? passrequirements= false)
        {
            if (MultipleStudentCourses == null)
            {
                MultipleStudentCourses = new List<CourseMultipleStudentItem>();
            }
           
            var model = new CourseModel(courseId);
            if (model.Course.EnrollmentStatistics.EnrollmentStatus == CourseEnrollmentStatus.Full)
            {
                throw new Exception("There's no space available. Please refresh the course list to update the availability of the course.");
            }

            if (CourseShoppingCart.Instance.GetCourseItem(courseId) != null)
            {
                if (CourseShoppingCart.Instance.GetCourseItem(courseId).PricingModel != null)
                {
                    if (CourseShoppingCart.Instance.GetCourseItem(courseId).PricingModel.CoursePricingOption != null)
                    {
                        coursePricingOptionId = CourseShoppingCart.Instance.GetCourseItem(courseId).PricingModel.CoursePricingOption.CoursePricingOptionId;
                    }
                    else
                    {
                        coursePricingOptionId = 0;
                    }
                }
            }

            var student = new Student();
            CourseMultipleStudentItem newStudent = new CourseMultipleStudentItem();
            if (studentId == 0)
            {
               
                student = AuthorizationHelper.CurrentUser as Student;
                if (student != null)
                {
                    SetSalesTaxPercentage(student.STATE);
                }
            }
            else
            {
                student = Student.GetStudent(studentId);
                newStudent.CourseId = courseId;
                newStudent.StudentId = studentId;
                CourseShoppingCart.Instance.MultipleOrder_SelectedStudent = studentId;
                model.SelectedStudentMembershipType = MembershipHelper.GetMembershipType(student.DISTEMPLOYEE);
                model.EffectivePrices = model.GetCoursePrices(model.SelectedStudentMembershipType);
                if (this.MultipleOrder_PrincipalStudent != 0)
                {
                    SetSalesTaxPercentage(Student.GetStudent(this.MultipleOrder_PrincipalStudent).STATE);
                }

            }

            
            PricingModel pricingModel = null;

            if (!freeCourseLikePartOfBundle)
            {
                if (coursePricingOptionId > 0)
                {
                    if (Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper.GetConfig().useClubPilates == 1 && model.Course.clubready_package_id > 0)
                    {
                        Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper ClubPilatesHelper = new Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper();
                        var pilatepricing = ClubPilatesHelper.GetPackageInstallmentDetails(model.Course.clubready_package_id.Value).Where(item => item.installmentId == coursePricingOptionId).FirstOrDefault();
                        if (pilatepricing != null)
                        {
                            PricingModel clubpilateprices = new PricingModel();
                            clubpilateprices.ClubReadyPriceDescription = ClubPilatesHelper.GetPackageDetails(model.Course.clubready_package_id.Value).Description;
                            clubpilateprices.NonOptionPrice = decimal.Parse(pilatepricing.DueAmountPerPayment);
                            clubpilateprices.MembershipType = MembershipType.Member;
                            clubpilateprices.Disabled = false;
                            clubpilateprices.ClubReadyInstallmentId = pilatepricing.installmentId;
                            clubpilateprices.CoursePricingOption = new CoursePricingOption();
                            clubpilateprices.CoursePricingOption.Price = decimal.Parse(pilatepricing.DueAmountPerPayment);
                            clubpilateprices.CoursePricingOption.PriceTypedesc = ClubPilatesHelper.GetPackageDetails(model.Course.clubready_package_id.Value).Description;
                            clubpilateprices.CoursePricingOption.CoursePricingOptionId = pilatepricing.installmentId;


                            clubpilateprices.PricingOption = new PricingOption();
                            clubpilateprices.PricingOption.Price = decimal.Parse(pilatepricing.DueAmountPerPayment);
                            clubpilateprices.PricingOption.PriceTypedesc = ClubPilatesHelper.GetPackageDetails(model.Course.clubready_package_id.Value).Description;
                            clubpilateprices.PricingOption.PricingOptionID = pilatepricing.installmentId;
                            clubpilateprices.PricingOption.NonPrice = decimal.Parse(pilatepricing.DueAmountPerPayment);
                            pricingModel = clubpilateprices;
                        }

                    }
                    else
                    {
                        try
                        {
                            if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
                            {
                                pricingModel = (from pm in model.EffectivePrices where pm.CoursePricingOption != null && pm.CoursePricingOption.CoursePricingOptionId == coursePricingOptionId select pm).First();
                                MultipleOrder_PricingOptionId = pricingModel.CoursePricingOption.PricingOptionId;
                            }
                            else
                            {
                                pricingModel = (from pm in model.EffectivePrices where pm.CoursePricingOption != null && pm.CoursePricingOption.CourseId == courseId && pm.CoursePricingOption.CoursePricingOptionId == coursePricingOptionId select pm).First();

                            }
                        }
                        catch
                        {
                            pricingModel = (from pm in model.EffectivePrices where pm.CoursePricingOption != null && pm.CoursePricingOption.CourseId == courseId && pm.CoursePricingOption.PricingOptionId == CourseShoppingCart.Instance.MultipleOrder_PricingOptionId select pm).First();
                        }
                    }
                }
                else
                {
                    if ((model.IsCourseHasPricingOptionInRange) && (!coursePricingOptionId.HasValue))
                    {
                        throw new Exception("Please select a pricing option.");
                    }
                    else
                    {
                        pricingModel = (from pm in model.EffectivePrices where pm.IsOption == false select pm).FirstOrDefault();
                    }
                }
            }

            if (PricingOptionsHelper.ExtraParticipantCollectionEnabled && pricingModel != null && pricingModel.PricingOption != null && pricingModel.PricingOption.CollectExtraParticipants && (extraParticipants == null || extraParticipants.Count < 1))
            {
                CourseEnrollmentStatistics stats = null;
                stats = model.Course.EnrollmentStatistics;
                int spaceAvailable = stats.SpaceAvailable < 0 ? 0 : stats.SpaceAvailable;
                int waitSpace = stats.WaitSpaceAvailable < 0 ? 0 : stats.WaitSpaceAvailable;

                if (spaceAvailable + waitSpace > 1)
                {
                    if (model.Course.eventid == 0 || model.Course.eventid == null)
                    {
                        throw new CourseExtraParticipantMissingException("For the selected pricing options, we require that the list of extra participants is provided!");
                    }
                } 
            }

            CourseChoice courseChoice = null;
            if (courseChoiceId > 0)
            {
                courseChoice = (from cc in model.CourseChoices where cc.CourseChoiceId == courseChoiceId select cc).First();
            }

            List<Material> materials = null;
            if (materialIds != null && materialIds.Count != 0) {
                using (var db = new SchoolEntities())
                {
                    int index = 0;
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    materials = (from m in db.Materials where materialIds.Contains(m.productID) select m).ToList();
                    if (qty != null)
                    {
						if( qty.Count() > 0){
							foreach (var material in materials)
							{
								material.QuantityPurchased = qty[index];
								if ((material.hidematerialprice == 0) || (material.hidematerialprice ==null))
								{
									material.price = material.price * qty[index];
								}
								else
								{
									material.price = 0;
								}
								index++;
							}
						}
                    }
                }
            }

            //Parent Event
            bool hiddenInCart = false;
            if (eventIds != null)
            {
                using (var db = new SchoolEntities())
                {
                    var checksessionduplicate = (from course in db.Courses where eventIds.Contains(course.COURSEID) select course).ToList();
                    var checksessionduplicateids = checksessionduplicate.Select(course => course.sessionid).ToList();
                    if (checksessionduplicateids.Count() != checksessionduplicateids.Distinct().Count())
                    {
                        throw new Exception("Error: Multiple courses in a session.");
                    }

                    var mainevent = checksessionduplicate.Where(event_ => event_.coursetype == 1).FirstOrDefault();// (from course in db.Courses where eventIds.Contains(course.COURSEID) && course.coursetype == 1 select course).FirstOrDefault();
                    if (mainevent != null)
                    {
                        var getallsessions = (from course in db.Courses where course.eventid == mainevent.COURSEID && course.coursetype == 2 && course.mandatory!=0 select course).ToList();
                        foreach (var mandatorysession in getallsessions)
                        {
                            if (checksessionduplicate.Where(session => session.sessionid == mandatorysession.COURSEID).FirstOrDefault() == null)
                            {
                                throw new Exception("Error: Course is required in a Mandatory session.");
                            }
                        }
                    }

                }
                
                foreach (var evntid in eventIds)
                {

                    var existing = GetCourseItem(evntid);
                    if (existing != null)
                    {
                        existing.HiddenInCart = false;
                    }
                }
                List<int> ExcldedEvents = null;
                foreach (var cartitem in items.Where(p => p.EventParent == courseId && p.HiddenInCart == true))
                {
                    ExcldedEvents.Add(cartitem.CourseId);
                }
                if (ExcldedEvents != null)
                {
                    foreach (var evntid in ExcldedEvents)
                    {
                        RemoveCourse(evntid);
                    }
                }
            }
            //Child Event
            if (eventParent > 0)
            {
                hiddenInCart = true;
            }

            AdminCheckAccessCode = false;
            if (!model.Course.IsAccessCodeValid(accessCode))
            {
                if (AuthorizationHelper.CurrentUser.LoggedInUserType != LoggedInUserType.Admin)
                {
                    throw new Exception("This course requires a valid access code.");
                }
                else
                {
                    AdminCheckAccessCode = true;
                    return; 
                }
            }

            if (model.Course.IsCancelled)
            {
                throw new Exception("The course you selected unfortunetely got cancelled.");
            }
            AdminCheckCourseRequirements = false;
            if (!CheckCourseRequirements(model.Course.COURSEID))
            {
                if (AuthorizationHelper.CurrentUser.LoggedInUserType != LoggedInUserType.Admin)
                {

                            throw new Exception("This course has restricted access. Your personal profile does not currently meet the requirements required to enroll in this course. - If you are not logged in you will not be able to add a restricted course to your cart.");
                }
                else
                {
                    if (passrequirements != null)
                    {
                        if (!passrequirements.Value)
                        {
                            AdminCheckCourseRequirements = true;
                            return;
                        }
                        else
                        {
                            AdminCheckCourseRequirements = false;
                        }
                    }
                    else
                    {
                        AdminCheckCourseRequirements = false;
                    }
                }
            }


            // need to check enrollment here dynamically based on logged in user!            
            if (student != null)
            {
                if (student.IsCurrentlyEnrolledInCourse(courseId))
                {
                    throw new Exception("You are already enrolled in this course.");
                }
                foreach (var course in model.BundledCourses)
                {
                    if (student.IsCurrentlyEnrolledInCourse(course.COURSEID))
                    {
                        throw new Exception("Sorry, but you are already enrolled in one of the courses in this bundle which prevents purchasing this bundle.");
                    }
                }

                if(studentId!=0)
                {
                   if( student.IsCurrentlyEnrolledInCourseForMultipleEnrollment(courseId,studentId))
                   {
                       throw new Exception("You are already enrolled in this course.");
                   }
                }
            }

            if ((selectedcredits == null) || (selectedcredits.Count <= 0))
            {
                if (model.CreditOption == 2 || model.CreditOption == 1)
                {
                    throw new Exception("Please select a credit option.");
                }
                else
                {
                }
            }


            if (!ContainsCourse(courseId))
            {
                List<CourseCredit> seelcted_credits = new List<CourseCredit>();
                if (selectedcredits != null)
                {
                    if (selectedcredits.Count > 0)
                    {
                        seelcted_credits = SetSelectedCredits(selectedcredits, model.Course);
                    }
                }
                items.Add(new CourseShoppingCartItem(courseId, pricingModel, courseChoice, materials, hiddenInCart, eventParent, eventIds, seelcted_credits, isBundledCourse, bundleParentCourseId, extraParticipants));

                AddBundledCourses(model, studentId);
                if (studentId != 0)
                {
                    ListingStudentModel student_details = SupervisorHelper.GetStudent(studentId);
                    newStudent.CourseId = courseId;
                    newStudent.StudentId = studentId;
                    newStudent.CourseName = model.Course.COURSENAME;
                    newStudent.FirstName = student_details.StudentFirstName;
                    newStudent.LastName = student_details.StudentLastName;
                    newStudent.UserName = student_details.UserName;
                    newStudent.PricingModel = pricingModel;
                    var coursemodel = new CourseModel(courseId);
                    var cart_student_course_count = MultipleStudentCourses.Where(c => c.CourseId == courseId).Count();
                    foreach (var s in MultipleStudentCourses)
                    {
                        if (s.StudentId == newStudent.StudentId)
                        {
                            newStudent.OrderNumber = s.OrderNumber;
                        }
                    }

                    if (newStudent.OrderNumber == "" || newStudent.OrderNumber==null)
                    {
                        EnrollmentFunction enroll = new EnrollmentFunction();
                        newStudent.OrderNumber = enroll.CheckGenerateOrderID(newStudent.StudentId);
                    }
                    if ((coursemodel.CourseSettings.ENROLLEDAVAIL - cart_student_course_count) <= 0)
                    {
                        newStudent.IsWaiting = 1;
                    }
                    else
                    {
                        newStudent.IsWaiting = 0;
                    }
                    MultipleStudentCourses.Add(newStudent);
                }
            }
            else
            {
                AddBundledCourses(model, studentId,1);
                if (studentId != 0)
                {
                    ListingStudentModel student_details = SupervisorHelper.GetStudent(studentId);
                    newStudent.CourseId = courseId;
                    newStudent.StudentId = studentId;
                    newStudent.CourseName = model.Course.COURSENAME;
                    newStudent.FirstName = student_details.StudentFirstName;
                    newStudent.LastName = student_details.StudentLastName;
                    newStudent.UserName = student_details.UserName;
                    newStudent.PricingModel = pricingModel;
                    var coursemodel = new CourseModel(courseId);
                    var cart_student_course_count = MultipleStudentCourses.Where(c => c.CourseId == courseId).Count();
                    foreach (var s in MultipleStudentCourses)
                    {
                        if (s.StudentId == newStudent.StudentId)
                        {
                            newStudent.OrderNumber = s.OrderNumber;
                        }
                    }

                    if (newStudent.OrderNumber == "" || newStudent.OrderNumber == null)
                    {
                        EnrollmentFunction enroll = new EnrollmentFunction();
                        newStudent.OrderNumber = enroll.CheckGenerateOrderID(newStudent.StudentId);
                    }
                
                    if ((coursemodel.CourseSettings.ENROLLEDAVAIL - cart_student_course_count) <= 0)
                    {
                        newStudent.IsWaiting = 1;
                    }
                    else
                    {
                        newStudent.IsWaiting = 0;
                    }
                    MultipleStudentCourses.Add(newStudent);
                }
                else
                {
                    throw new Exception("This course is already in your shopping cart.");
                }
                
                
            }
            using (var db = new SchoolEntities())
            {

                string AllowAutoReserveCourseOrder = System.Configuration.ConfigurationManager.AppSettings["AllowAutoReserveCourseOrder"];
                string ReserveOrderExpiry = System.Configuration.ConfigurationManager.AppSettings["ReserveOrderExpiry"];

                if (!string.IsNullOrEmpty(AllowAutoReserveCourseOrder))
                {
                    if (AllowAutoReserveCourseOrder == "true")
                    {
                        AutoReserveCourse autoReserveCourse = new AutoReserveCourse();
                        autoReserveCourse.CourseId = courseId;
                        if ((CourseShoppingCart.Instance.ReservationId != null) && (CourseShoppingCart.Instance.ReservationId.ToString() != "00000000-0000-0000-0000-000000000000"))
                        {
                            autoReserveCourse.ReservationId = CourseShoppingCart.Instance.ReservationId;
                        }
                        else
                        {
                            CourseShoppingCart.Instance.ReservationId = Guid.NewGuid();
                            autoReserveCourse.ReservationId = CourseShoppingCart.Instance.ReservationId;

                        }
                        try
                        {
                            autoReserveCourse.ExpiredTime = DateTime.Now.AddMinutes(int.Parse(ReserveOrderExpiry));
                        }
                        catch
                        {
                            autoReserveCourse.ExpiredTime = DateTime.Now.AddMinutes(30);
                        }
                        db.AutoReserveCourses.Add(autoReserveCourse);
                        db.SaveChanges();
                    }
                }
            }
        }

        public bool CheckCourseRequirements(int course_id,int studentid=0)
        {
            if (course_id == 0)
            {
                return true;
            }
            using (var db = new SchoolEntities())
            {
                bool requirement_check_status = false;
                bool requirement_check_school = false;
                bool requirement_check_district = false;
                bool requirement_check_grade = false;
                int validation_pass_count = 0;
                int? config_course_requirements = Settings.Instance.GetMasterInfo2().CourseRequirements;
                if (config_course_requirements == 2 || config_course_requirements == 1 || config_course_requirements == -1)
                {
                    var course_requirements = (from c in db.CoursesRequirements where c.CourseID == course_id select c).ToList();
                    if (course_requirements != null)
                    {
                        foreach (var coursereq in course_requirements)
                        {
                            int? student_district = AuthorizationHelper.CurrentStudentUser == null ? 0 : AuthorizationHelper.CurrentStudentUser.DISTRICT;
                            if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
                            {
                                student_district = (from s in db.Students where s.STUDENTID == CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent select s.DISTRICT).FirstOrDefault();
                            }
                            if (studentid != 0)
                            {
                                student_district = (from s in db.Students where s.STUDENTID == studentid select s.DISTRICT).FirstOrDefault();
                            }
                            if (student_district != 0)
                            {
                                if ((student_district == coursereq.Position))
                                {
                                    requirement_check_district = true;
                                    validation_pass_count = validation_pass_count + 1;
                                }
                            }
                            int? student_school= AuthorizationHelper.CurrentStudentUser == null ? 0 : AuthorizationHelper.CurrentStudentUser.SCHOOL;
                            if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
                            {
                                student_school =(from s in db.Students where s.STUDENTID == CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent select s.SCHOOL).FirstOrDefault();
                            }
                            if (studentid != 0)
                            {
                                student_school = (from s in db.Students where s.STUDENTID == studentid select s.SCHOOL).FirstOrDefault();
                            }
                            if (student_school != 0)
                            {
                                var sc_loc = (from c in db.Schools where c.locationid == coursereq.SchoolID select c.locationid).FirstOrDefault();
                                if (student_school == sc_loc)
                                {
                                    requirement_check_school= true;
                                    validation_pass_count = validation_pass_count + 1;
                                }
                            }

                            int? student_grade = AuthorizationHelper.CurrentStudentUser == null ? 0 : AuthorizationHelper.CurrentStudentUser.GRADE;
                            if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
                            {
                                student_grade = (from s in db.Students where s.STUDENTID == CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent select s.GRADE).FirstOrDefault(); 
                            }
                            if (studentid != 0)
                            {
                                student_grade = (from s in db.Students where s.STUDENTID == studentid select s.GRADE).FirstOrDefault(); 
                            }
                            if (student_grade != 0)
                            {
                                if ((student_grade == coursereq.GradeLevelID))
                                {
                                    requirement_check_grade = true;
                                    validation_pass_count = validation_pass_count + 1;
                                }
                            }
                        }

                        if (course_requirements.Count <= 0)
                        {
                            requirement_check_status = true;
                        }
                        else if (validation_pass_count>0)
                        {
                            requirement_check_status = true; 
                        }
                    }
                    else
                    {
                        requirement_check_status = true;
                    }
                }
                else
                {
                    requirement_check_status = true;
                }
                return requirement_check_status;
            }
        }
        private List<CourseCredit> SetSelectedCredits(List<string> credittypes, Course course)
        {
            List<CourseCredit> seelcted_credits = new List<CourseCredit>();
                       foreach(var a in credittypes){
                        if (a == CourseCreditType.Ceu.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Ceu);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Ceu, course.CEUCredit.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Credit.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Credit);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Credit, course.CREDITHOURS.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Custom.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Custom);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Custom, course.CustomCreditHours.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }

                        if (a == CourseCreditType.Graduate.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Graduate);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Graduate, course.GraduateCredit.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.InService.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.InService);
                            CourseCredit credit = new CourseCredit(CourseCreditType.InService, course.InserviceHours, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional1.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional1);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional1, course.Optionalcredithours1.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional2.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional2);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional2, course.Optionalcredithours2.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional3.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional3);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional3, course.Optionalcredithours3.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional4.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional4);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional4, course.Optionalcredithours4.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional5.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional5);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional5, course.Optionalcredithours5.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional6.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional6);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional6, course.Optionalcredithours6.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional7.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional7);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional7, course.Optionalcredithours7.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                        if (a == CourseCreditType.Optional8.ToString())
                        {
                            var creditLabel = CourseCreditHelper.GetCreditLabel(CourseCreditType.Optional8);
                            CourseCredit credit = new CourseCredit(CourseCreditType.Optional8, course.Optionalcredithours8.Value, creditLabel);
                            seelcted_credits.Add(credit);

                        }
                       
                    }

                       return seelcted_credits;
        }

        /// <summary>
        /// Adds bundled courses to the cart after the parent is added.
        /// </summary>
        /// <param name="model"></param>
        private void AddBundledCourses(CourseModel model, int? studentId = 0, int? isMultipleEnroll=0)
        {
            foreach (Course c in model.BundledCourses)
            {
                //get bundled materials
                string bundledMaterialString = string.Empty;
                using (var db = new SchoolEntities())
                {
                    try
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        bundledMaterialString = (from b in db.Courses where b.COURSEID == c.COURSEID select b.MATERIALS).First();
                    }
                    catch { }
                }
                List<int> bundledMaterialList = (!string.IsNullOrEmpty(bundledMaterialString)) ? bundledMaterialString.Replace("~", "").Split(',').Select(n => int.Parse(n)).ToList() : null;
                //get bundled access code: auto-entered if required so all courses get in
                string bundledCourseAccessCode = null;
                if (c.AccessCodeRequired)
                {
                    using (var db = new SchoolEntities())
                    {
                        try
                        {
                            db.Configuration.LazyLoadingEnabled = false;
                            db.Configuration.ProxyCreationEnabled = false;
                            bundledCourseAccessCode = (from b in db.Courses where b.COURSEID == c.COURSEID select b.courseinternalaccesscode).First();
                        }
                        catch { }
                    }
                }
                //pass null ints for the pricing option to get default pricing option for bundled courses
                int? bundledDefaultPricingOption = null;
                //get first alphabetic coursechoiceid for bundled courses that require course choice selection (this should not happen)
                //but prevents the bundling from getting hung if course choices are enabled on a bundled course
                int? bundledDefaultCourseChoiceId = null;
                if (Settings.Instance.GetMasterInfo2().ShowCourseType != 0 && c.StudentChoiceCourse > 0)
                {
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        List<CourseChoice> bundledCourseChoice = (from dbc in db.Courses.Include("CourseChoices") where dbc.COURSEID == c.COURSEID select dbc).First().CourseChoices.ToList();
                        if (bundledCourseChoice.Count > 0)
                        {
                            bundledDefaultCourseChoiceId = bundledCourseChoice.First().CourseChoiceId;
                        }
                    }
                }
                //since this is a recursive call, may need to make sure the bundled courses is not a main course with the same bundled course
                //e.g. prevent maincourse 1000 with bundled course 1001 from adding maincourse 1001 with bundled course 1000
                //this should never have but if it does, it will be an infinite loop
                if (isMultipleEnroll != 1)
                {
                    AddCourse(c.COURSEID, bundledDefaultPricingOption, bundledDefaultCourseChoiceId, bundledMaterialList, 0, null, null, bundledCourseAccessCode, true, true, model.CourseId);
                }
                if (studentId != 0)
                {
                    CourseMultipleStudentItem newStudent = new CourseMultipleStudentItem();
                    ListingStudentModel student_details = SupervisorHelper.GetStudent(studentId.Value);
                    newStudent.CourseId = c.COURSEID;
                    newStudent.StudentId = studentId.Value;
                    newStudent.CourseName = model.Course.COURSENAME;
                    newStudent.FirstName = student_details.StudentFirstName;
                    newStudent.LastName = student_details.StudentLastName;
                    // newStudent.PricingModel = bundledDefaultPricingOption;
                    var coursemodel = new CourseModel(c.COURSEID);
                    var cart_student_course_count = MultipleStudentCourses.Where(tc => tc.CourseId == c.COURSEID).Count();
                    foreach (var s in MultipleStudentCourses)
                    {
                        if (s.StudentId == newStudent.StudentId)
                        {
                            newStudent.OrderNumber = s.OrderNumber;
                        }
                    }

                    if (newStudent.OrderNumber == "" || newStudent.OrderNumber == null)
                    {
                        EnrollmentFunction enroll = new EnrollmentFunction();
                        newStudent.OrderNumber = enroll.CheckGenerateOrderID(newStudent.StudentId);
                    }
                    if ((coursemodel.CourseSettings.ENROLLEDAVAIL - cart_student_course_count) <= 0)
                    {
                        newStudent.IsWaiting = 1;
                    }
                    else
                    {
                        newStudent.IsWaiting = 0;
                    }
                    MultipleStudentCourses.Add(newStudent);
                }
            }
        }

        /// <summary>
        /// Removes a course from the cart if exists.
        /// </summary>
        /// <param name="courseId">The id of the course to check</param>
        public void RemoveCourse(int courseId)
        {
            var existing = GetCourseItem(courseId);
            if (existing != null)
            {
                items.Remove(existing);
            }

            var bundledCourses = (from ca in this.items where ca.IsBundledCourse && ca.BundleParentCourseId == courseId select ca).ToList();

            foreach (var bundledCourse in bundledCourses)
            {
                existing = GetCourseItem(bundledCourse.Course.COURSEID);
                if (existing != null)
                {
                    items.Remove(existing);
                }
            }

            if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentInstructorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
            {
                RemoveCoursewithMultipleStudents(courseId);
            }
        }

        public void RemoveStudentinCheckoutMultiple(int cid, int sid)
        {
            List<CourseMultipleStudentItem> list = new List<CourseMultipleStudentItem>();
            foreach (var item in this.MultipleStudentCourses)
            {
                if((item.CourseId!=cid) || (item.StudentId!=sid))
                {
                    list.Add(item);
                }

            }

            int itemcourse = (from course in list where course.CourseId == cid select course).Count();
            if(itemcourse==0)
            {
                RemoveCourse(cid);
            }


            this.MultipleStudentCourses = list;
        }

        public void RemoveCoursewithMultipleStudents(int cid)
        {
            List<CourseMultipleStudentItem> list = new List<CourseMultipleStudentItem>();
            foreach (var item in this.MultipleStudentCourses)
            {
                if ((item.CourseId != cid))
                {
                    list.Add(item);
                }

            }

            this.MultipleStudentCourses = list;
        }

        public string GetRemoveCourseAlertText(int courseId)
        {
            CourseModel model = new CourseModel(courseId);
            string bundledCourseLabel = Settings.Instance.GetMasterInfo3().FastTrackLabel;
            string alertText = "Are you sure you want to remove this course from the cart?";
            if (Settings.Instance.GetMasterInfo3().FastTrackCoursesOn > 0)
            {
                if (model.BundledCourses.Count > 0)
                {
                    alertText = string.Format("Are you sure you want to remove this course from the cart?\n{0} is a {1} course. Removing this course will remove all other included courses.",
                        model.Course.COURSENAME, bundledCourseLabel);
                }
                else if (model.ParentBundledCourseIds.Count > 0)
                {
                    int pcount = 0;
                    foreach (int pcid in model.ParentBundledCourseIds)
                    {
                        if (ContainsCourse(pcid))
                            pcount++;
                    }
                    if (pcount > 0)
                    {
                        alertText = string.Format("Are you sure you want to remove this course from the cart?\n{0} is included in a {1} course. Removing this course will disband the {1} courses in the cart.",
                            model.Course.COURSENAME, bundledCourseLabel);
                    }
                }
            }
            return alertText;
        }

        /// <summary>
        /// Clears the cart.
        /// </summary>
        public void Empty()
        {
            EmptyReserveCourses();
            items.Clear();
        }

        public void EmptyReserveCourses()
        {
            using (var db = new SchoolEntities())
            {
                try
                {
                    var reservecourses = (from _reservecourses in db.AutoReserveCourses where _reservecourses.ReservationId == CourseShoppingCart.Instance.ReservationId select _reservecourses).ToList();

                    foreach (var items in reservecourses)
                    {
                        items.ExpiredTime = DateTime.Now.AddMinutes(-20);
                    }
                    db.SaveChanges();
                }
                catch { }
                
            }
        }
        public void StartCheckout()
        {
            foreach (var item in items)
            {
                item.DiscountAmountPerCourse = null;
                item.DiscountCouponPerCourse = null;
            }         
        }

        public void Refresh()
        {
            items.ForEach(delegate(CourseShoppingCartItem item)
            {
                item.Refresh();
            });
        }

        public void updatePartialPayment(int courseId, bool val)
        {
            var newPrice = (from pm in items where pm.CourseId == courseId select pm).FirstOrDefault();
            newPrice.IsPartialPayment = val;
        }

        public void UpdateCRExtraParticipant(int courseId, string val)
        {
            var newPrice = (from pm in items where pm.CourseId == courseId select pm).FirstOrDefault();
            newPrice.CRExtraParticipant = val;
        }

        /// <summary>
        /// TRUE if the course with course id is in the shopping cart.
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public bool ContainsCourse(int courseId)
        {
            return  GetCourseItem(courseId) != null;
        }

        /// <summary>
        /// Returns the item or null if it is not in the cart.
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public CourseShoppingCartItem GetCourseItem(int courseId)
        {
            var exits = (from i in items where i.Course.COURSEID == courseId select i).FirstOrDefault();
            return exits;
        }

        /// <summary>
        /// Sum of line total in the cart.
        /// </summary>
        public decimal SubTotal
        {
            get
            {
                var subTotal = (from i in items select i.LineTotal).Sum();
                if (MultipleStudentCourses != null)
                {
                    if (MultipleStudentCourses.Count> 0)
                    {
                        subTotal = 0;
                        foreach(var student in MultipleStudentCourses)
                        {
                            
                            if (student.PricingModel != null)
                            {
                                if (student.useAdminPricing == 1)
                                {
                                    var cTotal = (from c in items where c.CourseId == student.CourseId && !c.IsBundledCourse select (c.MateriaTotal + student.CourseTotal)).FirstOrDefault();
                                    subTotal = subTotal + cTotal;
                                }
                                else
                                {
                                    var cTotal = (from c in items where c.CourseId == student.CourseId && !c.IsBundledCourse select (c.MateriaTotal + student.PricingModel.EffectivePrice)).FirstOrDefault();
                                    subTotal = subTotal + cTotal;
                                }
                            }
                        }

                    }
                }

                return subTotal;
            }
        }

        public bool PaidInfull
        {
            get;
            set;
        }

        public List<string> Validate()
        {
            this.Refresh();
            var result = new List<string>();
            var membershipValidationResults = ValidateMembershipType();
            result.AddRange(membershipValidationResults);
            var enrollmentValidationResults = ValidateEnrollment();
            result.AddRange(enrollmentValidationResults);
            var cancelledCourseResult = ValidateCancelledCourses();
            result.AddRange(cancelledCourseResult);
            return result;
        }

        private List<string> ValidateCancelledCourses()
        {
            var student = AuthorizationHelper.CurrentUser as Student;
            var messages = new List<string>();
            if (student == null)
            {
                return messages;
            }
            var removables = new List<CourseShoppingCartItem>();
            foreach (var item in this.items)
            {
                if (item.Course.IsCancelled)
                {
                    removables.Add(item);
                    messages.Add(
                        string.Format(
                            "Course \"{0}\" has been removed from your cart because it is cancelled.",
                            item.Course.COURSENAME
                        )
                    );
                }
            }

            foreach (var item in removables)
            {
                this.items.Remove(item);
            }
            return messages; 
        }

        private List<string> ValidateEnrollment()
        {
            var student = AuthorizationHelper.CurrentUser as Student;
            var messages = new List<string>();
            if (student == null)
            {
                return messages;
            }
            var removables = new List<CourseShoppingCartItem>();
            var rosters = student.AllRosters;
            int FoundMultiCourseInGroup = 0;
            foreach (var item in this.items)
            {
                if (student.IsCurrentlyEnrolledInCourse(item.Course.COURSEID, rosters))
                {
                    removables.Add(item);
                    messages.Add(
                        string.Format(
                            "Course \"{0}\" has been removed from your cart because you are already enrolled in this course.",
                            item.Course.COURSENAME
                        )
                    );
                }

                if (!IsEnrollmentCheckPass(item.Course.COURSEID, student.STUDENTID))
                {
                    removables.Add(item);
                    messages.Add(
                        string.Format(
                            "Course \"{0}\" has been removed from your cart because you are already enrolled in a course with the same Name or Number.",
                            item.Course.COURSENAME
                        )
                    );
                }

                if (item.Course.CourseColorGrouping != null && item.Course.CourseColorGrouping != 0)
                {
                    int studentid_forgroup = 0;
                    if (AuthorizationHelper.CurrentStudentUser != null && student.STUDENTID == 0)
                    {
                        studentid_forgroup = AuthorizationHelper.CurrentStudentUser.STUDENTID;
                    }
                    CourseColorGroupingHelper CourseColorGroupingHelper = new CourseColorGroupingHelper();
                    var groupcheck = CourseColorGroupingHelper.GetColorGroupingLimitRegistration(item.Course.CourseColorGrouping);
                    if (groupcheck != null)
                    {
                        if (groupcheck.IsLimitedtoOneRegistration)
                        {
                            if (CourseShoppingCart.Instance.Items.Where(cartitem => groupcheck.Courses.Contains(cartitem.CourseId)).Count() > 0)
                            {
                                FoundMultiCourseInGroup = 1;
                            }
                            if (FoundMultiCourseInGroup > 1)
                            {
                                removables.Add(item);
                                messages.Add(
                                    string.Format(
                                        "Course \"{0}\" has been removed from your cart because you already have a course with the Group in the cart.",
                                        item.Course.COURSENAME
                                    )
                                );
                                FoundMultiCourseInGroup = 0;
                            }
                            if (CourseColorGroupingHelper.IsEnrolledCoursesinSameGroupings(item.Course.CourseColorGrouping, studentid_forgroup))
                            {
                                removables.Add(item);
                                messages.Add(
                                    string.Format(
                                        "Course \"{0}\" has been removed from your cart because you are already enrolled in a course with the same Group.",
                                        item.Course.COURSENAME
                                    )
                                );
                            }
                        }
                    }
                }


            }



            foreach (var item in removables)
            {
                this.items.Remove(item);
            }
            return messages; 
        }

        private bool IsEnrollmentCheckPass(int courseid, int? student_id = 0)
        {
            bool allowed = true;
            if (student_id == 0)
            {
                student_id = Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser == null ? 0 : Gsmu.Api.Authorization.AuthorizationHelper.CurrentStudentUser.STUDENTID;
            }
            int? enrollmentCheck = Settings.Instance.GetMasterInfo3().EnrollmentCheck;
            if (enrollmentCheck != null && enrollmentCheck > 0)
            {
                using (var db = new SchoolEntities())
                {
                    var course_data = (from c in db.Courses where c.COURSEID == courseid select c).SingleOrDefault();

                    string course_name = course_data.COURSENAME;
                    string course_number = course_data.COURSENUM;
                    bool DisableEnrollmentCheckingSpecific = (course_data.CourseSpecificEnrollmentCheck == 1 ? true : false);

                    // IF LOGGED IN
                    if (student_id > 0)
                    {
                        // GET STUDENT ROSTER INFORMATION AND GET ALL COURSES
                        var course_roster_data = (from cr in db.Course_Rosters
                                                  join c in db.Courses on cr.COURSEID equals c.COURSEID
                                                  where cr.STUDENTID == student_id && cr.Cancel == 0
                                                  select c);
                        foreach (var data in course_roster_data)
                        {
                            if (enrollmentCheck == 1)
                            { // FILTER BY COURSE NAME
                                if (course_name == data.COURSENAME || course_name.Replace(" ", "") == (data.COURSENAME.Replace(" ", "")))
                                    allowed = false;
                            }
                            else if (enrollmentCheck == 2)
                            { // FILTER BY COURSE NUMBER
                                if (course_number == data.COURSENUM || course_number.Replace(" ", "") == (data.COURSENUM.Replace(" ", "")))
                                    allowed = false;
                            }
                            else if (enrollmentCheck == 3)
                            { // FILTER BY COURSE NUMBER && COURSE NAME
                                if ((course_name == data.COURSENAME || course_name.Replace(" ", "") == (data.COURSENAME.Replace(" ", ""))) && (course_number == data.COURSENUM || course_number.Replace(" ", "") == (data.COURSENUM.Replace(" ", ""))))
                                    allowed = false;
                            }
                        }
                        if (DisableEnrollmentCheckingSpecific) { allowed = true; }
                    }
                }
            }

            return allowed;
        }

        internal List<string> ValidateMembershipType()
        {
            var user = AuthorizationHelper.CurrentUser;
            if ((AuthorizationHelper.CurrentSupervisorUser != null) || (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
            {
                if (CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent > 0)
                {
                    user = Student.GetStudent(CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent);
                }                             

            }

            var messages = new List<string>();
            var removables = new List<CourseShoppingCartItem>();

            var nonBundledCourses = (from ci in this.items where ci.IsBundledCourse == false && (ci.CheckoutEventId == null || ci.CheckoutEventId == 0) select ci).ToList();

            foreach (var item in nonBundledCourses)
            {
                var courseModel = new CourseModel(item.Course.COURSEID);
                courseModel.SelectedStudentMembershipType = user.MembershipType;
                var effectivePrices = courseModel.EffectivePrices;
                if ( item.PricingModel == null && effectivePrices.Count != 0)
                {
                    if (Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper.GetConfig().useClubPilates == 1 && item.Course.clubready_package_id > 0)
                    {
                    }
                    else
                    {

                        messages.Add(this.AddRemoveCartMessage(item));
                        removables.Add(item);
                    }
                }
                else if (item.PricingModel == null || !item.PricingModel.IsOption)
                {
                    var newPrice = (from pm in effectivePrices where pm.IsOption == false select pm).FirstOrDefault();
                    if (newPrice != null)
                    {
                        item.PricingModel = newPrice;
                    }
                    else
                    {
                        item.PricingModel = new PricingModel();
                        item.PricingModel.MembershipType = user.MembershipType;
                    }
                }
                else if (item.PricingModel.MembershipType != user.MembershipType) 
                {
                    var currentPrice = item.PricingModel;
                    var newPrice = (from pm in effectivePrices where pm.PricingOption != null && pm.PricingOption.PricingOptionID == currentPrice.PricingOption.PricingOptionID select pm).FirstOrDefault();
                    if (newPrice != null)
                    {
                        item.PricingModel = newPrice;
                    }
                    else
                    {

                        if (Gsmu.Api.Integration.ClubPilates.ClubPilatesHelper.GetConfig().useClubPilates == 1 && item.Course.clubready_package_id > 0)
                        {
                        }
                        else
                        {
                            messages.Add(this.AddRemoveCartMessage(item));
                            removables.Add(item);
                        }
                    }
                }
            }

            foreach (var item in removables)
            {
                this.items.Remove(item);
            }
            return messages;
        }

        private string AddRemoveCartMessage(CourseShoppingCartItem item)
        {
            string ErrorMessageWrongPriceOption = System.Configuration.ConfigurationManager.AppSettings["ErrorMessageWrongPriceOption"];
            if (string.IsNullOrEmpty(ErrorMessageWrongPriceOption))
            { 
                ErrorMessageWrongPriceOption = " has been removed from your cart because the pricing option you selected is not available for your membership type.";
            }
                var message = string.Format(
                    "Course \"{0}\""+ ErrorMessageWrongPriceOption + ".",
                    item.Course.COURSENAME
                );

            return message;
        }
    }
}
