using Gsmu.Service.API.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gsmu.Service.API
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/auth/token"),
                Provider = new OAuthServiceProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
                AllowInsecureHttp = true,
                AuthenticationType = "Bearer"
            };
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}