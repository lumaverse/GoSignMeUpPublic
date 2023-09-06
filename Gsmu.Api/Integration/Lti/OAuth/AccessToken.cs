using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Lti.OAuth
{
    public class AccessToken : DotNetOpenAuth.OAuth.ChannelElements.IServiceProviderAccessToken
    {
        public DateTime? ExpirationDate
        {
            get { throw new NotImplementedException(); }
        }

        public string[] Roles
        {
            get { throw new NotImplementedException(); }
        }

        public string Token
        {
            get { throw new NotImplementedException(); }
        }

        public string Username
        {
            get { throw new NotImplementedException(); }
        }
    }
}
