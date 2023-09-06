using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Commerce
{
    public class AccountingData
    {
        public Course_Roster CourseRoster(int rosterId, SchoolEntities db) 
        {
            return db.Course_Rosters.Where(cr => (cr.RosterID == rosterId) && cr.Cancel != 0).SingleOrDefault();
        }
        public List<Course_Roster> CourseRosterByOrderNumber(string orderNumber, SchoolEntities db) 
        {
            return db.Course_Rosters.Where(cr => (cr.OrderNumber == orderNumber) && cr.Cancel != 0).ToList();
        }
        public List<rostermaterial> RosterMaterials(int rosterId, SchoolEntities db) 
        {
            return db.rostermaterials.Where(rm => rm.RosterID == rosterId && rm.Course_Roster.Cancel != 0).ToList();
        }

        public float Discount(string orderNumber, SchoolEntities db) 
        {
            var roster = CourseRosterByOrderNumber(orderNumber, db);
            var discountTotal = roster.Sum(cr => (
                cr.SingleRosterDiscountAmount.HasValue && cr.SingleRosterDiscountAmount.Value != 0 ? 
                cr.SingleRosterDiscountAmount.Value : 
                cr.CouponDiscount.HasValue ? 
                cr.CouponDiscount.Value : 0)
                );
            return float.Parse(discountTotal.ToString());

        }

        public decimal AmountPaid(string orderNumber, SchoolEntities db) 
        {
            var roster = (from cr in db.Course_Rosters
                          where cr.OrderNumber == orderNumber select cr);

            var credit = (from c in roster select new 
                { 
                    creditAppliedString = c.CreditApplied 
                })
                .ToList()
                .Select(r => new { creditApplied = decimal.Parse(r.creditAppliedString) });

            if(roster.Count() > 0)
            {
               return roster.Sum(cr => cr.TotalPaid.HasValue ? cr.TotalPaid.Value : 0) - (credit.Sum(cr => cr.creditApplied));
            }
            return 0;
        }
        /// <summary>
        /// //Material Price only
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public float MaterialCost(int rosterId, SchoolEntities db)
        {
            var materials = this.RosterMaterials(rosterId, db);
            float materialsCost = materials.Count() > 0 ? materials.Sum(rm => rm.price.HasValue ? rm.price.Value : 0) : 0;
            return materialsCost;
        }
        /// <summary>
        /// Material on asp datastore
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public float Material(int rosterId, SchoolEntities db)
        {
            var materials = this.RosterMaterials(rosterId, db);
            float materialsFee = materials.Count() > 0 ?
                  materials.Sum(rm =>
                  (rm.shipping_cost.HasValue ? rm.shipping_cost.Value : 0)  // shipping
                + ((rm.price.HasValue && rm.priceincluded != 0) ? rm.price.Value : 0) * (rm.taxable != 0 ? (rm.salestax.HasValue ? rm.salestax.Value / 100 : 1) : 1) // + (price * tax)
                  ) : 0;
            return materialsFee;
        }
        /// <summary>
        /// MaterialFee on asp datastore
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public float MaterialFee(int rosterId, SchoolEntities db)
        {

            var materialFeeRaw = (from rm in this.RosterMaterials(rosterId, db)
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
        /// same as CoursePrice
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public decimal CourseCost(int rosterId, SchoolEntities db)
        {
            return CourseRoster(rosterId, db).CourseCostDecimal;
        }
        /// <summary>
        /// same as CourseCost
        /// </summary>
        /// <param name="rosterId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public decimal CoursePrice(int rosterId, SchoolEntities db)
        {
            return CourseRoster(rosterId, db).CourseCostDecimal;
        }
        
        public decimal CourseTotal(int rosterId, SchoolEntities db)
        {
            return this.CourseCost(rosterId, db) + decimal.Parse(this.MaterialFee(rosterId, db).ToString());
        }

        public decimal TxTotal(int rosterId, SchoolEntities db) 
        {
            var roster = this.CourseRoster(rosterId, db);
            var courseCost = this.CourseCost(rosterId, db);
            var total = courseCost - decimal.Parse((roster.SingleRosterDiscountAmount.HasValue && roster.SingleRosterDiscountAmount.Value != 0 ? 
                roster.SingleRosterDiscountAmount.Value :
                roster.CouponDiscount.Value + this.MaterialFee(rosterId, db)
                ).ToString());
            return total;
        }
        public float GrandTotal(string orderNumber, SchoolEntities db) 
        {
            return (this.MaterialFee(0, db) + float.Parse(this.CourseCost(0, db).ToString())) - this.Discount(orderNumber, db);
        }
        public float GrandTotalSum(string orderNumber, SchoolEntities db)
        {
            return this.GrandTotal(orderNumber, db);
        }

        public decimal AmountOwe(int rosterId, SchoolEntities db)
        {
            var roster = this.CourseRoster(rosterId, db);
            var materials = this.RosterMaterials(rosterId, db);
            decimal courseCost = this.CourseCost(rosterId, db);
            float materialsCost = this.MaterialCost(rosterId, db);
            decimal totalPaid = roster.TotalPaid.HasValue ? roster.TotalPaid.Value : 0;
            decimal totalCost = courseCost + decimal.Parse(materialsCost.ToString());
            decimal amountOweRaw = (totalPaid - totalCost);
            decimal amountOwe = (amountOweRaw == 0 || roster.CouponCode != "" || roster.PaidInFull != 0) ? 0 : totalPaid == 0 ? totalCost : amountOweRaw < 0 ? amountOweRaw * -1 : amountOweRaw;
            return Decimal.Parse(amountOwe.ToString("0.00"));
        }
    }
}
