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
    
    public partial class StudentMergeHistory
    {
        public int StudentMergeHistoryId { get; set; }
        public int FromStudentId { get; set; }
        public int ToStudentId { get; set; }
        public Nullable<System.DateTime> MergeDate { get; set; }
    }
}
