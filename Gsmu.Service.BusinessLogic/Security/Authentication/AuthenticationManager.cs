using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.BusinessLogic.Students;
using Gsmu.Service.Interface.Security.Authentication;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.School;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Gsmu.Service.BusinessLogic.Security.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private ISchoolEntities _db;
        public AuthenticationManager()
        {
            string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);
            _db = new SchoolEntities(connString);
        }
        public AdminModel GetAdminByCredential(string userName, string password) {
           return _db.adminpasses.Where(a => a.username == userName && a.userpass == password).Select(a => new AdminModel() {
               AdminId = a.AdminID,
               UserName = a.username,
               Password = a.userpass,
               Email = a.email,
               DateAdded = a.dateadded,
               Disabled = a.disabled,
               PortalSettings = a.PortalSettings,
               WidgetSettings = a.WidgetSettings
           }).SingleOrDefault();
        }

        public StudentAuthenticationResponseModel LoginStudent(string UserName, string Password)
        {
            return StudentAuthenticationManager.LoginStudent(UserName,Password);

        }
        public StudentAuthenticationResponseModel GetLoginStudent()
        {
            return StudentAuthenticationManager.GetLoginStudent();

        }


    }
}
