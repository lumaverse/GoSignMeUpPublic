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
    
    public partial class licenseinfo
    {
        public int licenseid { get; set; }
        public string licensetype { get; set; }
        public string licensenum { get; set; }
        public string licensestate { get; set; }
        public Nullable<int> studentid { get; set; }
        public Nullable<System.DateTime> dateadded { get; set; }
        public string licenseexp { get; set; }
        public Nullable<int> nonedit { get; set; }
        public Nullable<int> courseid { get; set; }
    }
}