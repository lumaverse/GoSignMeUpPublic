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
    
    public partial class Membership
    {
        public int MembershipID { get; set; }
        public string MembershipTitle { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateExpired { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<int> StartFromDatePurchased { get; set; }
        public Nullable<int> Disabled { get; set; }
        public string Description { get; set; }
    }
}
