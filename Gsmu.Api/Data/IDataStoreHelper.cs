using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data
{
    public interface IDataStoreHelper
    {
        DataStoreResult List(int page, int pageSize, string sorters, string filters);
        void Create(Dictionary<string, string> data);
        void Update(Dictionary<string, string> data);
        void Delete(Dictionary<string, string> data);
    }
}
