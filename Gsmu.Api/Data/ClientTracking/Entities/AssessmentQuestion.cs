//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.ClientTracking.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AssessmentQuestion
    {
        public int id { get; set; }
        public Nullable<int> assessment_id { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public Nullable<int> question_points_earned { get; set; }
        public Nullable<int> question_possible_points { get; set; }
        public Nullable<int> question_num { get; set; }
        public Nullable<int> section_num { get; set; }
        public string section_name { get; set; }
        public Nullable<int> section_question_num { get; set; }
    }
}
