using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Courses
{
    public class MaterialModel
    {
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
    }
    public class MaterialBasicDetailsModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public bool IsRequired { get; set; }
    }
}
