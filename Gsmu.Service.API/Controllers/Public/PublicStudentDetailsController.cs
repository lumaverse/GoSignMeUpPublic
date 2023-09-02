using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.Interface.Security.Authentication;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Gsmu.Service.API.Controllers.Public
{
    public class PublicStudentDetailsController: ApiController
    {
       private IAuthenticationManager _AuthenticationManager;

       public PublicStudentDetailsController(IAuthenticationManager AuthenticationManager)
          {
              _AuthenticationManager = AuthenticationManager;
        }
        [HttpGet]
        [Route("PublicStudentDetails/GetLoginStudent")]
        [ResponseType(typeof(ResponseRecordModel<List<StudentAuthenticationResponseModel>>))]
        public StudentAuthenticationResponseModel GetLoginStudent()
        {
            var data = _AuthenticationManager.GetLoginStudent();
            StudentAuthenticationResponseModel StudentAuthenticationResponseModel =  new StudentAuthenticationResponseModel();
            StudentAuthenticationResponseModel.Studentid = data.Studentid;
            return StudentAuthenticationResponseModel;
        }
    }
}