using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Students
{
    public interface IStudentManager
    {
        StudentFullModel GetStudentFullInformation(int studentId);
        void SaveStudentFullInformation(StudentFullModel studentModel);
        StudentNotesModel GetStudentsNotes(int studentId);
        void SaveStudentsNotes(StudentNotesModel studentNotesMode);
        void DeactivateStudents(int studentid);
        StudentAuthModel GetStudentAuthBySessionId(string sessionId);
    }
}
