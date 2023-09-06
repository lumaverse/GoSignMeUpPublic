using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;

namespace Gsmu.Api.Language
{
    public class ReflectionHelper
    {
        public static List<ClassFieldDescriptor> GetFieldsByAttribue(Type typeOfClass, Type typeOfAttribute)
        {
            var fields = typeOfClass.GetProperties();
            var result = (from f in fields.Where(pi => pi.GetCustomAttributes(typeOfAttribute, false).Length > 0)
                          select new ClassFieldDescriptor()
                          {
                              Name = f.Name,
                              Type = f.PropertyType.Name
                          }).ToList<ClassFieldDescriptor>();

            return result;
        }

        public static void SetPropertyValue(object obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName).SetValue(obj, value, null);
        }

        public static object GetPropertyValue(object obj, string propName)
        {
            return obj.GetType().GetProperty(propName).GetValue(obj, null);
        }

        public static bool ObjectStringDataIsEmpty(object o)
        {
            if (o == null)
            {
                return true;
            }
            var objectDataIsEmpty = true;
            (
                from p
                in o.GetType().GetProperties()
                where p.PropertyType.Name == "String"
                select p.Name
            ).ToList().ForEach(delegate(string name)
            {
                var value = ReflectionHelper.GetPropertyValue(o, name) as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    objectDataIsEmpty = false;
                }
            });
            return objectDataIsEmpty;
        }
    }
}
