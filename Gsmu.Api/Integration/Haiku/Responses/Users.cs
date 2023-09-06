using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Gsmu.Api.Integration.Haiku;
using Gsmu.Api.Integration.Haiku.Responses.Entities;

namespace Gsmu.Api.Integration.Haiku.Responses
{
    public class Users : PaginatedList<User>
    {
        [XmlElement("user")]
        public List<User> UserList
        {
            get;
            set;
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public User FirstUser
        {
            get
            {
                return UserList[0];
            }
        }

    }
}
