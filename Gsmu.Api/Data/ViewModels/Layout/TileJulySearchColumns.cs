using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public class TileJulySearchColumns : SearchColumns
    {
        public TileJulySearchColumns() : base()
        {
            AdjustLayoutComponents = true;
            TileWidth = 230;
            TileImageWidth = 230;
            TileImageHeight = 133;

            CourseDescription = true;
            Sessions = true;
            Duration = false;
            Status = true;
            ClassSize = true;
            Image = true;

        }

        public bool AdjustLayoutComponents { get; set; }
        public int TileWidth { get; set; }
        public int TileImageWidth { get; set; }
        public int TileImageHeight { get; set; }
        public bool CourseDescription { get; set; }
        public bool Sessions { get; set; }
        public bool Duration { get; set; }
        public bool Status { get; set; }
        public bool ClassSize { get; set; }
        public bool Image { get; set; }

    }
}
