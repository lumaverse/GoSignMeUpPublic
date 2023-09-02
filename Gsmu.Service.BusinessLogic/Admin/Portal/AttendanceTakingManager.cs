using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.Interface.Admin.Portal;
using Gsmu.Service.Models.Admin.Portal;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.Courses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.Admin.Portal
{
    public class AttendanceTakingManager : IAttendanceTakingManager
    {
        private ISchoolEntities _db; 
        private string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);
        public AttendanceTakingManager()
        {
            _db = new SchoolEntities(connString);
        }
        public List<AdminAttendanceTakingModel> GetRosterList(int courseId)
        {
            List<AdminAttendanceTakingModel> AdminAttendanceTakingModel = new List<AdminAttendanceTakingModel>();
            try
            {
                               var CourseRoster = (from roster in _db.Course_Rosters
                                  join student in _db.Students on roster.STUDENTID equals student.STUDENTID
                                  where roster.COURSEID == courseId && roster.Cancel==0 select  new AdminAttendanceTakingModel
                                  {
                                      RosterId = roster.RosterID,
                                      FirstName = student.FIRST,
                                      LastName = student.LAST,
                                      Grade = roster.StudentGrade,
                                      AttendanceStatus = roster.AttendanceStatusId,
                                      SelectedCreditType = roster.CreditApplied,
                                      CreditHours = roster.HOURS,
                                      Inservice = roster.InserviceHours,
                                      CustomCredit = roster.CustomCreditHours,
                                      CEUCredit = roster.ceucredit,
                                      GraduateCredit = roster.graduatecredit,
                                      OptionalCredit1 = roster.Optionalcredithours1,
                                      OptionalCredit2 = roster.Optionalcredithours2,
                                      OptionalCredit3 = roster.Optionalcredithours3,
                                      OptionalCredit4 = roster.Optionalcredithours4,
                                      OptionalCredit5 = roster.Optionalcredithours5,
                                      OptionalCredit6 = roster.Optionalcredithours6,
                                      OptionalCredit7 = roster.Optionalcredithours7,
                                      OptionalCredit8 = roster.Optionalcredithours8,
                                      IsWaitListed = roster.WAITING,
                                      Parking = roster.Parking
                                  }).ToList();


               var CourseTranscript = _db.Transcripts.Where(transcript => transcript.CourseId == courseId).ToList();
               var AttendanceDate = new AttendanceDateList();
               foreach (var _roster in CourseRoster)
               {
                   _roster.IsTranscribed = CourseTranscript.Where(tran => tran.studrosterid == _roster.RosterId).Count();
                   _roster.AttendanceDateList = new List<AttendanceDateList>();
                   

                   foreach (var _attendance in _db.AttendanceDetails.Where(attendnce => attendnce.RosterId == _roster.RosterId).OrderBy(_ordering => _ordering.CourseDate).ToList())
                   {
                       AttendanceDate.AttendedHours = _attendance.AttendedHours;
                       AttendanceDate.Coursedate = _attendance.CourseDate.ToString("d");
                       AttendanceDate.IsAttended = _attendance.Attended;
                       _roster.AttendanceDateList.Add(AttendanceDate);
                       AttendanceDate = new AttendanceDateList();  
                   }
                   



               }

               AdminAttendanceTakingModel = CourseRoster;
            }
            catch
            {
            }

            return AdminAttendanceTakingModel;
        }
        public AttendanceCourseDetails GetCourseDetails(int courseId)
        {
            AttendanceCourseDetails AttendanceCourseDetails = new AttendanceCourseDetails();
            AttendanceCourseDetails.CourseBasicDetails = new CourseBasicDetails();
            AttendanceCourseDetails.CreditInformationModel = new CreditInformationModel();
            AttendanceCourseDetails.FieldLabel = new FieldLabel();
            try
            {
                var masterInfo = _db.MasterInfoes.FirstOrDefault();
                var masterInfo2 = _db.MasterInfo2.FirstOrDefault();
                var masterInfo3 = _db.MasterInfo3.FirstOrDefault();
                var masterInfo4 = _db.masterinfo4.FirstOrDefault();
                var Course = _db.Courses.Where(course => course.COURSEID == courseId).FirstOrDefault();
                if (Course != null)
                {
                    AttendanceCourseDetails.CourseBasicDetails.CourseName = Course.COURSENAME;
                    AttendanceCourseDetails.CourseBasicDetails.CourseId = Course.COURSEID;
                    AttendanceCourseDetails.CourseBasicDetails.CourseNumber = Course.COURSENUM;
                    AttendanceCourseDetails.CreditInformationModel.CEUCredit = Course.CEUCredit;
                    AttendanceCourseDetails.CreditInformationModel.CreditHours = Course.CREDITHOURS;
                    AttendanceCourseDetails.CreditInformationModel.CustomCreditHours = Course.CustomCreditHours;
                    AttendanceCourseDetails.CreditInformationModel.GraduateCredit = Course.GraduateCredit;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours1 = Course.Optionalcredithours1;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours2 = Course.Optionalcredithours2;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours3 = Course.Optionalcredithours3;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours4 = Course.Optionalcredithours4;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours5 = Course.Optionalcredithours5;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours6 = Course.Optionalcredithours6;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours7 = Course.Optionalcredithours7;
                    AttendanceCourseDetails.CreditInformationModel.Optionalcredithours8 = Course.Optionalcredithours8;
                    AttendanceCourseDetails.CourseDates = new List<CourseDate>();
                    CourseDate _CourseDate = new CourseDate();
                    foreach (var date in _db.Course_Times.Where(_date => _date.COURSEID == courseId).OrderBy(_ordering => _ordering.COURSEDATE).ToList())
                    {
                        _CourseDate.CourseDateItem = date.COURSEDATE.Value.ToString("d");
                        if (AttendanceCourseDetails.CourseDates.Where(_listeddate => _listeddate.CourseDateItem == _CourseDate.CourseDateItem).Count() == 0)
                        {
                            AttendanceCourseDetails.CourseDates.Add(_CourseDate);
                        }
                        _CourseDate = new CourseDate();
                    }
                    AttendanceCourseDetails.AttendanceStatus = new List<AttendanceStatus>();
                    AttendanceStatus _AttendanceStatus = new AttendanceStatus();
                    foreach (var stat in _db.AttendanceStatus.OrderBy(_ordering => _ordering.AttendanceStatusId).ToList())
                    {
                        _AttendanceStatus.Id = stat.AttendanceStatusId;
                        _AttendanceStatus.Status = stat.AttendanceStatus;
                        AttendanceCourseDetails.AttendanceStatus.Add(_AttendanceStatus);
                        _AttendanceStatus = new AttendanceStatus();
                    }
                    AttendanceCourseDetails.FieldLabel.LabelCreditHours = masterInfo2.CreditHoursName;
                    AttendanceCourseDetails.FieldLabel.LabelCustomCredit = masterInfo2.CustomCreditTypeName;
                    AttendanceCourseDetails.FieldLabel.LabelCEUCredit = masterInfo2.CEUCreditLabel;
                    AttendanceCourseDetails.FieldLabel.LabelInservice = masterInfo2.InserviceHoursName;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit1 = masterInfo3.OptionalcredithoursLabel1;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit2 = masterInfo3.OptionalcredithoursLabel2;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit3 = masterInfo3.OptionalcredithoursLabel3;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit4 = masterInfo3.OptionalcredithoursLabel4;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit5 = masterInfo3.OptionalcredithoursLabel5;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit6 = masterInfo3.OptionalcredithoursLabel6;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit7 = masterInfo3.OptionalcredithoursLabel7;
                    AttendanceCourseDetails.FieldLabel.LabelOptionalCredit8 = masterInfo3.OptionalcredithoursLabel8;
                    AttendanceCourseDetails.HiddenCreditFields = "";

                    if (masterInfo.DontDisplayCreditHours == -1)
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "credithours,";
                    }
                    if (masterInfo2.ShowCustomCreditType == 0)
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "customcredit,";
                    }
                    if (masterInfo.ShowInservice == 0)
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "inservice,";
                    }
                    if (masterInfo2.ShowCEUandGraduateCreditCourses == 0)
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "ceu,";
                    }
                    if (masterInfo3.OptionalCredithoursvisible1 == 0)
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "optionalcredits,";
                    }
                    if (masterInfo2.Parking == 0)
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "parking,";
                    }

                    if ((Course.CustomDropDownValueSequence != "") && (Course.CustomDropDownValueSequence != null))
                    {
                        if ((!Course.CustomDropDownValueSequence.Contains("CreditHours13")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("CreditHours13")))
                        {
                            AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "credithours,";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("OptionalCredit11")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("OptionalCredit11")))
                        {
                            AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "customcredit,";
                        }

                        if ((!Course.CustomDropDownValueSequence.Contains("CEUCredit9")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("CEUCredit9")))
                        {
                            AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "ceu,";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("Inservice12")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("Inservice12")))
                        {
                            AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "inservice,";
                        }


                        if ((!Course.CustomDropDownValueSequence.Contains("customoption1")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption1")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit1 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption2")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption2")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit2 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption3")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption3")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit3 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption4")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption4")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit4 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption5")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption5")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit5 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption6")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption6")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit6 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption7")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption7")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit7 = "";
                        }
                        if ((!Course.CustomDropDownValueSequence.Contains("customoption8")) && (!AttendanceCourseDetails.HiddenCreditFields.Contains("customoption8")))
                        {
                            AttendanceCourseDetails.FieldLabel.LabelOptionalCredit8 = "";
                        }
                    }
                    else
                    {
                        AttendanceCourseDetails.HiddenCreditFields = AttendanceCourseDetails.HiddenCreditFields + "optionalcredits,";
                    }
                }
            }
            catch
            {
            }

            return AttendanceCourseDetails;
        }
        public void SaveDateAttendance(int rosterid, string date, int status)
        {
            using (var db = new SchoolEntities(connString))
            {
                DateTime coursedate = DateTime.Parse(date);
                var CourseRoster = (from roster in db.Course_Rosters where roster.RosterID == rosterid select roster).FirstOrDefault();
                if (CourseRoster != null)
                {
                    var _attendancedetails = db.AttendanceDetails.Where(attendnce => attendnce.RosterId == CourseRoster.RosterID && attendnce.CourseID == CourseRoster.COURSEID &&  DbFunctions.TruncateTime(attendnce.CourseDate) == coursedate).FirstOrDefault();
                    if (_attendancedetails != null)
                    {
                        _attendancedetails.Attended = status;
                    }
                    else
                    {
                        AttendanceDetail attendancedetail = new AttendanceDetail();
                        attendancedetail.RosterId = CourseRoster.RosterID;
                        attendancedetail.CourseID = CourseRoster.COURSEID.Value;
                        attendancedetail.CourseDate = coursedate;
                        attendancedetail.Attended = status;

                        db.AttendanceDetails.Add(attendancedetail);
                    }
                    db.SaveChanges();
                }

            }
        }
        public void SaveStatusAttendance(int rosterid, int status)
        {
            using (var db = new SchoolEntities(connString))
            {
                var CourseRoster = (from roster in db.Course_Rosters where roster.RosterID == rosterid select roster).FirstOrDefault();
                if (CourseRoster != null)
                {
                    CourseRoster.AttendanceStatusId = status;
                    db.SaveChanges();
                }
            }
        }
        public void SaveAttendanceGrade(int rosterid, string grade)
        {
            using (var db = new SchoolEntities(connString))
            {
                var CourseRoster = (from roster in db.Course_Rosters where roster.RosterID == rosterid select roster).FirstOrDefault();
                if (CourseRoster != null)
                {
                    CourseRoster.StudentGrade = grade;
                    db.SaveChanges();
                }
            }
        }
        public void SaveAttendanceCredit(int rosterid, string type, int value)
        {
            using (var db = new SchoolEntities(connString))
            {
                var CourseRoster = (from roster in db.Course_Rosters where roster.RosterID == rosterid select roster).FirstOrDefault();
                if (CourseRoster != null)
                {
                    if (type == "custom")
                    {
                        CourseRoster.CustomCreditHours = value;
                    }
                    else if (type == "inservice")
                    {
                        CourseRoster.InserviceHours = value;
                    }
                    else if (type == "graduate")
                    {
                        CourseRoster.graduatecredit = value;
                    }
                    else if (type == "ceu")
                    {
                        CourseRoster.ceucredit = value;
                    }
                    else if (type == "opt1")
                    {
                        CourseRoster.Optionalcredithours1 = value;
                    }
                    else if (type == "opt2")
                    {
                        CourseRoster.Optionalcredithours2 = value;
                    }
                    else if (type == "opt3")
                    {
                        CourseRoster.Optionalcredithours3 = value;
                    }
                    else if (type == "opt4")
                    {
                        CourseRoster.Optionalcredithours4 = value;
                    }
                    else if (type == "opt5")
                    {
                        CourseRoster.Optionalcredithours5 = value;
                    }
                    else if (type == "opt6")
                    {
                        CourseRoster.Optionalcredithours6 = value;
                    }
                    else if (type == "opt7")
                    {
                        CourseRoster.Optionalcredithours7 = value;
                    }
                    else if (type == "opt8")
                    {
                        CourseRoster.Optionalcredithours8 = value;
                    }
                    db.SaveChanges();
                }
            }
        }

        public void TranscribeSingleStudent(int rosterid)
        {
            var CourseRoster = (from roster in _db.Course_Rosters where roster.RosterID == rosterid select roster).FirstOrDefault();
            var transcripted = (from trans in _db.Transcripts where trans.studrosterid == rosterid select trans).Count();
            if (CourseRoster != null && transcripted==0)
            {
                var coursedetails = (from course in _db.Courses where course.COURSEID == CourseRoster.COURSEID select course).FirstOrDefault();
                TranscribeStudent(rosterid, coursedetails, CourseRoster);
            }
        }
        public void TranscribeAllStudents(int courseid)
        {
            var CourseRoster = (from roster in _db.Course_Rosters where roster.COURSEID == courseid select roster).ToList();
            var coursedetails = (from course in _db.Courses where course.COURSEID == courseid  select course).FirstOrDefault();
            foreach( var roster in CourseRoster)
            {
                if ((from trans in _db.Transcripts where trans.studrosterid == roster.RosterID select trans).Count() == 0)
                {
                    TranscribeStudent(roster.RosterID, coursedetails, roster);
                }
            }
        }
        private void TranscribeStudent(int rosterid, Course coursedetails, Course_Roster CourseRoster)
        {
            using (var db = new SchoolEntities(connString))
            {
                if (CourseRoster != null)
                {
                    var studentdetails = (from student in _db.Students where student.STUDENTID == CourseRoster.STUDENTID select student).FirstOrDefault();
                    if( studentdetails!=null && coursedetails!=null){
                    Transcript transcript = new Transcript();
                    transcript.STUDENTID = CourseRoster.STUDENTID;
                    transcript.CourseId = CourseRoster.COURSEID;
                   // transcript.StudentsSchool = studentdetails.SCHOOL;
                    transcript.CourseNum = coursedetails.COURSENUM;
                    transcript.CourseName = coursedetails.COURSENAME;
                    transcript.CourseLocation = coursedetails.LOCATION;
                    transcript.DistPrice = coursedetails.DISTPRICE;
                    transcript.AccountNum = coursedetails.ACCOUNTNUM;
                    transcript.AttendanceDetail = ""; //to follow
                    transcript.AttendanceStatus = "";//to follow
                    transcript.ATTENDED = 0; //to follow
                    transcript.AuthNum = CourseRoster.AuthNum;
                    transcript.CardExp = CourseRoster.CardExp;
                    transcript.CertificateIssueDate = CourseRoster.CertificateIssueDate;
                    transcript.ceucredit = CourseRoster.ceucredit;
                    transcript.CourseCategoryName = coursedetails.MAINCATEGORY;
                    transcript.CourseCompletionDate = DateTime.Now; //to follow
                    transcript.CourseCost = CourseRoster.CourseCost;
                    transcript.CourseDate = ""; //to follow
                    transcript.CourseHoursType = CourseRoster.CourseHoursType;
                    transcript.CourseStartDate = DateTime.Now; //to follow
                    transcript.CreditHours = CourseRoster.HOURS;
                    transcript.CustomCreditHours = CourseRoster.CustomCreditHours;
                    transcript.DATEADDED = DateTime.Now;
                    transcript.DateAutoCertSent = CourseRoster.CertificateIssueDate;
                    transcript.datemodified = DateTime.Now;
                    transcript.datetranscribed = DateTime.Now;
                    transcript.Days = coursedetails.DAYS;
                    transcript.DIDNTATTEND = 0; //to follow
                    transcript.District = ""; //to follow
                    transcript.districtaddressinfo =""; //to follow
                    transcript.districtaddressinfo2 =""; //to follow
                    transcript.EventNum = CourseRoster.EventId.ToString();
                    transcript.gradeaddressinfo = ""; // to follow
                    transcript.GradeLevel = ""; //to follow
                    transcript.graduatecredit = CourseRoster.graduatecredit;
                    transcript.HOURS = CourseRoster.HOURS;
                        transcript.InserviceHours = (CourseRoster.InserviceHours==null? 0 : CourseRoster.InserviceHours.Value);
                        transcript.InstructorName = ""; //to follow
                        transcript.IsHoursPaid = 0;
                        transcript.IsHoursPaidInfo = "";
                        transcript.Job = CourseRoster.Job;
                        transcript.LinkedTranscriptID = 0;
                        transcript.NoDistPrice = coursedetails.NODISTPRICE;
                        transcript.onlinecourse = coursedetails.OnlineCourse;
                        transcript.OptionalCollectedInfo1 = CourseRoster.OptionalCollectedInfo1;
                        transcript.Optionalcredithours1 = CourseRoster.Optionalcredithours1;
                        transcript.Optionalcredithours2 = CourseRoster.Optionalcredithours2;
                        transcript.Optionalcredithours3 = CourseRoster.Optionalcredithours3;
                        transcript.Optionalcredithours4 = CourseRoster.Optionalcredithours4;
                        transcript.Optionalcredithours5 = CourseRoster.Optionalcredithours5;
                        transcript.Optionalcredithours6 = CourseRoster.Optionalcredithours6;
                        transcript.Optionalcredithours7 = CourseRoster.Optionalcredithours7;
                        transcript.Optionalcredithours8 = CourseRoster.Optionalcredithours8;
                        transcript.OrderNumber = CourseRoster.OrderNumber;
                        transcript.PaidInFull = CourseRoster.PaidInFull;
                        transcript.PaymentNotes = CourseRoster.PaymentNotes;
                        transcript.PAYMETHOD = CourseRoster.PAYMETHOD;
                        transcript.payNumber = CourseRoster.payNumber;
                        transcript.Period = ""; // to follow
                        transcript.Position = CourseRoster.Position;
                        transcript.PricingMember = CourseRoster.PricingMember;
                        transcript.PricingOption = CourseRoster.PricingOption;
                        transcript.RefundDue = 0;
                        transcript.RefundedAmount = 0;
                        transcript.Reminder2Sent = CourseRoster.Reminder2Sent;
                        transcript.ReminderSent = CourseRoster.ReminderSent;
                        transcript.Room = coursedetails.ROOM;
                        transcript.schooladdressinfo = ""; //to follow
                        transcript.schooladdressinfo2 = ""; //to follow 2
                        transcript.StudentGrade = CourseRoster.StudentGrade;
                        transcript.STUDENTID = studentdetails.STUDENTID;
                        transcript.StudentsSchool = "";
                        transcript.studrosterid = CourseRoster.RosterID;
                        transcript.TIMEADDED = DateTime.Now;
                        transcript.TotalPaid = CourseRoster.TotalPaid==null?"0": CourseRoster.TotalPaid.Value.ToString();
                        transcript.UserAddedFlag = 0;
                        transcript.UserEditedFlag = 0;

                        db.Transcripts.Add(transcript);



                    db.SaveChanges();
                    }
                }
            }
        }
    }
}
