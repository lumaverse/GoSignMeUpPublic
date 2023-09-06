using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;

namespace Gsmu.Api.Data.ViewModels.Grid
{
    public class GridModel<T> : GridPagerModel
    {
        public GridModel(int totalCount, QueryState state)
            :base(totalCount, state)
        {
            Result = new List<T>();
        }

        public List<T> Result
        {
            set;
            get;
        }

        public string SorterCallbackTemplate
        {
            get;
            set;
        }

        public IQueryable<T> Paginate(IQueryable<T> query)
        {
            return base.Paginate<T>(query);
        }
    }
}
