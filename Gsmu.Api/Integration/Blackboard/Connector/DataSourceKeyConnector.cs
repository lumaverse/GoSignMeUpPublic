using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Gsmu.Api.Integration.Blackboard.Connector
{
    public class DataSourceKeyConnector
    {
        public static BlackboardResponse List()
        {
            var result = ServiceInterface.QueryBlackboard(ConnectorEnum.DataSourceKeyConnector, "list","");
            return result;
        }

        public static string[] DataSourceKeys
        {
            get
            {
                var result = List();
                var keys = new List<string>();

                for (var index = 0; index < result.TargetClassCount; index++)
                {
                    var key = result[index, "BatchUid"];
                    keys.Add(key);
                }

                return keys.ToArray();
            }
        }

    }
}
