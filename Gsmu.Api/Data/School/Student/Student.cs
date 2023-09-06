using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Web;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Gsmu.Api.Commerce.ShoppingCart;

namespace Gsmu.Api.Data.School.Entities
{
    /// <summary>
    /// Make sure to call the refresh method if the user is logged in and a new enrollment is added!
    /// </summary>
    public partial class Student : AbstractSiteUser
    {
        public override int SiteUserId
        {
            get { return this.STUDENTID; }
        }



        public override string SiteUserEmailAddress
        {
            get { return this.EMAIL; }
        }

        public override string WelcomeName
        {
            get {
                return this.FIRST + " " + this.LAST;
            }
        }

        public override string LoggedInUsername
        {
            get
            {
                return this.USERNAME;
            }
        }

        public override LoggedInUserType LoggedInUserType
        {
            get {
                return LoggedInUserType.Student;
            }
        }

        public override bool IsLoggedIn
        {
            get
            {
                var user = AuthorizationHelper.CurrentUser;
                return user.LoggedInUserType == this.LoggedInUserType && user.LoggedInUsername == this.LoggedInUsername;
            }
        }

        public override MembershipType MembershipType {
            get
            {
                using (var db = new SchoolEntities())
                {
                    if (this.DISTRICT != null && this.DISTRICT != 0)
                    {
                        var districtmembership = (from dist in db.Districts
                                                  where dist.DISTID == this.DISTRICT
                                                  select dist).FirstOrDefault();
                        if (districtmembership != null)
                        {
                            int membershipflag = 0;
                            if (districtmembership.MembershipFlag == 2)
                            {
                                membershipflag = -1;
                            }
                            else if (districtmembership.MembershipFlag == 1)
                            {
                                membershipflag = 0;
                            }
                            else if (districtmembership.MembershipFlag == 3)
                            {
                                membershipflag = 3;
                            }
                            if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo().Membership_Status != 0)
                                    this.DISTEMPLOYEE = short.Parse(membershipflag.ToString());
                        }

                    }
                }



                return MembershipHelper.GetMembershipType(this.DISTEMPLOYEE);
            }
        }

        public string MemberhipTypeString
        {
            get
            {
                return MembershipType.ToString();
            }
        }

        /// <summary>
        /// Make sure to call the refresh method if the user is logged in and a new enrollment is added!
        /// </summary>
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<EnrollmentModel> CurrentRosters
        {
            get
            {
                using (var db = new SchoolEntities())
                {
                   var currentRosters = (from cr in db.Course_Rosters
                                         join c in db.Courses on cr.COURSEID equals c.COURSEID
                                         let maxdate = (from ct in db.Course_Times where ct.COURSEID == c.COURSEID select ct.COURSEDATE).Max()
                                         let trnscrptCount = (from tr in db.Transcripts where tr.STUDENTID == cr.STUDENTID && tr.CourseId == cr.COURSEID select tr.TranscriptID).Count()
                                    where cr.Cancel == 0 && cr.STUDENTID.HasValue == true && cr.STUDENTID.Value == this.STUDENTID
                                    && c.CANCELCOURSE == 0 && cr.WAITING == 0 && cr.PaidInFull == -1 && trnscrptCount==0
                                    select new EnrollmentModel() {
                                        Roster = cr,
                                        Course = c,
                                        MaxDate = maxdate
                                    }).ToList();

                   return currentRosters;
                }
            }
        }
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<EnrollmentModel> AllRosters
        {
            get
            {
                using (var db = new SchoolEntities())
                {
                    var allRosters = (from cr in db.Course_Rosters
                                          join c in db.Courses on cr.COURSEID equals c.COURSEID
                                          let maxdate = (from ct in db.Course_Times where ct.COURSEID == c.COURSEID select ct.COURSEDATE).Max()
                                          
                                          where cr.Cancel == 0 && cr.STUDENTID.HasValue == true && cr.STUDENTID.Value == this.STUDENTID
                                          && c.CANCELCOURSE == 0
                                          select new EnrollmentModel()
                                          {
                                              Roster = cr,
                                              Course = c,
                                              MaxDate = maxdate
                                          }).ToList();

                    return allRosters;
                }
            }
        }

        public bool IsCurrentlyEnrolledInCourse(int courseId, List<EnrollmentModel> rosterList = null)
        {
            if (rosterList == null) {
                rosterList = CurrentRosters;
            }
            var exits = (from rm in rosterList where rm.Course.COURSEID == courseId && rm.Roster.IsValidForClass select rm).Count() > 0;
            return exits;
        }

        public bool IsCurrentlyEnrolledInCourseForMultipleEnrollment(int cid,int sid)
        {
                            using (var db = new SchoolEntities())
                {
            var exits = (from rm in  db.Course_Rosters where rm.COURSEID == cid && rm.STUDENTID ==sid && rm.Cancel==0 select rm).Count() > 0;
            return exits;
                            }

        }

        
        public string UpdateLastLogin()
        {
            //using (var connection = Connections.GetSchoolConnection())
            //{
            //    this.lastlogin = DateTime.Now;
            //    var command = connection.CreateCommand();
            //    command.CommandText = "UPDATE Students SET lastlogin = GETDATE() where studentid = " + this.STUDENTID;
            //    command.ExecuteNonQueryAsync();
            //}
            string userSessionId = string.Empty;
            using (var db = new SchoolEntities())
            {
                var studentlog = (from s in db.Students where s.STUDENTID ==AuthorizationHelper.CurrentStudentUser.STUDENTID select s).SingleOrDefault();
                if (studentlog != null) { 
                    studentlog.lastlogin = DateTime.Now.Date;

                    try {
                        var chkout = CheckoutInfo.Instance;
                        if(chkout.HasActivePaymentProcess == null)
                        {
                            chkout.HasActivePaymentProcess = false;
                        }
                        if (!chkout.HasActivePaymentProcess.Value)
                        {
                            studentlog.UserSessionId = Guid.NewGuid();
                        }
                    }
                    catch { }
                    userSessionId = studentlog.UserSessionId.Value.ToString();
                   
                    db.SaveChanges();
                    return userSessionId;
                }
            }
            return userSessionId;
        }

        public System.Collections.Specialized.NameValueCollection LtiFormData
        {
            get
            {
                return System.Web.HttpUtility.ParseQueryString(this.lti_data);
            }
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public static List<EnrollmentModel> CurrentStudentEnrollments
        {
            get
            {
                var student = AuthorizationHelper.CurrentStudentUser;
                if ((Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn) && (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null))
                {
                    int mainStudentid = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                    student = GetStudent(mainStudentid);
                }
                if (student != null)
                {
                    var enrollments = ObjectHelper.GetRequestObject<List<EnrollmentModel>>(WebContextObject.CurrentStudentEnrollments);
                    if (enrollments == null)
                    {
                        enrollments = student.CurrentRosters;
                        ObjectHelper.SetRequestObject<List<EnrollmentModel>>(WebContextObject.CurrentStudentEnrollments, enrollments);
                    }
                    return enrollments;
                }
                return new List<EnrollmentModel>(0);
            }
        }
        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public static List<EnrollmentModel> AllStudentEnrollments
        {
            get
            {
                var student = AuthorizationHelper.CurrentStudentUser;
                if ((Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn) && (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser != null))
                {
                    int mainStudentid = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                    student = GetStudent(mainStudentid);
                }
                if (student != null)
                {
                    var enrollments = ObjectHelper.GetRequestObject<List<EnrollmentModel>>(WebContextObject.AllStudentEnrollments);
                    if (enrollments == null)
                    {
                        enrollments = student.AllRosters;
                        ObjectHelper.SetRequestObject<List<EnrollmentModel>>(WebContextObject.AllStudentEnrollments, enrollments);
                    }
                    return enrollments;
                }
                return new List<EnrollmentModel>(0);
            }
        }

        public static bool IsCurrentStudentEnrolledInCourse(int courseId)
        {
            return GetCurrentStudentEnrollmentInCourse(courseId) != null;
        }

        public static EnrollmentModel GetCurrentStudentEnrollmentInCourse(int courseId)
        {
            var student = AuthorizationHelper.CurrentStudentUser;
            if ((Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn) && (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
            {
                int mainStudentid = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                student = GetStudent(mainStudentid);
            }

           if (student != null)
            {
                var enrollments = CurrentStudentEnrollments;

                var existing = (from rm in enrollments where rm.Course.COURSEID == courseId select rm).FirstOrDefault();
                return existing;
            }
            return null;
        }

        public static EnrollmentModel GetCurrentStudentEnrollmentInCourseForMembership(int courseId)
        {
            var student = AuthorizationHelper.CurrentStudentUser;
            if ((Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn) && (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null))
            {
                int mainStudentid = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                student = GetStudent(mainStudentid);
            }

            if (student != null)
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;

                    var existing = (from rm in db.Course_Rosters
                                    where rm.COURSEID == courseId && rm.Cancel == 0 && rm.STUDENTID==student.STUDENTID
                                    select new EnrollmentModel
                            
                                    {
                                                Course =(from c in db.Courses where c.COURSEID== rm.COURSEID select c).FirstOrDefault()
                                    }).FirstOrDefault();
                    return existing;
                }
            }
            return null;
        }



        public static EnrollmentModel GetAllStudentEnrollmentInCourse(int courseId)
        {
            var student = AuthorizationHelper.CurrentStudentUser;
            if ((Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn) && (Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser != null))
            {
                int mainStudentid = CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent;
                student = GetStudent(mainStudentid);
            }

            if (student != null)
            {
                var enrollments = AllStudentEnrollments;
                var existing = (from rm in enrollments where rm.Course.COURSEID == courseId || rm.Course.eventid == courseId select rm).FirstOrDefault();
               
                return existing;
            }
            return null;
        }

        public static Student GetStudent(int StudentID)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var student = (from s in db.Students where s.STUDENTID == StudentID select s).FirstOrDefault();
                return student;
            }
        }

        public static Student GetStudentByEmail(string Email)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var student = (from s in db.Students where s.EMAIL == Email select s).FirstOrDefault();
                return student;
            }
        }

        public static Parent GetParent(int StudentID)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                Student Student = Student.GetStudent(Int32.Parse(StudentID.ToString()));

                var parent = (from p in db.Parents where p.ParentsID == Student.parentsid select p).FirstOrDefault();

                return parent;
            }
        }

        public static List<Supervisor> GetStudentSupervisors(int? schoolid, int? districtid, int? studentid)
        {
           int AssignSup2Stud = Convert.ToInt32(Settings.Instance.GetMasterInfo3().AssignSup2Stud);
           int AssignSup2StudEmail = Convert.ToInt32(Settings.Instance.GetMasterInfo3().AssignSup2StudEmail);
           int supervisor_email = Convert.ToInt32(Settings.Instance.GetMasterInfo2().supervisor_email);
            //int AssignSup2Stud = 0;
           // int AssignSup2StudEmail = 0;
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                // send emails to the supervisor.  set in the system config area.
                //// looking base on school/supervisor relation
                if ((schoolid != null) && (AssignSup2Stud == 0) && (supervisor_email == 1 || supervisor_email == 11))
                {
                    //var supervisor = (from s in db.Supervisors 
                    //                  from stud in s.Students where stud.STUDENTID == studentid && (s.NOTIFY==1 || s.NOTIFY==3) select s).ToList();
                    var supervisor = (from s in db.Supervisors
                                      from sch in s.SupervisorSchools where sch.SchoolID == schoolid && (s.NOTIFY == 1 || s.NOTIFY == 3)
                                      select s).ToList();
                    return supervisor;
                }
                /// looking by student/supervisor relation
                else if ((studentid != null) && (AssignSup2Stud == 1) && (AssignSup2StudEmail == 1))
                {
                    //var supervisor = (from s in db.Supervisors 
                    //                  from sch in s.SupervisorSchools where sch.SchoolID == schoolid && (s.NOTIFY==1 || s.NOTIFY==3)
                    //                  select s).ToList();
                    var supervisor = (from s in db.Supervisors 
                                      from sstud in s.SupervisorStudents where sstud.studentid == studentid && (s.NOTIFY==1 || s.NOTIFY==3)
                                      select s).ToList();                                        
                    return supervisor;
                }
                //// looking by district/supervisor relation
                else if ((districtid != null)&& (AssignSup2Stud==0))
                {
                    var supervisor = (from s in db.Supervisors where s.DISTRICT == districtid && (s.NOTIFY == 1 || s.NOTIFY == 3) select s).ToList();
                    return supervisor;
                }
                else
                {
                    return null;
                }
                
            }
        }


    }
}
