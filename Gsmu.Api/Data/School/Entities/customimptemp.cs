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
    
    public partial class customimptemp
    {
        public int customimportid { get; set; }
        public string templateName { get; set; }
        public Nullable<System.DateTime> dateadded { get; set; }
        public string maintable { get; set; }
        public string selectedfields { get; set; }
        public Nullable<System.DateTime> lastrun { get; set; }
        public Nullable<int> SubsiteId { get; set; }
        public Nullable<int> disabled { get; set; }
        public string idescription { get; set; }
        public string lastfile { get; set; }
        public string uploadfiletype { get; set; }
        public string delimiter { get; set; }
        public Nullable<int> finalizeattendance { get; set; }
        public string importaction { get; set; }
        public Nullable<int> isroutine { get; set; }
        public Nullable<int> skipStudentUponDupe { get; set; }
    }
}
