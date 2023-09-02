using Gsmu.Service.Models.Admin.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;

namespace Gsmu.Service.Interface.Admin.Portal
{
    public interface IAttendanceTakingManager
    {
        List<AdminAttendanceTakingModel> GetRosterList(int courseId);
        AttendanceCourseDetails GetCourseDetails(int courseId);
        void SaveStatusAttendance(int rosterid, int status);
        void SaveDateAttendance(int rosterid,string date, int status);
        void SaveAttendanceGrade(int rosterid, string grade);
        void SaveAttendanceCredit(int rosterid, string type, int value);
        void TranscribeSingleStudent(int rosterid);
        void TranscribeAllStudents(int courseid);
    }
}
