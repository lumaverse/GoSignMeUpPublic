//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.School.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class adminpass
    {
        public int AdminID { get; set; }
        public string username { get; set; }
        public string userpass { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> dateadded { get; set; }
        public short disabled { get; set; }
        public string PortalSettings { get; set; }
        public string WidgetSettings { get; set; }
        public Nullable<System.Guid> UserSessionId { get; set; }
    }
}
