using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gsmu.Service.API.Extensions
{
    public static class OwinContextExtentions
    {
        public static string GetAdminByCredential(this IOwinContext ctx)
        {
            var result = "-1";
            var claim = ctx.Authentication.User.Claims.FirstOrDefault(c => c.Type == "AdminID");
            if (claim != null)
            {
                result = claim.Value;
            }
            return result;
        }
    }
}