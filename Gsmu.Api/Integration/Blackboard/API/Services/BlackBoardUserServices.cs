using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static BlackBoardAPI.BlackBoardAPIModel;


namespace Gsmu.Api.Integration.Blackboard.API
{
    public class UserService 
    {
        HttpClient client;
        String access_token;

        public UserService(BBToken token)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
            access_token = token.access_token;
        }


        public  BBUser ReadBlackboardUser(string connectionurl,string UUID)
        {
            BBUser user = new BBUser();

            var uri = new Uri(connectionurl +"/learn/api/public/v1/users/" + "uuid:" + UUID);

            try
            {


                var myWebRequest = System.Net.WebRequest.Create(uri);
                var myHttpWebRequest = (System.Net.HttpWebRequest)myWebRequest;
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Headers.Add("Authorization", "Bearer " + access_token);
                myHttpWebRequest.Accept = "application/json";

                var myWebResponse = myWebRequest.GetResponse();
                var responseStream = myWebResponse.GetResponseStream();
                if (responseStream == null) return null;

                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                string json = myStreamReader.ReadToEnd();
                user = JsonConvert.DeserializeObject<BBUser>(json);
                responseStream.Close();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {

            }

            return user;
        }


        public BBUser GetCourseList(string connectionurl, string UUID)
        {
            BBUser user = new BBUser();

            var uri = new Uri(connectionurl + "/learn/api/public/v3/courses");

            try
            {


                var myWebRequest = System.Net.WebRequest.Create(uri);
                var myHttpWebRequest = (System.Net.HttpWebRequest)myWebRequest;
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Headers.Add("Authorization", "Bearer " + access_token);
                myHttpWebRequest.Accept = "application/json";

                var myWebResponse = myWebRequest.GetResponse();
                var responseStream = myWebResponse.GetResponseStream();
                if (responseStream == null) return null;

                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                string json = myStreamReader.ReadToEnd();
                user = JsonConvert.DeserializeObject<BBUser>(json);
                responseStream.Close();
                myWebResponse.Close();

                //var response =  client.GetAsync(uri).Result;
                //if (response.IsSuccessStatusCode)
                //{
                //    var content = await response.Content.ReadAsStringAsync();
                //    user = JsonConvert.DeserializeObject<BBUser>(content);
                //}
                //else
                //{
                //    response.EnsureSuccessStatusCode();
                //}
            }
            catch (Exception ex)
            {

            }

            return user;
        }



    }
}