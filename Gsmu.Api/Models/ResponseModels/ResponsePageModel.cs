using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.API.Models.ResponseModels
{
    public class ResponsePageModel<T> : ResponseListModel<T> where T : class
    {
        public int TotalRecordCount { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
    }
}
