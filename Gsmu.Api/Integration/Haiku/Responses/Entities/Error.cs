using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gsmu.Api.Integration.Haiku.Responses.Entities
{
    public class Error
    {
        [XmlAttribute("code")]
        public int Code
        {
            get;
            set;
        }

        [XmlAttribute("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlAttribute("details")]
        public string Details
        {
            get;
            set;
        }

    }
}
