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
    
    public partial class RoommateRequest
    {
        public int rmid { get; set; }
        public string reqRMName { get; set; }
        public string reqRMAddress { get; set; }
        public string reqRMEmail { get; set; }
        public Nullable<int> rosterid { get; set; }
        public Nullable<int> courseid { get; set; }
        public Nullable<System.DateTime> regdate { get; set; }
        public Nullable<int> reqRMApproved { get; set; }
        public Nullable<System.DateTime> reqdate { get; set; }
    }
}
