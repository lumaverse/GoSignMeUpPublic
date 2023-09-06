using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Lti.OAuth
{
    public class TokenManager : DotNetOpenAuth.OAuth.ChannelElements.IServiceProviderTokenManager 
    {
        public DotNetOpenAuth.OAuth.ChannelElements.IServiceProviderAccessToken GetAccessToken(string token)
        {
            throw new NotImplementedException();
        }

        public DotNetOpenAuth.OAuth.ChannelElements.IConsumerDescription GetConsumer(string consumerKey)
        {
            var description = new ConsumerDescription()
            {
                Key = Configuration.Instance.OAuthServiceKey,
                Secret = Configuration.Instance.OAuthServiceSecret,
                Callback = Configuration.Instance.ServiceUri,
                VerificationCodeFormat = DotNetOpenAuth.OAuth.VerificationCodeFormat.AlphaNumericNoLookAlikes                
            };
            return description;
        }

        public DotNetOpenAuth.OAuth.ChannelElements.IServiceProviderRequestToken GetRequestToken(string token)
        {
            throw new NotImplementedException();
        }

        public bool IsRequestTokenAuthorized(string requestToken)
        {
            throw new NotImplementedException();
        }

        public void UpdateToken(DotNetOpenAuth.OAuth.ChannelElements.IServiceProviderRequestToken token)
        {
            throw new NotImplementedException();
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            throw new NotImplementedException();
        }

        public string GetTokenSecret(string token)
        {
            return Configuration.Instance.OAuthServiceSecret;
        }

        public DotNetOpenAuth.OAuth.ChannelElements.TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        public void StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest request, DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage response)
        {
            throw new NotImplementedException();
        }
    }
}
