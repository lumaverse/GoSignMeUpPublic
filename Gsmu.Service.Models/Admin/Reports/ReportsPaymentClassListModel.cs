using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Reports
{
    public class ReportsPaymentClassListModel
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string Notes { get; set; }

        public int RosterId { get; set; }
        public int CourseId { get; set; }
        public short Waiting { get; set; }
        public short Cancel { get; set; }

        public string OrderNumber { get; set; }
        public string PayMethod { get; set; }
        public string CouponCode { get; set; }
        public float CouponDiscount { get; set; }
        public string CouponDetails { get; set; }
        public decimal CourseCost { get; set; }
        public decimal CourseTotal { get; set; }
        public short PaidInFull { get; set; }
        public decimal TotalPaid { get; set; }
        public float MaterialCost { get; set; }
        public string MaterialCostInfo { get; set; }

        public int CRPartialPaymentMade { get; set; }
        public float CRPartialPaymentPaidAmount { get; set; }
        public string CRPartialPaymentPaidNote { get; set; }
        public string CRPartialPaymentPaidMethod { get; set; }

        public string SchoolLocation { get; set; }
        public string District { get; set; }

        public string ProductName { get; set; }
        public float ProductCost { get; set; }
    }
}
