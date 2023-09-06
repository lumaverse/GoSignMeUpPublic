using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.User
{
    public class UserInfo
    {
        internal List<UserCertificatesCompleteds> certCompletedsNotCourseCert;

        public string usergroup { get; set; }

        public string usergroupAbv { get; set; }

        public int userid { get; set; }

        public string username { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string last { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string first { get; set; }

        public int? createdby { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string createdbyname { get; set; }

        public List<UserInfo> createdname { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string email { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string additionalemail { get; set; }

        public int usertype { get; set; }

        public int? schoolid { get; set; }

        public int? districtid { get; set; }

        public int? gradeid { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string schoolName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string districtName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string gradeName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string homephone { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string workphone { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string fax { get; set; }

        public string password { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string address { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string HiddenStudRegField1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string HiddenStudRegField2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string HiddenStudRegField3 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string HiddenStudRegField4 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReadOnlyStudRegField1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReadOnlyStudRegField2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReadOnlyStudRegField3 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReadOnlyStudRegField4 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField3 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField4 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField5 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField6 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField7 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField8 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField9 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField10 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField11 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField12 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField13 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField14 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField15 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField16 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField17 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField18 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField19 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StudRegField20 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string city { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string state { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string zip { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string country { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public short distemployee { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ss { get; set; }

        public List<UserCourses> courses { get; set; }

        public List<UserCertifications> certifications { get; set; }

        public string ProfileImage { get; set; }

        public string TempProfileImage { get; set; }

        public List<UserSurveys> usersurveys { get; set; }

        public List<UserCertificationsCompleteds> certificationsCompleteds { get; set; }

        public List<UserCertificatesCompleteds> certificatesCompleteds { get; set; }

        public List<UserSurveys> surveylists { get; set; }

        public List<UserSurveyCompleted> usersurveyCompleteds { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField3 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField4 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField5 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField6 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField7 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField8 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField9 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string InstructorRegField10 { get; set; }

        public int? district { get; set; }

        public int? school { get; set; }

        public int? grade { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string supervisor {get;set;}

        public int? LocationID2 { get; set; }

        public int? LocationID3 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string supervisornum { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string title { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string additionalemailaddresses { get; set; }

        public byte? notify { get; set; }

        public int? advanceoptions { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string advanceoptionsstr { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone { get; set; }

        //parents or supervisor
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentLevelOneId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentLevelTwoId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentLevelThreeId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentsFirstName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentsLastName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BranchOfService { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RankRateGrade { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CommandDepartment { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SpouseName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SpouseWorkPhone { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SpouseCellPhone { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SpouseEmail { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyName1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyHomePhone1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyAddress1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyPOA1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyName2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyHomePhone2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyWorkPhone2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyAddress2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmergencyPOA2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PCSDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField3 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField4 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField5 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField6 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField7 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField8 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField9 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ParentRegField10 { get; set; }


        public List<UserCertificatesCompleteds> EmailedCertificates { get; set; }

        public List<UserCertificatesReceived> CertificatesReceived { get; set; }
    }

    public class UserCourses
    {
        public int? COURSEID { get; set; }

        public decimal TotalPaid { get; set; }

        public string COURSENUM { get; set; }

        public string COURSENAME { get; set; }

        public DateTime? MaxDate { get; set; }

        public short Cancel { get; set; }

        public short WAITING { get; set; }

        public int? Transcriptcount { get; set; }

        public string CourseType { get; set; }

        public string OrderNumber { get; set; }

        public string MasterOrderNumber { get; set; }

        public int Rosterid { get; set; }

        public short ATTENDED { get; set; }

        public float? paidremainderamount { get; set; }

        public decimal PaidPerCourse { get; set; }

        public decimal CourseCostDecimal { get; set; }

        public decimal RosterMaterialTotalDecimal { get; set; }

        public bool PaidInFullBool { get; set; }

        public decimal TotalCourseCostDecimalbyOrder { get; set; }

        public float? CRPartialPaymentPaidAmount { get; set; }

        public string CourseCost { get; set; }

        public float? RosterMaterialTotal { get; set; }

        public DateTime? MinDate { get; set; }

        public string SelectedCredit { get; set; }

        public DateTime? CourseStartDateTime { get; set; }
        public DateTime? CourseEndDateTime { get; set; }

        public DateTime RegisteredDate { get; set; }
        public string CourseEvent { get; set; }
    }

    public class UserCertifications
    {
        public DateTime? ExpireDate { get; set; }

        public int? CertificationsId { get; set; }

        public string CertificationsTitle { get; set; }

        public int CertificationsStudentId { get; set; }
    }

    public class UserSurveys
    {
        public int SurveyID { get; set; }

        public string Name { get; set; }

        public int SurveyRequired { get; set; }

        public string SurveyRequiredText { get; set; }

        public int CourseID { get; set; }
    }

    public class UserCertificationsCompleteds
    {
        public int? CertificationsStudentId { get; set; }

        public int? CertificationsId { get; set; }

        public string CertificationsTitle { get; set; }

        public DateTime? CompletionDate { get; set; }
        public int? CertificateID { get; set; }
    }

    public class UserCertificatesCompleteds
    {
        public int? CertificationsStudentId { get; set; }

        public int? CertificationsId { get; set; }

        public string CertificationsTitle { get; set; }

        public DateTime? CompletionDate { get; set; }

        public int? CourseId { get; set; }

        public string CourseNum { get; set; }

        public string CourseName { get; set; }


        public DateTime? StartDate { get; set; }

        public string CertType { get; set; }

        public int CertNum { get; set; }

        public DateTime? DateAutoCertSent { get; set; }
        public int? MaxCourseReq { get; internal set; }
        public int CertID { get; internal set; }
    }

    public class UserSurveyCompleted
    {
        public int CourseID { get; set; }

        public DateTime DateTaken { get; set; }

        public int SurveyID { get; set; }
    }

    public class UserCertificatesReceived
    {


        public DateTime RecvdDate { get; set; }

        public string RecvdSubj { get; set; }

        public string Attachment { get; set; }
    }

}
