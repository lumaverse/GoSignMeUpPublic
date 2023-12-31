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

    public partial class Student
    {
        public Student()
        {
            this.CertificationsStudents = new HashSet<CertificationsStudent>();
            this.CertificationsStudentCompleteds = new HashSet<CertificationsStudentCompleted>();
            this.Course_Roster = new HashSet<Course_Roster>();
            this.StudentMedicalInfoes = new HashSet<StudentMedicalInfo>();
            this.StudentRates = new HashSet<StudentRate>();
            this.Transcripts = new HashSet<Transcript>();
        }

        public int STUDENTID { get; set; }
        public string STUDNUM { get; set; }
        public string FIRST { get; set; }
        public string CLASSIFICATION { get; set; }
        public string LAST { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string SS { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string COUNTRY { get; set; }
        public Nullable<int> GRADE { get; set; }
        public string DEPARTMENTNAME { get; set; }
        public Nullable<int> SCHOOL { get; set; }
        public Nullable<int> DISTRICT { get; set; }
        public string HOMEPHONE { get; set; }
        public string WORKPHONE { get; set; }
        public string FAX { get; set; }
        public string EXPERIENCE { get; set; }
        public short DISTEMPLOYEE { get; set; }
        public short TRADITIONAL_CYCLE { get; set; }
        public string NOTES { get; set; }
        public Nullable<int> loginTally { get; set; }
        public string StudRegField1 { get; set; }
        public string StudRegField2 { get; set; }
        public string StudRegField3 { get; set; }
        public string StudRegField4 { get; set; }
        public string StudRegField5 { get; set; }
        public string StudRegField6 { get; set; }
        public string StudRegField7 { get; set; }
        public string StudRegField8 { get; set; }
        public string StudRegField9 { get; set; }
        public string StudRegField10 { get; set; }
        public short PrivacyPol { get; set; }
        public short parentsid { get; set; }
        public string HiddenStudRegField1 { get; set; }
        public string HiddenStudRegField2 { get; set; }
        public string ReadOnlyStudRegField1 { get; set; }
        public string ReadOnlyStudRegField2 { get; set; }
        public string ReadOnlyStudRegField3 { get; set; }
        public string ReadOnlyStudRegField4 { get; set; }
        public int InActive { get; set; }
        public int LocationID2 { get; set; }
        public int LocationID3 { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public string SAPLastPendingReason { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public Nullable<int> SAPSyncCount { get; set; }
        public Nullable<System.DateTime> LastTimeSAPSync { get; set; }
        public string LastUpdateAccount { get; set; }
        public Nullable<System.DateTime> date_modified { get; set; }
        public Nullable<System.DateTime> date_bb_integrated { get; set; }
        public Nullable<int> AuthFromLDAP { get; set; }
        public Nullable<System.DateTime> ProfileViewedDateTime { get; set; }
        public Nullable<System.DateTime> date_imported_from_bb { get; set; }
        public string HiddenStudRegField3 { get; set; }
        public string HiddenStudRegField4 { get; set; }
        public Nullable<System.DateTime> lastlogin { get; set; }
        public string resetPasswordHash { get; set; }
        public Nullable<System.DateTime> resetPasswordDate { get; set; }
        public string shipaddress { get; set; }
        public string shipcity { get; set; }
        public string shipstate { get; set; }
        public string shipzip { get; set; }
        public string shipcountry { get; set; }
        public string QuestionnairesInfo { get; set; }
        public int CreatedBy { get; set; }
        public string StudRegField11 { get; set; }
        public string StudRegField12 { get; set; }
        public string StudRegField13 { get; set; }
        public string StudRegField14 { get; set; }
        public string StudRegField15 { get; set; }
        public string StudRegField16 { get; set; }
        public string StudRegField17 { get; set; }
        public string StudRegField18 { get; set; }
        public string StudRegField19 { get; set; }
        public string StudRegField20 { get; set; }
        public string BBPrimaryInstitutionRole { get; set; }
        public Nullable<int> google_user { get; set; }
        public string google_sso_refresh_token { get; set; }
        public Nullable<System.DateTime> membershipexpiredate { get; set; }
        public string ProfileImage { get; set; }
        public string TempProfileImage { get; set; }
        public int haiku_user_id { get; set; }
        public string haiku_import_id { get; set; }
        public string lti_user_id { get; set; }
        public Nullable<long> canvas_user_id { get; set; }
        public string lti_data { get; set; }
        public string blackboard_dsk { get; set; }
        public string additionalemail { get; set; }
        public Nullable<System.Guid> UserSessionId { get; set; }
        public Nullable<int> clubready_student_id { get; set; }
        public string Blackboard_user_UUID { get; set; }
        public Nullable<int> ResetPassword { get; set; }

        public virtual ICollection<CertificationsStudent> CertificationsStudents { get; set; }
        public virtual ICollection<CertificationsStudentCompleted> CertificationsStudentCompleteds { get; set; }
        public virtual ICollection<Course_Roster> Course_Roster { get; set; }
        public virtual ICollection<StudentMedicalInfo> StudentMedicalInfoes { get; set; }
        public virtual ICollection<StudentRate> StudentRates { get; set; }
        public virtual ICollection<Transcript> Transcripts { get; set; }
    }
}
