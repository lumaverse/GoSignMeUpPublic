using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Export.GradeCertificate;

namespace Gsmu.Api.Integration.Haiku.Import
{
    public class HaikuCsvImport
    {
        public static string ReportCsvStringFromGrades(System.IO.Stream streamCsvs)
        {
            var db = new entities.SchoolEntities();
                
            StreamReader streamRead = new StreamReader(streamCsvs, Encoding.UTF8);
            var csvString = streamRead.ReadToEnd();

            var message = new StringBuilder();

            var header = new string[] { "User Import ID", "User ID", "Class Import ID", "Class ID", "Term Name", "Grading Period Name", "Score Updated At", "Score Deleted At", "Letter Grade", "Percent Grade" };

            var results = csvString.Split('\n');
            string strDatetoday = DateTime.Now.ToShortDateString();

            int? rosterResponseClassid = null;
            Response rosterResponse = null;
            for (int index = 0; index < results.Length; index++)
            {
                // remove \r
                results[index] = results[index].TrimEnd('\r');

                // remove empty string
                if (results[index] == string.Empty)
                {
                    continue;
                }

                //  you find the csv in the result
                var result = results[index].Split(',');
               
                // check headers in 0 index
                if (index == 0)
                {
                    message.AppendFormat("Headers are in index 0.<br/>");
                    for (var headerIndex = 0; headerIndex < result.Length; headerIndex++)
                    {
                        if (header[headerIndex] != result[headerIndex])
                        {
                            throw new Exception(string.Format("Header is not same: ", header[headerIndex], results[headerIndex]));
                        }
                    }
                } 

                // files in index > 0 if not result ""
                if (index > 0)
                {
                    message.AppendFormat(" Csv data are in index {0}.<br/>", index);
                    HaikuImportCvs record = new HaikuImportCvs(result);
                    if (record.UserId != null && record.ClassID != null)
                    {
                        var student = Haiku.HaikuImport.SynchronizeStudent(record.UserId.Value);
                        if (rosterResponseClassid == null || !(rosterResponseClassid.Value == record.ClassID.Value))
                        {
                            rosterResponse = Haiku.HaikuImport.SynchronizeRoster(record.ClassID.Value);
                        }
                        
                        var course = (from c in db.Courses where c.haiku_course_id == record.ClassID.Value select c).FirstOrDefault();

                        //notice we assume cancelled registration will not have grade in Haiku
                        var roster = (from r in db.Course_Rosters where r.COURSEID == course.COURSEID && r.STUDENTID == student.STUDENTID select r).FirstOrDefault();
                                                
                        var course_times = (from ct in db.Course_Times
                                            where ct.COURSEID == course.COURSEID
                                            orderby 1
                                            select ct).ToList();
                        string AllCourseDates = string.Empty;
                        string courseStartDate = string.Empty;
                        int countidx = 0;
                        foreach (var selectedcoursetimes in course_times)
                        {
                            
                            //if (course_times.Count == 1)
                            if (countidx == 0)
                            {
                                courseStartDate = selectedcoursetimes.COURSEDATE.Value.ToShortDateString();
                                AllCourseDates = selectedcoursetimes.COURSEDATE.Value.ToShortDateString() + "|1";
                            }
                            else
                            {
                                AllCourseDates = AllCourseDates + selectedcoursetimes.COURSEDATE.Value.ToShortDateString() + "|1,";
                            }
                            countidx += 1;
                        }                        
                        AllCourseDates = AllCourseDates.TrimEnd(',');

                        roster.Cancel = 0;
                        if (string.IsNullOrEmpty(roster.OrderNumber))
                        {
                            roster.OrderNumber = string.Format("HAIKU{0}", roster.RosterID);
                        }

                        course.CANCELCOURSE = 0;
                        
                        var transcript = (from t in db.Transcripts where t.STUDENTID == student.STUDENTID && t.CourseId == course.COURSEID select t).FirstOrDefault();

                        if (transcript == null)
                        {
                            transcript = new entities.Transcript();
                            transcript.CourseCompletionDate = new DateTime(1900, 1, 1, 0, 0, 0);
                            transcript.CourseStartDate = DateTime.Now;
                            db.Transcripts.Add(transcript);
                        }
                        message.AppendFormat(" Course {0}, Student {1}, Grade {2} .<br/>", course.COURSEID, student.STUDENTID, record.LetterGrade);

                        transcript.STUDENTID = student.STUDENTID;
                        transcript.CourseId = course.COURSEID;
                        transcript.ATTENDED = 1;
                        transcript.DIDNTATTEND = 0;
                        transcript.StudentGrade = record.LetterGrade;
                        transcript.CourseName = course.COURSENAME;
                        transcript.CourseNum = course.COURSENUM;
                        //basic data
                        transcript.CourseDate = courseStartDate;
                        transcript.Days = course.DAYS;
                        transcript.DistPrice = course.DISTPRICE;
                        transcript.NoDistPrice = course.NODISTPRICE;
                        transcript.CreditHours = roster.HOURS;
                        transcript.DATEADDED = roster.DATEADDED;
                        transcript.TIMEADDED = roster.TIMEADDED;
                        transcript.HOURS = roster.HOURS;
                        transcript.CourseCost = roster.CourseCost;
                        transcript.OrderNumber = roster.OrderNumber;
                        transcript.TotalPaid = roster.TotalPaid.ToString();
                        transcript.PAYMETHOD = roster.PAYMETHOD;
                        transcript.payNumber = roster.payNumber;
                        transcript.AuthNum = roster.AuthNum;
                        transcript.PaidInFull = -1;
                        transcript.PricingMember = roster.PricingMember;
                        transcript.PricingOption = roster.PricingOption;
                        transcript.CourseHoursType = roster.CourseHoursType;

                        /////
                        // assuming the transcription date is the completion date.
                        // may need to reflect the system config option in this to get different date.
                        transcript.CourseCompletionDate = DateTime.Parse(strDatetoday);
                        //////
                        transcript.CourseStartDate =  DateTime.Parse(courseStartDate);
                        transcript.AttendanceDetail = AllCourseDates;
                        transcript.CustomCreditHours = roster.CustomCreditHours;
                        transcript.graduatecredit = roster.graduatecredit;
                        transcript.ceucredit = roster.ceucredit;
                        transcript.InserviceHours = 0;
                        transcript.Optionalcredithours1 = roster.Optionalcredithours1;
                        transcript.onlinecourse = 0;
                        transcript.datetranscribed = DateTime.Parse(strDatetoday);
                        transcript.CertificateIssueDate = DateTime.Parse("1990-01-01 00:00:00.000");

                        // 1-  reserved for manual add from student transcript add
                        // 2-  reserved for automatic add from Haiku Routine
                        transcript.UserAddedFlag = 2; 


                        // end basic data
                        roster.StudentGrade = record.LetterGrade;
                        roster.ATTENDED = -1;


                        //course.coursecertificate                        
                        //var customcetificate = (from cc in db.customcetificates where cc.customcertid in )
                        // it will do from HAIKU CERTS
                        /*
                        var doAllTheTimes = true;
                        if (course.coursecertificate != null &&
                            record.ScoreUpdatedAt != null && 
                                (transcript.CourseCompletionDate < record.ScoreUpdatedAt.Value || doAllTheTimes) && 
                                record.LetterGrade == "A")
                        {
                            transcript.CourseCompletionDate = record.ScoreUpdatedAt.Value;
                            var exportPdfGradeCert = new PdfGradeCertificate(course, roster, transcript);
                            exportPdfGradeCert.Execute();
                        }
                         */
                    }
                }
            }
            db.SaveChanges();
            return message.ToString();
        }

        public static int? ParseIntNull(string parseIntNull)
        {
            if (string.IsNullOrWhiteSpace(parseIntNull))
            {
                return null;
            }
            return int.Parse(parseIntNull);
        }

        public static float? ParseFloatNull(string parseFloatNull)
        {
            if (string.IsNullOrWhiteSpace(parseFloatNull))
            {
                return null;
            }
            return float.Parse(parseFloatNull);
        }


        public static int DateTimeMonthParse(string month)
        {
            var monthDay = 0;
            switch (month)
            {
                case "Jan":
                    monthDay = 1;
                    break;
                case "Feb":
                    monthDay = 2;
                    break;
                case "Mar":
                    monthDay = 3;
                    break;
                case "Apr":
                    monthDay = 4;
                    break;
                case "May":
                    monthDay = 5;
                    break;
                case "Jun":
                    monthDay = 6;
                    break;
                case "Jul":
                    monthDay = 7;
                    break;
                case "Aug":
                    monthDay = 8;
                    break;
                case "Sep":
                    monthDay = 9;
                    break;
                case "Oct":
                    monthDay = 10;
                    break;
                case "Nov":
                    monthDay = 11;
                    break;
                case "Dec":
                    monthDay = 12;
                    break;
                default: 
                    monthDay = DateTime.Now.Month;
                    break;
            }
            return monthDay;
        }

        public static DateTime? DateTimeParse(string dateString)
        {
            DateTime? date = null;
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return date;
            }
            // Wed Oct 22 17:16:04 GMT 2014
            var dateResult = dateString.Split(' ');

            // month
            var month = int.Parse(dateResult[2]);

            var monthDay = DateTimeMonthParse(dateResult[1]);

            var timeResult = dateResult[3].Split(':');
            var monthTimeHour = int.Parse(timeResult[0]);
            var monthTimeMinute = int.Parse(timeResult[1]);
            var monthTimeSecond = int.Parse(timeResult[2]);

            var monthYear = int.Parse(dateResult[5]);

            date = new DateTime(monthYear, monthDay, month, monthTimeHour, monthTimeMinute, monthTimeSecond);

            return date;
        }


    }
}
