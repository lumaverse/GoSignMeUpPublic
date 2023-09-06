using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Web;
using System.Xml;

namespace Gsmu.Api.Update.Modules
{
    internal class ConnectionStringsConfigFileUpdater : AbstractUpdater
    {
        internal override bool Execute()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            var cs = config.ConnectionStrings;
            if (string.IsNullOrWhiteSpace(cs.SectionInformation.ConfigSource))
            {
                var xml = cs.SectionInformation.GetRawXml();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                var path = HttpContext.Current.Server.MapPath("~/App_Data/ConnectionStrings.config");

                FileStream stream = new FileStream(path, FileMode.Create);
                var writer = XmlWriter.Create(stream, new XmlWriterSettings() {
                     ConformanceLevel = ConformanceLevel.Document,
                     Indent = true,
                     OmitXmlDeclaration = false,
                     WriteEndDocumentOnClose = true
                });
                xmlDoc.Save(writer);
                writer.Dispose();
                stream.Dispose();

                cs.SectionInformation.ConfigSource = "App_Data\\ConnectionStrings.config";
                config.Save();
            }
            return true;
        }
    }
}
