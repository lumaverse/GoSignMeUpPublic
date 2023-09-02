using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.School
{
    public class RosterModel : CourseRosterModel
    {
        public string Status { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
    public class CourseRosterModel
    {
        //TABLE RELATED PROPERTIES
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseNumber { get; set; }

        public string CancelCourse { get; set; }
        public string CourseNameId { get; set; }
        public string CourseDateId { get; set; }

        public string RosterId { get; set; }

        public string Location { get; set; }
        public string InternalClass { get; set; }
        public string MaxEnroll { get; set; }
        public string MaxWait { get; set; }
        public string Room { get; set; }
        public string AccountNumer{ get; set; }
        public string InstructorId1 { get; set; }
        public string InstructorId2 { get; set; }
        public string InstructorId3 { get; set; }
        public string Materials { get; set; }
        public string Days { get; set; }
        public string CreditHours { get; set; }
        public string CourseCloseDays { get; set; }
        public string Description { get; set; }
        public string StudentId { get; set; }
        public string Waiting { get; set; }
        public string InternalNote { get; set; }
        public string EnrollmentNote { get; set; }
        public string OrderNumber { get; set; }
        public string PayMethod { get; set; }
        public string PayNumber { get; set; }
        public string AuthNumber { get; set; }
        public string RefNumber { get; set; }
        public string CouponCode { get; set; }
        public string CouponDiscount { get; set; }
        public string CouponDetails { get; set; }
        public string StudentGrade { get; set; }
        public string PaidInFull { get; set; }
        public string AmountPaid { get; set; }
        
        public string CreditApplied { get; set; }
        public string Cancel { get; set; }
        public string Attended { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string DateAdded { get; set; }
        public string CriInitialAuditInfo { get; set; }
        public string CourseChoice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Homephone { get; set; }
        public string WorPphone { get; set; }
        public string Fax { get; set; }


        public string StudentSchool { get; set; }
        public string District { get; set; }
        public string StudentGradeLevel { get; set; }
       
        public string Instructor { get; set; }
        public string Instructor2 { get; set; }
        public string Instructor3 { get; set; }
        public string CancelledText { get; set; }
        public string WaitingText { get; set; }
        public string AttendedText { get; set; }
        public string PaidFullText { get; set; }
        public string Credited { get; set; }
        public string RMCount { get; set; }
        public string MaterialNames { get; set; }
        public string Material { get; set; }
        public string CourseTotal { get; set; }
        public string TexTotal { get; set; }
        public string CompleteAddress { get; set; }
        public string CourseLocation { get; set; }
        public string StartDate { get; set; }

        public int Count { get; set; }
        public int RowNumber { get; set; }

    }
}
