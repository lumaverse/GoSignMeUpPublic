using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.School
{
    public class MaterialModel
    {
        public int ProductId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public float ? Price { get; set; }
        public float ? ShippingCost { get; set; }
        public short PriceIncluded { get; set; }
        public short Taxable { get; set; }
        public float ? ShippingWeight { get; set; }
        public int ? Quantity { get; set; }
        public int ? UseQuantityFromMaterialId { get; set; }
        public int ? NonRefundable { get; set; }
        public short ? HideMaterialPrice { get; set; }
    }
}
