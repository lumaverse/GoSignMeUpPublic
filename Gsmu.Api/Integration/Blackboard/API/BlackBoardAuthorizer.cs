using Gsmu.Api.Data;
using Gsmu.Api.Integration.Blackboard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static BlackBoardAPI.BlackBoardAPIModel;


namespace Gsmu.Api.Integration.Blackboard.API
{
    public class Authorizer
    {
        HttpClient client;
        BBToken token { get; set; }

        public async Task<BBToken> Authorize(string secretkey, string applicationkey, string return_url,string connectionurl,string code="")
        {

            var authData = string.Format("{0}:{1}", applicationkey, secretkey);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            client = new HttpClient();
            var endpoint = new Uri(connectionurl + "/learn/api/public/v1/oauth2/token?code=" +code + "&redirect_uri="+ return_url+"/authme/BlackBoardAuthenticationResponseHanlder");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

            HttpContent body = new FormUrlEncodedContent(postData);
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(endpoint, body).ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<BBToken>(content);
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }


            }
            catch (Exception ex)
            {
    
            }

            return token;
        }

    }
}