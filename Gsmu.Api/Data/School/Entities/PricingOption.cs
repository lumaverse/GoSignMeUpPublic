//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.School.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PricingOption
    {
        public int PricingOptionID { get; set; }
        public Nullable<int> PriceTypeNumber { get; set; }
        public string PriceTypedesc { get; set; }
        public decimal Price { get; set; }
        public decimal NonPrice { get; set; }
        public int credithour { get; set; }
        public int inservicehour { get; set; }
        public decimal SpecialMemberPrice1 { get; set; }
        public Nullable<int> CollectionStyle { get; set; }
        public Nullable<int> rangestart { get; set; }
        public Nullable<int> rangeend { get; set; }
        public Nullable<int> SubscriptionLength { get; set; }
        public Nullable<int> SubscriptionReOccCharge { get; set; }
    }
}
