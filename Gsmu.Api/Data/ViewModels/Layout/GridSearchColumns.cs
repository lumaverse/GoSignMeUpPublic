using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public class GridSearchColumns : SearchColumns
    {

        public GridSearchColumns()
            : base()
        {
            Material = false;
            PreRequisite = false;
        }

        public bool Material { get; set; }
        public bool PreRequisite { get; set; }
    
    }
}
