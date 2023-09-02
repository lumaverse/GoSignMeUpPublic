
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gsmu.Service.Models.Admin.Reports;

namespace Gsmu.Service.Interface.Admin.Reports
{
    public interface IReportsManager
    {
        List<ReportsPaymentClassListModel> GeneratePaymentClassListReport(int courseId);
        OrderDetailModel GetOrderDetailReport(string orderNumber);
        ReportsPaymentInfoModel GetPaymentDetailReport(int rosterId);
    }
}
