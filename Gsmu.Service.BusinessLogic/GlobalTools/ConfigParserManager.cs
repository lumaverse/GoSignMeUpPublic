using Gsmu.Service.Models.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gsmu.Service.BusinessLogic.GlobalTools
{
    public class ConfigParserManager
    {
        /// <summary>
        /// Returns the config from another project by providing the source of the web config file
        /// </summary>
        /// <param name="name">Target KeyName = SchoolEntities</param>
        /// <param name="path">Path Target of the config file</param>
        /// <returns></returns>
        public static string ConnectionStringReader(string path, string name)
        {
            if (File.Exists(path))
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = path;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                string connString = config.ConnectionStrings.ConnectionStrings[name].ToString();
                return connString;
            }
            return string.Empty;
        }
        /// <summary>
        /// Returns the path of the web config using the constant config key values
        /// </summary>
        /// <returns></returns>
        public static string ConfigSourcePath(HttpContext context = null)
        {
            string webConfigFileSource = ConfigSettingConstant.webConfigFileSourceDev;
            return new DirectoryInfo(System.Web.Hosting.HostingEnvironment.MapPath("~")).Parent.FullName + webConfigFileSource;
        }
    }
}
