using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.Interface.Admin;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.Admin
{
    public class AdminManager : IAdminManager
    {
        private ISchoolEntities _db;
        private string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);
        public AdminManager()
        {
            _db = new SchoolEntities(connString);
        }
        public AdminCredsModel GetAdminAuthBySessionId(string sessionId) {
            var adminCreds = _db.adminpasses.Where(s => s.UserSessionId == Guid.Parse(sessionId)).SingleOrDefault();
            return new AdminCredsModel()
            {
                UserName = adminCreds.username,
                Password = adminCreds.userpass
            };
        }
    }
}
