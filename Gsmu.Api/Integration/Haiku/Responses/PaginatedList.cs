using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gsmu.Api.Integration.Haiku.Responses
{
    public class PaginatedList<T>
    {
        public PaginatedList() {
            AllRecords = new List<T>();
        }

        [XmlAttribute("page")]
        public int Page
        {
            get;
            set;
        }

        [XmlAttribute("records_per_page")]
        public int RecordsPerPage
        {
            get;
            set;
        }

        [XmlAttribute("total_record_count")]
        public int TotalRecordCount
        {
            get;
            set;
        }

        [XmlAttribute("page_count")]
        public int PageCount
        {
            get;
            set;
        }

        /// <summary>
        /// This variabe must be filled in manually and also it usually contains the results of a paginated list.
        /// Pay attention and check the other uses of it to make use of it.
        /// </summary>
        public List<T> AllRecords
        {
            get;
            set;
        }
    }
}
