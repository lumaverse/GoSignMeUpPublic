using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data
{
    public class DataStoreResult
    {
        public int Count { get; set; }

        public List<object> Data { get; set; }
    }
}
