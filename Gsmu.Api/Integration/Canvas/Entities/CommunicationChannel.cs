using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    /*
    {
        "id": 222274,
        "position": 1,
        "user_id": 173067,
        "workflow_state": "unconfirmed",
        "address": "patrik@gosignmeup.com",
        "type": "email"
    }     
     */
    public class CommunicationChannel
    {

        [json.JsonProperty("id")]
        public Int64 Id { get; set; }

        [json.JsonProperty("position")]
        public int? Position { get; set; }

        [json.JsonProperty("user_id")]
        public Int64 UserId { get; set; }

        /// <summary>
        /// confirmed, unconfirmed
        /// </summary>
        [json.JsonProperty("workflow_state")]
        [json.JsonConverter(typeof(json.Converters.StringEnumConverter))]
        public CommunicationChannelWorkflowState WorkflowState { get; set; }

        [json.JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Can be email or sms
        /// </summary>
        [json.JsonProperty("type")]
        [json.JsonConverter(typeof(json.Converters.StringEnumConverter))]
        public CommunicationChannelType Type { get; set; }

        [json.JsonProperty("skip_confirmation")]
        public bool SkipConfirmation { get; set; }
    }
}
