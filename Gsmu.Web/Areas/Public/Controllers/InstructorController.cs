using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using System.Drawing;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Networking.Mail;
using Gsmu.Api.Data.School.InstructorHelper;
using System.Web.Script.Serialization;
using Gsmu.Api.Data.School.CourseRoster;
using Gsmu.Api.Web;
using Gsmu.Api.Data.School.Attendance;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class InstructorConfiguration
    {
        public int allowstudentenroll
        {
            get;
            set;
        }
        public int allowstudentaddedit
        {
            get;
            set;
        }
    }
    [GsmuAuthorization(LoggedInUserType.Instructor, LoggedInUserType.Admin, LoggedInUserType.SubAdmin)]
    public class InstructorController : Controller
    {
        // GET: Public/Instructor
        public ActionResult Index()
        {
            ViewBag.HideCertificateLink = Settings.Instance.GetMasterInfo3().hideInstructorCertLink;
            ViewBag.AllowInstructortoTakeAttendace = Settings.Instance.GetMasterInfo().InstructorAttend;
            ViewBag.UserName = AuthorizationHelper.CurrentInstructorUser.USERNAME;
            ViewBag.VisibleStudentDISTRICT = Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo().VisibleStudentDISTRICT);
            ViewBag.VisibleStudentSCHOOL = Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo().VisibleStudentSCHOOL);
            ViewBag.VisibleStudentGRADE =Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo().VisibleStudentGRADE);
            ViewBag.HideManagerRoomInPublic = Settings.Instance.GetMasterInfo3().HideManagerRoomInPublic;

            ViewBag.HasSubAdminAccount = CheckSubAdminAccount();
            var ValInstructorConfiguration = GetInstructorConfiguration(Settings.Instance.GetMasterInfo4().InstructorConfiguration);
            if (AuthorizationHelper.CurrentInstructorUser != null)
            {
                ViewBag.CurrentInstructorId = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                if (!AuthorizationHelper.CheckValidUserSessionId())
                {
                    AuthorizationHelper.Logout();
                    return RedirectToAction("Browse", "Course", new
                    {
                        area = "Public",
                        message = "No valid Session"
                    });
                }
            }
            if (ValInstructorConfiguration != null)
            {
                ViewBag.allowstudentenroll = ValInstructorConfiguration.allowstudentenroll;
                ViewBag.allowstudentaddedit = ValInstructorConfiguration.allowstudentaddedit;
            }
            else
            {
                ViewBag.allowstudentenroll = 0;
                ViewBag.allowstudentaddedit = 0;
            }
            return View();
        }
        public InstructorConfiguration GetInstructorConfiguration(string jsonfields)
        {
            InstructorConfiguration fields = new InstructorConfiguration();
            List<InstructorConfiguration> ListSelectedFields = new List<InstructorConfiguration>();
            JavaScriptSerializer j = new JavaScriptSerializer();
            dynamic settingsconfig = j.Deserialize(jsonfields, typeof(object));
            fields = new InstructorConfiguration();
            fields.allowstudentenroll = int.Parse(settingsconfig["allowstudentenroll"]);
            fields.allowstudentaddedit = int.Parse(settingsconfig["allowstudentaddedit"]);

            return fields;
        }

        public int CheckSubAdminAccount()
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                return (from s in db.Managers where s.InstructorId == AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID select s).Count();
            }
        }
        public string SendStudentSurvey(int? cid)
        {

            Gsmu.Api.Data.School.Course.CourseModel course = new Gsmu.Api.Data.School.Course.CourseModel(cid.Value);
            string CourseDetails = "Course Name: " + course.Course.COURSENAME + " starting on "+ course.CourseStartAsDate.ToString() +" and ending on " + course.CourseEndAsDate.ToString();
            int survey_type = 0;
            if (course.CourseStartAsDate >= DateTime.Now)
            {
                survey_type = 0;
            }
            else
            {
                if (course.CourseEndAsDate <= DateTime.Now)
                    survey_type = 1;
                else
                    return "Course is still on going.";
            }
            return Gsmu.Api.Data.Survey.SurveyEmail.SendSurveyEmail(survey_type,cid.Value,CourseDetails);
        }

        public ActionResult GenerateRosterReport(int cid)
        {
            var callResult = RosterReport.GenerateRosterReport(Request, false, cid);
            var result = new JavaScriptResult();
            string arguments = SerializationHelper.SerializeEntity(callResult);
            result.Script = Request["callback"] + "(" + arguments + ");";
            return result;
        }
        public ActionResult GenerateAttendanceReport()
        {
            var callResult = AttendanceModel.AddAttendanceReport(Request, false);
            var result = new JavaScriptResult();
            string arguments = SerializationHelper.SerializeEntity(callResult);
            result.Script = Request["callback"] + "(" + arguments + ");";
            return result;
        }
        public ActionResult GetData()
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var instructor = (from s in db.Instructors where s.INSTRUCTORID == AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID select s).First();

                var result=  new JsonResult()
                {
                    Data = instructor
                };

                result.JsonRequestBehavior = result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        public ActionResult GetCourses(int start = 0, int limit = 5, string filter = null, string sort = null, int instructorid = 0)
        {
                        var result = new JsonResult();
                        string folderpath = Server.MapPath("/Temp/");
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
                var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);

                var queryState = new QueryState(start, limit)
                {
                    OrderByDirection = sorterResult.Value,
                    OrderFieldString = sorterResult.Key,
                    Filters = filterResult
                };
                instructorid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                result.Data = InstructorHelper.GetAllCourses(queryState, instructorid, folderpath,"current");
            }
            catch (Exception e)
            {
            }
            return result;
        }
        public ActionResult GetPastCourses(int start = 0, int limit = 5, string filter = null, string sort = null, int instructorid = 0)
        {
            var result = new JsonResult();
            string folderpath = Server.MapPath("/Temp/");
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
                var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);

                var queryState = new QueryState(start, limit)
                {
                    OrderByDirection = sorterResult.Value,
                    OrderFieldString = sorterResult.Key,
                    Filters = filterResult
                };
                instructorid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                result.Data = InstructorHelper.GetAllCourses(queryState, instructorid, folderpath,"past");
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public ActionResult GetCancelledCourses(int start = 0, int limit = 5, string filter = null, string sort = null, int instructorid = 0)
        {
            var result = new JsonResult();
            string folderpath = Server.MapPath("/Temp/");
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
                var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);

                var queryState = new QueryState(start, limit)
                {
                    OrderByDirection = sorterResult.Value,
                    OrderFieldString = sorterResult.Key,
                    Filters = filterResult
                };
                instructorid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                result.Data = InstructorHelper.GetAllCourses(queryState, instructorid, folderpath, "cancelled");
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public ActionResult GetNeedAttendanceCourses(int start = 0, int limit = 5, string filter = null, string sort = null, int instructorid = 0)
        {
            var result = new JsonResult();
            string folderpath = Server.MapPath("/Temp/");
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
                var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);

                var queryState = new QueryState(start, limit)
                {
                    OrderByDirection = sorterResult.Value,
                    OrderFieldString = sorterResult.Key,
                    Filters = filterResult
                };
                instructorid = AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID;
                result.Data = InstructorHelper.GetAllCourses(queryState, instructorid, folderpath, "needattendance");
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public ActionResult SaveInformation(string first, string last, string title, string address, string city, string state, string zip, string phone, string fax, string instructornum, string email, string additionalemailaddresses, string district, string grade, string school, string experience, string workphone)
        {
            var success = true;
            var message = "The updated data has been saved.";
            Instructor instructor = null;

            try
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    instructor = (from s in db.Instructors where s.INSTRUCTORID == AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID select s).First();

                    instructor.FIRST = first;
                    instructor.LAST = last;
                    instructor.ADDRESS = address;
                    instructor.CITY = city;
                    instructor.STATE = state;
                    instructor.ZIP = zip;
                    instructor.HOMEPHONE = phone;
                    instructor.FAX = fax;
                    instructor.INSTRUCTORNUM = instructornum;
                    instructor.EMAIL = email;
                    instructor.date_modified = DateTime.Now;
                    instructor.EXPERIENCE = experience;
                    instructor.WORKPHONE = workphone;
                    int dist,sch,gra;
                    if (int.TryParse(district, out dist))
                    {
                        instructor.DISTRICT = dist;
                    }
                    if (int.TryParse(school, out sch))
                    {
                        instructor.SCHOOL = sch;
                    }
                    if (int.TryParse(grade, out gra))
                    {
                        instructor.GRADELEVEL = gra.ToString();
                    }



                    AuthorizationHelper.CurrentUser = instructor;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("There was an error saving the data information: {0}", e.Message);
                instructor = null;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = success,
                    message = message,
                    instructor = instructor
                }
            };

        }


        public ActionResult SaveIdentity(string username, string password, HttpPostedFileBase upload, bool profileImageRemove)
        {
            var success = true;
            var message = "The updated data has been saved.";
            Instructor instructor = null;

            try
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    instructor = (from s in db.Instructors where s.INSTRUCTORID == AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID select s).First();

                    var profileDeleted = false;

                    Action deleteFile = delegate()
                    {
                        if (profileDeleted)
                        {
                            return;
                        }
                        System.IO.File.Delete(WebConfiguration.InstructorImageDirectoryAbsolutePath + instructor.PhotoImage);
                        instructor.PhotoImage = null;
                        profileDeleted = true;
                    };

                    if (profileImageRemove)
                    {
                        deleteFile();
                    }

                    if (upload != null && upload.ContentLength > 0 && upload.ContentType.ToLowerInvariant().Contains("image"))
                    {
                        if (!string.IsNullOrWhiteSpace(instructor.PhotoImage))
                        {
                            deleteFile();
                        }

                        var fileName = instructor.INSTRUCTORID.ToString() + Path.GetExtension(upload.FileName);
                        upload.SaveAs(WebConfiguration.InstructorImageDirectoryAbsolutePath + "\\" + fileName);
                        instructor.PhotoImage = fileName;
                    }

                    instructor.USERNAME = username;
                    if (password != "**********" && !string.IsNullOrEmpty(password))
                    {
                        instructor.PASSWORD = password;
                    }
                    instructor.date_modified = DateTime.Now;
                    AuthorizationHelper.CurrentUser = instructor;
                    if (Gsmu.Api.Integration.Canvas.Configuration.Instance.UserSynchronizationInDashboard && Gsmu.Api.Integration.Canvas.Configuration.Instance.EnableOAuth2Authentication)
                    {
                        Gsmu.Api.Integration.Canvas.CanvasExport.SynchronizeInstructor(instructor.INSTRUCTORID, db);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("There was an error saving the data information: {0}", e.Message);
                instructor = null;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = success,
                    message = message,
                    instructor = instructor
                }
            };
        }

        public ActionResult Students(int start = 0, int limit = 5, string filter = null, string sort = null,int courseId =0)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
                var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);

                var queryState = new QueryState(start, limit)
                {
                    OrderByDirection = sorterResult.Value,
                    OrderFieldString = sorterResult.Key,
                    Filters = filterResult
                };
                result.Data = Gsmu.Api.Data.School.Supervisor.SupervisorHelper.GetAllStudents(queryState, courseId);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public string SendSurvey(int? courseId)
        {
            Gsmu.Web.Areas.Public.Controllers.InstructorController instructorController = new Areas.Public.Controllers.InstructorController();
            return instructorController.SendStudentSurvey(courseId);

        }

        public ActionResult GetSurveyResultDetails(int start = 0, int limit = 5, int userid = 0, string filter = null, string sort = null)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
                var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);

                var queryState = new QueryState(start, limit)
                {
                    OrderByDirection = sorterResult.Value,
                    OrderFieldString = sorterResult.Key,
                    Filters = filterResult
                };
                result.Data = InstructorHelper.GetSurveyResultDetails(queryState, AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID);
            }
            catch (Exception)
            {
            }
            return result;
        }

    }
}