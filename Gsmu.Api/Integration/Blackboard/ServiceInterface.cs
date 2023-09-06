using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using Gsmu.Api.Data;
using System.Web;
using System.Xml;
using System.Configuration;
using BlackBoardAPI;
using static BlackBoardAPI.BlackBoardAPIModel;
namespace Gsmu.Api.Integration.Blackboard
{
    public class ServiceInterface
    {
        public static BlackboardResponse QueryBlackboard(ConnectorEnum connector, string requestService, string requestTargetClass, NameValueCollection serviceProperties = null, NameValueCollection serviceSetProperties = null)
        {


                var query = new NameValueCollection();
                string connectorName = null;
                switch (connector)
                {
                    case ConnectorEnum.ConnectorInfo:
                        connectorName = ConnectorMap.ConnectorInfo;
                        break;

                    case ConnectorEnum.CourseConnector:
                        connectorName = ConnectorMap.CourseConnector;
                        break;

                    case ConnectorEnum.CourseMembershipConnector:
                        connectorName = ConnectorMap.CourseMembershipConnector;
                        break;

                    case ConnectorEnum.DataSourceKeyConnector:
                        connectorName = ConnectorMap.DataSourceKeyConnector;
                        break;

                    case ConnectorEnum.GradeBookConnector:
                        connectorName = ConnectorMap.GradeBookConnector;
                        break;

                    case ConnectorEnum.RoleConnector:
                        connectorName = ConnectorMap.RoleConnector;
                        break;

                    case ConnectorEnum.UserConnector:
                        connectorName = ConnectorMap.UserConnector;
                        break;

                    default:
                        throw new NotImplementedException(
                            string.Format("Connector is not implemented: {0}. Please update code if missing.", Enum.GetName(typeof(ConnectorEnum), connector))
                        );
                }

                query["request.connector"] = connectorName;
                query["request.service"] = requestService;

                if (serviceProperties != null)
                {
                    foreach (var key in serviceProperties.AllKeys)
                    {
                        query["request.service." + key] = serviceProperties[key];
                    }
                }
                if (requestTargetClass != "")
                {
                    query["request.targetClass"] = requestTargetClass;
                }

                if (serviceSetProperties != null)
                {
                    foreach (var key in serviceSetProperties.AllKeys)
                    {
                        query["request." + requestTargetClass + "." + key] = serviceSetProperties[key];
                    }
                }

                var result = ExecuteQuery(query);
                return result;
            
            
        }
        public static BlackboardResponse ExecuteQuery(NameValueCollection query)
        {
            var config = Configuration.Instance;
            query["request.server"] = config.BlackboardConnectionUrl;
            var xmlString = "";
            int ExceptionDS = 0;
            var client = GetClient();
            string BlackboardRequiredTokenToProcess = ConfigurationManager.AppSettings["BlackboardRequiredTokenToProcess"];
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            if (!string.IsNullOrEmpty(BlackboardRequiredTokenToProcess))
            {
                if (bool.Parse(BlackboardRequiredTokenToProcess) == true)
                {
                    var post = "";
                    var at = "";
                    var comma = "";
                    var order = "";
                    foreach (string queryItem in query.AllKeys)
                    {
                        post = post + at + queryItem + "=" + query[queryItem];
                        order = order + comma + queryItem;
                        at = "&";
                        comma = ",";
                    }
                    post = post + at + "order=" + order;
                    string BBTokenKey = Settings.Instance.GetMasterInfo4().blackboard_security_key;
                    client.Headers["OAuthSignature"] = Gsmu.Api.Encryption.HmacSha1.Encode(post, BBTokenKey);

                    Uri download = new Uri(config.BlackboardConnectionUrl + "?" + post);
                    //                (config.BlackboardConnectionUrl, post);
                    xmlString = client.UploadString(download, post);
                    ExceptionDS = post.IndexOf("DataSourceKeyConnector");
                    string signature = client.ResponseHeaders["OAuthSignature"];
                    string xmlSignature = Gsmu.Api.Encryption.HmacSha1.Encode(xmlString, Settings.Instance.GetMasterInfo4().blackboard_security_key);

                    if (signature != xmlSignature && ExceptionDS == 0)
                    {
                        throw new ApplicationException("Invalid BB Token Key. Please contact Administrator");
                    }
                }
                else
                {
                    var data = client.UploadValues(config.BlackboardConnectionUrl, query);
                    xmlString = Encoding.UTF8.GetString(data);
                }
            }
            else
            {
                var data = client.UploadValues(config.BlackboardConnectionUrl, query);
                xmlString = Encoding.UTF8.GetString(data);
            }

            System.Xml.XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(xmlString);
            var result = new NameValueCollection();

            for (var index = 0; index < doc.DocumentElement.ChildNodes.Count; index++)
            {
                var node = doc.DocumentElement.ChildNodes[index];
                var value = node.InnerText;
                var key = node.Attributes["key"].Value;
                result[key] = value;
            }

            return new BlackboardResponse(result);
        }
        public static WebClient GetClient()
        {
            var client = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            return client;
        }

    }
}
