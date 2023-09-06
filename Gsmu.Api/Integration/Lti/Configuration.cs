using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.IO;
using Gsmu.Api.Data;

namespace Gsmu.Api.Integration.Lti
{
    public class Configuration
    {
        private static Configuration configuration = null;

        public static Configuration Instance
        {
            get
            {
                if (configuration == null)
                {
                    var value = Settings.Instance.GetMasterInfo4().LtiConfiguration;
                    configuration = System.Web.Helpers.Json.Decode<Configuration>(value);
                    if (configuration == null) {
                        configuration = new Configuration();
                    }
                }
                return configuration;
            }
        }

        public Configuration()
        {
        }

        public string OAuthServiceKey
        {
            get;
            set;
        }

        public string OAuthServiceSecret
        {
            get;
            set;
        }

        public Uri ServiceUri
        {
            get
            {
                var baseUri = new Uri(Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl);
                var serviceUri = new Uri(baseUri, "sso/lti");
                return serviceUri;
            }
        }


        public void Save()
        {
            var value = Json.Encode(this);
            using (var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
            {
                foreach (var mi4 in db.masterinfo4)
                {
                    mi4.LtiConfiguration = value;
                }
                db.SaveChanges();
            }
            Settings.Instance.Refresh();
        }

        public string LtiConfigurationUrl
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                string url;
                if (context == null)
                {
                    url = "http://unknown-host-running-without-httpcontext";
                }
                else
                {
                    url = context.Request.Url.Scheme + "://" + context.Request.Url.Host;
                }
                url += "/application/lticonfiguration?name={0}";
                return url;
            }
        }

        public string[] LtiConfigurations
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                if (context == null)
                {
                    return new string[] { "application-must-be-running-under-web-server" };
                }
                var dir = context.Server.MapPath("~/App_Data/");

#if DEBUG
                var files = Directory.GetFiles(dir, "Lti.*.Debug.xml");
#else
                var files = Directory.GetFiles(dir, "Lti.*.Release.xml");
#endif
                for (var index = 0; index < files.Length; index++)
                {
                    var file = Path.GetFileName(files[index]).ToLower();
#if DEBUG
                    files[index] = file.Replace("lti.", "").Replace(".debug.xml", "");
#else
                    files[index] = file.Replace("lti.", "").Replace(".release.xml", "");
#endif
                }
                    return files;
            }
        }

    }
}
