using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;
/*
 *        "JsonResult": [
            {
                "id": 40,
                "user_id": 41,
                "account_id": 1,
                "unique_id": "mini.rabit2",
                "sis_user_id": "GSMUST11yxz",
                "integration_id": null,
                "authentication_provider_id": null
            },
            {
                "id": 41,
                "user_id": 41,
                "account_id": 1,
                "unique_id": "mini.rabit2b",
                "sis_user_id": "GSMUST11vv",
                "integration_id": null,
                "authentication_provider_id": null
            }]
 * */
namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class UserLoginDetails
    {
        [json.JsonProperty("id")]
        public Int64? LD_Id { get; set; }

        [json.JsonProperty("user_id")]
        public Int64? LD_user_id { get; set; }

        [json.JsonProperty("account_id")]
        public Int64? LD_account_id { get; set; }

        [json.JsonProperty("unique_id")]
        public string LD_unique_id { get; set; }

        [json.JsonProperty("sis_user_id")]
        public string LD_sis_user_id { get; set; }

        [json.JsonProperty("authentication_provider_id")]
        public Int64? LD_authentication_provider_id { get; set; }
    }
}
