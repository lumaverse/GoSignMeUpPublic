using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Data
{
    public class ExtJsDataStoreHelper
    {
        public static Dictionary<string, string> ParseFilter(string filters)
        {
            if (filters == null)
            {
                return null;
            }
            var converter = new JavaScriptSerializer();
            var parseResult = converter.Deserialize<dynamic[]>(filters);
            var filterResult = new Dictionary<string, string>();
            foreach (var filter in parseResult)
            {
                try
                {
                    if ((filter["property"] == "paytype") || ((filter["property"] == "school")) || ((filter["property"] == "district")) || ((filter["property"] == "grade")))
                    {
                        string val = "";
                        foreach (var a in filter["value"])
                        {
                            if (val != "")
                                val = val + "," + a;
                            else
                                val = a;
                                
                        }
                        filterResult.Add(
                                 filter["property"],
                                 Convert.ToString(val)
                        );
                    }
                    else
                    {
                        filterResult.Add(
                            filter["property"],
                            Convert.ToString(filter["value"])
                        );
                    }
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {

                }
            }
            if (filterResult.Keys.Count == 0)
            {
                return null;
            }
            return filterResult;
        }

        public static Dictionary<string, string> ParseColumns(string filters)
        {
            if (filters == null)
            {
                return null;
            }
            var converter = new JavaScriptSerializer();
            var parseResult = converter.Deserialize<dynamic[]>(filters);
            var filterResult = new Dictionary<string, string>();
            foreach (var filter in parseResult)
            {
                try
                {
                    filterResult.Add(
                        filter["property"],
                        Convert.ToString(filter["value"])
                    );
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {

                }
            }
            if (filterResult.Keys.Count == 0)
            {
                return null;
            }
            return filterResult;
        }

        public static List<KeyValuePair<string, OrderByDirection>> ParseSorter(string sorters)
        {
            if (sorters == null)
            {
                return null;
            }
            var converter = new JavaScriptSerializer();
            var filterResult = new List<KeyValuePair<string, OrderByDirection>>();
            var parseResult = converter.Deserialize<dynamic[]>(sorters);
            foreach (var sorter in parseResult)
            {
                var field = sorter["property"];
                var direction = sorter["direction"] == "DESC" ? OrderByDirection.Descending : OrderByDirection.Ascending;
                filterResult.Add(new KeyValuePair<string, OrderByDirection>(field, direction));
            }
            return filterResult;
        }

        public static KeyValuePair<string, OrderByDirection> ParseSorterUnique(string sort)
        {
            var result = ParseSorter(sort);
            if (result == null)
            {
                return new KeyValuePair<string, OrderByDirection>(null, OrderByDirection.Ascending);
            }
            return result[0];
        }
    }
}
