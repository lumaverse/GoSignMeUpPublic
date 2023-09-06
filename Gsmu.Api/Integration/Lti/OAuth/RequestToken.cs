using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Lti.OAuth
{
    public class RequestToken : DotNetOpenAuth.OAuth.ChannelElements.IServiceProviderRequestToken
    {
        public Uri Callback
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string ConsumerKey
        {
            get { throw new NotImplementedException(); }
        }

        public Version ConsumerVersion
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime CreatedOn
        {
            get { throw new NotImplementedException(); }
        }

        public string Token
        {
            get { throw new NotImplementedException(); }
        }

        public string VerificationCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
