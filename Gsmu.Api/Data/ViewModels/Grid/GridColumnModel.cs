using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Grid
{
    public class GridColumnModel
    {
        public string ColumnName
        {
            get;
            set;
        }

        public Enum SortField
        {
            get;
            set;
        }

        public string Style { get; set; }
    }
}
