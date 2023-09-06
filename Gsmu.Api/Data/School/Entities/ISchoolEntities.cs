using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial interface ISchoolEntities : IDisposable
    {
        DbSet<AdHocReportTemplate> AdHocReportTemplates { get; set; }
        DbSet<adminpass> adminpasses { get; set; }
        DbSet<Attendance> Attendances { get; set; }
        DbSet<AttendanceDetail> AttendanceDetails { get; set; }
        DbSet<AttendanceStatu> AttendanceStatus { get; set; }
        DbSet<Audience> Audiences { get; set; }
        DbSet<AuditTrail> AuditTrails { get; set; }
        DbSet<BlackboardLog> BlackboardLogs { get; set; }
        DbSet<BlackboardServersInfo> BlackboardServersInfoes { get; set; }
        DbSet<BlackboardUserRole> BlackboardUserRoles { get; set; }
        DbSet<Certification> Certifications { get; set; }
        DbSet<CertificationsCourse> CertificationsCourses { get; set; }
        DbSet<CertificationsStudent> CertificationsStudents { get; set; }
        DbSet<CertificationsStudentCompleted> CertificationsStudentCompleteds { get; set; }
        DbSet<CitrixLog> CitrixLogs { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<Coupon> Coupons { get; set; }
        DbSet<Course_Roster> Course_Rosters { get; set; }
        DbSet<Course_Time> Course_Times { get; set; }
        DbSet<Course_Times_Menu> Course_Times_Menus { get; set; }
        DbSet<coursearchive> coursearchives { get; set; }
        DbSet<CourseCategory> CourseCategories { get; set; }
        DbSet<CourseChoice> CourseChoices { get; set; }
        DbSet<CourseExpense> CourseExpenses { get; set; }
        DbSet<CourseOutline> CourseOutlines { get; set; }
        DbSet<CoursePricingOption> CoursePricingOptions { get; set; }
        DbSet<Course> Courses { get; set; }
        DbSet<CoursesPendingRequest> CoursesPendingRequests { get; set; }
        DbSet<CoursesRequirement> CoursesRequirements { get; set; }
        DbSet<CourseSurveyNIU> CourseSurveyNIUs { get; set; }
        DbSet<customcetificate> customcetificates { get; set; }
        DbSet<customtranscript> customtranscripts { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<District> Districts { get; set; }
        DbSet<EmailAuditTrail> EmailAuditTrails { get; set; }
        DbSet<EmailList> EmailLists { get; set; }
        DbSet<EmployeeType> EmployeeTypes { get; set; }
        DbSet<FieldMask> FieldMasks { get; set; }
        DbSet<FieldSpec> FieldSpecs { get; set; }
        DbSet<Grade_Level> Grade_Levels { get; set; }
        DbSet<HeliusLMSlog> HeliusLMSlogs { get; set; }
        DbSet<Icon> Icons { get; set; }
        DbSet<InstructorRate> InstructorRates { get; set; }
        DbSet<Instructor> Instructors { get; set; }
        DbSet<licenseinfo> licenseinfoes { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Manager> Managers { get; set; }
        DbSet<MasterInfo2> MasterInfo2 { get; set; }
        DbSet<MasterInfo3> MasterInfo3 { get; set; }
        DbSet<masterinfo4> masterinfo4 { get; set; }
        DbSet<Material> Materials { get; set; }
        DbSet<NavLink> NavLinks { get; set; }
        DbSet<NEWEMP> NEWEMPS { get; set; }
        DbSet<OptionalInfo> OptionalInfoes { get; set; }
        DbSet<OrderInProgress> OrderInProgresses { get; set; }
        DbSet<OrderTransaction> OrderTransactions { get; set; }
        DbSet<ParentLevelOne> ParentLevelOnes { get; set; }
        DbSet<ParentLevelThree> ParentLevelThrees { get; set; }
        DbSet<ParentLevelTwo> ParentLevelTwoes { get; set; }
        DbSet<ParentLevelTwoToThreeRelated> ParentLevelTwoToThreeRelateds { get; set; }
        DbSet<Parent> Parents { get; set; }
        DbSet<Payment_Option> Payment_Options { get; set; }
        DbSet<PDFHeaderFooterInfo> PDFHeaderFooterInfoes { get; set; }
        DbSet<PricingOption> PricingOptions { get; set; }
        DbSet<ReportRequest> ReportRequests { get; set; }
        DbSet<RoomDirection> RoomDirections { get; set; }
        DbSet<RoommateRequest> RoommateRequests { get; set; }
        DbSet<RoomNumber> RoomNumbers { get; set; }
        DbSet<rostermaterial> rostermaterials { get; set; }
        DbSet<SalesTax> SalesTaxes { get; set; }
        DbSet<SAPIntegration> SAPIntegrations { get; set; }
        DbSet<SAPIntegrationFunction> SAPIntegrationFunctions { get; set; }
        DbSet<SchoolExtraInfo> SchoolExtraInfoes { get; set; }
        DbSet<School> Schools { get; set; }
        DbSet<SchoolsGradeLevelsRelated> SchoolsGradeLevelsRelateds { get; set; }
        DbSet<StudentMedicalInfo> StudentMedicalInfoes { get; set; }
        DbSet<StudentMergeHistory> StudentMergeHistories { get; set; }
        DbSet<StudentQuestionnaire> StudentQuestionnaires { get; set; }
        DbSet<StudentRate> StudentRates { get; set; }
        DbSet<StudentRegSortOrder> StudentRegSortOrders { get; set; }
        DbSet<Student> Students { get; set; }
        DbSet<StudentsGradeLog> StudentsGradeLogs { get; set; }
        DbSet<StudentValidation> StudentValidations { get; set; }
        DbSet<SubAdminToSubSite> SubAdminToSubSites { get; set; }
        DbSet<SubSite> SubSites { get; set; }
        DbSet<Supervisor> Supervisors { get; set; }
        DbSet<SupervisorSchool> SupervisorSchools { get; set; }
        DbSet<Terminology> Terminologies { get; set; }
        DbSet<transcriptimport> transcriptimports { get; set; }
        DbSet<Transcript> Transcripts { get; set; }
        DbSet<UpdateTracking> UpdateTrackings { get; set; }
        DbSet<uploadedfile> uploadedfiles { get; set; }
        DbSet<customimptemp> customimptemps { get; set; }
        DbSet<DistrictExtraInfo> DistrictExtraInfoes { get; set; }
        DbSet<EmailQueue> EmailQueues { get; set; }
        DbSet<GradeExtraInfo> GradeExtraInfoes { get; set; }
        DbSet<UserBBDateInformation> UserBBDateInformations { get; set; }
        DbSet<WebServiceSetting> WebServiceSettings { get; set; }
        DbSet<FastTrackCours> FastTrackCourses { get; set; }
        DbSet<MasterInfo> MasterInfoes { get; set; }
        DbSet<MainCategory> MainCategories { get; set; }
        DbSet<SubCategory> SubCategories { get; set; }
        DbSet<SubSubCategory> SubSubCategories { get; set; }
        DbSet<CourseExtraParticipant> CourseExtraParticipants { get; set; }
        DbSet<SupervisorStudent> SupervisorStudents { get; set; }
        DbSet<Membership> Memberships { get; set; }
        DbSet<Membership_Roster> Membership_Rosters { get; set; }
        DbSet<ReviewOrderView> ReviewOrderViews { get; set; }
        DbSet<RosterReportView> RosterReportViews { get; set; }
        DbSet<CoursesListView> CoursesListViews { get; set; }
     //   DbSet<UserSession> UserSessions { get; set; }

    }
}
