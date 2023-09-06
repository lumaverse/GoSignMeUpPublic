using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data
{
    /// <summary>
    /// Holds paging and ordering information
    /// </summary>
    public class QueryState
    {
        public QueryState() {
            OrderByDirection = Data.OrderByDirection.Ascending;
            OrderField = Gsmu.Api.Data.ViewModels.Grid.CourseOrderByField.CourseName;
        }

        public QueryState(int start, int limit) : this() {
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

        public Enum OrderField
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
