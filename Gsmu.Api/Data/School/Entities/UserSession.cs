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
    
    public partial class UserSession
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> UserType { get; set; }
        public string SessionId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public Nullable<bool> Expired { get; set; }
    }
}
