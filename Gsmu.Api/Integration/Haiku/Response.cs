using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Gsmu.Api.Integration.Haiku.Responses.Entities;

namespace Gsmu.Api.Integration.Haiku
{
    /// <summary>
    /// If something cannot be processed by xml or json, just add any of the following properties:
    /// XmlIgnore, ScriptIgnore, JsonIgnore
    /// </summary>
    [Serializable, XmlRoot("response")]
    public class Response
    {
        public  Response()
        {
            Exceptions = new List<Exception>();
            ClassesByClassId = new Dictionary<int, Class>();
            RostersByClassId = new Dictionary<int, List<Roster>>();
            UsersByUserId = new Dictionary<int, User>();
            Users = new Responses.Users();
            Rosters = new Responses.Rosters();
            Classes = new Responses.Classes();
            ResponseStart = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public DateTime ResponseStart { 
            get; 
            set; 
        }

        [XmlIgnore]
        public Guid Id
        {
            get;
            set;
        }

        [XmlIgnore]
        public string ResponseString
        {
            get;
            set;
        }


        [XmlAttribute("status")]
        public string Status
        {
            get;
            set;
        }

        [XmlElement("error")]
        public Error Error
        {
            get;
            set;
        }

        [XmlElement("authentication")]
        public Authentication Authentication
        {
            get;
            set;
        }

        [XmlElement("pong")]
        public Pong Pong
        {
            get;
            set;
        }

        [XmlElement("users")]
        public Responses.Users Users
        {
            get;
            set;
        }

        [XmlElement("rosters")]
        public Responses.Rosters Rosters
        {
            get;
            set;
        }

        [XmlElement("classes")]
        public Responses.Classes Classes
        {
            get;
            set;
        }

        [XmlIgnore]
        public HttpResponseMessage HttpResponse
        {
            get;
            set;
        }

        [XmlIgnore]
        public HttpClient HttpClient
        {
            get;
            set;
        }

        [XmlIgnore]
        public List<Exception> Exceptions
        {
            get;
            set;
        }

        [XmlIgnore]
        public Dictionary<int, Class> ClassesByClassId
        {
            get;
            set;
        }

        [XmlIgnore]
        public Dictionary<int, List<Roster>> RostersByClassId
        {
            get;
            set;
        }

        [XmlIgnore]
        public Dictionary<int, User> UsersByUserId
        {
            get;
            set;
        }

        public bool ContainsErrors
        {
            get
            {
                return Exceptions.Count > 0 || this.Error != null;
            }
        }

        public void JoinErrors(Response response)
        {
            if (response.Error != null)
            {
                this.Exceptions.Add(new Exception(
                    string.Format("Code: {0}, Description: {1}, Details: {2}", response.Error.Code, response.Error.Details, response.Error.Description)
                ));
            }
            this.Exceptions.AddRange(response.Exceptions);
        }
    }
}
