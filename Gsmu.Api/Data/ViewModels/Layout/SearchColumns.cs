using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public class SearchColumns
    {
        public SearchColumns()
        {
            CourseId = false;
            CourseNumber = true;
            CourseStart = true;
            CourseEnd = false;
            Credit = false;
            Price = false;
            Icons = true;
            Location = true;
        }

        public bool CourseId { get; set; }
        public bool CourseNumber { get; set; }
        public bool CourseStart { get; set; }
        public bool CourseEnd { get; set; }
        public bool Credit { get; set; }
        public bool Price { get; set; }
        public bool Icons { get; set; }
        public bool Location { get; set; }

    }
}
