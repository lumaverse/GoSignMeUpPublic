using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Gsmu.Api.Integration.Haiku.Responses.Entities;

namespace Gsmu.Api.Integration.Haiku.Responses
{
    public class Rosters : PaginatedList<Roster>
    {
        [XmlElement("roster")]
        public List<Entities.Roster> RosterList
        {
            get;
            set;
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public Entities.Roster FirstRoster
        {
            get
            {
                return RosterList[0];
            }
        }


    }
}
