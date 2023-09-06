using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    /*
        "id": 16,
        "name": "MVCC Board (new)",
        "parent_account_id": null,
        "root_account_id": null,
        "default_storage_quota_mb": 500,
        "default_user_storage_quota_mb": 50,
        "default_group_storage_quota_mb": 50,
        "default_time_zone": "America/Chicago"     
    */
    public class Account
    {
        [json.JsonProperty(PropertyName="id")]
        public int Id { get; set; }

        [json.JsonProperty("name")]
        public string Name { get; set; }

        [json.JsonProperty("parent_account_id")]
        public int? ParentAccountId { get; set; }

        [json.JsonProperty("root_account_id")]
        public int? RootAccountId { get; set; }

        [json.JsonProperty("default_storage_quota_mb")]
        public int?  DefaultStorageQuotaMb { get; set; }

        [json.JsonProperty("default_user_storage_quota_mb")]
        public int? DefaultUserStorageQuotaMb { get; set; }

        [json.JsonProperty("default_group_storage_quota_mb")]
        public int? DefaultGroupStorageQuotaMb { get; set; }

        [json.JsonProperty("default_time_zone")]
        public string DefaultTimeZone { get; set; }
    }
}
