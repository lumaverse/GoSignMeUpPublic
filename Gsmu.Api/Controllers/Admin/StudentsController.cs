using Gsmu.Service.API.Extensions;
using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.Interface.Students;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Gsmu.Service.API.Controllers.Admin
{
    public class StudentsController : ApiController
    {
        private IStudentManager _studentManager;

        public StudentsController(IStudentManager studentManager)
        {
            _studentManager = studentManager;
        }

        [HttpGet]
        [Route("Students/GetStudentNotes")]
        [ResponseType(typeof(ResponseRecordModel<List<StudentNotesModel>>))]
        public IHttpActionResult GetStudentNotes(int studentId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _studentManager.GetStudentsNotes(studentId);
                return new ResponseRecordModel<StudentNotesModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Student Notes Success",
                    Data = data
                };
            }));
        }

        [HttpPost]
        [Route("Students/SaveStudentNotes")]
        [ResponseType(typeof(ResponseRecordModel<StudentNotesModel>))]
        public IHttpActionResult SaveStudentNotes([FromBody] StudentNotesModel studentNotesModel)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _studentManager.SaveStudentsNotes(studentNotesModel);
                return new ResponseRecordModel<StudentNotesModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Successfully Saved Student Notes"
                };
            }));
        }
    }
}
