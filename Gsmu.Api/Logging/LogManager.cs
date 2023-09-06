using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using school = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Logging
{
    public static class LogManager
    {
        /// <summary>
        /// Make a call from any class like LogManager.LogException(this, e);
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        public static void LogException(string loggee, string message, Exception e)
        {
            using (var db = new school.SchoolEntities())
            {
                var fullDetails = string.Format("{0}\n{1}\n{2}\n{3}", loggee, message, e.Message, e.StackTrace);
                var log = new school.AuditTrail();
                if (AuthorizationHelper.CurrentStudentUser != null)
                {
                    log.StudentID = AuthorizationHelper.CurrentStudentUser.STUDENTID;
                }
                else
                {
                    log.StudentID = 0;
                }
                log.CourseID = 0;
                if (loggee.Length > 254)
                {
                    log.RoutineName = loggee.Substring(0, 254).PadLeft(254);
                }
                else
                {
                    log.RoutineName = loggee.PadLeft(254);
                }
                log.AuditDate = DateTime.Now;
                log.AuditAction = fullDetails;
                if (message.Length > 254)
                {
                    log.ShortDescription = message.Substring(0, 254).PadLeft(254);
                }
                else
                {
                    log.ShortDescription = message.PadLeft(254);
                }
                if (e.Message.Length > 248)
                {
                    log.DetailDescription = e.Message.Substring(0, 248).PadLeft(248);
                }
                else
                {
                    log.DetailDescription = e.Message.PadLeft(248);
                }
                log.ATErrorMsg = e.StackTrace;
                db.AuditTrails.Add(log);
                db.SaveChanges();
            }
        }

        public static void LogJsonSerializableObject(string loggee, string message, object result)
        {
            if (string.IsNullOrWhiteSpace(loggee))
            {
                throw new ArgumentException("LogJsonSerializableObject loggee argument must have a value!");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("LogJsonSerializableObject message argument must have a value!");
            }

            using (var db = new school.SchoolEntities())
            {
                var resultString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                var description = "The JSON object is in the ATErrorMsg field.";
                var fullDetails = string.Format("{0}\n{1}\n{2}\n{3}", loggee, description, message, resultString);
                var log = new school.AuditTrail();
                log.RoutineName = loggee.Length > 255 ? loggee.Substring(0, 255) : loggee;
                log.AuditDate = DateTime.Now;
                log.ShortDescription = message.Length > 255 ? message.Substring(0, 255) : message; 
                log.DetailDescription = description;
                log.ATErrorMsg = resultString;
                log.AuditAction = fullDetails;
                db.AuditTrails.Add(log);
                db.SaveChanges();
            }
        }

        public static void GenericLogToAuditTrail(AuditTrail log)
        {
            using (var db = new school.SchoolEntities())
            {
                db.AuditTrails.Add(log);
                db.SaveChanges();
            }
        }
    }



    public class LogManagerDispossable
    {

        public  int LogSiteActivity(school.AuditTrail AuditTrail)
        {
            using (var db = new school.SchoolEntities())
            {
                int id = db.AuditTrails.Add(AuditTrail).AuditID;
                int result = db.SaveChanges();
                return result;
            }
        }
    }
}
