using Gsmu.Service.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Portal
{
    public class AdminAttendanceTakingModel
    {
        public int RosterId { get; set; }
        public int IsTranscribed { get; set; }
        public int IsWaitListed { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int? AttendanceStatus { get; set; }
        public int? Parking { get; set; }
        public List<AttendanceDateList> AttendanceDateList { get; set; }

        public string Grade { get; set; }
        public string SelectedCreditType { get; set; }
        public string AdditionalCreditType { get; set; }
        public double? CreditHours { get; set; }
        public double? GraduateCredit { get; set; }
        public double? CustomCredit { get; set; }
        public double? Inservice { get; set; }
        public double? CEUCredit { get; set; }
        public double? OptionalCredit1 { get; set; }
        public double? OptionalCredit2 { get; set; }
        public double? OptionalCredit3 { get; set; }
        public double? OptionalCredit4 { get; set; }
        public double? OptionalCredit5 { get; set; }
        public double? OptionalCredit6 { get; set; }
        public double? OptionalCredit7 { get; set; }
        public double? OptionalCredit8 { get; set; }
    }

    public class AttendanceDateList
    {
        public string Coursedate { get; set; }
        public int IsAttended { get; set; }
        public float? AttendedHours { get; set; }

    }

    public class AttendanceCourseDetails
    {
        public CourseBasicDetails CourseBasicDetails { get; set; }
        public CreditInformationModel CreditInformationModel { get; set; }
        public FieldLabel FieldLabel { get; set; }
        public List<CourseDate> CourseDates {get;set;}
        public List<AttendanceStatus> AttendanceStatus { get; set; }
        public string HiddenCreditFields { get; set; }

    }
    public class FieldLabel
    {
        public string LabelCreditHours { get; set; }
        public string LabelGraduateCredit { get; set; }
        public string LabelCustomCredit { get; set; }
        public string LabelInservice{ get; set; }
        public string LabelCEUCredit { get; set; }
        public string LabelOptionalCredit1 { get; set; }
        public string LabelOptionalCredit2 { get; set; }
        public string LabelOptionalCredit3 { get; set; }
        public string LabelOptionalCredit4 { get; set; }
        public string LabelOptionalCredit5 { get; set; }
        public string LabelOptionalCredit6 { get; set; }
        public string LabelOptionalCredit7 { get; set; }
        public string LabelOptionalCredit8 { get; set; }
    }

    public class CourseDate
    {
        public string CourseDateItem { get; set; }
    }

    public class AttendanceStatus
    {
        public string Status { get; set; }
        public int Id { get; set; }

    }
    
}
