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
using System.Globalization;
using Gsmu.Service.Models.Courses;

namespace Gsmu.Service.BusinessLogic.GlobalTools
{
    public class AccountingCalculator
    {
        private ISchoolEntities _db;
        private string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);
        private string surveyConnString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.surveyEntitiesKey);
        public AccountingCalculator() {
            _db = new SchoolEntities(connString);
        }

        public Course_Roster CourseRoster(int rosterId)
        {
            return _db.Course_Rosters.Where(cr => (cr.RosterID == rosterId) && cr.Cancel == 0).SingleOrDefault();
        }
        public List<Course_Roster> CourseRosterByOrderNumber(string orderNumber)
        {
            return _db.Course_Rosters.Where(cr => (cr.OrderNumber == orderNumber) && cr.Cancel == 0).ToList();
        }
        public List<rostermaterial> RosterMaterials(int rosterId)
        {
            return _db.rostermaterials.Where(rm => rm.RosterID == rosterId && rm.Course_Roster.Cancel == 0).ToList();
        }
        public List<rostermaterial> RosterMaterialsByOrderNumber(string orderNumber) {
            var rosterMaterials = (from cr in _db.Course_Rosters
                                   join rm in _db.rostermaterials on cr.RosterID equals rm.RosterID
                                   where cr.OrderNumber == orderNumber && cr.Cancel == 0
                                   select rm)
                                   .Distinct()
                                   .ToList();
            return rosterMaterials;
        }
        public List<CourseBasicDetails> GetCoursesByOrderNumber(string orderNumber) {
            return (from cr in _db.Course_Rosters
                           join c in _db.Courses on cr.COURSEID equals c.COURSEID
                           where cr.OrderNumber == orderNumber &&
                           cr.Cancel == 0 && c.CANCELCOURSE == 0
                           select new CourseBasicDetails()
                           {
                               CourseId = c.COURSEID,
                               CourseNumber = c.COURSENUM,
                               CourseName = c.COURSENAME,
                               CourseCost = cr.CourseCost
                           })
                           .Distinct()
                           .ToList();
        }

        /// <summary>
        /// Discount By OrderNumber - Calculates the total Discount per order number
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public float DiscountByOrderNumber(string orderNumber)
        {
            var roster = CourseRosterByOrderNumber(orderNumber);
            var discountTotal = roster.Sum(cr => (
                cr.SingleRosterDiscountAmount.HasValue && cr.SingleRosterDiscountAmount.Value != 0 ?
                cr.SingleRosterDiscountAmount.Value :
                cr.CouponDiscount.HasValue ?
                cr.CouponDiscount.Value : 0)
                );
            return float.Parse(discountTotal.ToString());
        }
        /// <summary>
        /// Amount Paid By Roster Id - Calculates the Total Amount paid, by getting the Total Paid information
        /// by rosterid minus the credit
        /// </summary>
        /// <param name="rosterId"></param>
        /// <returns></returns>
        public decimal AmountPaidByRosterId(int rosterId) {
            var roster = (from cr in _db.Course_Rosters
                          where cr.RosterID == rosterId
                          select cr);

            var credit = (from c in roster
                          select new
                          {
                              creditAppliedString = c.CreditApplied
                          })
                .ToList()
                .Select(r => new { creditApplied = decimal.Parse(r.creditAppliedString) });

            if (roster.Count() > 0)
            {
                return roster.Sum(cr => cr.TotalPaid.HasValue ? cr.TotalPaid.Value : 0) - (credit.Sum(cr => cr.creditApplied));
            }
            return 0;
        }
        /// <summary>
        /// Amount Paid By Order Number (1 Order Number can have multiple Rosters) - Calculates the Total Amount paid, by getting the Total Paid information
        /// by rosterid minus the credit
        /// </summary>
        /// <param name="rosterId"></param>
        /// <returns></returns>
        public decimal AmountPaidByOrderNumber(string orderNumber)
        {
            var roster = (from cr in _db.Course_Rosters
                          where cr.OrderNumber == orderNumber
                          select cr);

            var credit = (from c in roster
                          select new
                          {
                              creditAppliedString = c.CreditApplied
                          })
                .ToList()
                .Select(r => new { creditApplied = decimal.Parse(r.creditAppliedString) });

            if (roster.Count() > 0)
            {
                return roster.Sum(cr => cr.TotalPaid.HasValue ? cr.TotalPaid.Value : 0) - (credit.Sum(cr => cr.creditApplied));
            }
            return 0;
        }
        
        /// <summary>
        /// Material Price only
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public float MaterialCostByRosterId(int rosterId)
        {
            var materials = this.RosterMaterials(rosterId);
            float materialsCost = materials.Count() > 0 ? materials.Sum(rm => rm.price.HasValue ? rm.price.Value : 0) : 0;
            return materialsCost;
        }
        /// <summary>
        /// Material on asp datastore - Shipping + Price * Tax
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public float MaterialsTotalByRosterId(int rosterId)
        {
            var materials = this.RosterMaterials(rosterId);
            float materialsFee = materials.Count() > 0 ?
                  materials.Sum(rm =>
                  (rm.shipping_cost.HasValue ? rm.shipping_cost.Value : 0)  // shipping
                + ((rm.price.HasValue) ? rm.price.Value : 0) //&& rm.priceincluded != 0 //temporarily removing this one
                * (rm.taxable != 0 ? (rm.salestax.HasValue ? rm.salestax.Value / 100 : 1) : 1) // + (price * tax)
                  ) : 0;
            return materialsFee;
        }
        /// <summary>
        /// MaterialFee on asp datastore
        /// </summary>
        /// <param name="rosterId"></param>
        /// <returns></returns>
        public float MaterialFeeByRosterId(int rosterId)
        {

            var materialFeeRaw = (from rm in this.RosterMaterials(rosterId)
                                  where rm.Course_Roster.Cancel != 0 && rm.Material.non_refundable != 0 && rm.priceincluded != 0
                                  select rm);

            float materialsFee = materialFeeRaw.Count() > 0 ?
                  materialFeeRaw.Sum(rm =>
                  (rm.shipping_cost.HasValue ? rm.shipping_cost.Value : 0) // + shipping
                + ((rm.price.HasValue ? rm.price.Value : 0) * (rm.taxable != 0 ? (rm.salestax.HasValue ? rm.salestax.Value / 100 : 1) : 1) // + (price * tax)
                  )) : 0;
            return materialsFee;
        }
        /// <summary>
        /// MaterialFee on asp datastore by Order Number
        /// Checks price include property - if price included is 0, the material price is zeroed out
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public float MaterialFeeByOrderNumber(string orderNumber) {
            var materialFeeRaw = (from rm in this.RosterMaterialsByOrderNumber(orderNumber)
                                  where rm.Course_Roster.Cancel != 0 && rm.Material.non_refundable != 0 && rm.priceincluded != 0
                                  select rm);

            float materialsFee = materialFeeRaw.Count() > 0 ?
                  materialFeeRaw.Sum(rm =>
                  (rm.shipping_cost.HasValue ? rm.shipping_cost.Value : 0) // + shipping
                + ((rm.price.HasValue ? rm.price.Value : 0) * (rm.taxable != 0 ? (rm.salestax.HasValue ? rm.salestax.Value / 100 : 1) : 1) // + (price * tax)
                  )) : 0;
            return materialsFee;
        }

        /// <summary>
        /// Course Cost - Cost of the Course Purchased
        /// </summary>
        /// <param name="rosterId"></param>
        /// <returns></returns>
        public decimal CourseCostByRosterId(int rosterId)
        {
            NumberFormatInfo MyNFI = new NumberFormatInfo();
            MyNFI.NegativeSign = "-";
            MyNFI.CurrencyDecimalSeparator = ".";
            MyNFI.CurrencyGroupSeparator = ",";
            MyNFI.CurrencySymbol = "$";
            var courseCost = CourseRoster(rosterId).CourseCost;
            return decimal.Parse(courseCost, NumberStyles.Currency, MyNFI);
        }
        /// <summary>
        /// Course Cost - Cost of the Course Purchased by orderNumber
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public decimal CourseCostByOrderNumber(string orderNumber)
        {
            NumberFormatInfo MyNFI = new NumberFormatInfo();
            MyNFI.NegativeSign = "-";
            MyNFI.CurrencyDecimalSeparator = ".";
            MyNFI.CurrencyGroupSeparator = ",";
            MyNFI.CurrencySymbol = "$";
            var courseCost = _db.Course_Rosters.Where(cr => cr.OrderNumber == orderNumber).FirstOrDefault().CourseCost;
            return decimal.Parse(courseCost, NumberStyles.Currency, MyNFI);
        }
        /// <summary>
        /// Course Cost By Course Id and MembershipType
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public decimal CoursePriceByMemType(int courseId, int membershipType = 0)
        {
            decimal coursePrice = 0;
            var course = _db.Courses.Where(c => c.COURSEID == courseId).SingleOrDefault();
            coursePrice = membershipType == 1 ? course.DISTPRICE.Value : membershipType == 2 ? course.SpecialDistPrice1.Value : course.NODISTPRICE.Value;
            return coursePrice;
        }
        /// <summary>
        /// Course Cost Total By Roster Id - Gets the Course Cost + the Total Materials Fee information
        /// </summary>
        /// <param name="rosterId"></param>
        /// <returns></returns>
        public decimal CourseTotalByRosterId(int rosterId)
        {
            return this.CourseCostByRosterId(rosterId) + decimal.Parse(this.MaterialsTotalByRosterId(rosterId).ToString());
        }

        public decimal TxTotal(int rosterId)
        {
            var roster = this.CourseRoster(rosterId);
            var courseCost = this.CourseCostByRosterId(rosterId);
            var total = courseCost - decimal.Parse((roster.SingleRosterDiscountAmount.HasValue && roster.SingleRosterDiscountAmount.Value != 0 ?
                roster.SingleRosterDiscountAmount.Value :
                roster.CouponDiscount.Value + this.MaterialFeeByRosterId(rosterId)
                ).ToString());
            return total;
        }
        /// <summary>
        /// Total Material Fee + Total Course Cost - Discount
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public float GrandTotal(string orderNumber)
        {
            return 
                ((this.MaterialFeeByOrderNumber(orderNumber) + float.Parse(this.CourseCostByOrderNumber(orderNumber).ToString()))) 
                - this.DiscountByOrderNumber(orderNumber);
        }
        /// <summary>
        /// Returns the Payment Status (Approved, Pending, Cancelled, Partial, Failed)
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public string OrderStatus(string orderNumber) {
            var rosters = this.CourseRosterByOrderNumber(orderNumber).ToList();
            if (rosters.Count() > 0) {
                if (rosters.Any(r => r.PaidInFull != 0)) {
                    return "Approved";
                }
                else if (rosters.Any(r => r.PaidInFull == 0) && rosters.Any(r => r.Cancel == 0)) {
                    return "Pending";
                }
                else if (rosters.Any(r => r.Cancel == 2))
                {
                    return "Incomplete Payment";
                }
                else if (rosters.Any(r => r.Cancel == 3))
                {
                    return "Failed Payment";
                }
                else
                {
                    return "Cancelled";
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Grand Total - Total Paid
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public decimal AmountOwe(string orderNumber)
        {
            return (decimal.Parse(this.GrandTotal(orderNumber).ToString()) - this.TotalPaid(orderNumber));
        }
        /// <summary>
        /// Total Shipping Fee of the Materials
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public decimal ShippingFeeByOrderNumber(string orderNumber) {
            var totalShippingCost = this.RosterMaterialsByOrderNumber(orderNumber).Sum(rm => rm.shipping_cost);
            return totalShippingCost.HasValue ? decimal.Parse(totalShippingCost.Value.ToString()) : 0;
        }
        /// <summary>
        /// Total Tax Fee of the Materials
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public decimal TaxByOrderNumber(string orderNumber)
        {
            var totalTax = this.RosterMaterialsByOrderNumber(orderNumber).Sum(rm => rm.salestax);
            return totalTax.HasValue ? decimal.Parse(totalTax.Value.ToString()) : 0;
        }
        /// <summary>
        /// Total Paid amount for this Order
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public decimal TotalPaid(string orderNumber) {
            var totalPaid = _db.Course_Rosters.Where(cr => cr.OrderNumber == orderNumber).Sum(cr => cr.TotalPaid);
            return totalPaid.HasValue ? totalPaid.Value : 0;
        }
    }
}
