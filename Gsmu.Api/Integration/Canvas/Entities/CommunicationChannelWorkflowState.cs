using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    // I am not using capitals here because the state is serialized exactly the enum is typed in letter.
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CommunicationChannelWorkflowState
    {
        unconfirmed,
        active
    }
}
