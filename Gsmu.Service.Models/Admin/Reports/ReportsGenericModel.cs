using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Reports
{
    /// <summary>
    /// Generic Report Models will be keeping some smaller chunks of models
    /// that can be inherited by bigger models for Report Usage
    /// </summary>
    public class ReportsGenericModel
    {
        
    }
    
    public class ReportStudentModel
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string State { get; set; }
        public string Notes { get; set; }
    }
    //purposely separated these model chunks so these can be inherited by bigger models
    //originally Roster should contain the payment models but we can separate them
    //for specific purpose (we can just merge them as needed)
    public class ReportRosterModel
    {
        public int RosterId { get; set; }
        public int CourseId { get; set; }
        public int Waiting { get; set; }
        public int Cancel { get; set; }
    }

    public class ReportsPaymentInfoModel
    {
        public string OrderNumber { get; set; }
        public string PayNumber { get; set; }
        public string PayMethod { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardExpire { get; set; }
        public string AuthNumber { get; set; }
        public string CouponCode { get; set; }
        public float CouponDiscount { get; set; }
        public string CouponDetails { get; set; }
        public string CourseCost { get; set; }
        public int PaidInFull { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal TotalPaid { get; set; }
        
        public string CRPartialPaymentMade { get; set; }
        public string CRPartialPaymentPaidAmount { get; set; }
        public string CRPartialPaymentPaidNote { get; set; }
        public string CRPartialPaymentPaidMethod { get; set; }
    }

    public class ReportsLocationModel
    {
        public string SchoolLocation { get; set; }
        public string District { get; set; }
    }

    public class ReportsRosterMaterialModel {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public short PriceInclude { get; set; }
        public decimal ProductCost { get; set; }
        public int QtyPurchase { get; set; }
    }
}
