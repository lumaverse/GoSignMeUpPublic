using Gsmu.Api.Authorization;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.ViewModels.Grid;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Supervisor
{
    public class SupervisorHelper
    {
        public static string SendingEmailStatus
        {
            get;
            set;
        }

        public static Entities.Supervisor GetSupervisor(string supervisorid)
        {
            using (var db = new SchoolEntities())
            {
                var supervisor = (from s in db.Supervisors where s.UserName == supervisorid select s).FirstOrDefault();
                return supervisor;
            }
        }
        public static ListingStudentModel GetStudent(int studentId)
        {
            using (var db = new SchoolEntities())
            {
                var student = (from s in db.Students
                               where s.STUDENTID == studentId
                               select new ListingStudentModel
                               {
                                   StudentId = s.STUDENTID,
                                   StudentFirstName = s.FIRST,
                                   StudentLastName = s.LAST,
                                   Email = s.EMAIL,
                                   UserName = s.USERNAME
                               }).FirstOrDefault();
                return student;
            }
        }
        public static string Export(string folderpath)
        {
            int AssignSup2Stud = Convert.ToInt32(Settings.Instance.GetMasterInfo3().AssignSup2Stud);
            var SupervisorStudentFilter = Convert.ToInt32(Settings.Instance.GetMasterInfo2().SupervisorStudentFilter);
            using (var db = new SchoolEntities())
            {

                var query = from e in db.Students
                            select new ListingStudentModel
                            {
                                StudentId = e.STUDENTID,
                                StudentFirstName = e.FIRST,
                                StudentLastName = e.LAST,
                                Email = e.EMAIL,
                                UserName = e.USERNAME,
                                Enrolled = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.Cancel == 0 select e.STUDENTID).Count(),
                                Waiting = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.WAITING != 0 && a.Cancel == 0 select e.STUDENTID).Count(),
                                Complete = (from c in db.Transcripts
                                            join cr in db.Course_Rosters on c.CourseId equals cr.COURSEID
                                            where c.STUDENTID == e.STUDENTID &&
                                            c.ATTENDED != 0 && cr.ATTENDED != 0 &&
                                            cr.Cancel == 0
                                            select new { cid = c.CourseId }).Distinct().Count(),
                                //inCheckout =(from b in CourseShoppingCart.Instance.MultipleStudentCourses where b.StudentId == e.STUDENTID && b.CourseId == courseid select e.STUDENTID).Count()
                                AssignSup2StudList = (from a in db.SupervisorStudents where a.studentid == e.STUDENTID && a.SupervisorID == Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select a.SupervisorID).Count(),
                                District = (from a in db.Supervisors where a.DISTRICT == e.DISTRICT && a.SUPERVISORID == Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select a.SUPERVISORID).Count(),
                                SupStudSchool = (from a in db.SupervisorSchools where a.SchoolID == e.SCHOOL && a.SupervisorID == Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select a.SupervisorID).Count()
                            };
                /*
                if ((AssignSup2Stud == 1))
                {
                    query = query.Where(e => e.District > 0);
                }
                else if ((AssignSup2Stud == 0))
                {
                    query = query.Where(e => e.School > 0);
                }
                query = query.OrderBy(e => e.StudentLastName);

                */
                if (SupervisorStudentFilter == 99)
                {
                    if ((AssignSup2Stud == 1))
                    {
                        query = query.Where(e => e.AssignSup2StudList > 0);
                    }
                    else if ((AssignSup2Stud == 0))
                    {
                        query = query.Where(e => e.District > 0);
                    }
                }
                //and
                else if (SupervisorStudentFilter == 0)
                {
                    if ((AssignSup2Stud == 1))
                    {
                        query = query.Where(e => (e.District > 0 && e.SupStudSchool > 0) || e.AssignSup2StudList > 0);
                    }
                    else
                    {
                        if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                        {
                            query = query.Where(e => e.District > 0 && e.SupStudSchool > 0);
                        }
                    }
                }
                //or
                else if (SupervisorStudentFilter == 1)
                {
                    if ((AssignSup2Stud == 1))
                    {
                        query = query.Where(e => e.District > 0 || e.SupStudSchool > 0 || e.AssignSup2StudList > 0);
                    }
                    else
                    {
                        query = query.Where(e => e.District > 0 || e.SupStudSchool > 0);
                    }
                }
                StringBuilder sb = new StringBuilder();

                sb.Append("First Name").Append(",");
                sb.Append("Last Name").Append(",");
                sb.Append("User Name").Append(",");
                sb.Append("Email").Append(",");
                sb.Append("Enrolled").Append(",");
                sb.Append("Course Complete").Append(",");
                sb.Append("Inactive").Append(",");
                sb.Append("Waiting").Append(",");
                sb.AppendLine();
                foreach (var row in query)
                {
                    sb.Append(row.StudentFirstName).Append(",");
                    sb.Append(row.StudentLastName).Append(",");
                    sb.Append(row.UserName).Append(",");
                    sb.Append(row.Email).Append(",");
                    sb.Append(row.Enrolled).Append(",");
                    sb.Append(row.Complete).Append(",");
                    sb.Append(row.InActive).Append(",");
                    sb.Append(row.Waiting).Append(",");
                    sb.AppendLine();
                }

                string filename = "StudentList" + DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour + ".csv";
                string path = folderpath + filename;
                File.WriteAllText(path, sb.ToString());
                return filename;
            }
        }

        public static void BuildExport(List<ListingStudentModel> query, string folderpath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("First Name").Append(",");
            sb.Append("Last Name").Append(",");
            sb.Append("User Name").Append(",");
            sb.Append("Email").Append(",");
            sb.Append("Enrolled").Append(",");
            sb.Append("Course Complete").Append(",");
            sb.Append("Inactive").Append(",");
            sb.Append("Waiting").Append(",");
            sb.AppendLine();
            foreach (var row in query)
            {
                sb.Append(row.StudentFirstName).Append(",");
                sb.Append(row.StudentLastName).Append(",");
                sb.Append(row.UserName).Append(",");
                sb.Append(row.Email).Append(",");
                sb.Append(row.Enrolled).Append(",");
                sb.Append(row.Complete).Append(",");
                sb.Append(row.InActive).Append(",");
                sb.Append(row.Waiting).Append(",");
                sb.AppendLine();
            }
            try
            {
                string filename = "StudentList-" + AuthorizationHelper.CurrentSupervisorUser.UserName + ".csv";
                string path = folderpath + filename;
                File.WriteAllText(path, sb.ToString());
            }
            catch { }
        }


        public static List<ListingStudentTranscript> GetStudentTranscript(int studentid)
        {
            using (var db = new SchoolEntities())
            {
                var query = (from e in db.Transcripts
                             join cr in db.Course_Rosters on e.CourseId equals cr.COURSEID
                             where e.STUDENTID == studentid &&
                             e.ATTENDED != 0 && cr.ATTENDED != 0 &&
                             cr.Cancel == 0


                             select new ListingStudentTranscript
                             {
                                 StudentId = e.STUDENTID.Value,
                                 CourseNumber = e.CourseNum,
                                 CourseName = e.CourseName,
                                 CompletionDate = e.CourseCompletionDate,
                                 StartDate = e.CourseStartDate,
                                 Grade = e.StudentGrade
                             }).Distinct().ToList();

                List<ListingStudentTranscript> newList = new List<ListingStudentTranscript>();
                foreach (var e in query)
                {
                    e.CompletionDate_string = e.CompletionDate.ToShortDateString();
                    e.StartDate_string = e.StartDate.ToShortDateString();
                    newList.Add(e);
                }
                return query;



            }
        }
        public static GridModel<ListingStudentModel> GetAllStudents(QueryState state, int courseid = 0, int studentid = 0, string folderpath = "")
        {
            int AssignSup2Stud = 0;
            int SupervisorStudentFilter = 0;
            int supervisorid = 0;
            if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
            {
                AssignSup2Stud = Convert.ToInt32(Settings.Instance.GetMasterInfo3().AssignSup2Stud);
                SupervisorStudentFilter = Convert.ToInt32(Settings.Instance.GetMasterInfo2().SupervisorStudentFilter);
                supervisorid = Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID;
            }
            using (var db = new SchoolEntities())
            {
                //db.Configuration.LazyLoadingEnabled = false;
                //db.Configuration.ProxyCreationEnabled = false;
                //var student = (from s in db.Students
                //               select new ListingStudentModel
                //               {
                //                   StudentId = s.STUDENTID,
                //                   StudentFirstName = s.FIRST,
                //                   StudentLastName = s.LAST

                //               }).ToList();
                //return student;
                var roster = (from a in db.Course_Rosters where a.Cancel == 0 && a.COURSEID == courseid select a).ToList();
                int rostercount = roster.Count();
                int multipleenrollmentcount = 0;
                int maxenroll = 0;

                CourseModel coursemodel = null;

                if (courseid != 0)
                {
                    coursemodel = new CourseModel(courseid);
                    maxenroll = coursemodel.Course.MAXENROLL.Value + coursemodel.Course.MAXWAIT.Value;
                }

                var query = from e in db.Students
                            select new ListingStudentModel
                            {
                                StudentId = e.STUDENTID,
                                StudentFirstName = e.FIRST,
                                StudentLastName = e.LAST,
                                Email = e.EMAIL,
                                UserName = e.USERNAME,
                                Enrolled = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.Cancel == 0 && a.WAITING == 0 select e.STUDENTID).Count(),
                                Waiting = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.WAITING != 0 && a.Cancel == 0 select e.STUDENTID).Count(),
                                HasBalance = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.Cancel == 0 && a.PaidInFull == 0 select e.STUDENTID).Count(),
                                Complete = (from c in db.Transcripts
                                            join cr in db.Course_Rosters on c.CourseId equals cr.COURSEID
                                            where c.STUDENTID == e.STUDENTID &&
                                            c.ATTENDED != 0 && cr.ATTENDED != 0 &&
                                            cr.Cancel == 0
                                            select new { cid = c.CourseId }).Distinct().Count(),
                                //inCheckout =(from b in CourseShoppingCart.Instance.MultipleStudentCourses where b.StudentId == e.STUDENTID && b.CourseId == courseid select e.STUDENTID).Count()
                                Isenrolled = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.Cancel == 0 && a.COURSEID == courseid select a.STUDENTID).Count(),
                                AssignSup2StudList = (from a in db.SupervisorStudents where a.studentid == e.STUDENTID && a.SupervisorID == supervisorid select a.SupervisorID).Count(),
                                District = (from a in db.Supervisors where a.DISTRICT == e.DISTRICT && a.SUPERVISORID == supervisorid select a.SUPERVISORID).Count(),
                                SupStudSchool = (from a in db.SupervisorSchools where a.SchoolID == e.SCHOOL && a.SupervisorID == supervisorid select a.SupervisorID).Count(),
                                InActive = e.InActive
                                //IsErroriNRequirements = !CourseShoppingCart.Instance.CheckCourseRequirements(courseid, e.STUDENTID)
                            };

                if (Authorization.AuthorizationHelper.CurrentInstructorUser != null || Authorization.AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null || AuthorizationHelper.CurrentStudentUser != null)
                {
                    query = (from e in db.Students
                             select e)
                            .Select(e => new ListingStudentModel()
                            {
                                StudentId = e.STUDENTID,
                                StudentFirstName = e.FIRST,
                                StudentLastName = e.LAST,
                                Email = e.EMAIL,
                                UserName = e.USERNAME,
                                Enrolled = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.Cancel == 0 && a.WAITING == 0 select e.STUDENTID).Count(),
                                Waiting = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.WAITING != 0 && a.Cancel == 0 select e.STUDENTID).Count(),
                                Complete = (from c in db.Transcripts
                                            join cr in db.Course_Rosters on c.CourseId equals cr.COURSEID
                                            where c.STUDENTID == e.STUDENTID &&
                                            c.ATTENDED != 0 && cr.ATTENDED != 0 &&
                                            cr.Cancel == 0
                                            select new { cid = c.CourseId }).Distinct().Count(),
                                Isenrolled = (from a in db.Course_Rosters where a.STUDENTID == e.STUDENTID && a.Cancel == 0 && a.COURSEID == courseid select a.STUDENTID).Count(),
                                AssignSup2StudList = 0,
                                District = 0,
                                SupStudSchool = 0,
                                InActive = e.InActive,
                                CreatedBy = e.CreatedBy
                                //IsErroriNRequirements = !CourseShoppingCart.Instance.CheckCourseRequirements(courseid, e.STUDENTID)
                            });

                }
                if (AuthorizationHelper.CurrentStudentUser != null)
                {
                    if (Settings.Instance.GetMasterInfo3().restrictStudentMultiSignup == 1)
                    {
                        query = query.Where(e => e.CreatedBy == AuthorizationHelper.CurrentStudentUser.STUDENTID);
                    }
                }
                if (state.Filters != null)
                {
                    if (state.Filters.ContainsKey("keyword"))
                    {
                        var keyword = state.Filters["keyword"];
                        query = query.Where(e => e.StudentFirstName.Contains(keyword) || e.StudentLastName.Contains(keyword) || e.UserName.Contains(keyword));
                    }
                    else if (state.Filters.ContainsKey("InActive"))
                    {
                        int inActiveValue = Convert.ToInt32(state.Filters["InActive"].ToString());
                        if (inActiveValue == 1)
                        {
                            query = query.Where(e => e.InActive == 0);
                        }
                    }
                }
                // this filter will be different than old Classic version
                if (SupervisorStudentFilter == 99)
                {
                    if ((AssignSup2Stud == 1))
                    {
                        query = query.Where(e => e.AssignSup2StudList > 0);
                    }
                    else if ((AssignSup2Stud == 0))
                    {
                        query = query.Where(e => e.District > 0);
                    }
                }
                //and
                else if (SupervisorStudentFilter == 0)
                {
                    if ((AssignSup2Stud == 1))
                    {
                        query = query.Where(e => (e.District > 0 && e.SupStudSchool > 0) || e.AssignSup2StudList > 0);
                    }
                    else
                    {
                        if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                        {
                            query = query.Where(e => e.District > 0 && e.SupStudSchool > 0);
                        }
                    }
                }
                //or
                else if (SupervisorStudentFilter == 1)
                {
                    if ((AssignSup2Stud == 1))
                    {
                        query = query.Where(e => e.District > 0 || e.SupStudSchool > 0 || e.AssignSup2StudList > 0);
                    }
                    else
                    {
                        query = query.Where(e => e.District > 0 || e.SupStudSchool > 0);
                    }
                }
                //district only
                //school only

                if (state.OrderFieldString != null)
                {
                    switch (state.OrderFieldString)
                    {
                        case "StudentFirstName":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.StudentFirstName);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.StudentFirstName);
                            }
                            break;

                        case "StudentLastName":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.StudentLastName);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.StudentLastName);
                            }
                            break;


                        case "Email":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.Email);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.Email);
                            }
                            break;
                        case "UserName":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.UserName);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.UserName);
                            }
                            break;
                        case "Enrolled":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.Enrolled);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.Enrolled);
                            }
                            break;
                        case "Complete":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.Complete);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.Complete);
                            }
                            break;
                        case "InActive":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.InActive);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.InActive);
                            }
                            break;
                        case "Waiting":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.Waiting);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.Waiting);
                            }
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(e => e.StudentLastName);
                }




                if (CourseShoppingCart.Instance.MultipleStudentCourses != null)
                {

                    //  query = query.Where(e => e.StudentId != CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent);
                }

                BuildExport(query.ToList(), folderpath);

                List<ListingStudentModel> newList = new List<ListingStudentModel>();

                var model = new GridModel<ListingStudentModel>(query.Count(), state);
                query = model.Paginate(query);
                int multiplestudentcheckedflag = 0;
                foreach (var a in query.ToList())
                {
                    if (CourseShoppingCart.Instance.MultipleStudentCourses != null)
                    {
                        foreach (var item in CourseShoppingCart.Instance.MultipleStudentCourses)
                        {
                            if (item.CourseId == courseid && item.StudentId == a.StudentId)
                            {
                                a.inCheckout = 1;
                            }
                            if ((item.CourseId == courseid) && (multiplestudentcheckedflag == 0))
                            {
                                multipleenrollmentcount = multipleenrollmentcount + 1;
                            }
                        }
                        multiplestudentcheckedflag = multiplestudentcheckedflag + 1;
                    }

                    newList.Add(a);

                }
                foreach (var item in newList)
                {
                    if ((item.inCheckout != 1) && (maxenroll <= multipleenrollmentcount + rostercount))
                    {
                        item.Isenrolled = 1;
                    }
                    item.AvailableSeats = maxenroll - (multipleenrollmentcount + rostercount);

                    if (item.UserName == "")
                    {
                        item.UserName = item.StudentId.ToString();
                    }
                    if (AuthorizationHelper.CurrentAdminUser != null || AuthorizationHelper.CurrentSubAdminUser != null)
                    {
                        item.IsErroriNRequirements = false;
                    }
                    else
                    {
                        item.IsErroriNRequirements = !CourseShoppingCart.Instance.CheckCourseRequirements(courseid, item.StudentId);
                    }
                }
                model.Result = newList;
                return model;


            }

        }
        public static List<WaitListingStudentModel> GetAllWaitingStudents(bool isApproved = false)
        {
            using (SchoolEntities db = new SchoolEntities())
            {
                int supervisorId = 0;
                int assignSup2Stud = 0;
                int supervisorStudentFilter = 0;
                if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                {
                    supervisorId = Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID;
                    assignSup2Stud = Convert.ToInt32(Settings.Instance.GetMasterInfo3().AssignSup2Stud);
                    supervisorStudentFilter = Convert.ToInt32(Settings.Instance.GetMasterInfo2().SupervisorStudentFilter);
                }
                dynamic enrollWaitlistConfig = new ExpandoObject();
                dynamic enrollWaitlistConfigSecondary = new ExpandoObject();
                enrollWaitlistConfig.Enabled = 1;
                enrollWaitlistConfigSecondary.Enabled = 1;

                if (isApproved)
                {
                    enrollWaitlistConfig.Enrolled = 1;
                    enrollWaitlistConfigSecondary.Waiting = 1;
                }
                string jsonConfig = Newtonsoft.Json.JsonConvert.SerializeObject(enrollWaitlistConfig);
                string jsonConfigSecondary = Newtonsoft.Json.JsonConvert.SerializeObject(enrollWaitlistConfigSecondary);

                jsonConfig = isApproved ? jsonConfig.Replace("}", "") : jsonConfig;
                jsonConfigSecondary = isApproved ? jsonConfigSecondary.Replace("}", "") : jsonConfigSecondary;

                string supervisorQuery = string.Empty;
                string whereQuery = string.Empty;

                string stringQuery = @"
                                    SELECT 
                                    CASE WHEN Cancellable = 1 THEN 
	                                    CASE WHEN DATEADD(DAY, CancelDays, CourseStartDate) > GETDATE() THEN 1 ELSE 0 END 
                                    ELSE 
	                                    0 
                                    END 
                                    as Cancellable,
                                    RosterId,
                                    OrderNumber,
                                    StudentId,
                                    StudentFirstName,
                                    StudentLastName,
                                    Email,
                                    DateAdded,
                                    CourseId,
                                    CourseName,
                                    CourseNumber,
                                    CourseStartDate,
                                    MaxEnroll,
                                    MaxWait,
                                    EnrolledCount,
                                    WaitingCount,
                                    MaxEnroll - EnrolledCount as RemainingSlots,
                                    MaxWait - WaitingCount as RemainingWaitSlots,
                                    EnrollToWaitListConfig,
                                    AssignSup2StudList,
									District,
									SupStudSchool,
                                    TranscriptCount
                                    FROM (
	                                SELECT 
	                                    (CASE WHEN AllowCancel <> 0 THEN
		                                    CASE 
		                                    WHEN AllowCancelOnlyNotPaid = 0 THEN 1
		                                    WHEN AllowCancelOnlyNotPaid = -1 THEN 
		                                    (
			                                    CASE WHEN PaidInFull <> -1 THEN 1 ELSE 0 END
		                                    )
		                                    WHEN AllowCancelOnlyNotPaid = 2 THEN 
		                                    (
			                                    CASE WHEN MaterialCost > 0 THEN 0 ELSE 1 END
		                                    )
		                                    WHEN AllowCancelOnlyNotPaid = 3 THEN 
		                                    (
			                                    CASE WHEN MaterialCost > 0 OR PaidInFull <> 0 THEN 0 ELSE 1 END
		                                    )
		                                    ELSE 1
		                                    END
	                                    END)
	                                    AS 'Cancellable',
	                                    WaitList.*
	                                    FROM (
		                                SELECT 
		                                DISTINCT
                                        (SELECT TOP 1 CourseCancelDays FROM Masterinfo) AS CancelDays,
		                                (SELECT TOP 1 allowcancel FROM Masterinfo) AS AllowCancel, 
		                                (SELECT TOP 1 allowcancelOnlyNotPaid FROM Masterinfo3) AS AllowCancelOnlyNotPaid,
		                                cr.RosterID as RosterId,
		                                cr.OrderNumber, 
		                                cr.PaidInFull,
		                                cr.CourseCost,
		                                ISNULL((SELECT SUM(rm.price * rm.qty_purchased) FROM RosterMaterials rm WHERE rm.rosterid = cr.rosterid),0) as MaterialCost,
                                        s.STUDENTID as StudentId,
		                                s.FIRST as StudentFirstName, 
		                                s.LAST as StudentLastName, 
		                                s.EMAIL as Email,
		                                cr.DATEADDED as DateAdded, 
		                                c.COURSEID as CourseId, 
		                                c.COURSENAME as CourseName, 
		                                c.COURSENUM as CourseNumber, 
                                        c.CourseCloseDays, 
                                        (SELECT TOP 1 COURSEDATE FROM [Course Times] WHERE courseid = c.COURSEID ORDER BY COURSEDATE ASC) as CourseStartDate,
		                                ISNULL(c.MAXENROLL, 0) as MaxEnroll,
		                                ISNULL(c.MAXWAIT, 0) as MaxWait,
                                        ISNULL((SELECT COUNT(*) FROM Transcripts t WHERE t.courseid = c.courseid AND t.studentid = s.studentid),0) as TranscriptCount,
		                                ISNULL((SELECT COUNT(rosterid) FROM [Course Roster] WHERE courseid = c.COURSEID AND Cancel = 0 AND WAITING = 0), 0) as EnrolledCount,
		                                ISNULL((SELECT COUNT(rosterid) FROM [Course Roster] WHERE courseid = c.COURSEID AND Cancel = 0 AND WAITING <> 0), 0) as WaitingCount,
		                                cr.EnrollToWaitListConfig,
                                        (SELECT COUNT(*) FROM SupervisorStudents a where a.studentid = s.STUDENTID AND a.SupervisorID = " + supervisorId + ") AS AssignSup2StudList,"
                                        + "(SELECT COUNT(*) FROM Supervisors a where a.district = s.district AND a.SUPERVISORID = " + supervisorId + ") AS District,"
                                        + "(SELECT COUNT(*) FROM SupervisorSchools a WHERE a.SchoolID = s.SCHOOL AND a.SupervisorID = " + supervisorId + ") AS SupStudSchool "
                                    + " FROM [Course Roster] cr "
                                    + " INNER JOIN Students s ON s.STUDENTID = cr.STUDENTID "
                                    + " INNER JOIN Courses c ON c.COURSEID = cr.COURSEID";

                if (supervisorStudentFilter == 99)
                {

                    if ((assignSup2Stud == 1))
                    {
                        supervisorQuery = " WHERE (AssignSup2StudList > 0)";
                    }
                    else if ((assignSup2Stud == 0))
                    {
                        supervisorQuery = " WHERE (District > 0)";
                    }


                }
                else if (supervisorStudentFilter == 0)
                {
                    if ((assignSup2Stud == 1))
                    {
                        supervisorQuery = " WHERE ((District > 0 AND SupStudSchool > 0) OR AssignSup2StudList > 0)";
                    }
                    else
                    {
                        if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                        {
                            supervisorQuery = " WHERE (District > 0 AND SupStudSchool > 0)";
                        }
                    }

                }
                else if (supervisorStudentFilter == 1)
                {
                    if ((assignSup2Stud == 1))
                    {
                        supervisorQuery = " WHERE (District > 0 OR SupStudSchool > 0 OR AssignSup2StudList > 0)";
                    }
                    else
                    {
                        supervisorQuery = " WHERE (District > 0 OR SupStudSchool > 0)";
                    }

                }
                // IF SECOND TAB IS ACCESSED GET ALL RECORDS THAT ARE BOTH ENROLLED AND ARE STILL WAITING
                string isApprovedQuery = isApproved ? "cr.Cancel = 0" : "cr.WAITING <> 0 AND cr.Cancel = 0";
                whereQuery = " WHERE (" + isApprovedQuery + ") AND (cr.EnrollToWaitListConfig LIKE '%" + jsonConfig + "%' OR cr.EnrollToWaitListConfig LIKE '%" + jsonConfigSecondary + "%')) as WaitList";
                stringQuery += whereQuery + supervisorQuery + (isApproved ? " AND TranscriptCount = 0" : "") + ") As WaitList2 ORDER BY DateAdded DESC";

                var data = db.Database.SqlQuery<WaitListingStudentModel>(stringQuery).ToList();
                return data;
            };
        }

        public static void ApproveWaitingStudent(int rosterid, bool sendEmail = true)
        {
            Student.EnrollmentFunction enrollmentFunction = new Student.EnrollmentFunction();
            enrollmentFunction.STMoveToEnroll(rosterid, sendEmail);
        }

        public static void MoveToApproveToWaitStudent(int rosterid)
        {
            if (WebConfiguration.EnrollToWaitList)
            {
                using (SchoolEntities db = new SchoolEntities())
                {
                    dynamic enrollWaitlistConfig = new ExpandoObject();
                    enrollWaitlistConfig.Enabled = 1;
                    enrollWaitlistConfig.Waiting = 1;
                    enrollWaitlistConfig.WaitingDate = DateTime.Now.ToShortDateString();
                    string jsonConfig = Newtonsoft.Json.JsonConvert.SerializeObject(enrollWaitlistConfig);
                    var roster = db.Course_Rosters.Where(cr => cr.RosterID == rosterid).SingleOrDefault();
                    if (roster != null)
                    {
                        int maxEnrolled = roster.Course.MAXENROLL ?? 0;
                        int courseid = roster.Course.COURSEID;
                        var course_roster_totalenroll = (from cr in db.Course_Rosters where cr.Cancel == 0 && cr.COURSEID == courseid && cr.WAITING == 0 select cr).Count();
                        if (maxEnrolled > course_roster_totalenroll)
                        {
                            EnrollmentFunction enrollmentFunction = new EnrollmentFunction();
                            enrollmentFunction.STMoveToEnroll(rosterid, true);
                        }
                        else
                        {
                            string query = @"UPDATE [Course Roster] SET EnrollToWaitListConfig = '" + jsonConfig + "' WHERE rosterid = " + rosterid;
                            db.Database.ExecuteSqlCommand(query);
                            Gsmu.Api.Networking.Mail.EmailFunction emailFunction = new Gsmu.Api.Networking.Mail.EmailFunction();
                            emailFunction.SendWaitListToApproveEmail(roster);
                        }
                    }
                }
            }
        }




        public static bool ValidateStudentSupervisor(int studentid)
        {
            int AssignSup2Stud = 0;
            int SupervisorStudentFilter = 0;
            if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
            {
                AssignSup2Stud = Convert.ToInt32(Settings.Instance.GetMasterInfo3().AssignSup2Stud);
                SupervisorStudentFilter = Convert.ToInt32(Settings.Instance.GetMasterInfo2().SupervisorStudentFilter);
            }
            using (var db = new SchoolEntities())
            {
                var query = from e in db.Students
                            where e.STUDENTID == studentid
                            select new ListingStudentModel
                            {
                                StudentId = e.STUDENTID,
                                InActive = e.InActive

                            };
                if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                {
                     query = from e in db.Students
                                where e.STUDENTID == studentid
                                select new ListingStudentModel
                                {
                                    StudentId = e.STUDENTID,
                                    AssignSup2StudList = (from a in db.SupervisorStudents where a.studentid == e.STUDENTID && a.SupervisorID == Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select a.SupervisorID).Count(),
                                    District = (from a in db.Supervisors where a.DISTRICT == e.DISTRICT && a.SUPERVISORID == Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select a.SUPERVISORID).Count(),
                                    SupStudSchool = (from a in db.SupervisorSchools where a.SchoolID == e.SCHOOL && a.SupervisorID == Gsmu.Api.Authorization.AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID select a.SupervisorID).Count(),
                                    InActive = e.InActive

                                };
                    // this filter will be different than old Classic version
                    if (SupervisorStudentFilter == 99)
                    {
                        if ((AssignSup2Stud == 1))
                        {
                            query = query.Where(e => e.AssignSup2StudList > 0);
                        }
                        else if ((AssignSup2Stud == 0))
                        {
                            query = query.Where(e => e.District > 0);
                        }
                    }
                    //and
                    else if (SupervisorStudentFilter == 0)
                    {
                        if ((AssignSup2Stud == 1))
                        {
                            query = query.Where(e => (e.District > 0 && e.SupStudSchool > 0) || e.AssignSup2StudList > 0);
                        }
                        else
                        {
                            if (Authorization.AuthorizationHelper.CurrentSupervisorUser != null)
                            {
                                query = query.Where(e => e.District > 0 && e.SupStudSchool > 0);
                            }
                        }
                    }
                    //or
                    else if (SupervisorStudentFilter == 1)
                    {
                        if ((AssignSup2Stud == 1))
                        {
                            query = query.Where(e => e.District > 0 || e.SupStudSchool > 0 || e.AssignSup2StudList > 0);
                        }
                        else
                        {
                            query = query.Where(e => e.District > 0 || e.SupStudSchool > 0);
                        }
                    }

                }
                if (query.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
