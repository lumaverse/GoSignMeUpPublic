using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gsmu.Api.Integration.Haiku.Responses.Entities
{
    public class Roster
    {
        [XmlAttribute("class_id")]
        public int ClassId
        {
            get;
            set;
        }

        [XmlAttribute("roster_id")]
        public int RosterId
        {
            get;
            set;
        }

        [XmlAttribute("role"), JsonIgnore]
        public string RoleString
        {
            get;
            set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserType Role
        {
            get
            {
                return User.GetUserType(RoleString);
            }
        }

        [XmlAttribute("first_name")]
        public string FirstName
        {
            get;
            set;
        }

        [XmlAttribute("middle_name")]
        public string MiddleName
        {
            get;
            set;
        }

        [XmlAttribute("last_name")]
        public string LastName
        {
            get;
            set;
        }

        [XmlAttribute("email")]
        public string Email
        {
            get;
            set;
        }

        [XmlAttribute("invitation_code")]
        public string InvitationCode
        {
            get;
            set;
        }

        [XmlAttribute("user_id")]
        public int UserId
        {
            get;
            set;
        }

    }
}
