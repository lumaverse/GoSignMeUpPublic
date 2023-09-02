using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.API.Models.ResponseModels
{
    public class ResponseListModel<T>: ResponseModel where T : class
    {
        public List<T> List { get; set; }
    }
}
