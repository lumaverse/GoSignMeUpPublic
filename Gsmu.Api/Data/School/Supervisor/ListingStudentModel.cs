using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Supervisor
{
    public class ListingStudentTranscript
    {
        public int StudentId
        {
            get;
            set;
        }

        public string CourseNumber
        {
            get;
            set;
        }
        public string CourseName
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime CompletionDate
        {
            get;
            set;
        }

        public string CompletionDate_string
        {
            get;
            set;
        }
        public string StartDate_string
        {
            get;
            set;
        }
        public string Grade
        {
            get;
            set;
        }
    }
    public class ListingStudentModel
    {
        public int StudentId
        {
            get;
            set;
        }
        public string StudentFirstName
        {
            get;
            set;
        }
        public string StudentLastName
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }

        public int Enrolled
        {
            get;
            set;
        }
        public int Complete
        {
            get;
            set;
        }

        public int inCheckout
        {
            get;
            set;
        }
        public int Isenrolled
        {
        get;
        set;

        }

        public int District
        {
            get;
            set;
        }
        public int School
        {
            get;
            set;
        }
        public int AssignSup2StudList
        {
            get;
            set;
        }
        public int SupStudSchool
        {
            get;
            set;
        }

        public int AvailableSeats
        {
            get;
            set;
        }

        public int InActive
        {
            get;
            set;
        }
        public bool IsErroriNRequirements
        {
            get;
            set;
        }
        public int Waiting { get; set; }
        public int HasBalance { get; set; }
        public int CreatedBy { get; set; }
    }

    public class WaitListingStudentModel : ListingStudentModel
    {
        //COURSE
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseNumber { get; set; }
        public int MaxEnroll { get; set; }
        public int MaxWait { get; set; }
        public DateTime CourseStartDate { get; set; }
        //ROSTER
        public int RosterId { get; set; }
        public DateTime? DateAdded { get; set; }

        public int EnrolledCount { get; set; }
        public int WaitingCount { get; set; }
        public int RemainingSlots { get; set; }
        public int RemainingWaitSlots { get; set; }
        public string EnrollToWaitListConfig { get; set; }
        public int Cancellable { get; set; }
    }
}
