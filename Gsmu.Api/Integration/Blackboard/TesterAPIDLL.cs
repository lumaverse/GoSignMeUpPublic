//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BlackBoardAPI;
//using Gsmu.Api.Data;
//using static BlackBoardAPI.BlackBoardAPIModel;

//namespace Gsmu.Api.Integration.Blackboard
//{
//    public class TesterAPIDLL
//    {
//            public BBUser GetUserDetails()
//        {
//            string application_key = Settings.Instance.GetMasterInfo4().blackboard_security_key;
//            string secret_key = Settings.Instance.GetMasterInfo3().BlackBoardSecretKey;
//            string return_url = Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl;
//            string connection_url = Settings.Instance.GetMasterInfo2().BlackboardConnectionUrl;
//            BlackboardAPIRequestHandler handelr = new BlackboardAPIRequestHandler();
//            return handelr.GetUserDetails(secret_key, application_key, return_url, connection_url);

//        }

//        public Course GetBlackBoardCourses()
//        {
//            string application_key = "3b402c30-3c6e-4cfb-89f2-7f46d77bbcaa"; // Settings.Instance.GetMasterInfo4().blackboard_security_key;
//            string secret_key = "DwKNBTeqHjNjkdWq4IsiF5ggkvv3nHhi";// Settings.Instance.GetMasterInfo3().BlackBoardSecretKey;
//            string return_url = Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl;
//            string connection_url = "https://bd-partner-a-original.blackboard.com"; //Settings.Instance.GetMasterInfo2().BlackboardConnectionUrl;
//            BlackboardAPIRequestHandler handelr = new BlackboardAPIRequestHandler();
//            return handelr.GetAllCourses(secret_key, application_key, return_url, connection_url);

//        }
//    }
//}
