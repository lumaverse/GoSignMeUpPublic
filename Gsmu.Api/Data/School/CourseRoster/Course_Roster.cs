using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Gsmu.Api.Integration.Haiku;
using Gsmu.Api.Integration.Haiku.Responses.Entities;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Gsmu.Api.Data.School.CourseRoster;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class Course_Roster
    {
        public static List<Course_Roster> GetRosterExtraParticipantsAsRosterList(List<Course_Roster> rosters)
        {
            var extraParticipantRosters = new List<Course_Roster>();

            using (var db = new SchoolEntities())
            {
                foreach (var roster in rosters)
                {
                    var extra = (from cep in db.CourseExtraParticipants
                                 join cr in db.Course_Rosters on cep.RosterId equals cr.RosterID
                                 where cep.RosterId == roster.RosterID
                                 select new
                                 {
                                     cep = cep,
                                     cr = cr
                                 });
                    foreach (var neue in extra)
                    {
                        extraParticipantRosters.Add(Gsmu.Api.Data.School.Entities.Course_Roster.CreateExtraParticipantRoster(neue.cr, neue.cep));
                    }
                }
                return extraParticipantRosters;
            }
        }


        public static Course_Roster CreateExtraParticipantRoster(Course_Roster cr, CourseExtraParticipant cep)
        {
            return cr;
        }


        public static void SetDefaultRosterDates(Course_Roster cr)
        {
            string strDatetoday = DateTime.Now.ToShortDateString();
            string strTimetoday = DateTime.Now.ToShortTimeString();
            cr.DATEADDED = DateTime.Parse(strDatetoday);
            cr.TIMEADDED = DateTime.Parse(strTimetoday);
            cr.CertificateIssueDate = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.LastTimeSAPSync = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.bb_graded_date = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.attendancedate = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.date_grade_notification = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.datepostauth = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.date_attendance_notified = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.date_transcript_notified = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.heliuslms_date_integrated = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.Reminder3Sent = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.InvoiceDate = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.g2w_attendance_date = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.ExpiredDate = DateTime.Parse("1990-01-01 00:00:00.000");
            cr.laststudentreplacetime = DateTime.Parse("1/1/1990");
            cr.SAPBookingMade = DateTime.Parse("1/1/1990");
            cr.SAPBookingCancelled = DateTime.Parse("1/1/1990");

        }

        public Course_Roster(bool setDates)
            : this()
        {
            if (setDates)
            {
                Course_Roster.SetDefaultRosterDates(this);
            }
        }

        private Student student = null;
        private Course course = null;
        private List<rostermaterial> materials = null;

        private CourseExtraParticipant cep = null;

        public bool IsExtraParticipantRoser
        {
            get
            {
                return cep != null;
            }
        }

        public CourseExtraParticipant SingleCourseExtraParticipant
        {
            get
            {
                return this.cep;
            }
            set
            {
                this.cep = value;
            }
        }

        public bool IsWaiting
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.WAITING);
            }
        }

        public bool IsCancelled
        {
            get
            {
                return CancelStatus != RosterCancelStatus.Valid;
            }
        }

        public RosterCancelStatus CancelStatus
        {
            get
            {
                var value = Math.Abs(this.Cancel);
                return (RosterCancelStatus)value;
            }
            set
            {
                var cancel = (short)value;
                this.Cancel = cancel;
            }
        }

        public bool IsExpired
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.Expire);
            }
        }

        public bool IsValidForClass
        {
            get
            {
                return !IsExpired && !IsCancelled;
            }
        }

        public bool IsPaidInFull
        {
            get
            {
                return Settings.GetVbScriptBoolValue(this.PaidInFull);
            }
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public Entities.Student StudentOld
        {
            get
            {
                if (student == null || student.STUDENTID != this.STUDENTID)
                {
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        student = (from s in db.Students where s.STUDENTID == this.STUDENTID select s).First();
                    }
                }
                return student;
            }
            set
            {
                student = value;
            }
        }

        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public List<rostermaterial> Materials
        {
            get
            {
                if (this.materials == null)
                {
                    using (var db = new SchoolEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        this.materials = (from m in db.rostermaterials where m.RosterID == this.RosterID select m).ToList();
                    }
                }
                return this.materials;
            }
        }


        [XmlIgnore, ScriptIgnore, JsonIgnore]
        public Course Course
        {
            get
            {
                if (this.course == null)
                {
                    using (var db = new SchoolEntities())
                    {
                        this.course = (from c in db.Courses where c.COURSEID == this.COURSEID select c).First();
                    }
                }
                return this.course;
            }
            set
            {
                this.course = value;
            }
        }

        public decimal CourseCostDecimal
        {
            get
            {
                var parse = this.CourseCost == null ? "0" : this.CourseCost;
                return decimal.Parse(parse, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.Number);
            }
        }

        public decimal MaterialTotalCost
        {
            get
            {
                var total = (from m in this.Materials where m.priceincluded == 0 select m).Sum(m => m.price.HasValue ? m.price.Value : 0);
                return (decimal)total;
            }
        }

        public void CancelRoster(string cancelNumber = null)
        {
            this.CancelStatus = RosterCancelStatus.InvalidOrCancelled;
            this.CancelDate = DateTime.Now;
            if (string.IsNullOrEmpty(cancelNumber))
            {
                Random randNumber = new Random();
                this.CancelNumber = "CRW" + DateTime.Today.Month.ToString() + DateTime.Today.Date.ToString() + DateTime.Today.Second.ToString() + randNumber.Next(100, 1000).ToString();
            }
            else
            {
                this.CancelNumber = cancelNumber;
            }
        }

        public void ActivateRoster()
        {
            this.CancelStatus = RosterCancelStatus.Valid;
        }

        public bool IsPaid
        {
            get
            {
                //return PaidInFull == -1;
                return Settings.GetVbScriptBoolValue(this.PaidInFull);
            }
        }
        public string CourseSession { get; set; }
        public int EventId { get; set; }
    
    }
}
