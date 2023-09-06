using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Blackboard.Connector
{
    public class NodeConnector
    {
        public static string NodeListJson
        {
            get
            {
                var client = ServiceInterface.GetClient();
                var config = Configuration.Instance;
                var result = client.DownloadString(config.BlackboardConnectionUrl + "?nodes");
                return result;
            }
        }
    }
}
