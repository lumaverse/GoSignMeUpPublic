using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Networking.Mail;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Data.School.Transcripts
{
    public class Transcripts
    {
        public void TranscribeStudent(int studentid, int courseId)
        {

            using (var db = new SchoolEntities())
            {
                var roster_data = (from roster in db.Course_Rosters where
                              roster.Cancel ==0
                              && roster.COURSEID == courseId
                              && roster.STUDENTID == studentid select roster).FirstOrDefault();

                if (roster_data != null)
                {
                    var transcript_data = (from transcipt in db.Transcripts
                                           where
                                               transcipt.STUDENTID == studentid
                                               && transcipt.CourseId == courseId
                                           select transcipt).FirstOrDefault();
                    if (transcript_data == null)
                    {
                        var course = (from c in db.Courses where c.COURSEID == courseId select c).FirstOrDefault();
                        
                        Transcript New_transcript = new Transcript();
                        New_transcript.datetranscribed = DateTime.Now;
                        New_transcript.datemodified = DateTime.Now;
                        New_transcript.STUDENTID = studentid;
                        try
                        {
                            New_transcript.StudentsSchool = StudentHelper.GetStudent(studentid).SCHOOL.ToString();
                            New_transcript.District = StudentHelper.GetStudent(studentid).DISTRICT.ToString();
                            New_transcript.GradeLevel = StudentHelper.GetStudent(studentid).GRADE.ToString();
                        }
                        catch { }
                        try
                        {
                            New_transcript.InstructorName = InstructorHelper.InstructorHelper.GetInstructor(course.INSTRUCTORID.Value).FIRST + " " + InstructorHelper.InstructorHelper.GetInstructor(course.INSTRUCTORID.Value).LAST;
                            New_transcript.InstructorName2 = InstructorHelper.InstructorHelper.GetInstructor(course.INSTRUCTORID2.Value).FIRST + " " + InstructorHelper.InstructorHelper.GetInstructor(course.INSTRUCTORID2.Value).LAST; ;
                            New_transcript.InstructorName3 = InstructorHelper.InstructorHelper.GetInstructor(course.INSTRUCTORID3.Value).FIRST + " " + InstructorHelper.InstructorHelper.GetInstructor(course.INSTRUCTORID3.Value).LAST; ;
                        }
                        catch{}
                        
                        try
                        {
                            string strAttendanceDetails = "";
                            
                            var attendancedetals = (from Atte in db.AttendanceDetails
                                                    where Atte.CourseID == courseId
                                                    && Atte.RosterId == roster_data.RosterID orderby Atte.CourseDate
                                                    select Atte).ToList();

                            foreach(var attendance in attendancedetals)
                            {
                                New_transcript.CourseStartDate = attendance.CourseDate;
                                strAttendanceDetails = attendance.CourseDate + "|" + attendance.Attended+ ",";

                            }
                            New_transcript.AttendanceDetail = strAttendanceDetails.Remove(strAttendanceDetails.Length - 1); ;
                            New_transcript.CourseCompletionDate = DateTime.Now;

                        }
                        catch
                        {
                        }
                        New_transcript.CourseId = courseId;
                        New_transcript.CourseNum =course.COURSENUM  ;
                        New_transcript.CourseName = course.COURSENAME;
                        New_transcript.onlinecourse = course.OnlineCourse;
                        New_transcript.CourseLocation = course.LOCATION;
                        New_transcript.DistPrice = course.DISTPRICE;
                        New_transcript.NoDistPrice = course.NODISTPRICE;
                        New_transcript.Room = course.ROOM;
                        New_transcript.Days = course.DAYS;
                        New_transcript.EventNum = course.EVENTNUM;
                        New_transcript.AccountNum = course.ACCOUNTNUM;
                        New_transcript.DATEADDED = roster_data.DATEADDED;
                        New_transcript.TIMEADDED = roster_data.TIMEADDED;
                        try
                        {
                            New_transcript.AttendanceStatus = roster_data.AttendanceStatusId.ToString();
                            New_transcript.TotalPaid = roster_data.TotalPaid.ToString();
                        }
                        catch { }
                        New_transcript.ATTENDED = roster_data.ATTENDED;
                        New_transcript.DIDNTATTEND = roster_data.DIDNTATTEND;
                        New_transcript.CourseCost = roster_data.CourseCost;
                        New_transcript.PAYMETHOD = roster_data.PAYMETHOD;
                        New_transcript.payNumber = roster_data.payNumber;
                        New_transcript.CardExp = roster_data.CardExp;
                        New_transcript.AuthNum = roster_data.AuthNum;
                        New_transcript.OrderNumber = roster_data.OrderNumber;
                        
                        New_transcript.PaymentNotes = roster_data.PaymentNotes;
                        New_transcript.Reminder2Sent = roster_data.Reminder2Sent;
                        New_transcript.ReminderSent = roster_data.ReminderSent;
                        New_transcript.PaidInFull = roster_data.PaidInFull;
                        New_transcript.Position = roster_data.Position;
                        New_transcript.Job = roster_data.Job;
                        New_transcript.StudentGrade = roster_data.StudentGrade;
                        New_transcript.PricingOption = roster_data.PricingOption;
                        New_transcript.PricingMember = roster_data.PricingMember;
                        New_transcript.CustomCreditHours = roster_data.CustomCreditHours;
                        New_transcript.InserviceHours = roster_data.InserviceHours.Value;
                        New_transcript.CourseHoursType = roster_data.CourseHoursType;
                        New_transcript.ceucredit=roster_data.ceucredit;
                        New_transcript.graduatecredit= roster_data.graduatecredit;

                        New_transcript.Optionalcredithours1 = roster_data.Optionalcredithours1;
                        try
                        {
                            New_transcript.Optionalcredithours2 = float.Parse(roster_data.Optionalcredithours2.ToString());
                            New_transcript.Optionalcredithours3 = float.Parse(roster_data.Optionalcredithours3.ToString());
                            New_transcript.Optionalcredithours4 = float.Parse(roster_data.Optionalcredithours4.ToString());
                            New_transcript.Optionalcredithours5 = float.Parse(roster_data.Optionalcredithours5.ToString());
                            New_transcript.Optionalcredithours6 = float.Parse(roster_data.Optionalcredithours6.ToString());
                            New_transcript.Optionalcredithours7 = float.Parse(roster_data.Optionalcredithours7.ToString());
                            New_transcript.Optionalcredithours8 = float.Parse(roster_data.Optionalcredithours8.ToString());
                        }
                        catch { }

                        New_transcript.HOURS = roster_data.HOURS;
                        New_transcript.CreditHours = roster_data.CustomCreditHours;
                        New_transcript.OptionalCollectedInfo1 = roster_data.OptionalCollectedInfo1;
                        New_transcript.LinkedTranscriptID  =0;

                        db.Transcripts.Add(New_transcript);
                        db.SaveChanges();




                    }
                }
            }
        }
        public Transcript StudentTranscriptedCourse(int studentid,int courseId)
        {

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var transcript = (from tran in db.Transcripts
                                  join c in db.Courses on tran.CourseId equals c.COURSEID
                                  join cr in db.Course_Rosters on tran.STUDENTID equals cr.STUDENTID

                                  where tran.STUDENTID.Value == studentid
                                  && cr.Cancel == 0
                                  && cr.COURSEID == courseId
                                  && c.CANCELCOURSE == 0
                                  && tran.CourseId == courseId
                                  
                                  select tran).FirstOrDefault();
                return transcript;
            }
        }
        public Course_Roster StudentTranscriptedRoster(int studentid, int courseId)
        {

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var transcript = (from  c in db.Courses 
                                  join cr in db.Course_Rosters on c.COURSEID equals cr.COURSEID

                                  where cr.STUDENTID.Value == studentid
                                  && cr.Cancel == 0
                                  && cr.COURSEID == courseId
                                  && c.CANCELCOURSE == 0

                                  select cr).FirstOrDefault();

                return transcript;
            }
        }
        public List<Transcript> StudentTranscriptedCourse(int studentid, DateTime? start=null, DateTime? end=null)
        {

            using (var db = new SchoolEntities())
            {
                var transcript = (from tran in db.Transcripts
                                  join c in db.Courses on tran.CourseId equals c.COURSEID 
                                  join cr in db.Course_Rosters on tran.STUDENTID equals cr.STUDENTID

                                  where tran.STUDENTID.Value == studentid
                                  && cr.Cancel == 0
                                  && cr.COURSEID == tran.CourseId
                                  && c.CANCELCOURSE == 0
                                  select tran).ToList();

                if (start != null && end != null)
                {
                    return transcript.Where(trans => (trans.CourseStartDate >= start && trans.CourseStartDate <= end)|| trans.onlinecourse==1).ToList();
                }
                return transcript;
            }
        }



        public List<TranscriptObject> GetStudentCourseHoursforPurchase(int studentid)
        {
            double baseprice = 0;
            double minimumcredit = 0;
            int latedaysallowed = 0;
            string customcreditsettings = Settings.Instance.GetMasterInfo4().customcreditsettings;
            
            JavaScriptSerializer json = new JavaScriptSerializer();
            dynamic settingsconfig = json.Deserialize(customcreditsettings, typeof(object));
            if (settingsconfig != null)
            {
                if (settingsconfig.ContainsKey("baseprice"))
                {
                    double.TryParse(settingsconfig["baseprice"], out baseprice);
                }
                if (settingsconfig.ContainsKey("minimumcredit"))
                {
                    double.TryParse(settingsconfig["minimumcredit"], out minimumcredit);
                }
                if (settingsconfig.ContainsKey("latepurchaseexpired"))
                {
                    int.TryParse(settingsconfig["latepurchaseexpired"], out latedaysallowed);
                }
            }
            
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                double creditcost = double.Parse(Settings.Instance.GetMasterInfo3().SingleCourseCreditCost.Value.ToString());
                var transcript = (from tran in db.Transcripts
                                  join c in db.Courses on tran.CourseId equals c.COURSEID
                                  join cr in db.Course_Rosters on tran.STUDENTID equals cr.STUDENTID

                                  where tran.STUDENTID.Value == studentid
                                  && cr.Cancel == 0
                                  && cr.COURSEID == tran.CourseId
                                  && c.CANCELCOURSE == 0
                                  && (tran.IsHoursPaid == 0 || tran.IsHoursPaid == null)
                                  && tran.CustomCreditHours > 0
                                  select new TranscriptObject
                                  {
                                      TranscriptID = tran.TranscriptID,
                                      CustomCreditHours = (tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value : 0),
                                      dateadded = cr.DATEADDED,
                                      coursename = c.COURSENAME,
                                      Amount = Math.Round((tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value:0) * creditcost,2),
                                      OrderNumber = cr.OrderNumber,
                                      CourseConfiguration = c.CourseConfiguration

                                  }).ToList();
                List<TranscriptObject> translist = new List<TranscriptObject>();
                foreach (var item in transcript)
                {
                    string allowcreditpurchase = "";
                    item.dateaddedstring = item.dateadded.Value.ToShortDateString();
                    item.Amount = Double.Parse(String.Format("{0:0.00}", item.Amount)) + baseprice;
                    if ((item.CourseConfiguration != "") && (item.CourseConfiguration != null))
                    {
                        try
                        {

                            int noOfdaystoPurchase = Settings.Instance.GetMasterInfo3().NoofDaysToPurchaseHours.Value;
                            latedaysallowed = latedaysallowed + noOfdaystoPurchase;
                            if (item.dateadded.Value.AddDays(noOfdaystoPurchase) < DateTime.Now)
                            {
                                item.Amount = item.Amount +  double.Parse(Settings.Instance.GetMasterInfo3().LateFeeCharge.Value.ToString());
                            }
                            JavaScriptSerializer j = new JavaScriptSerializer();
                            dynamic configuration = j.Deserialize(item.CourseConfiguration, typeof(object));
                            allowcreditpurchase = configuration["purchasecredit"];
                        }
                        catch
                        {
                            if ((Settings.Instance.GetMasterInfo3().UsePurchaseCredit == 1) || (Settings.Instance.GetMasterInfo3().UsePurchaseCredit == -1))
                            {
                                allowcreditpurchase = "1";
                            }
                            else
                            {
                                allowcreditpurchase = "0";
                            }
                        }
                    }
                    if (allowcreditpurchase.Trim() == "1")
                    {
                        if (item.Amount< minimumcredit)
                        {
                            item.Amount = minimumcredit;
                        }

                            if (latedaysallowed == 0)
                            {
                                translist.Add(item);
                            }
                            else if (item.dateadded.Value.AddDays(latedaysallowed) > DateTime.Now)
                            {
                                translist.Add(item);
                            }
                        
                    }
                }

                return translist;
            }
        }

        public TranscriptObject GetStudentCourseHoursPurchasedDetails(int transcriptid)
        {
            using (var db = new SchoolEntities())
            {
                var transcript = (from tran in db.Transcripts
                                  join c in db.Courses on tran.CourseId equals c.COURSEID
                                  join cr in db.Course_Rosters on tran.STUDENTID equals cr.STUDENTID

                                  where tran.TranscriptID == transcriptid
                                  && cr.Cancel == 0
                                  && cr.COURSEID == tran.CourseId
                                  && c.CANCELCOURSE == 0
                                  && (tran.IsHoursPaid == 1)
                                  && tran.CustomCreditHours > 0
                                  select new TranscriptObject
                                  {
                                      TranscriptID = tran.TranscriptID,
                                      CustomCreditHours = (tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value : 0),
                                      dateadded = cr.DATEADDED,
                                      coursename = c.COURSENAME,
                                      SoldTo = (from student in db.Students where student.STUDENTID == tran.STUDENTID select student.FIRST + " " + student.LAST).FirstOrDefault(),
                                      Amount = 0,
                                      OrderNumber = cr.OrderNumber,
                                      IsHoursPaidInfo = tran.IsHoursPaidInfo,
                                      CourseStartDate = (from coursedate in db.Course_Times where coursedate.COURSEID == c.COURSEID orderby coursedate.COURSEDATE ascending select coursedate.COURSEDATE).FirstOrDefault()

                                  }).FirstOrDefault();

                if (transcript != null)
                {
                    DateTime paydate = new DateTime();
                    DateTime coursestartdate = new DateTime();
                    string paymenttype = "";
                    if ((transcript.IsHoursPaidInfo != "") && (transcript.IsHoursPaidInfo != null))
                    {
                        JavaScriptSerializer j = new JavaScriptSerializer();
                        dynamic hourpaidinfo = j.Deserialize(transcript.IsHoursPaidInfo, typeof(object));
                        paymenttype = "Credit Card";
                        try
                        {
                            try
                            {
                                paymenttype = hourpaidinfo["Paymthod"];
                            }
                            catch { }
                            transcript.IsHoursPaidInfo = paymenttype + "<br>Payment Date: " + hourpaidinfo["Paydate"] + "<br> AuthNum #" + hourpaidinfo["AuthNum"] + "<br>TransactionID #" + hourpaidinfo["TransactionID"];

                            transcript.Amount = double.Parse(hourpaidinfo["Amount"]);
                            DateTime.TryParse(hourpaidinfo["paydate"].toString(), out paydate);
                            paymenttype = "";
                        }
                        catch { }
                    }

                    transcript.dateaddedstring = transcript.dateadded.Value.ToShortDateString();
                    paydate = transcript.CourseStartDate.Value;

                    coursestartdate = transcript.CourseStartDate.Value;
                    transcript.CourseStartDateString = transcript.CourseStartDate.Value.ToShortDateString();
                    transcript.Amount = Double.Parse(String.Format("{0:0.00}", transcript.Amount));

                }
                return transcript;
            }
            
        }

        public List<TranscriptObject> GetStudentCourseHoursPurchased(int studentid,string startdate, string enddate)
        {
            DateTime start=DateTime.Now.AddYears(-5);
            DateTime end = DateTime.Now.AddDays(2);
            if (!(DateTime.TryParse(startdate, out start)))
            {
                start = DateTime.Now.AddYears(-5);
            }
            if (!(DateTime.TryParse(enddate, out end)))
            {
                end = DateTime.Now.AddDays(2);
            }
           
            double baseprice = 0;
            double minimumcredit = 0;
            int latedaysallowed = 0;
            string customcreditsettings = Settings.Instance.GetMasterInfo4().customcreditsettings;

            JavaScriptSerializer json = new JavaScriptSerializer();
            dynamic settingsconfig = json.Deserialize(customcreditsettings, typeof(object));
            if (settingsconfig != null)
            {
                if (settingsconfig.ContainsKey("baseprice"))
                {
                    double.TryParse(settingsconfig["baseprice"], out baseprice);
                }
                if (settingsconfig.ContainsKey("minimumcredit"))
                {
                    double.TryParse(settingsconfig["minimumcredit"], out minimumcredit);
                }
                if (settingsconfig.ContainsKey("latepurchaseexpired"))
                {
                    int.TryParse(settingsconfig["latepurchaseexpired"], out latedaysallowed);
                }
            }

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                double creditcost = double.Parse(Settings.Instance.GetMasterInfo3().SingleCourseCreditCost.Value.ToString());
                var transcript = (from tran in db.Transcripts
                                  join c in db.Courses on tran.CourseId equals c.COURSEID
                                  join cr in db.Course_Rosters on tran.STUDENTID equals cr.STUDENTID

                                  where tran.STUDENTID.Value == studentid
                                  && cr.Cancel == 0
                                  && cr.COURSEID == tran.CourseId
                                  && c.CANCELCOURSE == 0
                                  && (tran.IsHoursPaid == 1)
                                  && tran.CustomCreditHours > 0
                                  && (cr.ATTENDED == 1 || cr.ATTENDED == -1)
                                  select new TranscriptObject
                                  {
                                      TranscriptID = tran.TranscriptID,
                                      CustomCreditHours =(tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value:0),
                                      dateadded = cr.DATEADDED,
                                      coursename = c.COURSENAME,
                                      Amount = 0,
                                      OrderNumber = cr.OrderNumber,
                                      IsHoursPaidInfo = tran.IsHoursPaidInfo,
                                      CourseStartDate = (from coursedate in db.Course_Times where coursedate.COURSEID == c.COURSEID orderby coursedate.COURSEDATE ascending select coursedate.COURSEDATE).FirstOrDefault()

                                  }).ToList();
                List<TranscriptObject> translist = new List<TranscriptObject>();
                DateTime paydate = new DateTime();
                DateTime coursestartdate = new DateTime();
                string paymenttype = "";
                string authnum = "";
                foreach (var item in transcript)
                {
                    if ((item.IsHoursPaidInfo != "") && (item.IsHoursPaidInfo!=null))
                    {
                        JavaScriptSerializer j = new JavaScriptSerializer();
                        dynamic hourpaidinfo = j.Deserialize(item.IsHoursPaidInfo, typeof(object));
                        paymenttype = "Credit Card";
                        try
                        {
                            try
                            {
                                paymenttype = hourpaidinfo["Paymthod"];
                            }
                            catch { }
                            item.IsHoursPaidInfo = paymenttype + "<br>Payment Date: " + hourpaidinfo["Paydate"] + "<br> AuthNum #" + hourpaidinfo["AuthNum"] + "<br>TransactionID #" + hourpaidinfo["TransactionID"];
                            
                            item.Amount = double.Parse(hourpaidinfo["Amount"]);
                            DateTime.TryParse(hourpaidinfo["paydate"].toString(), out paydate);
                            paymenttype = "";
                        }
                        catch { }
                    }

                    item.dateaddedstring = item.dateadded.Value.ToShortDateString();
                    paydate = item.CourseStartDate.Value;

                    coursestartdate = item.CourseStartDate.Value;
                    item.CourseStartDateString = item.CourseStartDate.Value.ToShortDateString();
                    item.Amount = Double.Parse(String.Format("{0:0.00}", item.Amount));

                    if (coursestartdate >= start && coursestartdate <= end)
                    {
                        translist.Add(item);
                    }
                }

                return translist;
            }
        }

        public void ApprovedCreditHoursPurchase(string Orderno, int TranscriptID, string strPaytype,double Amount)
        {

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var transcript = (from tran in db.Transcripts where tran.TranscriptID == TranscriptID select tran).FirstOrDefault();
                transcript.IsHoursPaid = 1;
                var Order = (from roster in db.Course_Rosters where roster.OrderNumber == Orderno && roster.COURSEID == transcript.CourseId && roster.STUDENTID == transcript.STUDENTID select roster).FirstOrDefault();
                Order.IsHoursPaid = 1;

                string PaidInfo = "{\"Paydate\": \"" + DateTime.Now + " \",\"Paymthod\": \"" + strPaytype + "\",  \"AuthNum\": \"" + Order.AuthNum + "\",  \"TransactionID\": \"" + TranscriptID + "\",  \"Amount\": \"" + Amount + "\" }";

                transcript.IsHoursPaidInfo = PaidInfo;
                string orderTranscriptid = TranscriptID.ToString();
                var orderinpProgress = (from orderinProgress in db.OrderInProgresses where orderinProgress.OrderNumber == orderTranscriptid select orderinProgress).FirstOrDefault();
                if (orderinpProgress != null)
                {
                    orderinpProgress.OrderCurStatus = "Successfully Paid";
                }
                db.SaveChanges();
                SendClockHoursEmailConfirmation(Order, transcript, strPaytype, Amount);
            }
        }
        public void SendClockHoursEmailConfirmation(Course_Roster roster, Transcript transcript, string strPaytype, double Amount)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var _student = (from student in db.Students where student.STUDENTID == roster.STUDENTID select student).FirstOrDefault();
                if (_student != null)
                {
                    string email = "";
                    email = email + "<div style='font-family:Arial;'>";
                    email = email + "<div style='font-weight:bold; font-size:xx-large'>Order Receipt</div><br /><br />";
                    email = email + "<table>";
                    email = email + "<tbody><tr>";
                    email = email + "<td class='confirmation-area-header' style='width:300px; background-color:#dddddd;font-weight:bold; font-size:large; padding:3px;'>Sold To: </td>";
                    email = email + "<td class='confirmation-area-header' style='width:300px; background-color:#dddddd;font-weight:bold; font-size:large; padding:3px;'>Payment Details:";

                    email = email + "</td>";
                    email = email + "</tr>";
                    email = email + "<tr>";
                    email = email + "<td> " + _student .FIRST +" " +_student.LAST+ "</td>";
                    email = email + "<td>PO<br>Payment Date:" + DateTime.Now + " <br> AuthNum #" + roster.AuthNum + "<br>TransactionID " + transcript.TranscriptID.ToString() + "</td>";
                    email = email + "</tr>";
                    email = email + "</tbody></table><br>";
                    email = email + "<div style='background-color:#dddddd;font-weight:bold; font-size:large; padding:3px;'>Checkout</div>";
                    email = email + "<table style='padding:10px 0px 50px 50px'>";
                    email = email + "<tbody><tr>";
                    email = email + "<td style='width:300px; font-weight:bold; border-bottom:solid; border-bottom-width:1px;'>Course Name</td>";
                    email = email + "<td style='width:200px; font-weight:bold; border-bottom:solid;border-bottom-width:1px;'>Course date</td>";
                    email = email + "<td style='width:150px; font-weight:bold; border-bottom:solid;border-bottom-width:1px;'>Purchased " + CourseCreditHelper.GetCreditLabel(CourseCreditType.Custom) +" </td>";
                    email = email + "<td style='width:150px; font-weight:bold; border-bottom:solid;border-bottom-width:1px;'>Amount</td>";
                    email = email + "</tr>";
                    email = email + "<tr>";

                    email = email + "<td style='width:300px'>" + transcript.CourseName + "</td>";
                    email = email + "<td style='width:200px'>" + transcript.CourseDate + "</td>";
                    email = email + "<td style='width:150px'>" + transcript.CustomCreditHours + "</td>";
                    email = email + "<td style='width:150px'>" +string.Format("{0:C}", Amount) + "</td>";
                    email = email + "</tr>";

                    email = email + "</tbody></table>";

                    email = email + "<div style='padding-left:68%; font-weight:bold'>Total: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + string.Format("{0:C}",Amount) + "</div>";
                    email = email + "</div>";
                    if (_student.EMAIL != "")
                    {
                        EmailFunction EmailFunction = new EmailFunction();
                        EmailFunction.SendEmail(_student.EMAIL, "Purchased " +CourseCreditHelper.GetCreditLabel(CourseCreditType.Custom) + " Receipt", email, "");
                    }
                }
            }

        }
        public void ClockHourPurchaseOrderinProgress(string ordernumber, string amount,int transcriptid)
        {
            using (var db = new SchoolEntities())
            {
                var coursedetails = new JObject();
                var userdetails = new JObject();
                var transcript = (from tran in db.Transcripts where tran.TranscriptID == transcriptid select tran).FirstOrDefault();

                OrderInProgress OrderInProgress = new OrderInProgress();
                OrderInProgress.OrderNumber = transcriptid.ToString();
                OrderInProgress.ordername = transcript.STUDENTID.ToString();
                OrderInProgress.OrderPrice = amount;
                OrderInProgress.OrderCurStatus = "Pending";
                OrderInProgress.orderdate = DateTime.Now;
                userdetails.Add("sid", transcript.STUDENTID);
                coursedetails.Add("cid", transcript.CourseId);
                OrderInProgress.userlogindetails = userdetails.ToString(Newtonsoft.Json.Formatting.None);
                OrderInProgress.coursedetails = coursedetails.ToString(Newtonsoft.Json.Formatting.None);
                db.OrderInProgresses.Add(OrderInProgress);
                db.SaveChanges();
                

            }
        }
        public TranscriptObject GetSingleTranscriptById(int TranscriptId)
        {
            TranscriptObject transobj = new TranscriptObject();
            using (var db = new SchoolEntities())
            {
                double creditcost = double.Parse(Settings.Instance.GetMasterInfo3().SingleCourseCreditCost.Value.ToString());
                 double baseprice=0;
                 double minimumcredit = 0;
                 string customcreditsettings = Settings.Instance.GetMasterInfo4().customcreditsettings;
            
            JavaScriptSerializer json = new JavaScriptSerializer();
            dynamic settingsconfig = json.Deserialize(customcreditsettings, typeof(object));
            if (settingsconfig != null)
            {
                if (settingsconfig.ContainsKey("baseprice"))
                {
                    double.TryParse(settingsconfig["baseprice"], out baseprice);
                }
                if (settingsconfig.ContainsKey("minimumcredit"))
                {
                    double.TryParse(settingsconfig["minimumcredit"], out minimumcredit);
                }
            }
                var transcript = (from tran in db.Transcripts where tran.TranscriptID == TranscriptId
                                       select new TranscriptObject{
                                           TranscriptID = tran.TranscriptID,
                                           CustomCreditHours = (tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value : 0),
                                           Amount = Math.Round((tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value:0) * creditcost, 2)+ baseprice,
                                           OrderNumber = tran.OrderNumber,
                                       }
                                      ).SingleOrDefault();

                if (transcript != null)
                {
                    int noOfdaystoPurchase = Settings.Instance.GetMasterInfo3().NoofDaysToPurchaseHours.Value;
                    var Dateadded = (from cr in db.Course_Rosters where cr.OrderNumber == transcript.OrderNumber select cr.DATEADDED).FirstOrDefault();
                    if (Dateadded.Value.AddDays(noOfdaystoPurchase) < DateTime.Now)
                    {
                        transobj.Amount = transcript.Amount + double.Parse(Settings.Instance.GetMasterInfo3().LateFeeCharge.Value.ToString());
                    }
                    else
                    {
                        transobj.Amount = transcript.Amount;
                    }
                    if (transobj.Amount < minimumcredit)
                    {
                        transobj.Amount = minimumcredit;
                    }
                    transobj.OrderNumber = transcript.OrderNumber;
                    transobj.TranscriptID = transcript.TranscriptID;
                    transobj.SoldTo= transcript.SoldTo;
                }
            }

            return transobj;

        }

        public int GetDefaultTranscript()
        {
            using (var db = new SchoolEntities())
            {

                    var  DefaultPdfTranscript = (from cc in db.customtranscripts where cc.isdefaultselected == 1 select cc).FirstOrDefault();
                    if (DefaultPdfTranscript != null)
                    {
                        return DefaultPdfTranscript.customtranid;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }
        

        public class TranscriptObject
        {
            public int TranscriptID { get; set; }
            public string OrderNumber { get; set; }
            public string coursename { get; set; }
            public DateTime? dateadded { get; set; }
            public string dateaddedstring { get; set; }
            public double CustomCreditHours { get; set; }
            public double Amount { get; set; }
            public string IsHoursPaidInfo { get; set; }
            public string CourseConfiguration { get; set; }
            public DateTime? CourseStartDate { get; set; }
            public string CourseStartDateString { get; set; }
            public string SoldTo { get; set; }
            

        }
    }

}
