using Gsmu.Api.Authorization;
using Gsmu.Service.BusinessLogic.Security.Authentication;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace Gsmu.Service.BusinessLogic.Students
{
    public static class StudentAuthenticationManager
    {
        public static StudentAuthenticationResponseModel LoginStudent(string UserName, string Password)
        {
            StudentAuthenticationResponseModel StudentAuthenticationResponseModel = new Models.Students.StudentAuthenticationResponseModel();
            try
            {
                StudentAuthenticationResponseModel.Message = AuthorizationHelper.LoginStudent(UserName, Password);
                if (AuthorizationHelper.CurrentUser != null)
                {
                    StudentAuthenticationResponseModel.Studentid = AuthorizationHelper.CurrentStudentUser.STUDENTID;
                }
                else
                {
                    StudentAuthenticationResponseModel.Studentid = 0;
                }
            }
            catch
            {
                StudentAuthenticationResponseModel.Studentid = 0;
            }
            return StudentAuthenticationResponseModel;

        }

        public static StudentAuthenticationResponseModel GetLoginStudent()
        {
            StudentAuthenticationResponseModel StudentAuthenticationResponseModel = new Models.Students.StudentAuthenticationResponseModel();

            if (AuthorizationHelper.CurrentUser != null)
            {
                StudentAuthenticationResponseModel.Studentid = AuthorizationHelper.CurrentStudentUser.STUDENTID;
            }
            else
            {
                StudentAuthenticationResponseModel.Studentid = 0;
            }
            return StudentAuthenticationResponseModel;
        }
    }
}
