using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.Interface.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Gsmu.Service.API.Extensions;

namespace Gsmu.Service.API.Controllers.Public
{
    public class PublicStudentRegistrationFieldsController:ApiController
    {
        private IStudentRegistrationField _StudentRegistrationField;

        public PublicStudentRegistrationFieldsController(IStudentRegistrationField StudentRegistrationField)
          {
              _StudentRegistrationField = StudentRegistrationField;
        }
        [HttpGet]
        [Route("PublicStudentRegistrationFields/GetStudentRegistrationFields")]
        [ResponseType(typeof(ResponseRecordModel<object>))]
        public IHttpActionResult GetStudentRegistrationFields()
        {
            var StudentRegistrationField = _StudentRegistrationField.GetStudentRegistrationWidgets();
            string Message = "Get StudentRegistration Widget Success.";
            if (StudentRegistrationField == null)
            {
                Message = "No Available Widgets";
            }
            var ScrubbedResult=  new System.Web.Mvc.JsonResult()
            {
                Data = StudentRegistrationField,
            };
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = ScrubbedResult;
                return new ResponseRecordModel<object>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = Message,
                    Data = data.Data
                };
            }));


        }
    }
}