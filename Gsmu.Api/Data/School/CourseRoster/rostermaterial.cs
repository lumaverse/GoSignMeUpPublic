using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class rostermaterial
    {
        public decimal EffectivePrice
        {
            get
            {
                if (Settings.GetVbScriptBoolValue(this.priceincluded))
                {
                    return 0;
                }
                return this.price.HasValue ? (decimal)this.price.Value : 0;
            }
        }
        public short hidematerialprice
        {
            get
            {
                using (var db = new SchoolEntities())
                {
                    var data = db.Materials.Where(m => m.productID == this.productID).SingleOrDefault();
                    if (data != null) 
                    {
                        if (data.hidematerialprice == null)
                        {
                            return 0;
                        }
                        else return (short)data.hidematerialprice;
                    }
                }
                return 0;
            }
        }
    }
}
