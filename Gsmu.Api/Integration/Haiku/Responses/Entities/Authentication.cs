using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gsmu.Api.Integration.Haiku.Responses.Entities
{
    [Serializable]
    public class Authentication
    {
        [XmlAttribute("response")]
        public string Response
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

        [XmlAttribute("user_id")]
        public string UserIdString
        {
            get;
            set;
        }

        [XmlIgnore]
        public int? UserId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UserIdString))
                {
                    return null;
                }
                return int.Parse(UserIdString);
            }
             
        }

        [XmlAttribute("login")]
        public string Login
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool Success
        {
            get
            {
                return Response == "success";
            }
        }
    }
}
