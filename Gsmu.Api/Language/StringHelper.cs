using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;

namespace Gsmu.Api.Language

{
    public static class StringHelper
    {
        public static string KeepAlphaNumberUnderscoreAndDash(string value) {
            Regex rgx = new Regex("[^a-zA-Z0-9-_]");
            value = rgx.Replace(value, "");
            return value;
        }

        public static string NameValueCollectionToQueryString(NameValueCollection qs, NameValueCollectionToQueryStringBehavior behavior = NameValueCollectionToQueryStringBehavior.SameKeyDivideValuesWithComma )
        {
            switch (behavior)
            {
                case NameValueCollectionToQueryStringBehavior.SameKeyRepeat:
                    string query = string.Empty;
                    string connector = string.Empty;
                    System.Text.StringBuilder builder = new StringBuilder();
                    foreach (var key in qs.AllKeys)
                    {
                        var values = qs.GetValues(key);
                        foreach (var value in values)
                        {
                            builder.Append(connector);
                            builder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                            connector = "&";
                        }
                    }
                    return builder.ToString();

                case NameValueCollectionToQueryStringBehavior.SameKeyDivideValuesWithComma:
                    return string.Join("&", Array.ConvertAll(qs.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(qs[key]))));

                default:
                    throw new Exception(string.Format("Behavior not implemented: {0}", behavior ));
            }

        }

        public static Dictionary<string, string> ConvertDashAndEqualSignStringToStringDictionary(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            var mapping = value.Split('|');
            var dict = new Dictionary<string, string>(mapping.Length);
            foreach (var map in mapping)
            {
                var items = map.Split('=');
                dict[items[0]] = items[1];
            }
            return dict;
        }
    }
}
