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
    
    public partial class Certification
    {
        public Certification()
        {
            this.CertificationsCourses = new HashSet<CertificationsCourse>();
            this.CertificationsStudents = new HashSet<CertificationsStudent>();
            this.CertificationsStudentCompleteds = new HashSet<CertificationsStudentCompleted>();
        }
    
        public int CertificationsId { get; set; }
        public string CertificationsTitle { get; set; }
        public Nullable<int> CertificationsAutoDistrictId { get; set; }
        public Nullable<int> CertificationsAutoSchoolId { get; set; }
        public Nullable<int> CertificationsAutoGradeLevelsId { get; set; }
        public Nullable<float> CertificationsYearsToExpire { get; set; }
        public Nullable<int> NotAllowDuplicate { get; set; }
        public Nullable<int> AutoCreateAccount { get; set; }
        public Nullable<int> CertificationsHowManyCoursesRequired { get; set; }
        public Nullable<int> CertificationsCustomCertId { get; set; }
    
        public virtual ICollection<CertificationsCourse> CertificationsCourses { get; set; }
        public virtual ICollection<CertificationsStudent> CertificationsStudents { get; set; }
        public virtual ICollection<CertificationsStudentCompleted> CertificationsStudentCompleteds { get; set; }
    }
}
