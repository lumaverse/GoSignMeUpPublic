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
    
    public partial class Material
    {
        public Material()
        {
            this.rostermaterials = new HashSet<rostermaterial>();
        }
    
        public int productID { get; set; }
        public string product_num { get; set; }
        public string product_name { get; set; }
        public Nullable<float> price { get; set; }
        public Nullable<float> shipping_cost { get; set; }
        public short priceincluded { get; set; }
        public short taxable { get; set; }
        public Nullable<float> shipping_weight { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<int> use_qty_from_materialid { get; set; }
        public Nullable<int> non_refundable { get; set; }
        public Nullable<short> hidematerialprice { get; set; }
    
        public virtual ICollection<rostermaterial> rostermaterials { get; set; }
    }
}