using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Language
{
    public static class ConvertHelper
    {
        public static T ChangeTypeForNullable<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t); ;
            }

            return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeTypeForNullable(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t); ;
            }

            return Convert.ChangeType(value, t);
        }

        public static bool ToBoolean(string value)
        {
            if (value != null)
            {
                value = value.ToLowerInvariant();
            }
            switch (value) { 
                case "true":
                case "1":
                case "-1":
                    return true;

                default:
                    return false;
            }
        }
    }
}
