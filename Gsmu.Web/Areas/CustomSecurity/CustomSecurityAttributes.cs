using Gsmu.Api.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Public
{

    [AttributeUsage(AttributeTargets.Method)]
    public class CustomSecurityforCrossSiteForgeryAttributes:AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string referrer = httpContext.Request.RawUrl;
            if ((referrer.ToString().ToLower().Contains("secure.authorize.net")) || referrer.ToString().ToLower().Contains("test.authorize.net") || referrer.ToString().ToLower().Contains("/public/cart/authorizeredirectsilentpost") || referrer.ToString().ToLower().Contains("/public/cart/authorizeredirectsilentpost") || referrer.ToString().ToLower().Contains("nelnettest.asp"))
            {
                return true;
            }
            if (httpContext.Request.Url.AbsoluteUri.ToLower().Contains("/public/calendar") && httpContext.Request.UrlReferrer == null)
            {
                return true;
            }
            if (httpContext.Request.Url.AbsoluteUri.ToLower().Contains("/public/sharer") && httpContext.Request.UrlReferrer == null)
            {
                return true;
            }           
            if (httpContext.Request.UrlReferrer != null)
            {
                if (httpContext.Request.UrlReferrer.Authority.ToLower() == httpContext.Request.Url.Authority.ToLower())
                {
                    return true;
                }
            }


            string whitelistedsites = System.Configuration.ConfigurationManager.AppSettings["whitelistedsites"];
            if (!string.IsNullOrEmpty(whitelistedsites))
           {
               if (whitelistedsites.Contains(','))
               {
                   foreach (var site in whitelistedsites.Split(','))
                   {
                       if (httpContext.Request.UrlReferrer.ToString().ToLower().Contains(site))
                       {
                           return true;
                       }
                   }
               }
               else
               {
                   try
                   {
                       if (httpContext.Request.UrlReferrer.ToString().ToLower().Contains(whitelistedsites))
                       {
                           return true;
                       }
                   }
                   catch
                   {
                       return true;
                   }

               }
           }

            return false;
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PaymentCustomSecurityforCrossSiteForgeryAttributes : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string referrer = httpContext.Request.RawUrl + httpContext.Request.UrlReferrer;
            if ((referrer.ToString().ToLower().Contains("secure.touchnet.com") || referrer.ToString().ToLower().Contains("secure.authorize.net") || referrer.ToString().ToLower().Contains("gosignmeup.com")) || referrer.ToString().ToLower().Contains("test.authorize.net") || referrer.ToString().ToLower().Contains("/public/cart/authorizeredirectsilentpost") || referrer.ToString().ToLower().Contains("/public/cart/authorizeredirectsilentpost"))
            {
                return true;
            }
            return false;
        }
    }

    public static class StripHtmlTags
    {
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        public static string Strip(string source){
            if (source != null)
                return _htmlRegex.Replace(source, string.Empty);
            else
                return string.Empty;

        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class UserLoginSecuredAttributes : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (AuthorizationHelper.CurrentUser.IsLoggedIn)
            {
                return true;
            }
            return false;
        }
    }
    }