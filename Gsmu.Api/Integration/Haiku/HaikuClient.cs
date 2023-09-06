using System;
using System.Collections.Generic;
using specials = System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;
using net = System.Net;
using xml = System.Xml;
using lang = Gsmu.Api.Language;
using school = Gsmu.Api.Data.School.Entities;
using student = Gsmu.Api.Data.School.Student;
using log = Gsmu.Api.Logging;
using web = System.Web;
using io = System.IO;

namespace Gsmu.Api.Integration.Haiku
{
    public static class HaikuClient
    {
        public static List<string> ThrottleTest()
        {
            var result = new List<string>();

            var counter = 25;
            while (counter-- > 0)
            {
                var response = GetResponse();
                result.Add("-----------------------------------");
                result.Add(counter.ToString());
                foreach (var header in response.HttpResponse.Headers)
                {
                    result.Add(header.Key + ": " + String.Join("; ", header.Value));
                }
                result.Add("-----------------------------------");
            }
            return result;
        }


        public static Response Authenticate(string username, string password)
        {
            string relativeUri = string.Format(
                "authentication/{0}?password={1}", username, password
            );

            var response = GetResponse(
                RelativeUrl: relativeUri,
                HttpMethod: http.HttpMethod.Post
            );
            return response;
        }

        public static Response GetResponse(string RelativeUrl = "test/ping/gosignmeup", http.HttpMethod HttpMethod = null, bool disregard404Error = false)
        {
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            }
            else
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
            }
            try
            {
                if (HttpMethod == null)
                {
                    HttpMethod = http.HttpMethod.Get;
                }

                var config = Configuration.Instance;
                var uri = new Uri(new Uri(new Uri(config.HaikuUrl), "do/services/"), RelativeUrl);

                DotNetOpenAuth.OAuth.OAuth1HmacSha1HttpMessageHandler handler = new DotNetOpenAuth.OAuth.OAuth1HmacSha1HttpMessageHandler(new http.WebRequestHandler());
                handler.AccessToken = config.OAuthRequestToken;
                handler.AccessTokenSecret = config.OAuthRequestSecret;
                handler.ConsumerKey = config.OAuthServiceKey;
                handler.ConsumerSecret = config.OAuthServiceSecret;
                handler.Location = DotNetOpenAuth.OAuth.OAuth1HttpMessageHandlerBase.OAuthParametersLocation.AuthorizationHttpHeader;

                http.HttpRequestMessage request = new http.HttpRequestMessage();
                string lastRequestUrl = null;
                using (var client = new http.HttpClient())
                {
                    http.HttpResponseMessage response = null;
                    string sendResult = null;
                    Func<bool> go = delegate()
                    {
                        request = new http.HttpRequestMessage();
                        request.Method = HttpMethod;
                        request.RequestUri = uri;
                        handler.ApplyAuthorization(request);

                        lastRequestUrl = uri.ToString();
                        response = client.SendAsync(request).Result;
                        sendResult = response.Content.ReadAsStringAsync().Result;
                        var header = (from h in response.Headers where h.Key == "Retry-After" select h).FirstOrDefault();
                        if (header.Key == "Retry-After")
                        {
                            var goodTime = DateTime.Parse(String.Join(string.Empty, header.Value));
                            var wait = (DateTime.Now - goodTime).Milliseconds;
                            if (wait < 1 || wait > 1000)
                            {
                                wait = 1000;
                            }
                            System.Threading.Thread.Sleep(wait);
                            return false;
                        }
                        return true;
                    };

                    var maxRetries = 10;
                    var maxRetriesCounter = maxRetries;
                    do
                    {
                        --maxRetriesCounter;
                        if (maxRetriesCounter == 0)
                        {
                            throw new ApplicationException(
                                string.Format("The request could not be completed {0} times. At {1}", maxRetries, request.RequestUri)
                            );
                        }
                    } while (!go());


                    xml.Serialization.XmlSerializer serializer;
                    serializer = new xml.Serialization.XmlSerializer(typeof(Response));
                    io.StringReader strReader = new System.IO.StringReader(sendResult);
                    Response result = (Response)serializer.Deserialize(strReader);

                    result.HttpClient = client;
                    result.HttpResponse = response;
                    result.ResponseString = sendResult;

                    if (disregard404Error && result.Error != null && (result.Error.Code == 404 || result.Error.Code == 420))
                    {
                        result.Error = null;
                    }

                    if (result.ContainsErrors)
                    {
                        if (result.Error != null)
                        {
                            throw new Exception(
                                string.Format("Haikue error! Code: {1}, Description: {2}, Details: {3}, Request: {0}", lastRequestUrl, result.Error.Code, result.Error.Description, result.Error.Details)
                            );
                        }
                        if (result.Exceptions.Count > 0)
                        {
                            throw result.Exceptions[0];
                        }
                    }

                    return result;
                }

            }
            catch (Exception e)
            {
                log.LogManager.LogException(typeof(HaikuClient).Name, "Exception during Haiku communication.", e);
                Exception inner = e.GetBaseException();
                throw new Exception(
                    string.Format("Haiku communication failure. ({0})", System.Web.HttpUtility.HtmlEncode(inner.Message)),
                    e
                );
            }
        }

        public static Response Ping()
        {
            Response response = new Response();
            response = GetResponse("test/ping/gosignmeup");
            return response;
        }

    }
}
