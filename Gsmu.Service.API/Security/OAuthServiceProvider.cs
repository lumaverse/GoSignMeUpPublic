using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Gsmu.Service.Models.School;
using Gsmu.Service.BusinessLogic.Security.Authentication;

namespace Gsmu.Service.API.Security
{
    public class OAuthServiceProvider : OAuthAuthorizationServerProvider
    {
        Gsmu.Service.Interface.Security.Authentication.IAuthenticationManager _authenticationManager;
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {

                var username = context.UserName;
                var password = context.Password;

                _authenticationManager = new AuthenticationManager();
                AdminModel admin = _authenticationManager.GetAdminByCredential(username, password);
                if (admin != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim("AdminID", admin.AdminId.ToString()),
                        new Claim(ClaimTypes.Role, "admin"),
                        new Claim("UserName", admin.UserName),
                        new Claim(ClaimTypes.NameIdentifier, admin.UserName),
                        new Claim(ClaimTypes.Email, admin.Email)
                    };

                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { }));
                }
                else
                {
                    context.SetError("invalid_grant", "Error");
                }

            });
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }
    }
}