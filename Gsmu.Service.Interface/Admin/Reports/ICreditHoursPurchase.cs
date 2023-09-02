using Gsmu.Api.Data;
using Gsmu.Service.Models.Admin.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Admin.Reports
{
    public interface ICreditHoursPurchase
    {
        CreditHoursPurchaseResultModel GetStudentCourseHoursPurchased(CreditHoursPurchaseParamenterModel CreditHoursPurchaseParamenterModel, QueryState queryState);
    }
}
