using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gsmu.Service.Interface.Admin.Reports;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.Admin.Reports;
using Gsmu.Api.Data.School.Entities;
using System.Data.SqlClient;
using Gsmu.Service.Models.Courses;

namespace Gsmu.Service.BusinessLogic.Admin.Reports
{
    public class ReportsManager : IReportsManager
    {
        private ISchoolEntities _db;
        private string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);
        private string surveyConnString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.surveyEntitiesKey);

        public ReportsManager() {
            _db = new SchoolEntities(connString);
            
        }
        public List<ReportsPaymentClassListModel> GeneratePaymentClassListReport(int courseId)
        {
            AccountingCalculator calculator = new AccountingCalculator();
            var report = (from s in _db.Students
                          join cr in _db.Course_Rosters on s.STUDENTID equals cr.STUDENTID
                          join sc in _db.Schools on s.SCHOOL equals sc.locationid into ssc
                          from sscAlias in ssc.DefaultIfEmpty()
                          join d in _db.Districts on s.DISTRICT equals d.DISTID into sd
                          from sdAlias in sd.DefaultIfEmpty()
                          where cr.COURSEID == courseId && cr.WAITING == 0 && cr.Cancel == 0
                          select new { s, cr, sscAlias, sdAlias }).ToList();
            return report.Select(r => new ReportsPaymentClassListModel()
            {

                StudentId = r.s.STUDENTID,
                FirstName = r.s.FIRST,
                LastName = r.s.LAST,
                MaterialCost = calculator.MaterialsTotalByRosterId(r.cr.RosterID),
                State = r.s.STATE,
                Notes = r.s.NOTES,
                RosterId = r.cr.RosterID,
                CourseId = r.cr.COURSEID.Value,
                Waiting = r.cr.WAITING,
                Cancel = r.cr.Cancel,
                OrderNumber = r.cr.OrderNumber,
                PayMethod = !string.IsNullOrEmpty(r.cr.PAYMETHOD) ? r.cr.PAYMETHOD : "",
                CouponCode = r.cr.CouponCode,
                CouponDiscount = r.cr.CouponDiscount.HasValue ? r.cr.CouponDiscount.Value : 0,
                CouponDetails = r.cr.CouponDetails,
                CourseTotal = calculator.CourseTotalByRosterId(r.cr.RosterID),
                CourseCost = calculator.CourseCostByRosterId(r.cr.RosterID),
                PaidInFull = r.cr.PaidInFull,
                TotalPaid = r.cr.TotalPaid.HasValue ? r.cr.TotalPaid.Value : 0,
                CRPartialPaymentMade = r.cr.CRPartialPaymentMade.HasValue ? r.cr.CRPartialPaymentMade.Value : 0,
                CRPartialPaymentPaidAmount = r.cr.CRPartialPaymentPaidAmount.HasValue ? r.cr.CRPartialPaymentPaidAmount.Value : 0,
                CRPartialPaymentPaidMethod = r.cr.CRPartialPaymentPaidMethod,
                CRPartialPaymentPaidNote = r.cr.CRPartialPaymentPaidNote
            }).ToList();
            //SP IMPLEMENTATION
            //using (var db = new SchoolEntities(connString))
            //{
            //    return db.Database.SqlQuery<ReportsPaymentClassListModel>
            //            ("SP_ReportsPaymentClassList @courseId", new SqlParameter("@courseId", courseId))
            //            .ToList().Select(r => new ReportsPaymentClassListModel()
            //            {

            //                StudentId = r.StudentId,
            //                FirstName = r.FirstName,
            //                LastName = r.LastName,
            //                State = r.State,
            //                Notes = r.Notes,
            //                RosterId = r.RosterId,
            //                CourseId = r.CourseId,
            //                Waiting = r.Waiting,
            //                Cancel = r.Cancel,
            //                OrderNumber = r.OrderNumber,
            //                PayMethod = r.PayMethod,
            //                CouponCode = r.CouponCode,
            //                CouponDiscount = r.CouponDiscount,
            //                CouponDetails = r.CouponDetails,
            //                PaidInFull = r.PaidInFull,
            //                TotalPaid = r.TotalPaid,
            //                MaterialCost = calculator.MaterialsTotalByRosterId(r.RosterId),
            //                CourseTotal = calculator.CourseTotalByRosterId(r.RosterId),
            //                CourseCost = calculator.CourseCostByRosterId(r.RosterId),
            //                CRPartialPaymentMade = r.CRPartialPaymentMade,
            //                CRPartialPaymentPaidAmount = r.CRPartialPaymentPaidAmount,
            //                CRPartialPaymentPaidMethod = r.CRPartialPaymentPaidMethod,
            //                CRPartialPaymentPaidNote = r.CRPartialPaymentPaidNote
            //            }).ToList();
            //}   
        }

        public OrderDetailModel GetOrderDetailReport(string orderNumber) {
            AccountingCalculator calculator = new AccountingCalculator();
            //can be done by SP but use this for now
            var orderDetail = (from s in _db.Students
                               join cr in _db.Course_Rosters on s.STUDENTID equals cr.STUDENTID
                               join c in _db.Courses on cr.COURSEID equals c.COURSEID
                               join sc in _db.Schools on s.SCHOOL equals sc.locationid into ssc
                               from schoolAlias in ssc.DefaultIfEmpty()
                               join d in _db.Districts on s.DISTRICT equals d.DISTID into sd
                               from districtAlias in sd.DefaultIfEmpty()
                               join g in _db.Grade_Levels on s.GRADE equals g.GRADEID into gl
                               from gradeAlias in gl.DefaultIfEmpty()
                               where cr.OrderNumber == orderNumber && cr.WAITING == 0 && cr.Cancel == 0
                               select new { s, c, cr, schoolAlias, districtAlias, gradeAlias }).AsEnumerable();

                                return orderDetail.Select(r => new OrderDetailModel()
                                {
                                    RosterId = r.cr.RosterID,
                                    Status = calculator.OrderStatus(r.cr.OrderNumber),
                                    CRPartialPaymentList = r.cr.CRPartialPaymentList,
                                    CardNumber = r.cr.CardNumber,
                                    CardName = r.cr.CardName,
                                    PayMethod = r.cr.PAYMETHOD,
                                    ChargeDate = r.cr.ChargeDate.HasValue ? r.cr.ChargeDate.Value : DateTime.MinValue,
                                    AuthNum = r.cr.AuthNum,
                                    ResponseMessage = r.cr.AuthNum,
                                    ReferenceNumber = r.cr.RefNumber,
                                    PayNumber = r.cr.payNumber,
                                    OrderNumber = r.cr.OrderNumber,
                                    DateAdded = r.cr.DATEADDED.HasValue ? r.cr.DATEADDED.Value : DateTime.MinValue,
                                    TimeAdded = r.cr.TIMEADDED.HasValue ? r.cr.TIMEADDED.Value : DateTime.MinValue,
                                    StudentChoiceCourse = r.cr.StudentChoiceCourse.HasValue ? r.cr.StudentChoiceCourse.Value : 0,
                                    CourseDetailsModel = new Models.Courses.CourseBasicDetails() {
                                        CourseId = r.c.COURSEID,
                                        CourseName = r.c.COURSENAME,
                                        CourseNumber = r.c.COURSENUM
                                    },
                                    StudentAddressModel = new Models.Students.StudentAddressContactInfoModel() {
                                        Address = r.s.additionalemail,
                                        City = r.s.CITY,
                                        State = r.s.STATE,
                                        Zip = r.s.ZIP,
                                        Country = r.s.COUNTRY,
                                        HomePhone = r.s.HOMEPHONE,
                                        WorkPhone = r.s.WORKPHONE,
                                        Fax = r.s.FAX,
                                        Email = r.s.EMAIL
                                    },
                                    StudentId = r.s.STUDENTID,
                                    LastName = r.s.LAST,
                                    FistName = r.s.FIRST,
                                    School = r.schoolAlias != null ? r.schoolAlias.LOCATION : "",
                                    GradeLevel = r.gradeAlias != null ? r.gradeAlias.GRADE : "",
                                    District = r.districtAlias != null ? r.districtAlias.DISTRICT1 : "",
                                    ShippingFee = calculator.ShippingFeeByOrderNumber(r.cr.OrderNumber),
                                    Tax = calculator.ShippingFeeByOrderNumber(r.cr.OrderNumber),
                                    CreditCardFee = r.cr.creditcardfee.HasValue ? r.cr.creditcardfee.Value : 0,
                                    GrandTotal = decimal.Parse(calculator.GrandTotal(r.cr.OrderNumber).ToString()),
                                    TotalPaid = calculator.TotalPaid(r.cr.OrderNumber),
                                    AmountDue = calculator.AmountOwe(r.cr.OrderNumber),
                                    RosterMaterialsModel = calculator.RosterMaterialsByOrderNumber(r.cr.OrderNumber).Select(rm => new ReportsRosterMaterialModel() {
                                        ProductId = rm.productID.Value,
                                        ProductName = rm.product_name,
                                        PriceInclude = rm.priceincluded,
                                        ProductCost = rm.price.HasValue ? decimal.Parse(rm.price.Value.ToString()) : 0,
                                        QtyPurchase =  rm.qty_purchased.HasValue ? rm.qty_purchased.Value : 0
                                    }).ToList(),
                                    RosterCoursesModel = calculator.GetCoursesByOrderNumber(r.cr.OrderNumber).Select(rm => new CourseBasicDetails()
                                    {
                                        CourseId = rm.CourseId,
                                        CourseCost = rm.CourseCost,
                                        CourseName = rm.CourseName,
                                        CourseNumber = rm.CourseNumber
                                    }).ToList()

                                }).FirstOrDefault();
        }

        public ReportsPaymentInfoModel GetPaymentDetailReport(int rosterId) {
            AccountingCalculator calculator = new AccountingCalculator();
            return _db.Course_Rosters.Where(cr => cr.RosterID == rosterId)
                .AsEnumerable()
                .Select(r => new ReportsPaymentInfoModel()
                {
                    CourseCost = r.CourseCost,
                    PayNumber = r.payNumber,
                    CreditCardNumber = r.CardNumber,
                    CreditCardExpire = r.CardExp,
                    AuthNumber = r.AuthNum,
                    OrderNumber = r.OrderNumber,
                    OrderTotal = decimal.Parse(calculator.GrandTotal(r.OrderNumber).ToString()),
                    TotalPaid = r.TotalPaid.HasValue ? decimal.Parse(r.TotalPaid.Value.ToString()) : 0,
                    PaidInFull = r.PaidInFull,
                    PayMethod = r.PAYMETHOD
                }).SingleOrDefault();
        }
    }
}
