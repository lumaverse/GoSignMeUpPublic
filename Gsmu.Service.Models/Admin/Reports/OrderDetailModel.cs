using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Reports
{
    public class OrderDetailModel
    {
        //ROSTER INFO
        //public DateTime MinCourseDate { get; set; }
        //public DateTime MaxCourseDate { get; set; }
        public string CRPartialPaymentList { get; set; }
        public int StudentChoiceCourse { get; set; }
        public string CourseChoice { get; set; }
        public string CreditApplied { get; set; }
        public int RosterId { get; set; }
        public string Status { get; set; }

        public DateTime? FullDateAdded { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime TimeAdded { get; set; }
        public string AccuntNum { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public short? WaitingStatus { get; set; }

        public string PayMethod { get; set; }
        public DateTime ChargeDate { get; set; }
        public string AuthNum { get; set; }
        public string ResponseMessage { get; set; }
        public string ReferenceNumber { get; set; }
        public string PayNumber { get; set; }

        public int? Cancel { get; set; }
        public int? PaidInFull { get; set; }

        public string MasterOrderNumber { get; set; }
        public string OrderNumber { get; set; }
        public string OrderId { get; set; }
        //ACCOUNTING INFO
        public decimal CreditCardFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountedSubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Tax { get; set; }
        public string OrderPrice { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal TotalRefund { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal AmountDue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal GrandTotal { get; set; }
        //STUDENT INFO
        public int? StudentId { get; set; }
        public string LastName { get; set; }
        public string FistName { get; set; }
        public int? SchoolId { get; set; }
        public int? GradeId { get; set; }
        public int? DistrictId { get; set; }
        public string School { get; set; }
        public string GradeLevel { get; set; }
        public string District { get; set; }

        public StudentAddressContactInfoModel StudentAddressModel { get; set; }
        public CourseBasicDetails CourseDetailsModel { get; set; }
        public List<ReportsRosterMaterialModel> RosterMaterialsModel { get; set; }
        public List<CourseBasicDetails> RosterCoursesModel { get; set; }
    }
}
