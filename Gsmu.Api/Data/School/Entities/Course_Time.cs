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
    
    public partial class Course_Time
    {
        public int ID { get; set; }
        public Nullable<int> COURSEID { get; set; }
        public Nullable<System.DateTime> COURSEDATE { get; set; }
        public Nullable<System.DateTime> STARTTIME { get; set; }
        public Nullable<System.DateTime> FINISHTIME { get; set; }
    
        public virtual Course Cours { get; set; }
    }
}