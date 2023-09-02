using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.Interface.Students;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.Students
{
    public class StudentManager : IStudentManager
    {
        private ISchoolEntities _db;
        private string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);
        private string surveyConnString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.surveyEntitiesKey);

        public StudentManager() {
            _db = new SchoolEntities(connString);
        }
        /// <summary>
        /// Gets Full Information of the Student
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public StudentFullModel GetStudentFullInformation(int studentId) {
            return new StudentFullModel();
        }
        /// <summary>
        /// Save Full Information of the Student
        /// </summary>
        /// <param name="studentModel"></param>
        public void SaveStudentFullInformation(StudentFullModel studentModel) {

        }
        /// <summary>
        /// Gets Student Notes Information
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public StudentNotesModel GetStudentsNotes(int studentId) {
            return _db.Students.Where(s => s.STUDENTID == studentId)
                   .Select(s => new StudentNotesModel()
                   {
                       StudentId = s.STUDENTID,
                       StudentNumber = s.STUDNUM,
                       FirstName = s.FIRST,
                       LastName = s.LAST,
                       Notes = s.NOTES,
                       LastUpdatedDate = s.LastUpdateTime
                   }).SingleOrDefault();
        }
        /// <summary>
        /// Saves Student Notes Information
        /// </summary>
        /// <param name="studentModel"></param>
        public void SaveStudentsNotes(StudentNotesModel studentNotesMode)
        {
            using (var db = new SchoolEntities(connString))
            {
                var student = db.Students.Where(s => s.STUDENTID == studentNotesMode.StudentId).SingleOrDefault();
                if (student != null) {
                    student.NOTES = studentNotesMode.Notes;
                    db.SaveChanges();
                }
            }
        }

        public void DeactivateStudents(int studentid)
        {
            using (var db = new SchoolEntities(connString))
            {
                var student = db.Students.Where(s => s.STUDENTID == studentid).SingleOrDefault();
                if (student != null)
                {
                    student.InActive = 1;
                    db.SaveChanges();
                }
            }
        }

        public StudentAuthModel GetStudentAuthBySessionId(string sessionId) {
            var studentCreds = _db.Students.Where(s => s.UserSessionId == Guid.Parse(sessionId)).SingleOrDefault();
            return new StudentAuthModel()
            {
                UserName = studentCreds.USERNAME,
                Password = studentCreds.PASSWORD
            };
        }
    }
}
