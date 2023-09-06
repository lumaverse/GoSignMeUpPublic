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
    
    public partial class MasterInfo2
    {
        public int MasterInfo2ID { get; set; }
        public short Sort1 { get; set; }
        public short Sort2 { get; set; }
        public short Sort3 { get; set; }
        public short PricingVisible { get; set; }
        public string EmailMergeSubject { get; set; }
        public string EmailMergeBody { get; set; }
        public short HideLocationName { get; set; }
        public short HideCourseNumber { get; set; }
        public short disallownewuser { get; set; }
        public string publicannouncement { get; set; }
        public short SubOptions { get; set; }
        public short EventNo { get; set; }
        public short ContactInfo { get; set; }
        public short DisallowPasswordEdit { get; set; }
        public short DisallowUserEdit { get; set; }
        public short DisallowDistrictEdit { get; set; }
        public short DisplayUsername { get; set; }
        public string CreditHoursName { get; set; }
        public string InserviceHoursName { get; set; }
        public short EnableCourseColors { get; set; }
        public short PricingOptionOverride { get; set; }
        public short LogoOnTranscript { get; set; }
        public short AllowParentLevel { get; set; }
        public string ParentLevelTitle { get; set; }
        public int AllowReleaseForm { get; set; }
        public string ReleaseFormTitle { get; set; }
        public string ReleaseFormText { get; set; }
        public string PLDateEmailSubject { get; set; }
        public string PLDateEmailBody { get; set; }
        public int CCVOn { get; set; }
        public int PricingHourType { get; set; }
        public short SubAdminHideStudPass { get; set; }
        public int PricingOrCreditType { get; set; }
        public int VBSEmailAudit { get; set; }
        public string SocialSecurityMemo { get; set; }
        public string PublicAddressLabel { get; set; }
        public short ShowRebill { get; set; }
        public short ShowCourseComments { get; set; }
        public short ShowCheckoutComments { get; set; }
        public string CheckoutCommentsLabel { get; set; }
        public int SignupSheetDefaultColumn1 { get; set; }
        public int SignupSheetDefaultColumn2 { get; set; }
        public int SignupSheetDefaultColumn3 { get; set; }
        public short CertificatesDefaultBorder { get; set; }
        public short CertificatesDefaultInstructor { get; set; }
        public short CertificatesDefaultInstructorLine { get; set; }
        public short CertificatesDefaultShowHours { get; set; }
        public short CertificatesDefaultEmail { get; set; }
        public int ConfirmAlert { get; set; }
        public int PendingRefundsLink { get; set; }
        public short supervisor_email { get; set; }
        public int HidePaymentInfo { get; set; }
        public short SignupSheetNoSS { get; set; }
        public int associatecourseoutline { get; set; }
        public int showotherpaymentnumber { get; set; }
        public string CourseFullMessage { get; set; }
        public int LDAPOn { get; set; }
        public string LDAPServer { get; set; }
        public string LDAPUserName { get; set; }
        public string LDAPPassword { get; set; }
        public string DataIntegrationTableName { get; set; }
        public int HideSeatsAvailable { get; set; }
        public short Sort4 { get; set; }
        public short ShowCourseTime { get; set; }
        public int ShowReqFlexDays { get; set; }
        public string Period { get; set; }
        public string NextPeriod { get; set; }
        public string DateOfCutoff { get; set; }
        public string ReqHoursExceededLabel { get; set; }
        public string CreditBankingBeginDate { get; set; }
        public string membertypememberlabel { get; set; }
        public string membertypenonmemberlabel { get; set; }
        public Nullable<int> CatalogModuleOn { get; set; }
        public string CourseWaitListMessage { get; set; }
        public string membertypemembercomment { get; set; }
        public string membertypenonmembercomment { get; set; }
        public Nullable<int> AllowAttendanceDetail { get; set; }
        public Nullable<int> PublicTerminology { get; set; }
        public string paytypelabel { get; set; }
        public Nullable<short> ShowCustomCreditType { get; set; }
        public string CustomCreditTypeName { get; set; }
        public short ShowCEUandGraduateCreditCourses { get; set; }
        public Nullable<int> CourseRequirements { get; set; }
        public string AlternateConfirmation { get; set; }
        public string CustomCourseFieldLabel1 { get; set; }
        public string CustomCourseFieldLabel2 { get; set; }
        public Nullable<int> CustomCourseFieldShow1 { get; set; }
        public Nullable<int> CustomCourseFieldShow2 { get; set; }
        public short allowStudentEditMemberType { get; set; }
        public Nullable<int> allowStudentSelectMembershipOnRegistration { get; set; }
        public int ShowCancelComments { get; set; }
        public int DontDefaultAttendance { get; set; }
        public int BlackBoardWatchDays { get; set; }
        public int BlackBoardEnrollmentThreshold { get; set; }
        public Nullable<int> Parking { get; set; }
        public string AltEmailConfirmation { get; set; }
        public int BlackBoardEnrollmentThresholdPercentage { get; set; }
        public int DefaultPublicPricingType { get; set; }
        public string MemberTypeSpecialMemberLabel1 { get; set; }
        public string SubstituteInfoConfirmText { get; set; }
        public string MemberTypeSpecialMemberComment1 { get; set; }
        public string FTPAddress { get; set; }
        public string FTPPort { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public string FTPRemoteStartUpSubDir { get; set; }
        public int AllowInstructorMultiPassAttendance { get; set; }
        public int CheckForUpdatesLastOption { get; set; }
        public int ShowStandardOnCatalog { get; set; }
        public int LDAPStudentDataIn { get; set; }
        public int DuplicateEnrollPercentage { get; set; }
        public int DuplicateMaxEnrollment { get; set; }
        public int DuplicateSetting { get; set; }
        public int CourseDuplicateWhenMax { get; set; }
        public int CourseDuplicateWhenPercentMax { get; set; }
        public string PublicAnnouncement2 { get; set; }
        public int BufferValue { get; set; }
        public int DefaultNewStudentMemberType { get; set; }
        public Nullable<int> CapitalizeMaskFields { get; set; }
        public Nullable<int> AllowAttendanceStatus { get; set; }
        public Nullable<int> SubSiteId { get; set; }
        public Nullable<int> StudentValidate { get; set; }
        public string StudentValidateImportFileName { get; set; }
        public Nullable<int> StudentValidateDistrictId { get; set; }
        public Nullable<int> FilterMaskData { get; set; }
        public Nullable<System.DateTime> RemindersRanDate { get; set; }
        public Nullable<System.DateTime> RoomMgmtRemindersRanDate { get; set; }
        public Nullable<System.DateTime> MustRunRanDate { get; set; }
        public Nullable<System.DateTime> CustomImport1RanDate { get; set; }
        public Nullable<System.DateTime> CustomExport1RanDate { get; set; }
        public Nullable<System.DateTime> ReportStartRanDate { get; set; }
        public Nullable<System.DateTime> CustomImport2RanDate { get; set; }
        public string CourseImport1Filename { get; set; }
        public string CourseImport1BaseExternalKey { get; set; }
        public Nullable<int> CourseImport1NonBaseMainCategory { get; set; }
        public Nullable<int> CourseImport1NonBaseSubCategory { get; set; }
        public Nullable<int> CourseImport1BaseMainCategory { get; set; }
        public Nullable<int> CourseImport1BaseSubCategory { get; set; }
        public string CourseFullMessageLong { get; set; }
        public Nullable<int> useICSSCMPAPI { get; set; }
        public string ICSMerchantID { get; set; }
        public string ICSServer { get; set; }
        public string PublicSearchCourseImage { get; set; }
        public string CreditRequirementLinkText { get; set; }
        public string WaitingListStatement { get; set; }
        public string LDAPContext { get; set; }
        public string LDAPUserNameField { get; set; }
        public Nullable<int> CommonPublicLogin { get; set; }
        public string AltCourseConfirmation { get; set; }
        public string LDAPServer2 { get; set; }
        public string LDAPContext2 { get; set; }
        public string LDAPUserNameField2 { get; set; }
        public Nullable<int> AllowPublicBreakCommonLogin { get; set; }
        public Nullable<int> HideCheckoutApproval { get; set; }
        public Nullable<int> AdminCurrentCourseDays { get; set; }
        public Nullable<int> INHOUSETRANSFER { get; set; }
        public Nullable<int> INHOUSETRANSFER_DISTRICT { get; set; }
        public Nullable<int> HideForgotPassword { get; set; }
        public string BlackboardConnectionUrl { get; set; }
        public string BlackboardCourseIdFieldName { get; set; }
        public string BlackboardIntegrationReportEmailRecipients { get; set; }
        public Nullable<int> AllowStudentMultiEnroll { get; set; }
        public Nullable<int> AllowModifyMultiEnroll { get; set; }
        public Nullable<int> MinStudentNum4MultiEnrollDiscount { get; set; }
        public Nullable<int> Percent4MultiEnrollDiscount { get; set; }
        public Nullable<int> MaskApplyDistrict { get; set; }
        public int MaskPasswordInEmail { get; set; }
        public int LDAPSyncPassword { get; set; }
        public string PayPalEmailAddress { get; set; }
        public string PricingOptionsLabel { get; set; }
        public Nullable<int> CertificationsOnOff { get; set; }
        public string CertificationsReminderSubject { get; set; }
        public string CertificationsReminderBody { get; set; }
        public Nullable<int> CertificationsSupervisorReminderEveryDays { get; set; }
        public Nullable<int> CertificationsSupervisorReminderDaysBefore { get; set; }
        public Nullable<System.DateTime> CertificationsSupervisorReminderLastSent { get; set; }
        public Nullable<int> CourseCostingOn { get; set; }
        public string CustomCourseFieldLabel3 { get; set; }
        public string CustomCourseFieldLabel4 { get; set; }
        public Nullable<int> CustomCourseFieldShow3 { get; set; }
        public Nullable<int> CustomCourseFieldShow4 { get; set; }
        public Nullable<int> EditClassListAlternatePositionColumn { get; set; }
        public Nullable<int> ExtraSubOptions { get; set; }
        public string ChildLevelTitle { get; set; }
        public int CollectOptionalInfo { get; set; }
        public int InternalCourseShowByDisctrictId { get; set; }
        public int ParentLevelCopyParentInfoToStudent { get; set; }
        public Nullable<float> ChargeTimeDifferentialHours { get; set; }
        public Nullable<float> ChargeTimeCutoff { get; set; }
        public string CourseAlreadyEnrolledMessage { get; set; }
        public int ShowCourseDatesInCart { get; set; }
        public int ShowCompensationColumn { get; set; }
        public int SendCalendarEventViaEmail { get; set; }
        public int ShowInstructor { get; set; }
        public int ShowOfflineStudentNames { get; set; }
        public int AutoCourseNum { get; set; }
        public int SendEmailWhenCourseDatesChange { get; set; }
        public short AdminHideStudPass { get; set; }
        public string ReminderLogoImageURL { get; set; }
        public Nullable<int> EmailCOComment { get; set; }
        public string EmailCOCommentTo { get; set; }
        public string EmailCOCommentText { get; set; }
        public Nullable<int> EmailCOCommentCC { get; set; }
        public Nullable<int> CheckoutQuestion { get; set; }
        public string CheckoutQuestionText { get; set; }
        public string CheckoutQuestionAnswer { get; set; }
        public Nullable<int> CheckoutQuestionReq { get; set; }
        public Nullable<int> LDAPOption5CustomField { get; set; }
        public Nullable<int> SupervisorStudentFilter { get; set; }
        public Nullable<int> DisallowUserPublicEdit { get; set; }
        public string DisallowUserPublicEditText { get; set; }
        public Nullable<int> AuthFromLdap { get; set; }
        public string CalendarEmailCOnfirmSubject { get; set; }
        public string CalendarEmailConfirmbody { get; set; }
        public Nullable<int> OrderBillToOptions { get; set; }
        public string CollectionStyleLabel { get; set; }
        public string RegisterNewUserLinkText { get; set; }
        public Nullable<int> PricingOptRange { get; set; }
        public string EmailBBCertSubject { get; set; }
        public string EmailBBCertBody { get; set; }
        public Nullable<int> DisallowSchoolEdit { get; set; }
        public Nullable<int> parentsSyncStudDropDown { get; set; }
        public string studregmask5initialtext { get; set; }
        public Nullable<int> CertificationsNoExpire { get; set; }
        public Nullable<float> CertificationsYearsToExpire { get; set; }
        public int ShowInvoiceInfo { get; set; }
        public int ShowCourseType { get; set; }
        public Nullable<int> AllowEditingOfCoursePrice { get; set; }
        public Nullable<int> LDAPOption5CustomField2 { get; set; }
        public Nullable<int> AdminShowFutureCourseIn { get; set; }
        public Nullable<int> usePubDateFormat { get; set; }
        public Nullable<int> LDAPOption11CustomField1 { get; set; }
        public Nullable<int> LDAPOption11CustomField2 { get; set; }
        public Nullable<int> LDAPOption11CustomField3 { get; set; }
        public Nullable<int> LDAPOption11CustomField4 { get; set; }
        public Nullable<int> LDAPOption11CustomField5 { get; set; }
        public Nullable<int> LDAPOption11CustomField6 { get; set; }
        public string pubicloginmessage { get; set; }
        public string CEUCreditLabel { get; set; }
        public Nullable<int> DisallowGradeEdit { get; set; }
        public Nullable<int> SignInSheetMultipleDate { get; set; }
        public string SetCharsetType { get; set; }
        public Nullable<int> useCASAuth { get; set; }
        public string CASAuthURL { get; set; }
        public string CASAuthFields { get; set; }
        public string LDAPServiceAccountUsername { get; set; }
        public string LDAPServiceAccountPassword { get; set; }
        public string LDAPServiceAccountContext { get; set; }
        public int LDAPUseServiceAccount { get; set; }
        public string LdapCustomFieldName1 { get; set; }
        public Nullable<int> LDAPOption11CustomField7 { get; set; }
        public string LDAPOption1CustomField1 { get; set; }
        public string LdapCustomFieldName11 { get; set; }
        public string accessbuildingpermission { get; set; }
        public string SubAdminTemplateStr { get; set; }
        public string LDAPCustomSearchObj { get; set; }
        public string ThankYouMessage { get; set; }
        public string CustomCourseFieldLabel5 { get; set; }
        public Nullable<int> CustomCourseFieldShow5 { get; set; }
        public Nullable<short> Sort5 { get; set; }
        public Nullable<int> CertificationsStudentReminderDaysBeforeExpired { get; set; }
        public Nullable<int> CertificationQualificationDateBasis { get; set; }
    }
}