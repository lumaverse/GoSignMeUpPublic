using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    /*

        {
            "id": 173067,
            "name": "Patrik Laszlo",
            "sortable_name": "Laszlo, Patrik",
            "short_name": "Patrik",
            "sis_user_id": "patrik",
            "sis_login_id": "patrik@gosignmeup.com",
            "login_id": "patrik@gosignmeup.com"
        }     
     */
    public class User
    {
        [json.JsonProperty("id")]
        public Int64? Id { get; set; }

        [json.JsonProperty("name")]
        public string Name { get; set; }

        [json.JsonProperty("sortable_name")]
        public string SortableName { get; set; }

        [json.JsonProperty("short_name")]
        public string ShortName { get; set; }

        [json.JsonProperty("sis_user_id")]
        public string SisUserId { get; set; }

        [json.JsonProperty("sis_login_id")]
        public string SisLoginId { get; set; }

        [json.JsonProperty("login_id")]
        public string LoginId { get; set; }

        [json.JsonProperty("primary_email")]
        public string PrimaryEmail { get; set; }

        [json.JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [json.JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [json.JsonProperty("calendar")]
        public Calendar Calendar { get; set; }

        [json.JsonProperty("title")]
        public string Title { get; set; }

        [json.JsonProperty("bio")]
        public string Bio { get; set; }

        [json.JsonProperty("cpass")]
        public string cpass { get; set; }

    }
}
