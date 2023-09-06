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
    
    public partial class Supervisor
    {
        public Supervisor()
        {
            this.SupervisorSchools = new HashSet<SupervisorSchool>();
            this.SupervisorStudents = new HashSet<SupervisorStudent>();
        }
    
        public int SUPERVISORID { get; set; }
        public string FIRST { get; set; }
        public string LAST { get; set; }
        public string TITLE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string PHONE { get; set; }
        public string FAX { get; set; }
        public string SUPERVISORNUM { get; set; }
        public string UserName { get; set; }
        public string PASSWORD { get; set; }
        public Nullable<int> SCHOOL { get; set; }
        public Nullable<int> DISTRICT { get; set; }
        public string EMAIL { get; set; }
        public short ACTIVE { get; set; }
        public Nullable<int> GRADE { get; set; }
        public Nullable<byte> NOTIFY { get; set; }
        public Nullable<int> AdvanceOptions { get; set; }
        public string AdditionalEmailAddresses { get; set; }
        public Nullable<System.DateTime> date_modified { get; set; }
        public string ProfileImageFile { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<int> CreatedFrom { get; set; }
        public string resetPasswordHash { get; set; }
        public Nullable<System.DateTime> resetPasswordDate { get; set; }
        public Nullable<System.Guid> UserSessionId { get; set; }
        public Nullable<long> canvas_user_id { get; set; }
    
        public virtual ICollection<SupervisorSchool> SupervisorSchools { get; set; }
        public virtual ICollection<SupervisorStudent> SupervisorStudents { get; set; }
    }
}
