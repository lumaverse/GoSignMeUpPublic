using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Gsmu.Api.Web
{
    public class NoVirtualPropertySerializationContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (member.DeclaringType.GetProperty(member.Name).GetGetMethod().IsVirtual)
            {
                prop.ShouldSerialize = obj => false;
            }

            return prop;
        }
    }
}
