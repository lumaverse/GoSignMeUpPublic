using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;

namespace Gsmu.Api.Integration.Canvas.Clients
{
    public class AccountClient
    {

        public static Response GetAccountAuthProvider(int accountId)
        {
            var url = string.Format("/api/v1/accounts/{0}/authentication_providers", accountId);
            var response = CanvasClient.GetResponse("GetAccountAuthProvider",http.HttpMethod.Get, url);            
            return response;
        }
        public static Response GetAccount(int accountId)
        {
            var url = string.Format("/api/v1/accounts/{0}", accountId);
            var response = CanvasClient.GetResponse("GetAccount",http.HttpMethod.Get, url);
            return response;
        }
        public static Response GetListSubAccounts(int accountId, int IsRecursiveMode)
        {
            //use flag recursive to pull all tree under single account id
            var query = new NameValueCollection();
            if (IsRecursiveMode == 1) { query.Add("recursive", "True"); }
            var url = string.Format("/api/v1/accounts/{0}/sub_accounts", accountId);
            var response = CanvasClient.GetResponse("GetListSubAccounts", http.HttpMethod.Get, url,query);
            //var response = CanvasClient.GetResponse(http.HttpMethod.Get, url);
            return response;
        }
        public static Response GetListAccountsAdminCanSee()
        {
            var url = string.Format("/api/v1/course_accounts");
            var response = CanvasClient.GetResponse("GetListAccountsAdminCanSee", http.HttpMethod.Get, url);
            return response;
        }

        public static Response GetListMainAccounts
        {
            get
            {
                var response = CanvasClient.GetResponse("GetListMainAccounts", http.HttpMethod.Get, "/api/v1/accounts");
                return response;
            }
        }
    }
}
