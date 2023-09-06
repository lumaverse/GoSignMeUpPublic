using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Haiku
{
    public class HaikuResponseErrorException : Exception
    {
        public HaikuResponseErrorException(Response r) : base(BuildMessage(r))
        {
            this.Response = r;
        }

        private static string BuildMessage(Response r)
        {
            return string.Format(
                "Code: {0}, Description: {1}, Details: {2}",
                r.Error.Code,
                r.Error.Description,
                r.Error.Details
            );
        }

        public Response Response
        {
            get;
            set;
        }

        public Responses.Entities.Error Error
        {
            get
            {
                return this.Response.Error;
            }
        }
    }
}
