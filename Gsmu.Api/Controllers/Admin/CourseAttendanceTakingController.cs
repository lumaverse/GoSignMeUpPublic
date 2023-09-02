using Gsmu.Service.API.Extensions;
using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.Interface.Admin.Portal;
using Gsmu.Service.Interface.Admin.Reports;
using Gsmu.Service.Models.Admin.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;


namespace Gsmu.Service.API.Controllers.Admin
{
    public class CourseAttendanceTakingController : ApiController
    {
        private IAttendanceTakingManager _IAttendanceTakingManager;
        public CourseAttendanceTakingController(IAttendanceTakingManager attendanceTakingManager)
        {
            _IAttendanceTakingManager = attendanceTakingManager;
        }
        [HttpGet]
        [Route("CourseAttendanceTaking/GetCourseDetails")]
        [ResponseType(typeof(ResponseRecordModel<AttendanceCourseDetails>))]
        public IHttpActionResult GetCourseDetails(int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _IAttendanceTakingManager.GetCourseDetails(courseId);
                return new ResponseRecordModel<AttendanceCourseDetails>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Attendance Course Details Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("CourseAttendanceTaking/GetRosterList")]
        [ResponseType(typeof(ResponseRecordModel<List<AdminAttendanceTakingModel>>))]
        public IHttpActionResult GetRosterList(int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _IAttendanceTakingManager.GetRosterList(courseId);
                return new ResponseRecordModel<List<AdminAttendanceTakingModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Attendance Rosters List Success",
                    Data = data
                };
            }));
        }

        [HttpPost]
        [Route("CourseAttendanceTaking/SaveDateAttendance")]
        public void SaveDateAttendance(int rosterId, string date, int status)
        {
            _IAttendanceTakingManager.SaveDateAttendance(rosterId,date,status);
        }
        [HttpPost]
        [Route("CourseAttendanceTaking/SaveStatusAttendance")]
        public void SaveStatusAttendance(int rosterId, int status)
        {
            _IAttendanceTakingManager.SaveStatusAttendance(rosterId, status);
        }
        [HttpPost]
        [Route("CourseAttendanceTaking/SaveAttendanceGrade")]
        public void SaveAttendanceGrade(int rosterId, string grade)
        {
            _IAttendanceTakingManager.SaveAttendanceGrade(rosterId,grade);
        }
        [HttpPost]
        [Route("CourseAttendanceTaking/SaveAttendanceCredit")]
        public void SaveAttendanceCredit(int rosterId, string type, int creditvalue)
        {
            _IAttendanceTakingManager.SaveAttendanceCredit(rosterId,type,creditvalue);
        }
        [HttpPost]
        [Route("CourseAttendanceTaking/TranscribeSingleStudent")]
        public void TranscribeSingleStudent(int rosterId)
        {
            _IAttendanceTakingManager.TranscribeSingleStudent(rosterId);
        }
        [HttpPost]
        [Route("CourseAttendanceTaking/TranscribeAllStudents")]
        public void TranscribeAllStudents(int courseid)
        {
            _IAttendanceTakingManager.TranscribeAllStudents(courseid);
        }
    }
}