//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.RoomManagement.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class member
    {
        public int member_id { get; set; }
        public string member_login { get; set; }
        public string member_password { get; set; }
        public string member_first_name { get; set; }
        public string member_last_name { get; set; }
        public string member_email { get; set; }
        public string member_county { get; set; }
        public Nullable<int> country_id { get; set; }
        public string member_city { get; set; }
        public string member_zip { get; set; }
        public string member_address1 { get; set; }
        public string member_address2 { get; set; }
        public string member_phone { get; set; }
        public string member_fax { get; set; }
        public Nullable<short> member_status { get; set; }
        public Nullable<System.DateTime> member_date_added { get; set; }
        public short member_notification { get; set; }
        public string RoomMgmtID { get; set; }
    
        public virtual listing_countries listing_countries { get; set; }
    }
}
