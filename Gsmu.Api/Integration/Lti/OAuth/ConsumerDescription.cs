using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Lti.OAuth
{
    public class ConsumerDescription : DotNetOpenAuth.OAuth.ChannelElements.IConsumerDescription
    {
        public Uri Callback
        {
            get;
            set;
        }

        public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public string Secret
        {
            get;
            set;
        }

        public DotNetOpenAuth.OAuth.VerificationCodeFormat VerificationCodeFormat
        {
            get;
            set;
        }

        public int VerificationCodeLength
        {
            get;
            set;
        }
    }
}
