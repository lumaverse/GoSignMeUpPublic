using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public class CourseSearchViewConfiguration
    {
        private ListingType defaultView;
        public CourseSearchViewConfiguration()
        {
            Grid = new GridSearchColumns();
            TileJuly = new TileJulySearchColumns();
            GridViewEnabled = true;
            TileJulyViewEnabled = true;
            DefaultView = ListingType.TileJuly;
        }

        public GridSearchColumns Grid { get; set; }
        public TileJulySearchColumns TileJuly { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ListingType DefaultView
        {
            get
            {
                if (!GridViewEnabled && defaultView == ListingType.Grid)
                {
                    return ListingType.TileJuly;
                }
                if (!TileJulyViewEnabled && defaultView == ListingType.TileJuly)
                {
                    return ListingType.Grid;
                }
                return defaultView;
            }
            set
            {
                this.defaultView = value;
            }
        }

        public bool GridViewEnabled
        {
            get;
            set;
        }

        public bool TileJulyViewEnabled
        {
            get;
            set;
        }
    }
}
