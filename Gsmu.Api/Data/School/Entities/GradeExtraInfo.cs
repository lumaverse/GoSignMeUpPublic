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
    
    public partial class GradeExtraInfo
    {
        public int gexId { get; set; }
        public string gradeshortdesc { get; set; }
        public string manufacturer { get; set; }
        public string gaddress { get; set; }
        public string gcity { get; set; }
        public string gstate { get; set; }
        public string gzip { get; set; }
        public Nullable<int> gradeid { get; set; }
    
        public virtual Grade_Level Grade_Levels { get; set; }
    }
}
