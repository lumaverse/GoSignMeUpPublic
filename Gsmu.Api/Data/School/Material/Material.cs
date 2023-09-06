using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class Material
    {
        public bool PriceIncludedAsBoolean
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.priceincluded);
            }
        }

        // you can put the shipping price here once it is enabled, as this is the field used everywhere
        public decimal ActualPriceTotal
        {
            get
            {
                if (!PriceIncludedAsBoolean && this.price != null)
                {
                    return (decimal)this.price.Value;
                } else {
                    return 0;
                }
            }
        }
        public int QuantityPurchased
        {
            get;
            set;
        }
    }
}
