using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class PricingOption
    {

        public bool CollectExtraParticipants
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.CollectionStyle);
            }
        }

    }
}
