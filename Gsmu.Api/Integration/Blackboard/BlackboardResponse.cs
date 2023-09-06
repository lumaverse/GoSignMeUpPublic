using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Gsmu.Api.Integration.Blackboard
{
    public class BlackboardResponse
    {
        private NameValueCollection result = null;

        public BlackboardResponse(NameValueCollection response)
        {
            result = response;
        }

        public NameValueCollection Raw
        {
            get
            {
                return result;
            }
        }

        public string this[string property] 
        {
            get
            {
                return result["response." + property];
            }
        }

        public string this[string responseObject, string property] {
            get
            {
                return result["response." + responseObject + "." + property];
            }
        }

        /// <summary>
        /// Returns the target class
        /// </summary>
        public string TargetClass {
            get
            {
                return this["targetClass"];
            }
        }

        /// <summary>
        /// Returns the number of the target class.
        /// </summary>
        public int TargetClassCount
        {
            get
            {
                return int.Parse(this[TargetClass, "count"]);
            }
        }

        /// <summary>
        /// This returns an indexed target class property.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public string this[int index, string property]
        {
            get
            {
                return this[TargetClass, index.ToString() + "." + property];
            }
        }
        public bool IsSuccess
        {
            get
            {
                return result["response.result"] == "success";
            }
        }
    }
}
