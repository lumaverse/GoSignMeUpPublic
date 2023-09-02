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
using System.Web.Script.Serialization;
using System.Dynamic;

namespace Gsmu.Web.Areas.Public.Controllers
{
    [GsmuAuthorization(LoggedInUserType.Supervisor, LoggedInUserType.Instructor, LoggedInUserType.Admin, LoggedInUserType.SubAdmin, LoggedInUserType.Student)]
    public class SupervisorController : Controller
    {
        // GET: Public/Supervisor
        public ActionResult Index(int userid = 0)
        {
            ViewBag.AdminView = false;
            if (AuthorizationHelper.CurrentAdminUser != null && userid > 0)
            {
                AuthorizationHelper.SetSupervisor(userid);
                AuthorizationHelper.UpdateCurrentLoginSupervisor(userid);
                ViewBag.AdminView = true;
            }
            ViewBag.UserName = AuthorizationHelper.CurrentSupervisorUser.UserName;
            ViewBag.AllowEnrollment = Settings.Instance.GetMasterInfo().PublicSignupAbilityOff;
            if (!AuthorizationHelper.CheckValidUserSessionId())
            {
                AuthorizationHelper.Logout();
                return RedirectToAction("Browse", "Course", new
                {
                    area = "Public",
                    message = "No valid Session"
                });
            }
            return View();
        }

        public ActionResult GetData()
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var supervisor = (from s in db.Supervisors where s.SUPERVISORID == AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select s).First();

                var district = new District();
                try
                {
                    district = (from dst in db.Districts where dst.DISTID == supervisor.DISTRICT select dst).First();
                }
                catch
                {
                }

                var school = new List<School>();
                try
                {
                    school = (from supsch in db.SupervisorSchools
                              join sch in db.Schools on supsch.SchoolID equals sch.locationid
                              where supsch.SupervisorID == supervisor.SUPERVISORID
                              orderby sch.LOCATION
                              select sch).ToList();
                }
                catch
                {
                }

                return new JsonResult()
                {
                    Data = new
                    {
                        supervisor = supervisor,
                        district = district,
                        school = school
                    }
                };
            }
        }

        public String SaveIdentity(string username, string password, HttpPostedFileBase upload, bool profileImageRemove)
        {
            var success = true;
            var message = "The updated data has been saved.";
            Supervisor supervisor = null;

            try
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    supervisor = (from s in db.Supervisors where s.SUPERVISORID == AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select s).First();

                    var profileDeleted = false;

                    Action deleteFile = delegate()
                    {
                        if (profileDeleted)
                        {
                            return;
                        }
                        System.IO.File.Delete(WebConfiguration.SupervisorProfileImageDirectoryAbsolutePath + supervisor.ProfileImageFile);
                        supervisor.ProfileImageFile = null;
                        profileDeleted = true;
                    };

                    if (profileImageRemove)
                    {
                        deleteFile();
                    }

                    if (upload != null && upload.ContentLength > 0 && upload.ContentType.ToLowerInvariant().Contains("image"))
                    {
                        if (!string.IsNullOrWhiteSpace(supervisor.ProfileImageFile))
                        {
                            deleteFile();
                        }

                        var fileName = supervisor.SUPERVISORID.ToString() + Path.GetExtension(upload.FileName);
                        upload.SaveAs(WebConfiguration.SupervisorProfileImageDirectoryAbsolutePath + "\\" + fileName);
                        supervisor.ProfileImageFile = fileName;
                    }

                    supervisor.UserName = username;
                    if (password != "**********" && !string.IsNullOrEmpty(password))
                    {
                        supervisor.PASSWORD = password;
                    }
                    supervisor.date_modified = DateTime.Now;

                    AuthorizationHelper.CurrentUser = supervisor;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("There was an error saving the data information: {0}", e.Message);
                supervisor = null;
            }

            JsonResult jsonResult = new JsonResult
            {
                Data = new
                {
                    success = success,
                    message = message,
                    supervisor = supervisor
                }
            };
            // to fix the wierd error in IE when submitting xtype:fieldcontainer it download a json file. Return must be in string
            string json = new JavaScriptSerializer().Serialize(jsonResult.Data);
            return json;
        }

        public ActionResult SaveInformation(string first, string last, string title, string address, string city, string state, string zip, string phone, string fax, string supervisornum, string email, string additionalemailaddresses)
        {
            var success = true;
            var message = "The updated data has been saved.";
            Supervisor supervisor = null;

            try
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    supervisor = (from s in db.Supervisors where s.SUPERVISORID == AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select s).First();

                    supervisor.FIRST = first;
                    supervisor.LAST = last;
                    supervisor.TITLE = title;
                    supervisor.ADDRESS = address;
                    supervisor.CITY = city;
                    supervisor.STATE = state;
                    supervisor.ZIP = zip;
                    supervisor.PHONE = phone;
                    supervisor.FAX = fax;
                    supervisor.SUPERVISORNUM = supervisornum;
                    supervisor.EMAIL = email;
                    supervisor.AdditionalEmailAddresses = additionalemailaddresses;
                    supervisor.date_modified = DateTime.Now;

                    AuthorizationHelper.CurrentUser = supervisor;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("There was an error saving the data information: {0}", e.Message);
                supervisor = null;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = success,
                    message = message,
                    supervisor = supervisor
                }
            };

        }

        public ActionResult SaveSettings(int? advanceoptions)
        {
            var success = true;
            var message = "The updated data has been saved.";
            Supervisor supervisor = null;

            try
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    supervisor = (from s in db.Supervisors where s.SUPERVISORID == AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select s).First();

                    supervisor.AdvanceOptions = advanceoptions.HasValue ? advanceoptions.Value : 0;
                    supervisor.date_modified = DateTime.Now;

                    AuthorizationHelper.CurrentUser = supervisor;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("There was an error saving the data information: {0}", e.Message);
                supervisor = null;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = success,
                    message = message,
                    supervisor = supervisor
                }
            };

        }
        public string GetPrincipalStudent(int cid)
        {
            string principalStudentName = "";
            if (CourseShoppingCart.Instance.MultipleStudentCourses != null)
            {
                foreach (var item in CourseShoppingCart.Instance.MultipleStudentCourses)
                {
                    if (item.CourseId == cid)
                    {
                        if (principalStudentName.IndexOf(item.UserName +" "+item.FirstName + " " + item.LastName + "") <= 0)
                        {
                            principalStudentName = principalStudentName + item.UserName +" " +item.FirstName + " " + item.LastName + "";
                        }
                    }
                }
            }

            return principalStudentName;

        }
        public ActionResult Students(int start = 0, int limit = 5, string filter = null, string sort = null, int courseId = 0)
        {
            string folderpath = Server.MapPath("/Temp/");
            if (!Directory.Exists(folderpath))
            {
                System.IO.Directory.CreateDirectory(folderpath);
            }
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
                result.Data = Gsmu.Api.Data.School.Supervisor.SupervisorHelper.GetAllStudents(queryState, courseId, 0, folderpath);
            }
            catch (Exception e)
            {
                result.Data = e.InnerException + " " + e.Message;

            }
            return result;
        }

        public ActionResult StudentsWaitingList()
        {
            return View();
        }

        public ActionResult WaitingStudentsData()
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.Data = Gsmu.Api.Data.School.Supervisor.SupervisorHelper.GetAllWaitingStudents();
            }
            catch (Exception e)
            {
                result.Data = e.InnerException + " " + e.Message;

            }
            return result;
        }

        public ActionResult ApprovedListStudentsData()
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.Data = Gsmu.Api.Data.School.Supervisor.SupervisorHelper.GetAllWaitingStudents(true);
            }
            catch (Exception e)
            {
                result.Data = e.InnerException + " " + e.Message;

            }
            return result;
        }

        [HttpPost]
        public ActionResult ApproveWaitingStudent(int rosterid, bool sendEmail)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                Gsmu.Api.Data.School.Supervisor.SupervisorHelper.ApproveWaitingStudent(rosterid, sendEmail);
                dynamic response = new ExpandoObject();
                response.Success = true;
                result.Data = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                result.Data = e.InnerException + " " + e.Message;

            }
            return result;
        }

        [HttpPost]
        public ActionResult MoveToApproveToWaitStudent(int rosterid)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                Gsmu.Api.Data.School.Supervisor.SupervisorHelper.MoveToApproveToWaitStudent(rosterid);
                dynamic response = new ExpandoObject();
                response.Success = true;
                result.Data = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                result.Data = e.InnerException + " " + e.Message;

            }
            return result;
        }

        public ActionResult SendStudentEmail(string Subject, string To, string Message)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                EmailAuditTrail email = new EmailAuditTrail();
                email.EmailTo = To;
                email.EmailSubject = Subject;
                email.EmailBody = HttpUtility.HtmlDecode(Message);
                email.AuditProcess = "Supervisor Dash";
                email.AuditDate = DateTime.Now;
                EmailFunction.SendEmail(email);
                if (Gsmu.Api.Data.School.Supervisor.SupervisorHelper.SendingEmailStatus == "Sent")
                {
                    result.Data = "Message Sent.";
                }
                else
                {
                    result.Data = "Message Not Sent.";
                }
            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }
        public ActionResult GetStudentRecord(int studentId)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.Data = Gsmu.Api.Data.School.Supervisor.SupervisorHelper.GetStudent(studentId);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public string SetPrincipalStudentonCart(int studentId)
        {
            string result = "";
            try
            {
                if (CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent > 0 && CourseShoppingCart.Instance.Count > 0)
                {
                    var student = Gsmu.Api.Data.School.Entities.Student.GetStudent(CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent);
                    result = "notallowed:" + student.FIRST + " " + student.LAST;
                }
                else
                {
                    Gsmu.Api.Data.School.DataLists dlists = new Gsmu.Api.Data.School.DataLists();
                    String resultui = dlists.CheckReqMissingFields(studentId, "ST");

                    if(resultui== "NoMissingReqFields")
                    {

                        CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent = studentId;
                        result = "success";
                    }
                    else
                    {
                        result = "missingfield"+ studentId;
                    }
                    
                }
            }
            catch (Exception)
            {
                result = "error";
            }
            return result;
        }

        public ActionResult GetStudentTranscript(int studentId)
        {
            var result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.Data = Gsmu.Api.Data.School.Supervisor.SupervisorHelper.GetStudentTranscript(studentId);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public string ExporttoCSVfile()
        {
            string folderpath = Server.MapPath("/Temp/");
            return Gsmu.Api.Data.School.Supervisor.SupervisorHelper.Export(folderpath);
        }


        public ActionResult EditStudentInfo(int sid)
        {
            if (Gsmu.Api.Data.School.Supervisor.SupervisorHelper.ValidateStudentSupervisor(sid))
            {
                ViewBag.SelectedUserid = sid.ToString();
                SetPrincipalStudentonCart(sid);
                return View();
            }
            else
            {
                return View("Index");
            }
        }

    }
}