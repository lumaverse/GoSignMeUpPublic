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
    
    public partial class UpdateTracking
    {
        public int ID { get; set; }
        public string OrigFilename { get; set; }
        public string OrigPath { get; set; }
        public string UniqueFilename { get; set; }
        public string Session { get; set; }
        public Nullable<System.DateTime> DateStamp { get; set; }
        public Nullable<int> enabled { get; set; }
    }
}