using Gsmu.Service.Models.School;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Security.Authentication
{
    public interface IAuthenticationManager
    {
        AdminModel GetAdminByCredential(string userName, string password);
        StudentAuthenticationResponseModel LoginStudent(string UserName, string Password);
        StudentAuthenticationResponseModel GetLoginStudent();
    }
}
