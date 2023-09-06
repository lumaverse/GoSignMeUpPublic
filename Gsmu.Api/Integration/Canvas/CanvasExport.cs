using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using json = Newtonsoft.Json;
using http = System.Net.Http;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using System.Collections.Specialized;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Integration.Canvas.Clients;
using canvas = Gsmu.Api.Integration.Canvas;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.User;
using Gsmu.Api.Data;
using Gsmu.Api.Authorization;
using System.Data.Entity;
using System.Web.Script.Serialization;


namespace Gsmu.Api.Integration.Canvas
{
    public class CanvasExport
    {
        public const string GSMU_PREFIX = "GSMU";

        public static int GetCourseSectionID(string gsmuCourseConfiguration)
        {
            int paramCourseSectionID = 0;
            JavaScriptSerializer j = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(gsmuCourseConfiguration) && gsmuCourseConfiguration.ToString() != "{}")
            {
                dynamic CourseInfoconfiguration = j.Deserialize(gsmuCourseConfiguration, typeof(object));
                if (CourseInfoconfiguration.ContainsKey("canvassectionID") && CourseInfoconfiguration["canvassectionID"] != "")
                {
                    paramCourseSectionID = Convert.ToInt32(CourseInfoconfiguration["canvassectionID"]);
                }
            }
            return paramCourseSectionID;
        }

        public static Response SynchronizeStudent(Int64 studentId, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();
            var student = (from s in db.Students where s.STUDENTID == studentId select s).First();
            Response response = SynchronizeStudent(student, db);

            db.SaveChanges();

            return response;
        }

        public static Response SynchronizeStudent(entities.Student gsmuStudent, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();
            Entities.User user = new Entities.User();
            user.Name = gsmuStudent.FIRST + " " + gsmuStudent.LAST;
            user.ShortName = gsmuStudent.FIRST + " " + gsmuStudent.LAST; 
            user.SortableName = gsmuStudent.LAST + ", " + gsmuStudent.FIRST;
            user.PrimaryEmail = gsmuStudent.EMAIL;
            user.SisLoginId = gsmuStudent.USERNAME;
            if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "" && canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "username")
            {
                if (string.IsNullOrEmpty(gsmuStudent.StudRegField1))
                {
                    user.SisUserId = WebConfiguration.SystemPrefix + GSMU_PREFIX + UserModel.LoggedInUserTypeToUserGroupPrefix(LoggedInUserType.Student) + gsmuStudent.STUDENTID;
                }
                else
                {
                    user.SisUserId = gsmuStudent.StudRegField1;
                }
            }
            else
            {
                user.SisUserId = WebConfiguration.SystemPrefix + GSMU_PREFIX + UserModel.LoggedInUserTypeToUserGroupPrefix(LoggedInUserType.Student) + gsmuStudent.STUDENTID;
            }
            Response response;
            if (gsmuStudent.canvas_user_id != null && gsmuStudent.canvas_user_id > 0)
            {
                user.Id = gsmuStudent.canvas_user_id.Value;
                response = Clients.UserClient.UpdateUser(user, "STUDENT");
                //update user password.
                if (canvas.Configuration.Instance.enableGSMUMasterAuthentication == true && !string.IsNullOrEmpty(gsmuStudent.STUDNUM.ToString()))
                {
                    if (gsmuStudent.STUDNUM.ToString().IndexOf("canvas") == -1)
                    {
                        Response loginresp;
                        user.cpass = gsmuStudent.STUDNUM.ToString();
                        loginresp = Clients.UserClient.UpdateUserPass(user);

                    }
                }
            }
            else
            {
                response = Clients.UserClient.InsertUser(user, gsmuStudent.STUDNUM);
            }
            gsmuStudent.canvas_user_id = Convert.ToInt64(response.User.Id.ToString());
            //////////////////////////////////
            ///not sure why have to do this to save the instructor ID
            ///because db.SaveChanges() just dont work
            using (var db_instance = new entities.SchoolEntities())
            {
                var Stud_ind = (from e in db_instance.Students where e.STUDENTID == gsmuStudent.STUDENTID select e).FirstOrDefault();
                Stud_ind.canvas_user_id = Convert.ToInt64(response.User.Id.ToString());
                db_instance.SaveChanges();
            }
            //////////////////////////////////
            db.SaveChanges();                    
            return response;
        }

        public static void SynchronizeInstructor(Int64 instructorId, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();

            var instructor = (from i in db.Instructors where i.INSTRUCTORID == instructorId select i).First();
            SynchronizeInstructor(instructor);

            db.SaveChanges();
        }

        public static void SynchronizeInstructor(entities.Instructor gsmuInstructor, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();

            Entities.User user = new Entities.User();
            user.Name = gsmuInstructor.FIRST + " " + gsmuInstructor.LAST;
            user.ShortName = gsmuInstructor.FIRST;
            user.SortableName = gsmuInstructor.LAST + ", " + gsmuInstructor.FIRST;
            user.PrimaryEmail = gsmuInstructor.EMAIL;
            user.SisLoginId = gsmuInstructor.USERNAME;
            //user.SisUserId = WebConfiguration.SystemPrefix + GSMU_PREFIX + UserModel.LoggedInUserTypeToUserGroupPrefix(LoggedInUserType.Instructor) + gsmuInstructor.INSTRUCTORID;
            if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "" && canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "username")
            {
                if (string.IsNullOrEmpty(gsmuInstructor.InstructorRegField1))
                {
                    user.SisUserId = WebConfiguration.SystemPrefix + GSMU_PREFIX + UserModel.LoggedInUserTypeToUserGroupPrefix(LoggedInUserType.Instructor) + gsmuInstructor.INSTRUCTORID;
                }
                else
                {
                    user.SisUserId = gsmuInstructor.InstructorRegField1;
                }
            }
            else
            {
                user.SisUserId = WebConfiguration.SystemPrefix + GSMU_PREFIX + UserModel.LoggedInUserTypeToUserGroupPrefix(LoggedInUserType.Instructor) + gsmuInstructor.INSTRUCTORID;
            }

            Response response;
            if (gsmuInstructor.canvas_user_id != null && gsmuInstructor.canvas_user_id > 0)
            {
                user.Id = gsmuInstructor.canvas_user_id.Value;
                response = Clients.UserClient.UpdateUser(user, "INSTRUCTOR");
                //update user password.
                if (canvas.Configuration.Instance.enableGSMUMasterAuthentication == true && !string.IsNullOrEmpty(gsmuInstructor.PASSWORD.ToString()))
                {
                    if (gsmuInstructor.PASSWORD.ToString().IndexOf("canvas") == -1)
                    {
                        Response loginresp;
                        user.cpass = gsmuInstructor.PASSWORD.ToString();
                        loginresp = Clients.UserClient.UpdateUserPass(user);

                    }
                }
            }
            else
            {
                response = Clients.UserClient.InsertUser(user, gsmuInstructor.PASSWORD);
            }
            try
            {
                gsmuInstructor.canvas_user_id = Convert.ToInt64(response.User.Id.ToString());
                //////////////////////////////////
                ///not sure why have to do this to save the instructor ID
                ///because db.SaveChanges() just dont work
                using (var db_instance = new entities.SchoolEntities())
                {
                    var Instr_ind = (from e in db_instance.Instructors where e.INSTRUCTORID == gsmuInstructor.INSTRUCTORID select e).FirstOrDefault();
                    Instr_ind.canvas_user_id = Convert.ToInt64(response.User.Id.ToString());
                    db_instance.SaveChanges();
                }
            }
            catch { }
            //////////////////////////////////
            db.SaveChanges();
        }
        public static void SynchronizeSupervisor(Int64 supervisorId, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();

            var gsmusupervisor = (from i in db.Supervisors where i.SUPERVISORID == supervisorId select i).First();
            SynchronizeSupervisor(gsmusupervisor);

            db.SaveChanges();
        }
        public static Response SynchronizeSupervisor(entities.Supervisor gsmuSupervisor, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();

            Entities.User user = new Entities.User();
            user.Name = gsmuSupervisor.FIRST + " " + gsmuSupervisor.LAST;
            user.ShortName = gsmuSupervisor.FIRST;
            user.SortableName = gsmuSupervisor.LAST + ", " + gsmuSupervisor.FIRST;
            user.PrimaryEmail = gsmuSupervisor.EMAIL;
            user.SisLoginId = gsmuSupervisor.UserName;
            user.SisUserId = WebConfiguration.SystemPrefix + GSMU_PREFIX + UserModel.LoggedInUserTypeToUserGroupPrefix(LoggedInUserType.Supervisor) + gsmuSupervisor.SUPERVISORID;
            Response response;
            if (gsmuSupervisor.canvas_user_id != null && gsmuSupervisor.canvas_user_id > 0)
            {
                user.Id = gsmuSupervisor.canvas_user_id.Value;
                response = Clients.UserClient.UpdateUser(user,"SUPERVISOR");
            }
            else
            {
                response = Clients.UserClient.InsertUser(user, gsmuSupervisor.PASSWORD);
            }
            gsmuSupervisor.canvas_user_id = Convert.ToInt64(response.User.Id.ToString());
            //////////////////////////////////
            ///not sure why have to do this to save the instructor ID
            ///because db.SaveChanges() just dont work
            using (var db_instance = new entities.SchoolEntities())
            {
                var Sup_ind = (from e in db_instance.Supervisors where e.SUPERVISORID == gsmuSupervisor.SUPERVISORID select e).FirstOrDefault();
                Sup_ind.canvas_user_id = Convert.ToInt64(response.User.Id.ToString());
                db_instance.SaveChanges();
            }
            //////////////////////////////////
            db.SaveChanges();
            return response;
        }

        public static Entities.Enrollment[] SynchronizeCourse(entities.Course gsmuCourse, entities.SchoolEntities db = null, Entities.Enrollment[] enrollments = null)
        {
            db = db ?? new entities.SchoolEntities();

            var course = new Entities.Course();
            var model = new CourseModel(db, gsmuCourse);
            string CourseConfiguration = "";

            course.AccountId = Canvas.Configuration.Instance.CanvasAccountId.Value;
            course.Name = gsmuCourse.COURSENAME;
            course.PublicDescription = gsmuCourse.DESCRIPTION;
            course.CourseCode = gsmuCourse.COURSENUM;
            string CustomCourseSisID = canvas.Configuration.Instance.allowCanvasCustomCourseSISID;
            string currentCustomCourseFieldData = "C";
            if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo2().CustomCourseFieldShow3 == 1 && !string.IsNullOrEmpty(gsmuCourse.CustomCourseField3))
            {
                currentCustomCourseFieldData = gsmuCourse.CustomCourseField3.ToString();
            }
            if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo2().CustomCourseFieldShow4 == 1 && !string.IsNullOrEmpty(gsmuCourse.CustomCourseField4))
            {
                currentCustomCourseFieldData = gsmuCourse.CustomCourseField4.ToString();
            }
            if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo2().CustomCourseFieldShow5 == 1 && !string.IsNullOrEmpty(gsmuCourse.CustomCourseField5))
            {
                currentCustomCourseFieldData = gsmuCourse.CustomCourseField5.ToString();
            }
            switch (CustomCourseSisID)
            {
                case "random":
                    course.SisCourseId = WebConfiguration.SystemPrefix + GSMU_PREFIX + currentCustomCourseFieldData + gsmuCourse.COURSEID.ToString();
                    break;
                case "partialedit":

                    course.SisCourseId = WebConfiguration.SystemPrefix + GSMU_PREFIX + currentCustomCourseFieldData + gsmuCourse.COURSEID.ToString();
                    break;
                case "fulledit":
                    course.SisCourseId = currentCustomCourseFieldData;
                    break;
                default:
                    course.SisCourseId = WebConfiguration.SystemPrefix + GSMU_PREFIX + currentCustomCourseFieldData + gsmuCourse.COURSEID.ToString();
                    break;
            }
            course.StartAt = model.CourseStartAsDate;
            course.EndAt = model.CourseEndAsDate;
            course.IsPublic = true;
            course.WorkflowState = Entities.CourseWorkflowState.available;
            CourseConfiguration = gsmuCourse.CourseConfiguration;
            Response response = null;
            if (gsmuCourse.canvas_course_id > 0)
            {
                if (canvas.Configuration.Instance.ExportEnrollmentNotUpdateCourse == false)
                {
                    course.Id = gsmuCourse.canvas_course_id.Value;
                    response = CourseClient.UpdateCourse(course);
                }
                else
                {
                    course.Id = gsmuCourse.canvas_course_id.Value;
                }
            }
            else
            {
                var Context = new SchoolEntities();
                Course c = Context.Courses.First(c1 => c1.COURSEID == gsmuCourse.COURSEID);
                string paramCanvasAccountID = "0";
                JavaScriptSerializer j = new JavaScriptSerializer();
                if (!string.IsNullOrEmpty(c.CourseConfiguration))
                {
                    dynamic CourseInfoconfiguration = j.Deserialize(c.CourseConfiguration, typeof(object));
                    if (CourseInfoconfiguration.ContainsKey("canvasaccountid"))
                    {
                        paramCanvasAccountID = CourseInfoconfiguration["canvasaccountid"];
                    }

                    if (string.IsNullOrEmpty(paramCanvasAccountID) || paramCanvasAccountID.ToString() == "0")
                    {
                        paramCanvasAccountID = course.AccountId.ToString();
                    }
                }
                else
                {
                    paramCanvasAccountID = course.AccountId.ToString();
                }

                response = CourseClient.InsertCourse(course, paramCanvasAccountID);
                c.canvas_course_id = response.Course.Id;
                if (c.CustomCreditHours == null)
                {
                    c.CustomCreditHours = 0;
                }
                Context.SaveChanges();
                gsmuCourse.canvas_course_id = response.Course.Id;
            }
            if (WebConfiguration.CanvasManuallyPublishCourse == "") { 
                CourseClient.PublishCourse(gsmuCourse.canvas_course_id.Value);
            }

            if (enrollments == null) 
            {
                int currentCourseSectionID = 0;
                if (!string.IsNullOrEmpty(gsmuCourse.CourseConfiguration) && gsmuCourse.CourseConfiguration.ToString() != "{}")
                {
                    currentCourseSectionID = GetCourseSectionID(gsmuCourse.CourseConfiguration.ToString());
                }
                if (currentCourseSectionID != 0)
                {
                    var canvasEnrollmentResponse = EnrollmentClient.ListSectionEnrollments(gsmuCourse.canvas_course_id.Value, currentCourseSectionID);
                    enrollments = canvasEnrollmentResponse.Enrollments;
                }
                else
                {
                    var canvasEnrollmentResponse = EnrollmentClient.ListCourseEnrollments(gsmuCourse.canvas_course_id.Value);
                    enrollments = canvasEnrollmentResponse.Enrollments;
                }
            }
            
            foreach (var instructor in model.Instructors)
            {
                db.Instructors.Attach(instructor);
                if (WebConfiguration.CanvasforceSyncInstructor == "false")
                {
                    SynchronizeInstructor(instructor, db);
                }
                var enrollmentType = Entities.EnrollmenType.TeacherEnrollment;
                if (instructor.canvas_user_id != null)
                {
                    var userId = instructor.canvas_user_id.Value;
                    if (userId > 0)
                    {
                        if (canvas.Configuration.Instance.defaultInstructorCourseRole == "TaEnrollment")
                        {
                            enrollmentType = Entities.EnrollmenType.TaEnrollment;
                        }

                        var canvasEnrollment = (from e in enrollments where e.UserId == userId select e).FirstOrDefault();
                        if (canvasEnrollment == null)
                        {
                            Response enrollmentResponse = ExportEnrollment(gsmuCourse, enrollmentType, userId, 0);
                        }
                    }
                }
                
            }

            db.SaveChanges();            
            return enrollments;
        }


        public static void ExportSupervisorStudentRelation2Canvas(entities.SchoolEntities db = null, int curSupervisorID = 0, int curStudentID = 0, int curStudCanvasID = 0)
        {
            //supervisor student relation in canvas - only use when using Assign Student mode
            if (Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().AssignSup2Stud == 1 && canvas.Configuration.Instance.allowSyncSupStudRelationIndEnrollment == true)
            {
                //db = db ?? new entities.SchoolEntities();
                int FoundRelationLink = 0;
                long currentSupCanvasID = 0;
                long SupervisorSessionCanvasID = 0;
                if (AuthorizationHelper.CurrentSupervisorUser != null) {
                    if (AuthorizationHelper.CurrentSupervisorUser.canvas_user_id != null)
                    {
                        SupervisorSessionCanvasID = (Int64)AuthorizationHelper.CurrentSupervisorUser.canvas_user_id;
                    }
                }
                using (var db_instance = new entities.SchoolEntities())
                {
                    var supervisor_list = (from ssup in db_instance.SupervisorStudents
                                           join sp in db_instance.Supervisors on ssup.SupervisorID equals sp.SUPERVISORID
                                           where ssup.studentid == curStudentID && ssup.SupervisorID == curSupervisorID
                                           select new { SupervisorCanvasID = sp.canvas_user_id }).FirstOrDefault();

                    if (supervisor_list.SupervisorCanvasID == null || supervisor_list.SupervisorCanvasID == 0)
                    {
                        var mySupervisor = (from s in db_instance.Supervisors where s.SUPERVISORID == curSupervisorID select s).FirstOrDefault();
                        var newSuptoCanvas = SynchronizeSupervisor(mySupervisor, null);
                        if (newSuptoCanvas.User.Id.ToString() != null)
                        {
                            currentSupCanvasID = long.Parse(newSuptoCanvas.User.Id.ToString());
                        }
                    }
                    else
                    {
                        currentSupCanvasID = long.Parse(supervisor_list.SupervisorCanvasID.ToString());
                    }
                    if ((supervisor_list != null && supervisor_list.SupervisorCanvasID != null && supervisor_list.SupervisorCanvasID != 0) || currentSupCanvasID != 0 || (SupervisorSessionCanvasID != 0))
                    {
                        //check to see if relation exist or not - unused for now
                        //var lookUpSupObservee = canvas.Clients.UserClient.GetSupervisorObservee((Int64)AuthorizationHelper.CurrentSupervisorUser.canvas_user_id);
                        var lookUpSupObservee = new canvas.Response(null);
                        if (currentSupCanvasID != 0) { 
                            lookUpSupObservee = canvas.Clients.UserClient.GetSupervisorObservee(currentSupCanvasID);
                        } else if (SupervisorSessionCanvasID != 0) {
                            lookUpSupObservee = canvas.Clients.UserClient.GetSupervisorObservee(SupervisorSessionCanvasID);
                        }

                        var obsresponse = lookUpSupObservee.Users;
                        foreach (var childnode in obsresponse)
                        {
                            if (childnode.Id == curStudCanvasID)
                            {
                                FoundRelationLink = 1;
                            }
                        }

                        //if (lookUpSupObservee.Users.Count() > 0 && st.canvas_user_id != null && st.canvas_user_id != 0)
                        if (curStudCanvasID != 0 && FoundRelationLink == 0)
                        {
                            var tempSupervisorCanvasID = supervisor_list.SupervisorCanvasID;
                            if (currentSupCanvasID != 0)
                            {
                                tempSupervisorCanvasID = currentSupCanvasID;
                            }
                            var AddResponse = canvas.Clients.UserClient.InsertObservee((Int64)tempSupervisorCanvasID, (Int64)curStudCanvasID);
                            var TurnOnDebugTracingMode = System.Configuration.ConfigurationManager.AppSettings["TurnOnDebugTracingMode"];
                            if (TurnOnDebugTracingMode != null)
                            {
                                if (TurnOnDebugTracingMode.ToLower() == "on" || 1==1)
                                {
                                    string ObserveRelationID = "";
                                    if (!string.IsNullOrEmpty(AddResponse.User.Id.ToString()))
                                    {
                                        ObserveRelationID = AddResponse.User.Id.ToString();
                                    }
                                    var Context = new SchoolEntities();
                                    Gsmu.Api.Data.School.Entities.Student student = Context.Students.First(st => st.STUDENTID == curStudentID);
                                    student.HiddenStudRegField4 = ObserveRelationID;
                                    student.ProfileViewedDateTime = DateTime.Now;
                                    Context.SaveChanges();

                                        Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                                    Audittrail.TableName = "User";
                                    Audittrail.AuditDate = DateTime.Now;
                                    Audittrail.RoutineName = "User - Add Canvas observee" + AuthorizationHelper.CurrentUser.LoggedInUserType;
                                    Audittrail.UserName = AuthorizationHelper.CurrentUser.LoggedInUsername;
                                    try
                                    {
                                        Audittrail.AuditAction = "Create relation during Roster Export: " + curStudCanvasID;
                                    }
                                    catch { }
                                    Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                                    LogManager.LogSiteActivity(Audittrail);
                                }
                            }
                        }
                    }
                }
            }
            //end supervisor student
        }

        public static Entities.Enrollment[] ExportCourseWithRoster(int courseId, entities.SchoolEntities db = null, Entities.Enrollment[] enrollments = null)
        {
            db = db ?? new entities.SchoolEntities();

            var gsmuCourse = (from c in db.Courses where c.COURSEID == courseId select c).FirstOrDefault();
            enrollments = SynchronizeCourse(gsmuCourse, db);
            ExportCourseRosters(gsmuCourse, db, enrollments);

            db.SaveChanges();
            return enrollments;
        }
        public static string ProcessCourseSectionReg(entities.Course gsmuCourse, CourseModel currentCourse, Course_Roster currentRoster, Int64? canvas_user_id=0, entities.SchoolEntities db = null)
        {
            string returnSection = "";
            //custom programming - create individual Course Section for each registration.
            if (canvas.Configuration.Instance.allowCourseSectionPerRegistration)
            {
                string CourseSectionType = "";
                string CourseSectionDuration = "";
                int CheckNumCount = 0;
                var courseSection = new Entities.CourseSection();
                JavaScriptSerializer j = new JavaScriptSerializer();

                if (!string.IsNullOrEmpty(gsmuCourse.CourseConfiguration) || currentRoster.OptionalCollectedInfo1 == "")
                {
                    dynamic CourseInfoconfiguration = j.Deserialize(gsmuCourse.CourseConfiguration, typeof(object));
                    if (CourseInfoconfiguration.ContainsKey("CourseSectionType"))
                    {
                        CourseSectionType = CourseInfoconfiguration["CourseSectionType"];
                    }
                    if (CourseInfoconfiguration.ContainsKey("CourseSectionDuration"))
                    {
                        CourseSectionDuration = CourseInfoconfiguration["CourseSectionDuration"];
                    }
                    if (!string.IsNullOrEmpty(CourseSectionType) || CourseSectionType.ToString() != "0")
                    {
                        try {
                            CheckNumCount = (from audit in db.AuditTrails where audit.ShortDescription.Contains(CourseSectionType) select audit).Count();
                            courseSection.Name = CourseSectionType + CheckNumCount.ToString("D5");
                            //courseSection.StartAt = currentCourse.CourseStartAsDate;
                            courseSection.StartAt = DateTime.Now;
                            DateTime EndDateTime = new DateTime();
                            //EndDateTime = currentCourse.CourseEndAsDate ?? DateTime.Now;
                            EndDateTime = DateTime.Now;
                            EndDateTime = EndDateTime.AddDays(Int32.Parse(CourseSectionDuration));
                            courseSection.EndAt = EndDateTime;
                            courseSection.sis_section_id = "BOD" + currentRoster.RosterID.ToString();
                            courseSection.restrict_enrollments_to_section_dates = true;
                            var CSresponse = CourseClient.InsertCourseSection(courseSection, gsmuCourse.canvas_course_id.ToString());
                            //c.canvas_course_id = response.Course.Id;
                            int newSectionID = 0;
                            if (!string.IsNullOrEmpty(CSresponse.Section.SectionId.ToString()))
                            {
                                newSectionID = CSresponse.Section.SectionId;
                                var sectionEnrollmentResp = ExportEnrollment(gsmuCourse, Entities.EnrollmenType.StudentEnrollment, canvas_user_id, newSectionID);

                                Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                                Audittrail.TableName = "[Course Roster]";
                                Audittrail.AuditDate = DateTime.Now;
                                Audittrail.RoutineName = "Create Course Section" + AuthorizationHelper.CurrentUser.LoggedInUserType;
                                Audittrail.UserName = AuthorizationHelper.CurrentUser.LoggedInUsername;
                                Audittrail.ShortDescription = CourseSectionType + CheckNumCount.ToString("D5");
                                Audittrail.AuditAction = "Create course section: " + "BOD" + currentRoster.RosterID.ToString();

                                Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                                LogManager.LogSiteActivity(Audittrail);

                                using (var db_instance = new entities.SchoolEntities())
                                {
                                    var roster_ind = (from e in db_instance.Course_Rosters where e.RosterID == currentRoster.RosterID select e).FirstOrDefault();
                                    roster_ind.OptionalCollectedInfo1 = newSectionID + "|" + CourseSectionType + CheckNumCount.ToString("D5");
                                    roster_ind.canvas_roster_id = sectionEnrollmentResp.Enrollment.Id;
                                    db_instance.SaveChanges();
                                }

                                returnSection = newSectionID + "|" + CourseSectionType + CheckNumCount.ToString("D5"); 
                            }
                        }
                        catch (Exception e) {
                            string sectionerror = e.Message;
                            return null;
                        }
                    }
                }
            }
            return returnSection;
        }

        public static void ExportCourseRosters(entities.Course gsmuCourse, entities.SchoolEntities db = null, Entities.Enrollment[] enrollments = null)
        {
            db = db ?? new entities.SchoolEntities();

            var model = new CourseModel(db, gsmuCourse);
            if (!model.Course.canvas_course_id.HasValue || model.Course.canvas_course_id.Value < 1)
            {
                SynchronizeCourse(gsmuCourse);
            }
            var canvasCourseId = model.Course.canvas_course_id.Value;

            if (enrollments == null)
            {
                int currentCourseSectionID = 0;
                if (!string.IsNullOrEmpty(gsmuCourse.CourseConfiguration) && gsmuCourse.CourseConfiguration.ToString() != "{}")
                {
                    currentCourseSectionID = GetCourseSectionID(gsmuCourse.CourseConfiguration.ToString());
                }
                if (currentCourseSectionID !=0) 
                {
                    var canvasEnrollmentResponse = EnrollmentClient.ListSectionEnrollments(gsmuCourse.canvas_course_id.Value,currentCourseSectionID);
                    enrollments = canvasEnrollmentResponse.Enrollments;
                } 
                else 
                {
                    var canvasEnrollmentResponse = EnrollmentClient.ListCourseEnrollments(gsmuCourse.canvas_course_id.Value);
                    enrollments = canvasEnrollmentResponse.Enrollments;
                }
            }
            var rosters = model.Course.PaidInFullRosters;
            if (WebConfiguration.CanvasSyncOnPendingPayment == "true")
            {
                rosters = model.Course.AvailableRosters;
            }
             // only non cancel, non waiting list people can get in
            // student enrollments            
            foreach (Course_Roster roster in rosters)
            {
                string RosterSectionType = "";
                bool isError = false; //remain false if no exception occurs 
                int curStudCanvasID = 0;
                var student = roster.StudentOld;
                try
                {
                    if (WebConfiguration.CanvasSkipSyncUserAccount == "false")
                    {
                        var studentResponse = SynchronizeStudent(student, db);
                        if (AuthorizationHelper.CurrentUser.LoggedInUserType == LoggedInUserType.Supervisor)
                        {
                            if (student.canvas_user_id == null || student.canvas_user_id == 0)
                            {
                                curStudCanvasID = int.Parse(studentResponse.User.Id.ToString());
                            }
                            else
                            {
                                curStudCanvasID = int.Parse(student.canvas_user_id.ToString());
                            }
                            canvas.CanvasExport.ExportSupervisorStudentRelation2Canvas(null, AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID, int.Parse(student.STUDENTID.ToString()), curStudCanvasID);
                        }
                    }

                    var enrollmentType = Entities.EnrollmenType.StudentEnrollment;
                    var userId = student.canvas_user_id;

                    //need to make this into a function since there are 3 places using this block of code
                    //here and line #752 and SSO Authme

                    ///////////////////////
                    if (!string.IsNullOrEmpty(student.canvas_user_id.ToString()))
                    {
                        curStudCanvasID = int.Parse(student.canvas_user_id.ToString());
                    }
                    if ((string.IsNullOrEmpty(student.canvas_user_id.ToString()) || curStudCanvasID == 0) && Gsmu.Api.Integration.Canvas.Configuration.Instance.EnableOAuth2Authentication && Gsmu.Api.Integration.Canvas.Configuration.Instance.autoMapCanvasAccount)
                    {
                        var lookUpUserInCanvas = Gsmu.Api.Integration.Canvas.Clients.UserClient.SearchCanvasUserByUserName(student.USERNAME.ToString());
                        string currentCustomIDField = "nothing here";
                        if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield1" && !string.IsNullOrEmpty(student.StudRegField1))
                        {
                            currentCustomIDField = student.StudRegField1;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield2")
                        {
                            currentCustomIDField = student.StudRegField2;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield3")
                        {
                            currentCustomIDField = student.StudRegField3;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield4")
                        {
                            currentCustomIDField = student.StudRegField4;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield5")
                        {
                            currentCustomIDField = student.StudRegField5;
                        }
                        if (lookUpUserInCanvas == null && Gsmu.Api.Integration.Canvas.Configuration.Instance.autoMapCanvasAccount && canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "username" && !string.IsNullOrEmpty(currentCustomIDField))
                        {
                            lookUpUserInCanvas = Gsmu.Api.Integration.Canvas.Clients.UserClient.SearchCanvasUserBySisUserID(currentCustomIDField);
                        }
                        //null mean no matching (per function catch return
                        if (lookUpUserInCanvas != null)
                        {
                            curStudCanvasID = int.Parse(lookUpUserInCanvas.User.Id.ToString());
                            student.canvas_user_id = curStudCanvasID;
                            using (var db_instance1 = new entities.SchoolEntities())
                            {
                                var Stud_ind = (from e in db_instance1.Students where e.STUDENTID == roster.STUDENTID select e).FirstOrDefault();
                                Stud_ind.canvas_user_id = Convert.ToInt64(curStudCanvasID);
                                db_instance1.SaveChanges();
                            }
                            userId = curStudCanvasID;
                        }
                    }
                    ///////////////////////
                    if (userId != null && userId != 0)
                    {
                        var existingEnrollment = (
                            from e in enrollments
                            where
                                (e.UserId == student.canvas_user_id &&
                                    (e.Type == Entities.EnrollmenType.Student || e.Type == Entities.EnrollmenType.StudentEnrollment)
                                )
                            select e).FirstOrDefault();

                        // enrollment exists
                        if (existingEnrollment != null)
                        {
                            // gsmu has enrollment
                            if (roster.canvas_roster_id > 0 && existingEnrollment.Id != roster.canvas_roster_id && canvas.Configuration.Instance.DisableRosterNormalizationOnExport == false)
                            {
                                DeleteEnrollment(gsmuCourse.canvas_course_id.Value, roster.canvas_roster_id.Value);
                            }
                            roster.canvas_roster_id = existingEnrollment.Id;
                            string currentCourseSectionType = roster.OptionalCollectedInfo1;
                            if (string.IsNullOrEmpty(currentCourseSectionType))
                            {
                                RosterSectionType = ProcessCourseSectionReg(gsmuCourse, model, roster, student.canvas_user_id.Value, db);
                            }
                            //custom section reg. need to check if it exist or not.
                        }
                        else
                        {
                            // enrollment does not exsit
                            if (!canvas.Configuration.Instance.allowCourseSectionPerRegistration)
                            {
                                Response enrollmentResponse = ExportEnrollment(gsmuCourse, enrollmentType, userId.Value, 0);
                                roster.canvas_roster_id = enrollmentResponse.Enrollment.Id;
                            }
                            else
                            {
                                //custom section reg
                                RosterSectionType = ProcessCourseSectionReg(gsmuCourse, model, roster, student.canvas_user_id.Value, db);
                            }
                        }
                        using (var db_instance = new entities.SchoolEntities())
                        {
                            var roster_ind = (from e in db_instance.Course_Rosters where e.RosterID == roster.RosterID select e).FirstOrDefault();
                            roster_ind.canvas_roster_id = roster.canvas_roster_id;
                            if (canvas.Configuration.Instance.allowCourseSectionPerRegistration && RosterSectionType != "")
                            {
                                //roster_ind.OptionalCollectedInfo1 = RosterSectionType;
                            }
                            else
                            {
                                roster_ind.canvas_roster_id = roster.canvas_roster_id;
                            }
                            db_instance.SaveChanges();
                        }
                    } else
                    {
                        // userID is not available = no enrollsync
                    }
                }
                catch (Exception einfo)
                {
                    string exceptioninfo = einfo.Message;
                    isError = true;
                    using (var db_instance = new entities.SchoolEntities())
                    {
                        var roster_ind = (from e in db_instance.Course_Rosters where e.RosterID == roster.RosterID select e).FirstOrDefault();
                        roster_ind.canvas_skip = 1;
                        db_instance.SaveChanges();
                    }
                    //need to have auditrail log here
                }
                if (isError) continue;   // move to next item if error occur
            }
            // end student enrollments
            db.SaveChanges();
        }

        private static Response ExportEnrollment(entities.Course gsmuCourse, Entities.EnrollmenType enrollmentType, Int64? userId, int CustomSection)
        {
            int paramCourseSectionID = 0;
            if (CustomSection == 0)
            {
                if (!string.IsNullOrEmpty(gsmuCourse.CourseConfiguration) && gsmuCourse.CourseConfiguration.ToString() != "{}")
                {
                    paramCourseSectionID = GetCourseSectionID(gsmuCourse.CourseConfiguration.ToString());
                }
            }
            else
            {
                paramCourseSectionID = CustomSection;
            }
            Response enrollmentResponse = null;
            var enrollment = new Entities.Enrollment();
            enrollment.CourseId = gsmuCourse.canvas_course_id.Value;
            enrollment.UserId = userId.Value;
            enrollment.Type = enrollmentType;
            enrollment.CourseSectionId = paramCourseSectionID;
            enrollment.EnrollmentState = Entities.EnrollmentState.active;
            enrollmentResponse = EnrollmentClient.InsertEnrollment(enrollment);
            return enrollmentResponse;
        }

        public static void SynchronizeOrder(string orderNumber, entities.SchoolEntities db = null, Response response = null)
        {
            db = db ?? new entities.SchoolEntities();
            int curStudCanvasID = 0;
            var synchronizedCourses = new List<int>();

            var rosters = (from cr in db.Course_Rosters where (cr.OrderNumber == orderNumber || cr.MasterOrderNumber == orderNumber) && (cr.PaidInFull == -1 || cr.PaidInFull == -1) && !(cr.Cancel == -1 || cr.Cancel == 1) && (cr.WAITING == 0) select cr).ToList();
            if (WebConfiguration.CanvasSyncOnPartialPayment == "true")
            {
                rosters = (from cr in db.Course_Rosters where (cr.OrderNumber == orderNumber || cr.MasterOrderNumber == orderNumber) && (cr.PaidInFull == -1 || cr.PaidInFull == -1 || cr.TotalPaid > 0) && !(cr.Cancel == -1 || cr.Cancel == 1) && (cr.WAITING == 0) select cr).ToList();
            }            
            
            foreach (var roster in rosters)
            {
                string RosterSectionType = "";
                var student = (from s in db.Students where s.STUDENTID == roster.STUDENTID select s).FirstOrDefault();
                var course = (from c in db.Courses where c.COURSEID == roster.COURSEID select c).FirstOrDefault();
                if (roster.COURSEID.HasValue && student != null && course != null && course.canvas_course_id.HasValue)
                {
                    //if (!synchronizedCourses.Contains(course.COURSEID))
                    //{
                        //no need to create course in Canvas when it's not in canvas intentionally.
                        //SynchronizeCourse(course, db);
                    //}
                    if (!string.IsNullOrEmpty(student.canvas_user_id.ToString()))
                    { 
                        curStudCanvasID = int.Parse(student.canvas_user_id.ToString());
                    }
                    if ((string.IsNullOrEmpty(student.canvas_user_id.ToString()) || curStudCanvasID == 0) && Gsmu.Api.Integration.Canvas.Configuration.Instance.EnableOAuth2Authentication && Gsmu.Api.Integration.Canvas.Configuration.Instance.autoMapCanvasAccount)
                    {
                        var lookUpUserInCanvas = Gsmu.Api.Integration.Canvas.Clients.UserClient.SearchCanvasUserByUserName(student.USERNAME.ToString());
                        string currentCustomIDField = "nothing here";
                        if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield1" && !string.IsNullOrEmpty(student.StudRegField1))
                        {
                            currentCustomIDField = student.StudRegField1;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield2")
                        {
                            currentCustomIDField = student.StudRegField2;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield3")
                        {
                            currentCustomIDField = student.StudRegField3;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield4")
                        {
                            currentCustomIDField = student.StudRegField4;
                        }
                        else if (canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField == "studregfield5")
                        {
                            currentCustomIDField = student.StudRegField5;
                        }
                        if (lookUpUserInCanvas == null && Gsmu.Api.Integration.Canvas.Configuration.Instance.autoMapCanvasAccount && canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "username" && !string.IsNullOrEmpty(currentCustomIDField))
                        {
                            lookUpUserInCanvas = Gsmu.Api.Integration.Canvas.Clients.UserClient.SearchCanvasUserBySisUserID(currentCustomIDField);
                        }
                        //null mean no matching (per function catch return
                        if (lookUpUserInCanvas != null)
                        {
                            curStudCanvasID = int.Parse(lookUpUserInCanvas.User.Id.ToString());
                            student.canvas_user_id = curStudCanvasID;
                            using (var db_instance1 = new entities.SchoolEntities())
                            {
                                var Stud_ind = (from e in db_instance1.Students where e.STUDENTID == roster.STUDENTID select e).FirstOrDefault();
                                Stud_ind.canvas_user_id = Convert.ToInt64(curStudCanvasID);
                                db_instance1.SaveChanges();
                            }
                        }
                    }

                    if (course.canvas_course_id != 0)
                    {
                        if (WebConfiguration.CanvasSkipSyncUserAccount == "false")
                        {
                            var studresp = SynchronizeStudent(student, db);
                            if (AuthorizationHelper.CurrentUser.LoggedInUserType == LoggedInUserType.Supervisor)
                            {
                                if (student.canvas_user_id == null || student.canvas_user_id == 0)
                                {
                                    curStudCanvasID = int.Parse(studresp.User.Id.ToString());
                                }
                                //else
                                //{
                                //    curStudCanvasID = int.Parse(student.canvas_user_id.ToString());
                                //}
                                canvas.CanvasExport.ExportSupervisorStudentRelation2Canvas(null, AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID, int.Parse(student.STUDENTID.ToString()), curStudCanvasID);
                            }
                        }
                        if (!canvas.Configuration.Instance.allowCourseSectionPerRegistration)
                        {
                            var enrollmentResponse = ExportEnrollment(course, Entities.EnrollmenType.StudentEnrollment, curStudCanvasID, 0);
                            roster.canvas_roster_id = enrollmentResponse.Enrollment.Id;
                        }
                        //custom section reg
                        var modelC = new CourseModel(db, course);
                        RosterSectionType = ProcessCourseSectionReg(course, modelC, roster, curStudCanvasID, db);

                        using (var db_instance = new entities.SchoolEntities())
                        {
                            var roster_ind = (from e in db_instance.Course_Rosters where e.RosterID == roster.RosterID select e).FirstOrDefault();
                            if (canvas.Configuration.Instance.allowCourseSectionPerRegistration && RosterSectionType != "")
                            {
                                //roster_ind.OptionalCollectedInfo1 = RosterSectionType;
                            }
                            else
                            {
                                roster_ind.canvas_roster_id = roster.canvas_roster_id;
                            }
                            db_instance.SaveChanges();
                        }
                    }
                }
            }
            db.SaveChanges();
        }

        public static Response DeleteEnrollment(int canvasCourseId, long canvasEnrollmentId)
        {
            var response = EnrollmentClient.DeleteEnrollment(canvasCourseId, canvasEnrollmentId);
            return response;
        }
    }
}
