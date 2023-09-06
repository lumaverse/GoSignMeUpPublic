using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using school = Gsmu.Api.Data.School.Entities;
using cs = Gsmu.Api.Data.School.Course;

using Newtonsoft.Json;
using System.Web.Mvc;
using System.Web;
using System.Data.Entity;
using System.Data;

using Gsmu.Api.Language;

namespace Gsmu.Api.Data.School.Attendance
{
    public class AttendanceModel
    {
        public cs.CourseModel CourseModel { get; set; }

        public List<school.Course_Roster> Rosters { get; set; }

        public List<school.Student> Students { get; set; }

        public List<school.AttendanceStatu> AttendanceStatus { get; set; }
        public List<school.AttendanceDetail> AttendanceDetails { get; set; }
        public List<school.Attendance> Attendance { get; set; }
        public List<school.Transcript> Transcripts { get; set; }

        public AttendanceModel(int courseId)
        {
            using (var db = new school.SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                CourseModel = new cs.CourseModel(db, courseId, true);

                // make sure this follows configuration options
                Rosters = CourseModel.Course.AvailableRosters.ToList();

                Students = Rosters.Where(r => r.STUDENTID != null).GroupBy(r => r.STUDENTID).Select(g => school.Student.GetStudent(g.First().STUDENTID.Value)).ToList();

                AttendanceStatus = (from atts in db.AttendanceStatus select atts).ToList();

                Attendance = (from a in db.Attendances where a.COURSEID == courseId select a).ToList();

                AttendanceDetails = (from ad in db.AttendanceDetails where ad.CourseID == courseId select ad).ToList();

                Transcripts = (from t in db.Transcripts where t.CourseId == courseId select t).ToList();

            }
        }
        public static QueryState BuildRequestQuery(HttpRequestBase Request)
        {
            var jresult = new JsonResult();
            var filter = @"[{'property':'keyword','value':''}]";
            if (Request.QueryString["filter"] != null)
            {
                filter = Request.QueryString["filter"];
            }

            var sort = "[{'property':'coursedateid','direction':'DESC'}]";

            if (Request.QueryString["sort"] != null)
            {
                sort = Request.QueryString["sort"];
            }

            var FldHeader = "[{'property':'CourseName','value':''}]";
            if (Request.QueryString["columns"] != null)
            {
                FldHeader = Request.QueryString["columns"];
            }

            var start = 0;
            if (Request.QueryString["start"] != null)
            {
                start = int.Parse(Request.QueryString["start"]);
            }
            var limit = 50;
            if (Request.QueryString["limit"] != null)
            {
                limit = int.Parse(Request.QueryString["limit"]);
            }
            var page = 1;
            if (Request.QueryString["page"] != null)
            {
                page = int.Parse(Request.QueryString["page"]);
            }

            jresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
            var FldHeaderResult = ExtJsDataStoreHelper.ParseColumns(FldHeader);

            var sorterResult = ExtJsDataStoreHelper.ParseSorter(sort);

            //reparse for initial sorting
            if (sorterResult.Count == 1)
            {
                sort =  "[{'property':'coursedateid','direction':'DESC'}]";
                sorterResult = ExtJsDataStoreHelper.ParseSorter(sort);
            }

            var queryState = new QueryState(start, limit)
            {
                OrderByDirection = sorterResult.Count > 1 ? sorterResult[1].Value : sorterResult[0].Value,
                OrderFieldString = sorterResult.Count > 1 ? sorterResult[1].Key : sorterResult[0].Key,
                Filters = filterResult,
                FldHeaders = FldHeaderResult,
                Page = page
            };

            return queryState;
        }
        public static AttendanceReportResponseModel AddAttendanceReport(HttpRequestBase Request, bool isExport)
        {
            //TODO : Separate concerns
            //Separate Raw query data to filtered data to Exportin
            try
            {
                QueryState queryState = BuildRequestQuery(Request);
                var page = queryState.Page;
                var start = (queryState.Page - 1) * queryState.PageSize;
                var limit = queryState.PageSize;
                string keyword = string.Empty;
                string dateFrom = string.Empty;
                string dateTo = string.Empty;
                string mainCategory = string.Empty;
                string subCategory = string.Empty;
                string subSubCategory = string.Empty;
                string district = string.Empty;
                string school = string.Empty;
                string grade = string.Empty;
                string sturegfld1 = string.Empty;
                bool includeCancelled = true;
                string exportedFileName = string.Empty;
                if (queryState.Filters != null)
                {
                    //export
                    if (queryState.Filters.ContainsKey("export"))
                    {
                        string exportcmd = queryState.Filters["export"];
                        if (exportcmd == "exportall") { isExport = true; }
                    }
                    //filter keyword
                    if (queryState.Filters.ContainsKey("keyword"))
                    {
                        keyword = queryState.Filters["keyword"];
                    }
                    //filter  date-from
                    if (queryState.Filters.ContainsKey("date-from"))
                    {
                        dateFrom = queryState.Filters["date-from"];
                    }
                    else
                    {
                        dateFrom = DateTime.Now.AddDays(-15).Date.ToString();
                    }
                    //filter date-to
                    if (queryState.Filters.ContainsKey("date-to"))
                    {
                        dateTo = queryState.Filters["date-to"];
                    }
                    else
                    {
                        dateTo = DateTime.Now.AddDays(15).ToString();
                    }
                    //filter cancelledtext
                    if (queryState.Filters.ContainsKey("cancelledtxt"))
                    {
                        string filterValue = queryState.Filters["cancelledtxt"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            //by default it loads the non cancelled records to
                            //enable this to include cancelled
                            if (Convert.ToInt32(filterValue) == 1)
                            {
                                includeCancelled = true;
                            }
                        }
                    }
                    //filter category-main
                    if (queryState.Filters.ContainsKey("category-main"))
                    {
                        string filterValue = queryState.Filters["category-main"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            mainCategory = filterValue;

                        }
                    }
                    //filter category-sub
                    if (queryState.Filters.ContainsKey("category-sub"))
                    {
                        string filterValue = queryState.Filters["category-sub"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            subCategory = filterValue;
                        }
                    }
                    //filter category-subsub
                    if (queryState.Filters.ContainsKey("category-subsub"))
                    {
                        string filterValue = queryState.Filters["category-subsub"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            subSubCategory = filterValue;
                        }
                    }
                    //filter district
                    if (queryState.Filters.ContainsKey("district-fltr"))
                    {
                        string filterValue = queryState.Filters["district-fltr"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            district = filterValue;
                        }
                    }
                    //filter school
                    if (queryState.Filters.ContainsKey("school-fltr"))
                    {
                        string filterValue = queryState.Filters["school-fltr"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            school = filterValue;
                        }
                    }
                    //filter grade
                    if (queryState.Filters.ContainsKey("grade-fltr"))
                    {
                        string filterValue = queryState.Filters["grade-fltr"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            grade = filterValue;
                        }
                    }
                    //filter sturegfld1
                    if (queryState.Filters.ContainsKey("sturegfld1"))
                    {
                        string filterValue = queryState.Filters["sturegfld1"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            sturegfld1 = filterValue;
                        }
                    }                
                }
                else
                {
                    dateFrom = DateTime.Now.AddDays(-15).Date.ToString();
                    dateTo = DateTime.Now.AddDays(15).ToString();
                }
                using (var db = new school.SchoolEntities())
                {
                    DateTime fromDate = Convert.ToDateTime(dateFrom);
                    DateTime toDate = Convert.ToDateTime(dateTo);
                    if (Settings.Instance.GetMasterInfo2().AllowAttendanceDetail == 0)
                    {
                        //when using 1 attendance for all date
                    }
                    else
                    {
                        // multi days attendance
                    }
                    var attendanceReportData = (from cr in db.Course_Rosters
                                                join s in db.Students on cr.STUDENTID equals s.STUDENTID
                                                join di in db.Districts on s.DISTRICT equals di.DISTID into dii 
                                                     from diii in
                                                            dii.Where(t => t.DISTID == s.DISTRICT).DefaultIfEmpty()
                                               join sc in db.Schools on s.SCHOOL equals sc.locationid into scc
                                                    from sccc in
                                                        scc.Where(t=>t.locationid == s.SCHOOL).DefaultIfEmpty()
                                                join gl in db.Grade_Levels on s.GRADE equals gl.GRADEID into gll 
                                                     from glll in
                                                        gll.Where(t=> t.GRADEID == s.GRADE).DefaultIfEmpty()
                                                join c in db.Courses on cr.COURSEID equals c.COURSEID
                                                join ct in db.Course_Times on c.COURSEID equals ct.COURSEID
                                                join a in db.AttendanceDetails on cr.RosterID equals a.RosterId
                                                join t in db.Transcripts on c.COURSEID equals t.CourseId into tt
                                                from ttt in
                                                    tt.Where(t => t.STUDENTID == s.STUDENTID && t.CourseId == c.COURSEID).DefaultIfEmpty()
                                                where
                                                (
                                                    ((cr.Cancel == 0)) &&
                                                    (c.CANCELCOURSE == 0 && c.COURSENAME != "~ZZZZZZ~")
                                                ) &&
                                                (DbFunctions.TruncateTime(ct.COURSEDATE.Value) >= fromDate &&
                                                 DbFunctions.TruncateTime(ct.COURSEDATE.Value) <= toDate) && // attendance from last year till today
                                                 (
                                                    DbFunctions.TruncateTime(a.CourseDate).ToString().Contains(DbFunctions.TruncateTime(ct.COURSEDATE.Value).ToString())
                                                 )
                                                select new AttendanceReportModel
                                                {
                                                    Instructor1 = c.INSTRUCTORID,
                                                    Instructor2 = c.INSTRUCTORID2,
                                                    Instructor3 = c.INSTRUCTORID3,
                                                    RosterId = cr.RosterID,
                                                    CourseId = c.COURSEID,
                                                    CourseNum = c.COURSENUM,
                                                    CourseName = c.COURSENAME,
                                                    StudentId = s.STUDENTID,
                                                    StudRegField1 = s.StudRegField1,
                                                    StudentFirstName = s.FIRST,
                                                    StudentUsername = s.USERNAME,
                                                    //District = string.IsNullOrEmpty(ttt.District.ToString()) ?"" : ttt.District,
                                                    //School = string.IsNullOrEmpty(ttt.StudentsSchool.ToString()) ?"": ttt.StudentsSchool,
                                                    //GradeLevel = string.IsNullOrEmpty(ttt.GradeLevel.ToString()) ? "": ttt.GradeLevel,
                                                    District = string.IsNullOrEmpty(ttt.District.ToString()) ? diii.DISTRICT1 : ttt.District,
                                                    School = string.IsNullOrEmpty(ttt.StudentsSchool.ToString()) ? sccc.LOCATION : ttt.StudentsSchool,
                                                    GradeLevel = string.IsNullOrEmpty(ttt.GradeLevel.ToString()) ? glll.GRADE : ttt.GradeLevel,
                                                    StudentLast = s.LAST,
                                                    StudentEmail = s.EMAIL,
                                                    StudRegField3 = s.StudRegField3,
                                                    AttendanceDate = a.CourseDate,
                                                    AttendanceDetailId = a.AttendanceDetailId,
                                                    Attended = a.Attended,
                                                    AttendedHours = a.AttendedHours, //clock hours
                                                    InserviceHours = cr.InserviceHours,
                                                    CustomCreditHours = cr.CustomCreditHours,
                                                    CEUCredits = cr.ceucredit,
                                                    GraduateCredits = cr.graduatecredit,
                                                    Grade = cr.StudentGrade,
                                                    Addresss = c.LOCATION,
                                                    CourseLocation = c.LOCATION + " "
                                                                                + (string.IsNullOrEmpty(c.STREET) ? "" : ", " + c.STREET)
                                                                                + (string.IsNullOrEmpty(c.CITY) ? "" : ", " + c.CITY)
                                                                                + (string.IsNullOrEmpty(c.STATE) ? "" : ", " + c.STATE)
                                                                                + (string.IsNullOrEmpty(c.ZIP) ? "" : ", " + c.ZIP)
                                                })
                                                .Distinct()
                                                .ToList();
                    if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser != null)
                    {
                        attendanceReportData = attendanceReportData.Where(mc => mc.Instructor1 == Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID ||
                            mc.Instructor2 == Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID ||
                            mc.Instructor3 == Gsmu.Api.Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID
                            ).ToList();
                    }
                    List<AttendanceReportModel> attendanceReportDataFinal
                                                = attendanceReportData.Select(a => new AttendanceReportModel
                                                {
                                                    RosterId = a.RosterId,
                                                    CourseId = a.CourseId,
                                                    CourseNum = a.CourseNum,
                                                    CourseName = a.CourseName,
                                                    StudentId = a.StudentId,
                                                    StudRegField1 = a.StudRegField1,
                                                    StudentFirstName = a.StudentFirstName,
                                                    StudentLast = a.StudentLast,
                                                    StudentUsername = a.StudentUsername,
                                                    StudentEmail = a.StudentEmail,
                                                    StudRegField3 = a.StudRegField3,
                                                    District = a.District,
                                                    School = a.School,
                                                    GradeLevel = a.GradeLevel,
                                                    AttendanceDate = a.AttendanceDate,
                                                    AttendanceDateString = a.AttendanceDate.Date.ToString("MM/dd/yyyy"),
                                                    Attended = a.Attended,
                                                    AttendedHours = a.AttendedHours.HasValue ? a.AttendedHours.Value : 0,
                                                    InserviceHours = a.InserviceHours.HasValue ? a.InserviceHours.Value : 0,
                                                    CustomCreditHours = a.CustomCreditHours.HasValue ? a.CustomCreditHours.Value : 0,
                                                    CEUCredits = a.CEUCredits.HasValue ? a.CEUCredits.Value : 0,
                                                    GraduateCredits = a.GraduateCredits.HasValue ? a.GraduateCredits.Value : 0,
                                                    Grade = a.Grade,
                                                    CourseDateStart = (from ct in db.Course_Times
                                                                       where ct.COURSEID == a.CourseId
                                                                       select ct).ToList().OrderBy(t => t.COURSEDATE).FirstOrDefault().COURSEDATE.Value.Date.ToString("MM/dd/yyyy"),
                                                    CourseDateEnd = (from ct in db.Course_Times
                                                                     where ct.COURSEID == a.CourseId
                                                                     select ct).ToList().OrderByDescending(t => t.COURSEDATE).FirstOrDefault().COURSEDATE.Value.Date.ToString("MM/dd/yyyy"),
                                                    Addresss = a.Addresss,
                                                    CourseLocation = a.CourseLocation,
                                                    MainCategories = String.Join(", ", (from mc in db.MainCategories
                                                                      where mc.CourseID == a.CourseId
                                                                      orderby mc.MainOrder ascending
                                                                      select mc.MainCategory1).ToArray<String>()),
                                                    SubCategories = String.Join(", ", (from mc in db.MainCategories
                                                                      join sc in db.SubCategories on mc.MainCategoryID equals sc.MainCategoryID
                                                                      where mc.CourseID == a.CourseId
                                                                      orderby mc.MainOrder ascending
                                                                      select sc.SubCategory1).ToArray<String>()),
                                                    //Total Attended Hours is Clockhours
                                                    TotalAttendedHours = IsLastAttendanceStudentRecord(a.AttendanceDetailId, a.RosterId) == 1 ? db.AttendanceDetails.Where(ad => ad.RosterId == a.RosterId).Select(ads => ads.AttendedHours.HasValue ? ads.AttendedHours.Value : 0).Sum() : -1,
                                                    //Removed Clock hours, hours column on course roster is the total of the attended hours on attendance detail
                                                    //TotalClockHours = IsLastAttendanceStudentRecord(a.AttendanceDetailId, a.RosterId) == 1 ? db.Course_Rosters.Where(cr => cr.COURSEID == a.CourseId && cr.STUDENTID == a.StudentId).Select(ads => ads.HOURS ?? 0).Sum()  : -1,
                                                    //Removed Total Values - they are not needed
                                                    //as per Anthony
                                                    //TotalCreditsHours = IsLastAttendanceStudentRecord(a.AttendanceDetailId, a.RosterId) == 1 ? db.Course_Rosters.Where(cr => cr.COURSEID == a.CourseId && cr.STUDENTID == a.StudentId).Select(ads => ads.ceucredit ?? 0).Sum()  : -1,
                                                    //TotalGraduateCreditsHours = IsLastAttendanceStudentRecord(a.AttendanceDetailId, a.RosterId) == 1 ? db.Course_Rosters.Where(cr => cr.COURSEID == a.CourseId && cr.STUDENTID == a.StudentId).Select(ads => ads.graduatecredit ?? 0).Sum() : -1,
                                                    LastAttendanceRecord = IsLastAttendanceStudentRecord(a.AttendanceDetailId, a.RosterId),
                                                    AttendanceDetailId = a.AttendanceDetailId
                                                })
                        //.OrderBy(ad => ad.RosterId).ThenBy(ad => ad.AttendanceDate)
                                                .ToList();
                    //filter phase
                    if (keyword != string.Empty)
                    {
                        keyword = keyword.ToLower();
                        attendanceReportDataFinal = attendanceReportDataFinal
                            .Where(a =>
                            (a.CourseId.ToString().ToLower().StartsWith(keyword) || a.CourseId.ToString().ToLower().EndsWith(keyword)) ||
                            (a.CourseName.ToLower().StartsWith(keyword) || a.CourseName.ToLower().EndsWith(keyword)) ||
                            (a.CourseNum.ToLower().StartsWith(keyword) || a.CourseNum.ToLower().EndsWith(keyword)) ||
                            (a.StudentFirstName.ToLower().StartsWith(keyword) || a.StudentFirstName.ToLower().EndsWith(keyword)) ||
                            (a.StudentLast.ToLower().StartsWith(keyword) || a.StudentLast.ToLower().EndsWith(keyword)) ||
                            (a.CourseLocation.ToLower().StartsWith(keyword) || a.CourseLocation.ToLower().EndsWith(keyword))
                            ).OrderBy(ad => ad.RosterId).ThenBy(ad => ad.AttendanceDate).ToList();
                    }
                    if (mainCategory != string.Empty)
                    {
                        var mainCategoryCourseIds = (from mc in db.MainCategories where mc.MainCategory1.StartsWith(mainCategory) select mc.CourseID).ToList();
                        if (mainCategoryCourseIds.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => mainCategoryCourseIds.Contains(mc.CourseId)).ToList();
                        }
                    }
                    if (subCategory != string.Empty)
                    {
                        var subCategoryCourseIds = (from sc in db.SubCategories
                                                    join mc in db.MainCategories on sc.MainCategoryID equals mc.MainCategoryID
                                                    where sc.SubCategory1.StartsWith(subCategory)
                                                    select sc.MainCategoryID).ToList();
                        if (subCategoryCourseIds.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => subCategoryCourseIds.Contains(mc.CourseId)).ToList();
                        }
                    }
                    if (subSubCategory != string.Empty)
                    {
                        var subCategoryCourseIds = (from sc in db.SubSubCategories
                                                    join mc in db.MainCategories on sc.MainCategoryID equals mc.MainCategoryID
                                                    where sc.SubSubCategory1.StartsWith(subSubCategory)
                                                    select sc.MainCategoryID).ToList();
                        if (subCategoryCourseIds.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => subCategoryCourseIds.Contains(mc.CourseId)).ToList();
                        }
                    }
                    if (district != string.Empty)
                    {
                        List<string> districtlst = district.Split(',').ToList();
                        if (districtlst.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => districtlst.Contains(mc.District)).ToList();
                        }
                    }
                    if (school != string.Empty)
                    {
                        List<string> schoollst = school.Split(',').ToList();
                        if (schoollst.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => schoollst.Contains(mc.School)).ToList();
                        }
                    }
                    if (grade != string.Empty)
                    {
                        List<string> gradelst = grade.Split(',').ToList();
                        if (gradelst.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => gradelst.Contains(mc.GradeLevel)).ToList();
                        }
                    }
                    if (sturegfld1 != string.Empty)
                    {
                        List<string> sturegfld1lst = sturegfld1.Split(',').ToList();
                        if (sturegfld1lst.Count() > 0)
                        {
                            attendanceReportDataFinal = attendanceReportDataFinal.Where(mc => sturegfld1lst.Contains(mc.StudRegField1)).ToList();
                        }
                    }

                    //order phase
                    if (isExport)
                    {
                        //TODO : Make a way to use of threading
                        //create a separate thread to execute exporting
                        //since this may take time to finish
                        //Task mytask = Task.Run(() =>
                        //{
                        //    ExportRosterToExcel(attendanceReportDataFinal, queryState);
                        //});
                        exportedFileName = ExportRosterToExcel(attendanceReportDataFinal, queryState);
                    }

                    var attended = attendanceReportDataFinal.Count(item => item.Attended == 1);
                    var didntattend = attendanceReportDataFinal.Count(item => item.Attended != 1);                    
                    var recordCount = attendanceReportDataFinal.Count();

                    if (queryState.OrderFieldString.Length > 0)
                    {
                        if (queryState.OrderFieldString == "coursenameid")
                        {
                            if (queryState.OrderByDirection == OrderByDirection.Descending)
                            {
                                attendanceReportDataFinal = attendanceReportDataFinal.OrderByDescending(e => e.CourseName).Skip((page - 1) * limit).Take(limit).ToList();
                            }
                            else
                            {
                                attendanceReportDataFinal = attendanceReportDataFinal.OrderBy(e => e.CourseName).Skip((page - 1) * limit).Take(limit).ToList();
                            }

                        }

                        if (queryState.OrderFieldString == "coursedateid")
                        {
                            if (queryState.OrderByDirection == OrderByDirection.Descending)
                            {
                                attendanceReportDataFinal = attendanceReportDataFinal.OrderByDescending(e => e.AttendanceDate).Skip((page - 1) * limit).Take(limit).ToList();
                            }
                            else
                            {
                                attendanceReportDataFinal = attendanceReportDataFinal.OrderBy(e => e.AttendanceDate).Skip((page - 1) * limit).Take(limit).ToList();
                            }

                        }
                    }


                    return new AttendanceReportResponseModel()
                    {
                        attendanceReportList = attendanceReportDataFinal,
                        recordCount = recordCount,
                        registered = recordCount,
                        attended = attended,
                        didntattend = didntattend,                        
                        exportFileName = exportedFileName
                    };
                }


            }
            catch (Exception ex)
            {
                return new AttendanceReportResponseModel()
                {
                    errorMessage = ex.Message
                };
            }

        }
        //determines if the record on the list is the last record
        //this is used to mark where the total values should be added
        //this is also used as flag where to put the total values on excel
        public static int IsLastAttendanceStudentRecord(int attendanceDetId, int rosterId)
        {
            using (var db = new school.SchoolEntities())
            {
                var lastRecord = (from ad in db.AttendanceDetails
                                  where ad.RosterId == rosterId
                                  select ad).OrderByDescending(ad => ad.CourseDate).ToList();
                if (lastRecord.FirstOrDefault() != null)
                {
                    if (lastRecord.FirstOrDefault().AttendanceDetailId == attendanceDetId)
                    {
                        return 1;
                    }
                }
                if (lastRecord.Count() == 1)
                {
                    return 1;
                }
            };

            return 0;
        }
        public static string ExportRosterToExcel(List<AttendanceReportModel> AddAttendanceReportData, QueryState queryState)
        {
            try
            {
                string exportFileName = "AttendanceReport" + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".csv";
                string directory = AppDomain.CurrentDomain.BaseDirectory + @"Temp\";
                StringBuilder sb = new StringBuilder();
                Dictionary<string, string> FldHeadersText = queryState.FldHeaders;

                DataTable attendanceListTable = AddAttendanceReportData.ToDataTable();
                foreach (var dta in attendanceListTable.Columns.Cast<DataColumn>())
                {
                    string nwText = dta.ColumnName;
                    string noval = "";
                    bool hasVal = FldHeadersText.TryGetValue(dta.ColumnName, out noval);
                    if (hasVal)
                    {
                        nwText = FldHeadersText[dta.ColumnName];
                        dta.Caption = nwText;
                    }
                    else
                    {
                        dta.Caption = null;
                    }
                }
                sb.AppendLine(string.Join(",", queryState.FldHeaders.Values));
                foreach (DataRow row in attendanceListTable.Rows)
                {
                    var fields = new List<string>();
                    foreach (var col in queryState.FldHeaders)
                    {
                        string dta = row[col.Key].ToString().Replace(",", " ");
                        if (col.Key == "Attended")
                        {
                            dta = dta == "0" ? "No" : "Yes";
                        }
                        else if (col.Key == "TotalAttendedHours")
                        {
                            if (dta == "-1" || dta == "-1.0")
                            {
                                dta = "";
                            }
                        }
                        fields.Add(dta);
                    }
                    sb.AppendLine(string.Join(",", fields.ToArray()));
                }
                System.IO.File.WriteAllText(directory + exportFileName, sb.ToString());
                return exportFileName;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //TODO : Transfer to a model directory/folder
        //Model for the Attendance Report
        public class AttendanceReportModel
        {
            public int RosterId { get; set; }
            public int CourseId { get; set; }
            public string CourseNum { get; set; }
            public string CourseName { get; set; }
            public string CourseLocation { get; set; }
            public string Addresss { get; set; }
            public int StudentId { get; set; }
            public string StudentFirstName { get; set; }
            public string StudentLast { get; set; }
            public string StudentUsername { get; set; }
            public string District { get; set; }
            public string School { get; set; }
            public string GradeLevel { get; set; }
            public string CourseDateStart { get; set; }
            public string CourseDateEnd { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string AttendanceDateString { get; set; }
            public int Attended { get; set; }
            public float? AttendedHours { get; set; }
            public double? InserviceHours { get; set; }
            public double? CustomCreditHours { get; set; }
            public double? ClockHours { get; set; }
            public double? CEUCredits { get; set; }
            public double? GraduateCredits { get; set; }
            public string Grade { get; set; }
            public double TotalAttendedHours { get; set; }
            public double TotalClockHours { get; set; }
            public double TotalCreditsHours { get; set; }
            public double TotalNumberOfDays { get; set; }
            public double TotalGraduateCreditsHours { get; set; }
            public int LastAttendanceRecord { get; set; }
            public int AttendanceDetailId { get; set; }
			public string MainCategories{ get; set; }
			public string SubCategories{ get; set; }
            public int? Instructor1 { get; set; }
            public int? Instructor2 { get; set; }
            public int? Instructor3 { get; set; }

            public string StudRegField1 { get; set; }
            public string StudentEmail { get; internal set; }
            public string StudRegField3 { get; internal set; }
        }
        public class AttendanceReportResponseModel
        {
            public List<AttendanceReportModel> attendanceReportList { get; set; }
            //make a generic class that can be inherited with these commone properties
            public int recordCount { get; set; }
            public string exportFileName { get; set; }
            public string errorMessage { get; set; }

            public int registered { get; set; }

            public int attended { get; set; }

            public int didntattend { get; set; }
        }


        public static string sturegfld1 { get; set; }
    }
}
