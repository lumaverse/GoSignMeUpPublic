using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gsmu.Api.Integration.Haiku.Responses.Entities
{
    public class Pong
    {
        [XmlAttribute("id")]
        public string Id
        {
            get;
            set;
        }
    }
}
