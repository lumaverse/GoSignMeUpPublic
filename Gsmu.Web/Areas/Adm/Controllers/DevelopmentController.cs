using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Web;
using Gsmu.Api.Networking.Mail;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class DevelopmentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult Email(string from = "patrik@gosignmeup.com", string to = "patrik@gosignmeup.com", string subject = "test subject", string body = "test body", bool isHtml = true, MailHandler mailHandler = MailHandler.DotNet)
        {
            var result = new JsonResult();
            try
            {
                MailClient.SendEmail(from, to, subject, body, isHtml, mailHandler);
            }
            catch (Exception e)
            {
                result.Data = e.InnerException == null ? e.Message : e.InnerException.Message;
            }
            return result;
        }

        [HttpPost]
        public ActionResult SetDevelopmentMode(bool state)
        {
            Gsmu.Api.Data.WebConfiguration.DevelopmentMode = state;
            return null;
        }

        public ActionResult Login(string username, string type = "student", int id = 0)
        {
            var message = string.Empty;
            using (var db = new SchoolEntities())
            {
                type = type.ToLower();
                switch (type)
                {
                    case "student":
                        var student = (from s in db.Students where s.STUDENTID == id || s.USERNAME == username select s).FirstOrDefault();
                        AuthorizationHelper.CompleteStudentLogin(student);
                        message = string.Format("Student with Id: {0} and username: {1}, logged in.", student.STUDENTID, student.USERNAME) ;
                        break;

                    case "instructor":
                        var instructor = (from i in db.Instructors where i.INSTRUCTORID == id || i.USERNAME == username select i).FirstOrDefault();
                        AuthorizationHelper.CompleteInstructorLogin(instructor);
                        message = string.Format("Instructor with Id: {0} and username: {1}, logged in.", instructor.INSTRUCTORID, instructor.USERNAME) ;
                        break;

                    default:
                        throw new ApplicationException(string.Format("Invalid user type {0}", type));
                }
            }


            return new ContentResult()
            {
                Content = message
            };
        }

    }
}
