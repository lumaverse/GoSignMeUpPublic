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
    
    public partial class StudentRate
    {
        public int StudentRatesId { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<decimal> Rate { get; set; }
    
        public virtual Student Student { get; set; }
    }
}
