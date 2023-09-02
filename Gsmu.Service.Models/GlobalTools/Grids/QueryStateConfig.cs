using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.GlobalTools.Grids
{
    public class QueryStateConfig
    {
          public QueryStateConfig() {
            OrderByDirection = OrderByDirection.Ascending;
            OrderField = CourseOrderByFieldSet.CourseName;
        }

          public QueryStateConfig(int start, int limit)
              : this()
          {
            PageSize = limit;
            Page = (int)Math.Floor((double)start / limit)+1;
        }

        public OrderByDirection OrderByDirection
        {
            get;
            set;
        }

        public int Page
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public System.Enum OrderField
        {
            get;
            set;
        }

        public string OrderFieldString
        {
            get;
            set;
        }

        public Dictionary<string, string> Filters
        {
            get;
            set;
        }

        public string Query { get; set; }


        public Dictionary<string, string> FldHeaders { get; set; }


        public List<KeyValuePair<string, OrderByDirection>> Sorters { get; set; }
    }
}
