using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;

namespace Gsmu.Api.Data.ViewModels.Grid
{
    public class GridPagerModel
    {
        public GridPagerModel(int totalCount, QueryState state)
        {
            TotalCount = totalCount;
            var pageSize = state.PageSize;
            var page = state.Page;

            pageSize = pageSize < 0 ? 1 : pageSize;
            PageSize = pageSize;

            page = page < 1 ? 1 : page;
            page = page > TotalPages ? TotalPages : page;
            Page = page;

            OrderByField = state.OrderField;
            OrderByDirection = state.OrderByDirection;
        }

        public OrderByDirection OrderByDirection
        {
            get;
            set;
        }

        public Enum OrderByField
        {
            get;
            set;
        }

        public int TotalCount
        {
            get;
            protected set;
        }

        public int Page
        {
            get;
            protected set;
        }

        public int PageSize
        {
            get;
            protected set;
        }

        public int TotalPages
        {
            get
            {
                double pageCount = (double)TotalCount / (double)PageSize;
                return (int)Math.Ceiling(pageCount);
            }
        }

        public bool IsFirstPage
        {
            get
            {
                return Page == 1;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return Page == TotalPages;
            }
        }

        public bool HasPrevousPage
        {
            get
            {
                return Page > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return Page < TotalPages;
            }
        }

        public string PagerCallbackTemplate
        {
            get;
            set;
        }

        public string GetPagerCallbackFunction(int page, int? pageSize = null)
        {
            pageSize = pageSize ?? PageSize;
            return string.Format(PagerCallbackTemplate, page, pageSize.Value);
        }

        public IQueryable<T> Paginate<T>(IQueryable<T> query)
        {
            return query.Skip((this.Page - 1) * this.PageSize).Take(this.PageSize);
        }
    }
}
