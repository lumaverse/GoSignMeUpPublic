using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.School
{
    public class PricingOptionsModel
    {
        public int PricingOptionId { get; set; }
        public int ? PriceTypeNumber { get; set; }
        public string PriceTypeDescription { get; set; }
        public decimal Price { get; set;}
        //Currently not needed now. Please uncomment if needed
        public decimal NonPrice { get; set; }
        public int CreditHours { get; set; }
        public int InserviceHours { get; set; }
        public decimal SpecialMemberPrice { get; set; }
        public int CollectionStyle { get; set; }
        public int RangeStart { get; set; }
        public int RangeEnd { get; set; }
        public int SubscriptionLength { get; set; }
    }
}
