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
    
    public partial class rostermaterial
    {
        public int RosterMaterialsID { get; set; }
        public Nullable<int> RosterID { get; set; }
        public Nullable<int> productID { get; set; }
        public string product_name { get; set; }
        public Nullable<float> price { get; set; }
        public Nullable<float> shipping_cost { get; set; }
        public short priceincluded { get; set; }
        public short taxable { get; set; }
        public Nullable<float> salestax { get; set; }
        public Nullable<int> qty_purchased { get; set; }
    
        public virtual Course_Roster Course_Roster { get; set; }
        public virtual Material Material { get; set; }
    }
}
