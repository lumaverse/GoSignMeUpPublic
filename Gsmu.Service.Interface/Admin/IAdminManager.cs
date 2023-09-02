using Gsmu.Service.Models.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Admin
{
    public interface IAdminManager
    {
        AdminCredsModel GetAdminAuthBySessionId(string sessionId);
    }
}
