using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.API.Models.ResponseModels
{
    /// <summary>
    /// Model class that is used to return dynamically assigned models that 
    /// contains ResponseModel properties.
    /// </summary>
    /// <typeparam name="T">Parameter object of type T</typeparam>
    public class ResponseRecordModel<T> : ResponseModel where T : class
    {
        /// <summary>
        /// Contains the returned property of type T.
        /// </summary>
        public T Data { get; set; }
        public T results { get; set; }
    }
}
