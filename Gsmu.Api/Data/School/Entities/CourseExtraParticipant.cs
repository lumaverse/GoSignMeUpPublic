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
    
    public partial class CourseExtraParticipant
    {
        public int CourseExtraParticipantId { get; set; }
        public Nullable<int> RosterId { get; set; }
        public string StudentFirst { get; set; }
        public string StudentLast { get; set; }
        public string StudentEmail { get; set; }
        public string CustomField2 { get; set; }
    
        public virtual Course_Roster Course_Roster { get; set; }
    }
}