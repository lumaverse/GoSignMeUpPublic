using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using http = System.Net.Http;

namespace Gsmu.Api.Integration.Canvas
{
    public class PaginationHelper
    {
        private Dictionary<PaginationType, string> pages = new Dictionary<PaginationType, string>(5);

        public PaginationHelper(http.HttpResponseMessage httpResponse)
        {
            Response = httpResponse;
            if (httpResponse.Headers.Contains("Link"))
            {
                var linkHeader = httpResponse.Headers.GetValues("Link");
                PagingDescriptor = linkHeader.First();
                ExtractLinks();
            }
        }

        public PaginationHelper(string linkDescriptor)
        {
            PagingDescriptor = linkDescriptor;
            ExtractLinks();
        }

        private void ExtractLinks()
        {
            var descriptions = PagingDescriptor.Split(',');
            foreach (var descriptor in descriptions)
            {
                var items = descriptor.Split(';');
                var link = items[0].Substring(1, items[0].Length -2);
                var type = items[1].Replace("rel=", "").Replace("\"", "").Trim();

                switch (type)
                {
                    case "first":
                        Pages.Add(PaginationType.First, link);
                        break;

                    case "last":
                        Pages.Add(PaginationType.Last, link);
                        break;

                    case "next":
                        Pages.Add(PaginationType.Next, link);
                        break;

                    case "prev":
                        Pages.Add(PaginationType.Previous, link);
                        break;

                    case "current":
                        Pages.Add(PaginationType.Current, link);
                        break;
                }
            }
        }

        public http.HttpResponseMessage Response { get; private set; }

        public string PagingDescriptor { get; private set; }

        public Dictionary<PaginationType, string> Pages {
            get
            {
                return this.pages;
            }
        }

        public bool HasNext
        {
            get
            {
                return Pages.ContainsKey(PaginationType.Next);
            }
        }

        public string NextUrl
        {
            get
            {
                return Pages[PaginationType.Next];
            }
        }
    }
}
