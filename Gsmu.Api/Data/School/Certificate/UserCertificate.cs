using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Certificate
{
    public class UserCertificate
    {
        public string GetCertificateName(string courseNumber)
        {

            using (var db = new SchoolEntities())
            {
                if ((Settings.Instance.GetMasterInfo2().CertificationsOnOff == 1) ||  (Settings.Instance.GetMasterInfo2().CertificationsOnOff == 2))
                {
                    var certificate = (from cc in db.CertificationsCourses join c in db.Certifications on cc.CertificationsId equals c.CertificationsId where cc.CourseNum == courseNumber select c.CertificationsTitle).FirstOrDefault();
                    if ((certificate == "") || (certificate == null))
                    {
                        certificate = "";
                    }
                    return certificate;
                }
                else
                {
                    return "";
                }
            }
        }
        public List<UserCertifacteDetailsModel> StudentCertifiacteDetails(int certifiacteId)
        {
            List<UserCertifacteDetailsModel> certificatedetails = new List<UserCertifacteDetailsModel>();
            List<UserCertifacteDetailsModel> certificatedetails_with_status = new List<UserCertifacteDetailsModel>();

            using (var db = new SchoolEntities())
            {
               int studentid = 0;
                if (AuthorizationHelper.CurrentStudentUser != null)
                {
                    studentid = AuthorizationHelper.CurrentStudentUser.STUDENTID;
                }
                var certificateNoCourseRequired = (from cc1 in db.Certifications where cc1.CertificationsId == certifiacteId select cc1.CertificationsHowManyCoursesRequired).FirstOrDefault();
                var certifactecourses = (from cc in db.CertificationsCourses where cc.CertificationsId == certifiacteId select cc).ToList();
                var StudentCetrtification = (from ce in db.CertificationsStudents where ce.CertificationsId == certifiacteId && ce.StudentId == studentid select ce).FirstOrDefault();
                UserCertifacteDetailsModel usercertificate = new UserCertifacteDetailsModel();
                usercertificate.Courses_Requirements = "";
                usercertificate.certificate_expiry = StudentCetrtification.ExpireDate.ToString() ;
                usercertificate.ExpiryDate = StudentCetrtification.ExpireDate;
                usercertificate.QualificationStartDate = StudentCetrtification.QualificationStartDate;
                int enrollcoursecount = 0;
                List<string> list_cname = new List<string>();
                List<string> list_cname2 = new List<string>();
                foreach (var certcourse in certifactecourses)
                {
                    usercertificate = new UserCertifacteDetailsModel();
                    usercertificate.Courses_Requirements = ""; 
                    usercertificate.certificate_expiry = StudentCetrtification.ExpireDate.ToString();
                    usercertificate.CertificateId = certcourse.CertificationsId.Value;
                    usercertificate.CourseNumber = certcourse.CourseNum;
                    usercertificate.ExpiryDate = StudentCetrtification.ExpireDate;
                    usercertificate.QualificationStartDate = StudentCetrtification.QualificationStartDate;
                    //list of all courses the student has in the certification
                    var studentincertificate = (from cr in db.Course_Rosters join c in db.Courses on cr.COURSEID equals c.COURSEID where cr.STUDENTID == studentid && c.COURSENUM == usercertificate.CourseNumber && cr.Cancel == 0  select cr).ToList();
                    //list of all completed courses student has in certification
                    var studentCompletedCourse = (from csc in db.CertificationsStudentCompleteds where csc.CertificationsId == certifiacteId && csc.StudentId == studentid && csc.CompletionBasedOn.Contains(certcourse.CourseNum) select csc).ToList();
                    //list of courses in certificate with details.
                     var courses_group = (from c in db.Courses where c.COURSENUM == usercertificate.CourseNumber select c).ToList();
                    if (Settings.Instance.GetMasterInfo2().CertificationsOnOff == 2)
                    {
                        studentincertificate = (from cr in db.Course_Rosters join c in db.Courses on cr.COURSEID equals c.COURSEID where cr.STUDENTID == studentid && c.CustomCourseField5 == usercertificate.CourseNumber && cr.Cancel == 0 select cr).ToList();
                        courses_group = (from c in db.Courses where c.CustomCourseField5 == usercertificate.CourseNumber select c).ToList();
                    }

                    if (studentincertificate == null || studentincertificate.Count() == 0)
                    {
                        
                        usercertificate.Status = "Not Completed";
                    }
                    else
                    {
                        var progress =0;
                        var completed =0;
                        var courses = "";
                        var attendedcourses = "";
                        //loop on checking certification courses that student enrolled
                        foreach(var roster in studentincertificate)
                        {
                            if (!list_cname.Contains(roster.Course.COURSENAME))
                            {
                                if ((roster.ATTENDED == 1) || (roster.ATTENDED == -1))
                                {
                                    completed = 1;
                                    var sdates = (from sdate in db.Course_Times where sdate.COURSEID == roster.COURSEID orderby sdate.COURSEDATE select sdate).FirstOrDefault();
                                    var _transcriptdate = (from transcriptdate in db.Transcripts where transcriptdate.CourseNum == roster.Course.COURSENUM && transcriptdate.STUDENTID == roster.STUDENTID && transcriptdate.ATTENDED != 0 && transcriptdate.CourseId == roster.COURSEID select transcriptdate).FirstOrDefault(); 
                                    
                                    if (sdates.COURSEDATE >= usercertificate.QualificationStartDate && sdates.COURSEDATE <= usercertificate.ExpiryDate)
                                    {
                                        attendedcourses = attendedcourses + "<br />Course Name: " + roster.Course.COURSENAME + "<br />Status: <font color='green'> Completed </font><br />";
                                        usercertificate.Status = "Completed";
                                        usercertificate.Courses = attendedcourses + courses;
                                    }
                                    else
                                    {
                                        if (_transcriptdate != null)
                                        {
                                            if (_transcriptdate.CourseStartDate >= usercertificate.QualificationStartDate && _transcriptdate.CourseStartDate <= usercertificate.ExpiryDate)
                                            {
                                                         attendedcourses = attendedcourses + "<br />Course Name: " + roster.Course.COURSENAME + "<br />Status: <font color='green'> Completed </font><br />";
                                                         usercertificate.Status = "Completed";
                                                         usercertificate.Courses = attendedcourses + courses;
                                            }
                                            else
                                            {
                                                attendedcourses = attendedcourses + "<br />Course Name: " + roster.Course.COURSENAME + "<br />Status: <font color='red'> Not Completed </font><br />";
                                                usercertificate.Status = "Not Completed";
                                                usercertificate.Courses = attendedcourses + courses;
                                            }
                                        }
                                        else
                                        {
                                            attendedcourses = attendedcourses + "<br />Course Name: " + roster.Course.COURSENAME + "<br />Status: <font color='red'> Not Completed </font><br />";
                                            usercertificate.Status = "Not Completed";
                                            usercertificate.Courses = attendedcourses + courses;
                                        }
                                    }
                                }
                                if (roster.ATTENDED == 0)
                                {
                                    progress = 1;
                                    var sdates = (from sdate in db.Course_Times where sdate.COURSEID == roster.COURSEID orderby sdate.COURSEDATE select sdate).FirstOrDefault();
                                    if (sdates.COURSEDATE >= usercertificate.QualificationStartDate && sdates.COURSEDATE <= usercertificate.ExpiryDate)
                                    {
                                        courses = courses + "<br />Course Name: " + roster.Course.COURSENAME + "<br />Start date: " + sdates.COURSEDATE.Value.ToString("MM/dd/yyyy") + "<br />";
                                        usercertificate.Status = "In Progress";
                                        usercertificate.Courses = attendedcourses + courses;
                                    }
                                    else
                                    {
                                        attendedcourses = attendedcourses + "<br />Course Name: " + roster.Course.COURSENAME + "<br />Status: <font color='red'> Not Completed </font><br />";
                                        usercertificate.Status = "Not Completed";
                                        usercertificate.Courses = attendedcourses + courses;
                                    }
                                }
                                list_cname.Add(roster.Course.COURSENAME);
                            }
                        }
                        int ccount = 0;

                        //loop on checking all remain course on certification
                        foreach (var course in courses_group)
                        {
                            if (!list_cname2.Contains(course.COURSENAME))
                            {
                                var coursenotenrolled = (from ca in studentincertificate where ca.COURSEID == course.COURSEID select ca).Count();
                                if (coursenotenrolled <= 0 && studentincertificate.Count<=0)
                                {
                                    attendedcourses = attendedcourses + "<br />Course Name: " + course.COURSENAME + "<br />Status: <font color='red'> Not Completed </font><br />";
                                    usercertificate.Status = "Not Completed";
                                    usercertificate.Courses = attendedcourses + courses;
                                }
                                else
                                {
                                    enrollcoursecount = enrollcoursecount + 1;
                                }
                                ccount = ccount + 1;
                                usercertificate.Courses_Requirements = usercertificate.Courses_Requirements + "<br />" + course.COURSENAME;
                                list_cname2.Add(course.COURSENAME);
                            }
                        }
                        /*if((completed == 1) && (progress == 0) && (ccount<=studentincertificate.Count()))
                        {
                              usercertificate.Status = "Completed";
                        }
                        else if ((progress == 0) && (completed == 0))
                        {
                            usercertificate.Status = "Not Completed";

                        }
                        else if (enrollcoursecount >= certificateNoCourseRequired)
                        {
                            usercertificate.Status = "Completed";
                        }
                        else
                        {
                            usercertificate.Status = "In Progress";
                            usercertificate.Courses = attendedcourses + courses;
                        }*/

                        //if # of required course completed then all completed.
                        if (certificateNoCourseRequired >= 1)
                        {
                            if (studentCompletedCourse.Count >= certificateNoCourseRequired)
                            {
                                usercertificate.Status = "Completed";
                            }
                        }
                    }

                    certificatedetails.Add(usercertificate);
                }

                if (enrollcoursecount >= certificateNoCourseRequired)
                {
                    foreach (var certificate in certificatedetails)
                    {
                        //certificate.Status = "Completed";
                        certificatedetails_with_status.Add(certificate);
                    }
                }

                else
                {
                    certificatedetails_with_status = certificatedetails;
                }
            }


            return certificatedetails_with_status;
        }
    }

    public class UserCertifacteDetailsModel
    {
        public int CertificateId {get;set;}
        public string CourseNumber { get; set; }
        public string Status { get; set; }
        public string Courses { get; set; }
        public string Courses_Requirements { get; set; }
        public string certificate_expiry { get; set; }
        public DateTime? QualificationStartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
