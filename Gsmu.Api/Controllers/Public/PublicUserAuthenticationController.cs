using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.API.Security;
using Gsmu.Service.Interface.Security.Authentication;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.SessionState;

namespace Gsmu.Service.API.Controllers.Public
{
   // [Authorize]// -- uncomment this and this will ask for a token - uses OWIN implementation
    [GSMUCustomAuthorize] // -- use custom implementation
    public class PublicUserAuthenticationController: ApiController
    {
        private IAuthenticationManager _AuthenticationManager;

        public PublicUserAuthenticationController(IAuthenticationManager AuthenticationManager)
          {
              _AuthenticationManager = AuthenticationManager;
        }
        [HttpGet]
        [Route("PublicUserAuthentication/LoginStudent")]
        [ResponseType(typeof(ResponseRecordModel<List<StudentAuthenticationResponseModel>>))]
        public StudentAuthenticationResponseModel LoginStudent()
        {
            var data = _AuthenticationManager.GetLoginStudent();
            StudentAuthenticationResponseModel StudentAuthenticationResponseModel =  new StudentAuthenticationResponseModel();
            StudentAuthenticationResponseModel.Studentid = data.Studentid;
            return StudentAuthenticationResponseModel;
        }

    }
}