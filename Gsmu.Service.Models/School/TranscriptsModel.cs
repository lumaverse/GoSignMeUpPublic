using Gsmu.Service.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.School
{
    public class TranscriptsModel
    {
        public int TranscriptID { get; set; }
        public int ? StudentId { get; set; }
        public string StudentsSchool { get; set; }
        public string District { get; set; }
        public string InstructorName { get; set; }
        public string InstructorName2 { get; set; }
        public string InstructorName3 { get; set; }
        public string GradeLevel { get; set; }
        public int ? CourseId { get; set; }
        public string CourseNum { get; set; }
        public string CourseName { get; set; }
        public string CourseLocation { get; set; }
        public string CourseDate { get; set; }
        public decimal ? DistPrice { get; set; }
        public decimal ? NoDistPrice { get; set; }
        public string Room { get; set; }
        public int ? Days { get; set; }
        public double ? CreditHours { get; set; }
        public string EventNum { get; set; }
        public string AccountNum { get; set; }
        public DateTime ? DateAdded { get; set; }
        public DateTime ? TimeAdded { get; set; }
        public short Attended { get; set; }
        public short DidntAttend { get; set; }
        public double ? Hours { get; set; }
        public string CourseCost { get; set; }
        public string PAYMETHOD { get; set; }
        public string payNumber { get; set; }
        public string CardExp { get; set; }
        public string AuthNum { get; set; }
        public string OrderNumber { get; set; }
        public string TotalPaid { get; set; }
        public string PaymentNotes { get; set; }
        public DateTime ? ReminderSent { get; set; }
        public short PaidInFull { get; set; }
        public string Position { get; set; }
        public string Job { get; set; }
        public DateTime ? Reminder2Sent { get; set; }
        public string StudentGrade { get; set; }
        public string PricingOption { get; set; }
        public short PricingMember { get; set; }
        public double InserviceHours { get; set; }
        public string CourseHoursType { get; set; }
        public short ?  UserEditedFlag { get; set; }
        public string CourseCategoryName { get; set; }
        public System.DateTime CourseCompletionDate { get; set; }
        public System.DateTime CourseStartDate { get; set; }
        public string Period { get; set; }
        public int LinkedTranscriptID { get; set; }
        public string AttendanceDetail { get; set; }
        public double ? CustomCreditHours { get; set; }
        public float ? graduatecredit { get; set; }
        public float ? CEUCredit { get; set; }
        public string AttendanceStatus { get; set; }
        public string OptionalCollectedInfo1 { get; set; }
        public int ? RefundedAmount { get; set; }
        public int ? RefundDue { get; set; }
        public float ? Optionalcredithours1 { get; set; }
        public int ? onlinecourse { get; set; }
        public DateTime ? datemodified { get; set; }
        public DateTime ? datetranscribed { get; set; }
        public int ? UserAddedFlag { get; set; }
        public DateTime ? DateAutoCertSent { get; set; }
        public string districtaddressinfo { get; set; }
        public string schooladdressinfo { get; set; }
        public string gradeaddressinfo { get; set; }
        public DateTime ? CertificateIssueDate { get; set; }
        public string districtaddressinfo2 { get; set; }
        public string schooladdressinfo2 { get; set; }
        public int ? studrosterid { get; set; }
        public int ? IsHoursPaid { get; set; }
        public double ? Optionalcredithours2 { get; set; }
        public double ? Optionalcredithours3 { get; set; }
        public double ? Optionalcredithours4 { get; set; }
        public double ? Optionalcredithours5 { get; set; }
        public double ? Optionalcredithours6 { get; set; }
        public double ? Optionalcredithours7 { get; set; }
        public double ? Optionalcredithours8 { get; set; }
        public string IsHoursPaidInfo { get; set; }

        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }

        public virtual CourseModel Cours { get; set; }
        public virtual StudentModel Student { get; set; }
    }
}
