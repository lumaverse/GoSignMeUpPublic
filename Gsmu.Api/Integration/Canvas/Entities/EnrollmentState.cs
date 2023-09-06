﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum EnrollmentState
    {
        active,
        invited,
        creation_pending,
        deleted,
        rejected,
        completed,
        inactive
    }
}