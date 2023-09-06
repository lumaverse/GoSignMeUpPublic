using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Canvas
{
    public class Response
    {
        private List<RawResponse> rawResponses = new List<RawResponse>();

        public Response(RawResponse response)
        {
            this.RawResponse = response;
            this.rawResponses.Add(response);
            this.Exceptions = new List<Exception>();
        }

        public RawResponse RawResponse { get; set; }

        public List<RawResponse> RawResponseList
        {
            get
            {
                return this.rawResponses;
            }
        }

        private T[] GetResponseObjects<T>()
        {
            var list = new List<T>();
            foreach(var rawResponse in rawResponses) {
                var items = rawResponse.JsonResult.ToObject<T[]>();
                list.AddRange(items);
            }
            return list.ToArray();
        }


        public Entities.Authentication Authentication {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.Authentication>();
            }
        }

        public Entities.AuthenticationProvider[] AuthenticationProviders
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.AuthenticationProvider[]>();
                }
                return this.GetResponseObjects<Entities.AuthenticationProvider>();
            }
        }

        public Entities.AuthenticationProvider AuthenticationProvider
        {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.AuthenticationProvider>();
            }
        }

        public Entities.Account[] Accounts
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.Account[]>();
                }
                return this.GetResponseObjects<Entities.Account>();
            }
        }

        public Entities.Account Account
        {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.Account>();
            }
        }

        public Entities.User[] Users
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.User[]>();
                }
                return this.GetResponseObjects<Entities.User>();
            }
        }

        public Entities.User User
        {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.User>();
            }
        }

        public Entities.UserLoginDetails[] CanvasLoginDetails
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.UserLoginDetails[]>();
                }
                return this.GetResponseObjects<Entities.UserLoginDetails>();
            }
        }

        public Entities.UserLoginDetails[] CanvasLoginDetail
        {
            get
            {
                return this.GetResponseObjects<Entities.UserLoginDetails>();
            }
        }

        public Entities.CourseSection[] Sections
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.CourseSection[]>();
                }
                return this.GetResponseObjects<Entities.CourseSection>();
            }
        }

        public Entities.CourseSection Section
        {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.CourseSection>();
            }
        }
        public Entities.CommunicationChannel[] CommunicationChannels
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.CommunicationChannel[]>();
                }
                return this.GetResponseObjects<Entities.CommunicationChannel>();
            }
        }

        public Entities.Enrollment[] Enrollments
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.Enrollment[]>();
                }
                return this.GetResponseObjects<Entities.Enrollment>();
            }
        }

        public Entities.Enrollment Enrollment
        {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.Enrollment>();
            }
        }

        public Entities.Course Course
        {
            get
            {
                return this.RawResponse.JsonResult.ToObject<Entities.Course>();
            }
        }

        public Entities.Course[] Courses
        {
            get
            {
                if (rawResponses.Count < 2)
                {
                    return this.RawResponse.JsonResult.ToObject<Entities.Course[]>();
                }
                return this.GetResponseObjects<Entities.Course>();
            }
        }
        public int CourseNoOfpages
        {
            get;
            set;
        }


        public Entities.Error Error
        {
            get
            {
                var result = this.RawResponse.JsonResult;
                try
                {
                    return result.ToObject<Entities.Error>();
                }
                catch (Newtonsoft.Json.JsonSerializationException)
                {
                    if (result.Type == Newtonsoft.Json.Linq.JTokenType.Object && result["errors"] != null)
                    {
                        return new Entities.Error()
                        {
                            ErrorDetails = new Entities.ErrorDetails[] { 
                                new Entities.ErrorDetails()
                                {
                                    Message = this.RawResponse.JsonResult["errors"].ToString()
                                }
                            }
                        };
                    }
                    return null;
                }
            }
        }

        public List<Exception> Exceptions
        {
            get;
            private set;
        }

    }
}
