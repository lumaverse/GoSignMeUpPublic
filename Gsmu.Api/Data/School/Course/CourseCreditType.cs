using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CourseCreditType
    {
        Credit,
        InService,
        Custom,
        Ceu,
        Graduate,
        Optional1,
        Optional2,
        Optional3,
        Optional4,
        Optional5,
        Optional6,
        Optional7,
        Optional8
    }
}
