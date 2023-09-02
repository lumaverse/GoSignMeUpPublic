using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.API.Models.ResponseModels
{

    /// <summary>
    /// Model class the is used to return standard properties.
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// The success integer code.
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// The Error integer code.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// The response string returned.
        /// </summary>
        public string Message { get; set; }

        public object Extras { get; set; }
    }
}
