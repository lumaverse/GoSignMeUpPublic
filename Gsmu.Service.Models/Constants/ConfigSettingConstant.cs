using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Constants
{
    public class ConfigSettingConstant
    {
        //@TODO : Values here can be stored elsewhere -- AppConfig Perhaps
        //DAL constants
        public const string schoolEntitiesKey = "SchoolEntities";
        public const string eventsEntitiesKey = "EventsEntities";
        public const string roomManagementEntitiesKey = "RoomManagementEntities";
        public const string surveyEntitiesKey = "SurveyEntities";

        //APP And Connection Config contsants
        public const string webConfigFileSourceDev = @"\Gsmu.Web\Web.config";
        public const string webConfigFileSource = @"\Web\Web.config";

        //API RESPONSES
        public const string REQUEST_NOT_AUTHORIZED = "Authorization has been denied for this request. Wrong credentials or account is not authorized.";
        public const string MISSING_TOKEN_VALUE = "Authorization has been denied for this request. Missing Token value.";
        public const string MISSING_TOKEN = "Authorization has been denied for this request. Missing Token.";

        public const string webRootUrlDev = @"\Gsmu.Web\";
        public const string webRootUrl = @"\Web\";

        public const string webRootAdminUrlDev = @"\Gsmu.Web\admin\";
        public const string webRootAdminUrlLive = @"\Web\admin\";

        public const string imageDestinationDirectory = @"\admin\CourseTiles\"; //@TODO : transfer to app config if necessary
    }
}
