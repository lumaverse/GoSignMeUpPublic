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
    
    public partial class StudentMedicalInfo
    {
        public int StudentMedicalInfoId { get; set; }
        public string Conditions { get; set; }
        public int Question1 { get; set; }
        public int Question2 { get; set; }
        public int Question3 { get; set; }
        public int Question4 { get; set; }
        public int Question4Details1 { get; set; }
        public int Question4Details2 { get; set; }
        public int Question4Details3 { get; set; }
        public int Question5 { get; set; }
        public int Question5Details1 { get; set; }
        public int Question5Details2 { get; set; }
        public int Question5Details3 { get; set; }
        public int Question6 { get; set; }
        public int Question6Details1 { get; set; }
        public int Question6Details2 { get; set; }
        public int Question6Details3 { get; set; }
        public int Question6Details4 { get; set; }
        public int Question7 { get; set; }
        public int Question7Details1 { get; set; }
        public int Question7Details2 { get; set; }
        public int ReleaseFormOnFile { get; set; }
        public int StudentId { get; set; }
    
        public virtual Student Student { get; set; }
    }
}