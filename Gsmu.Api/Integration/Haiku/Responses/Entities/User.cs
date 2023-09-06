using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gsmu.Api.Integration.Haiku.Responses.Entities
{
    public class User
    {
        [XmlAttribute("display_id")]
        public string DisplayId {
            get;
            set;
        }

        [XmlAttribute("nickname")]
        public string Nickname
        {
            get;
            set;
        }

        [XmlAttribute("import_id")]
        public string ImportId
        {
            get;
            set;
        }


        [XmlAttribute("google_email")]
        public string GoogleEmail
        {
            get;
            set;
        }

        [XmlAttribute("id")]
        public int Id
        {
            get;
            set;
        }

        [XmlAttribute("google_enabled")]
        public bool GoogleEnabled
        {
            get;
            set;
        }

        [XmlAttribute("enabled")]
        public bool Enabled
        {
            get;
            set;
        }

        [XmlAttribute("user_type"), JsonIgnore]
        public string UserType
        {
            get;
            set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserType UserTypeAsEnum
        {
            get
            {
                return User.GetUserType(UserType);
            }
        }

        [XmlAttribute("organization_id")]
        public int OrganizationId
        {
            get;
            set;
        }

        [XmlAttribute("first_name")]
        public string FirstName
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

        [XmlAttribute("login")]
        public string Login
        {
            get;
            set;
        }

        [XmlAttribute("suffix")]
        public string Suffix
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

        [XmlAttribute("unconfirmed_email ")]
        public string UnconfirmedEmail
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

        internal static Entities.UserType GetUserType(string type)
        {
            switch (type)
            {
                case "T":
                    return Gsmu.Api.Integration.Haiku.Responses.Entities.UserType.Teacher;

                case "P":
                    return Gsmu.Api.Integration.Haiku.Responses.Entities.UserType.Parent;

                case "S":
                    return Gsmu.Api.Integration.Haiku.Responses.Entities.UserType.Student;

                default:
                    return Gsmu.Api.Integration.Haiku.Responses.Entities.UserType.Unknown;

            }
        }
    }
}
