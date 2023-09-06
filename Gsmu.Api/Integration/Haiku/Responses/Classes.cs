using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Gsmu.Api.Integration.Haiku.Responses.Entities;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gsmu.Api.Integration.Haiku.Responses
{
    public class Classes : PaginatedList<Class>
    {
        [XmlElement("class")]
        public List<Class> ClassList
        {
            get;
            set;
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public Class FirstClass
        {
            get
            {
                return ClassList[0];
            }
        }

    }
}
