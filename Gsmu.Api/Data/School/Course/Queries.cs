using Gsmu.Api.Data;
using Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.ViewModels.Grid;
using Gsmu.Api.Authorization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace Gsmu.Api.Data.School.Course
{
    /// <summary>
    /// Holds queries realted to courses.
    /// </summary>
    public static class Queries
    {
        /*
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
         */

        /*
         * 
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
         * 
         */

        /*
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
         */

        /// <summary>
        /// EVERY FILTER HERE MUST BE PRESENT IN THE CATEGORY QUERY AS WELL
        /// see <see cref="Gsmu.Api.Data.School.Category.Queries"/>
        /// </summary>
        /// <param name="queryState"></param>
        /// <param name="text"></param>
        /// <param name="mainCategory"></param>
        /// <param name="subCategory"></param>
        /// <param name="subCategoryIsSubSub"></param>
        /// <param name="state"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <param name="courseInternalState"></param>
        /// <param name="cancelState"></param>
        /// <returns></returns>
        public static GridModel<CourseModel> Search(QueryState queryState, string text = null, string mainCategory = null, string subCategory = null,string subsubcattext = null, bool subCategoryIsSubSub = false, CourseActiveState state = CourseActiveState.All, DateTime? from = null, DateTime? until = null, CourseInternalState courseInternalState = CourseInternalState.InternalAndPublic, CourseCancelState cancelState = CourseCancelState.NotCancelled)
        {
            using (var connection = Connections.GetSchoolConnection())
            {
                /*
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 */

                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.Connection = connection;

                var page = queryState.Page;
                var start = (queryState.Page - 1) * queryState.PageSize;
                var limit = queryState.PageSize;

                var mi1 = Settings.Instance.GetMasterInfo();
                var mi2 = Settings.Instance.GetMasterInfo2();
                var mi3 = Settings.Instance.GetMasterInfo3();

                var selectcoursefields = string.Empty;
                selectcoursefields += "c.cancelcourse, C.courseid, c.COURSENAME, c.COURSENUM, c.Location, c.internalclass";
                selectcoursefields += ", c.instructorid, c.instructorid2, c.instructorid3";
                selectcoursefields += ", c.CourseCloseDays";
                selectcoursefields += ", cast(c.DESCRIPTION as nvarchar) as description";
                selectcoursefields += ", i1.first as i1first, i1.last as i1last ";
                selectcoursefields += ", i2.first as i2first, i2.last as i2last ";
                selectcoursefields += ", i3.first as i3first, i3.last as i3last ";

                selectcoursefields += ", (select top 1 convert(datetime, convert(varchar(5),month(coursedate)) + '/' + convert(varchar(5), day(coursedate)) + '/' + convert(varchar(5), year(coursedate)) + ' ' + convert(varchar(5), datepart(hour, STARTTIME)) + ':' + convert(varchar(5), datepart(minute, starttime))) from [course times] where courseid = C.COURSEID order by coursedate asc, STARTTIME asc) as coursestart";
                selectcoursefields += ", (select top 1 convert(datetime, convert(varchar(5),month(coursedate)) + '/' + convert(varchar(5),day(coursedate)) + '/'+ convert(varchar(5), year(coursedate))) from [course times] where courseid = C.COURSEID order by coursedate asc, STARTTIME asc) as coursedate";
                selectcoursefields += ", (select top 1 convert(datetime, convert(varchar(5),datepart(hour, STARTTIME))  + ':' + convert(varchar(5),datepart(minute, starttime))) from [course times] where courseid = C.COURSEID order by coursedate asc, STARTTIME asc) as coursetime";

                /*
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 */

                var groupbyfields = string.Empty;
                groupbyfields += "c.cancelcourse, C.courseid, c.COURSENAME, c.COURSENUM, c.Location, c.internalclass";
                groupbyfields += ", c.instructorid, c.instructorid2, c.instructorid3";
                groupbyfields += ", c.CourseCloseDays, c.viewpastcoursesdays, c.OnlineCourse";
                groupbyfields += ", cast(c.DESCRIPTION as nvarchar)";
                groupbyfields += ", i1.first, i1.last ";
                groupbyfields += ", i2.first, i2.last ";
                groupbyfields += ", i3.first , i3.last ";
                groupbyfields += ", ec.courseid, ec.internalclass ";

                var joinCategoriesMain = false;
                var joinCategoriesSub = false;
                var joinCategoriesSubSub = false;

                var join = string.Empty;
                var where = string.Empty;
                var having = string.Empty;

                if (!string.IsNullOrWhiteSpace(mainCategory))
                {
                    where += " AND cm.maincategory = @mainCategory";
                    joinCategoriesMain = true;
                    cmd.Parameters.Add(new SqlParameter("@mainCategory", mainCategory));
                }

                if (!string.IsNullOrWhiteSpace(subCategory))
                {
                    if (subCategoryIsSubSub)
                    {
                        where += " AND CSS.SubSubCategory = @subsubcattext";
                        joinCategoriesSubSub = true;
                        cmd.Parameters.Add(new SqlParameter("@subsubcattext", subsubcattext));
                        where += " AND CS.SubCategory = @subCategory";
                        joinCategoriesSub = true;
                    }
                    else
                    {
                        where += " AND CS.SubCategory = @subCategory";
                        joinCategoriesSub = true;
                    }
                    subCategory = subCategory.Replace("&amp;", "&");
                    cmd.Parameters.Add(new SqlParameter("@subCategory", subCategory));
                }

                /*
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 */

                if (!string.IsNullOrEmpty(text))
                {
                    where += " AND (";
                    where += " C.COURSEID LIKE @text";
                    where += " OR C.COURSENAME LIKE @text";
                    where += " OR C.COURSENUM LIKE @text";
                    where += " OR C.description LIKE @text";
                    where += " OR C.location LIKE @text";
                    where += " OR i1.first LIKE @text";
                    where += " OR i2.first LIKE @text";
                    where += " OR i3.first LIKE @text";
                    where += " OR i1.last LIKE @text";
                    where += " OR i2.last LIKE @text";
                    where += " OR i3.last LIKE @text";
                    where += " OR (i1.first + ' ' + i1.last) LIKE @text";
                    where += " OR (i2.first + ' ' + i2.last) LIKE @text";
                    where += " OR (i3.first + ' ' + i3.last) LIKE @text";
                    where += ") ";
                    cmd.Parameters.AddWithValue("@text", "%" + text + "%");
                }

                switch (courseInternalState)
                {
                    case CourseInternalState.Internal:
                        if (Gsmu.Api.Authorization.AuthorizationHelper.VisibleInternalCourses == 1)
                        {
                            where += " AND c.internalclass = -1";
                        }
                        else
                        {
                            where += " AND (c.internalclass <> -1 AND c.internalclass <> 0) ";
                        }
                        break;


                    case CourseInternalState.Public:
                        where += " AND c.internalclass = 0";
                        break;
                }

                switch (cancelState)
                {
                    case CourseCancelState.Cancelled:
                        where += " AND (c.cancelcourse = -1 OR c.cancelcourse = 1 ) ";
                        break;

                    case CourseCancelState.NotCancelled:
                        where += " AND NOT (c.cancelcourse = -1 OR c.cancelcourse = 1 ) ";
                        break;
                }

                /*
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 */

                var now = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                var showPastOnlineCourses = mi1.ShowPastOnlineCoursesAsBoolean;
                var allowViewPastCoursesDays = Settings.GetVbScriptBoolValue(mi3.allowviewpastcoursesdays);
                if (Gsmu.Api.Authorization.AuthorizationHelper.CurrentAdminUser != null || Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.LoggedInUserType == Gsmu.Api.Authorization.LoggedInUserType.SubAdmin)
                {
                    showPastOnlineCourses = true;
                    allowViewPastCoursesDays = true;
                }

                    switch (state)
                {
                    case CourseActiveState.Current:
                        if (from == null)
                        {
                            from = now;
                        }
                        break;

                    case CourseActiveState.Past:
                        if (until == null)
                        {
                            until = now;
                        }
                        break;
                }

                if (from != null)
                {
                    var fromDate = from.Value.ToString("M/d/yyyy");
                    having += " AND (";

                    having += " min(coursedate) >= '" + fromDate + "'";
                    if (allowViewPastCoursesDays)
                    {
                        having += " OR ";
                        having += " DATEADD(d, case when c.viewpastcoursesdays is null then 0 else c.viewpastcoursesdays end, min(coursedate)) >= '" + fromDate + "'";
                    }
                    else
                    {
                        having += " OR ";
                        having += " ((DATEADD(d, case when c.viewpastcoursesdays is null then 0 else c.viewpastcoursesdays end, min(coursedate)) >= '" + fromDate + "') AND c.viewpastcoursesdays>0)";
                    }
                    if (showPastOnlineCourses)
                    {
                        having += " OR ";
                        having += " (";
                        having += " min(coursedate) <= '" + fromDate + "'";
                        having += " AND max(coursedate) >= '" + fromDate + "'";
                        having += " AND (c.onlinecourse = 1 OR c.onlinecourse = -1)";
                        having += " )";
                    }
                    having += ") ";
                }

                if (until != null)
                {
                    var untilDate = until.Value.ToString("M/d/yyyy");
                    having += " AND (";

                    having += " min(coursedate) <= '" + untilDate + "'";
                    if (allowViewPastCoursesDays)
                    {
                        having += " OR ";
                        having += " DATEADD(d, case when c.viewpastcoursesdays is null then 0 else c.viewpastcoursesdays end, min(coursedate)) <= '" + untilDate + "'";
                    }
                    else
                    {
                        having += " OR ";
                        having += "(( DATEADD(d, case when c.viewpastcoursesdays is null then 0 else c.viewpastcoursesdays end, min(coursedate)) <= '" + untilDate + "') AND  c.viewpastcoursesdays>0)";

                    }
                    if (showPastOnlineCourses)
                    {
                        having += " OR ";
                        having += " (";
                        having += " max(coursedate) <= '" + untilDate + "'";
                        having += " AND (c.onlinecourse = 1 OR c.onlinecourse = -1)";
                        having += " )";
                    }
                    having += ") ";
                }

                if (joinCategoriesMain)
                {
                    join += " LEFT JOIN MainCategories AS CM ON CM.COURSEID = C.COURSEID ";
                }

                if (joinCategoriesSub)
                {
                    join += " LEFT JOIN SubCategories AS CS ON CS.MainCategoryID = CM.MainCategoryID ";
                }

                if (joinCategoriesSubSub)
                {
                    join += " LEFT JOIN SubSubCategories AS CSS ON CSS.MainCategoryID = CM.MainCategoryID ";
                }

                var orderby = CalculateOrderBy(queryState);

                /*
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.CATEGORY.QUERIES!!!!
                 */

                var sql = " With Query AS (";
                sql += "SELECT ";
                sql += selectcoursefields;
                sql += " FROM courses AS C";
                sql += " LEFT JOIN [Course Times] AS CT ON C.COURSEID = CT.COURSEID";
                sql += " LEFT JOIN Instructors AS I1 ON I1.instructorid = C.instructorid";
                sql += " LEFT JOIN Instructors AS I2 ON I2.instructorid = C.instructorid2";
                sql += " LEFT JOIN Instructors AS I3 ON I3.instructorid = C.instructorid3";
                sql += " left join courses ec on c.COURSEID = ec.eventid";
                sql += join;
                sql += " WHERE  c.coursenum != '~ZZZZZZ~' AND (c.eventid =0 OR c.eventid is null) ";
                sql += where;
                sql += " GROUP BY " + groupbyfields;
                sql += " HAVING (ec.courseid is null or ec.internalclass is null or ec.internalclass = 0)";
                sql += having;

                sql += ")";

                var countSQL = sql;
                countSQL = countSQL + " SELECT COUNT(*) FROM Query  ";
                var countCmd = new SqlCommand();
                countCmd.CommandText = countSQL;
                countCmd.Connection = connection;

                var cmdParams = cmd.Parameters.GetEnumerator();
                while(cmdParams.MoveNext()) {
                    var param = cmdParams.Current as SqlParameter;
                    var newParam = new SqlParameter();
                    countCmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                }
                int count = (int)countCmd.ExecuteScalar();

                sql += " ,Numbered AS (";
                sql += " SELECT *, ";
                sql += " ROW_NUMBER() OVER (" + orderby + ") AS RowNumber";
                sql += " FROM Query";
                sql += " )";
                sql += " SELECT * FROM Numbered";
                sql += " WHERE RowNumber BETWEEN " + (start+1) + " AND " + (start + limit);

                sql += " " + orderby;

                var result = new List<CourseModel>();
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var courseidIndex = reader.GetOrdinal("courseid");
                        var courseid = reader.GetInt32(courseidIndex);
                        result.Add(new CourseModel(courseid));
                    }
                    reader.Close();
                }

                var model = new GridModel<CourseModel>(count, queryState);
                if (model.TotalCount > 0)
                {
                    model.Result = result;
                }

                connection.Close();

                return model;
            } 
        }
        public static GridModel<CourseModel> NewSearchFromView(QueryState queryState, string text = null, string mainCategory = null, string subCategory = null, string subsubcattext = null, bool subCategoryIsSubSub = false, CourseActiveState state = CourseActiveState.All, DateTime? from = null, DateTime? until = null, CourseInternalState courseInternalState = CourseInternalState.InternalAndPublic, CourseCancelState cancelState = CourseCancelState.NotCancelled, bool showinternal = false, bool showclosedpast = false, bool showmembership=false)
        {
            using (var db = new SchoolEntities())
            {
                if (showclosedpast)
                {
                    state = CourseActiveState.All;
                    if (from == null)
                    {
                        from = DateTime.Now.AddYears(-10);
                    }
                }
                var page = queryState.Page;
                var start = (queryState.Page - 1) * queryState.PageSize;
                var limit = queryState.PageSize;

                var mi1 = Settings.Instance.GetMasterInfo();
                var mi2 = Settings.Instance.GetMasterInfo2();
                var mi3 = Settings.Instance.GetMasterInfo3();
                var now = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                var showPastOnlineCourses = mi1.ShowPastOnlineCoursesAsBoolean;
                var allowViewPastCoursesDays = Settings.GetVbScriptBoolValue(mi3.allowviewpastcoursesdays);
                var courses = (from _courseList in db.CoursesListViews select _courseList);
                switch (state)
                {
                    case CourseActiveState.Current:
                        if( (from < now) || (from ==null))
                        {
                            from = now;
                        }
                        
                        break;

                    case CourseActiveState.Past:
                        if (until == null)
                        {
                            until = now;
                        }
                        break;
                }
                if (from != null)
                {
                    var fromDate =from;
                    if (allowViewPastCoursesDays)
                    {
                        if (showPastOnlineCourses)
                        {
                            courses = courses.Where(course => course.coursestart >= fromDate || (course.passcoursedate >= fromDate && course.viewpastcoursesdays > 0) || ( course.enddate >= fromDate &&( course.OnlineCourse == 1 || course.OnlineCourse == -1)));

                        }
                        else
                        {
                             courses = courses.Where(course => course.coursestart >= fromDate || (course.passcoursedate >= fromDate && course.viewpastcoursesdays > 0));
                        }
                    }
                    else
                    {
                        if (showPastOnlineCourses)
                        {
                            courses = courses.Where(course => course.coursestart >= fromDate || ( course.enddate >= fromDate &&( course.OnlineCourse == 1 || course.OnlineCourse == -1)));
                        }
                        else
                        {
                            courses = courses.Where(course => course.coursestart >= fromDate || (course.passcoursedate >= fromDate && course.viewpastcoursesdays > 0));
                        }

                    }
                }
                if (until != null)
                {
                    var untilDate = until.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (from != null)
                    {
                        if (showmembership)
                        {
                            courses = courses.Where(course => (course.coursestart >= from && course.coursestart <= untilDate) || (course.enddate <= untilDate && course.COURSENUM.Contains("~ZZZZZZ~")));
                        }
                        else
                        {
                            courses = courses.Where(course => (course.coursestart >= from && course.coursestart <= untilDate));
                        }
                    }
                    else
                    {
                        if (showmembership)
                        {
                            courses = courses.Where(course => (course.coursestart <= untilDate) || (course.enddate <= untilDate && course.COURSENUM.Contains("~ZZZZZZ~")));

                        }
                        else
                        {
                            courses = courses.Where(course => course.coursestart <= untilDate);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(mainCategory))
                {
                    mainCategory = System.Net.WebUtility.HtmlDecode(mainCategory);
                    courses = courses.Where(course => course.MainCategory == mainCategory);

                }

                if (!string.IsNullOrWhiteSpace(subCategory))
                {
                    subCategory = System.Net.WebUtility.HtmlDecode(subCategory);
                    if (subCategoryIsSubSub)
                    {
                        subsubcattext = System.Net.WebUtility.HtmlDecode(subsubcattext);
                        courses = courses.Where(course => course.SubSubCategory == subsubcattext);
                    }
                    else
                    {
                        courses = courses.Where(course => course.SubCategory == subCategory);
                    }
                    courses = courses.Where(course => course.SubCategory == subCategory);
                }
                if (showmembership)
                {
                    courses = courses.Where(course => course.COURSENUM.Contains("~ZZZZZZ~"));
                }
                else
                {
                    courses = courses.Where(course => course.COURSENUM != "~ZZZZZZ~");
                }
                courses = courses.Where(course=> course.eventid ==0 || course.eventid == null);

                string HideCourseNameOnPreReqList = System.Configuration.ConfigurationManager.AppSettings["HideCourseNameOnPreReqList"];

                if (!string.IsNullOrEmpty(text))
                {
                    if (HideCourseNameOnPreReqList == "true")
                    {
                        courses = courses.Where(course => course.COURSENUM.Contains(text));
                    }
                    else
                    {


                        int keyword = 0;
                        if (int.TryParse(text, out keyword))
                        {
                            courses = courses.Where(course => course.COURSENAME.Contains(text)
                                || course.COURSENUM.Contains(text)
                                || course.description.Contains(text)
                                || course.LOCATION.Contains(text)
                                || course.i1first.Contains(text)
                                || course.i1last.Contains(text)
                                || course.i2first.Contains(text)

                                || course.i2last.Contains(text)
                                || course.i3first.Contains(text)
                                || course.i3last.Contains(text)
                                || course.COURSEID == keyword

                                );
                        }
                        else
                        {
                            courses = courses.Where(course => course.COURSENAME.Contains(text)
                                || course.COURSENUM.Contains(text)
                                || course.description.Contains(text)
                                || course.LOCATION.Contains(text)
                                || course.i1first.Contains(text)
                                || course.i1last.Contains(text)
                                || course.i2first.Contains(text)

                                || course.i2last.Contains(text)
                                || course.i3first.Contains(text)
                                || course.i3last.Contains(text)

                                );
                        }
                    }


                }

                switch (courseInternalState)
                {
                    case CourseInternalState.Internal:
                        if (Gsmu.Api.Authorization.AuthorizationHelper.VisibleInternalCourses == 1)
                        {
                            courses = courses.Where(course => course.InternalClass == -1);

                        }
                        else
                        {
                            courses = courses.Where(course => course.InternalClass != -1);
                            courses = courses.Where(course => course.InternalClass != 0);

                        }
                        break;


                    case CourseInternalState.Public:
                        if (!showinternal)
                        {
                            courses = courses.Where(course => course.InternalClass == 0);
                        }
                        else
                        {
                            courses = courses.Where(course => course.InternalClass == 1 || course.InternalClass==-1);
                        }

                        break;
                }

                switch (cancelState)
                {
                    case CourseCancelState.Cancelled:
                        courses = courses.Where(course => course.CANCELCOURSE == -1 || course.CANCELCOURSE == 1);
                        break;

                    case CourseCancelState.NotCancelled:
                        courses = courses.Where(course => course.CANCELCOURSE != -1 && course.CANCELCOURSE != 1);
                        break;
                }

                var direction = (queryState.OrderByDirection == OrderByDirection.Ascending ? "ASC" : "DESC");
                var usersort = queryState.OrderField.ToString();

                //var sort1 = mi2.Sort1 > 0 && mi2.Sort1 < 6 ? (SystemSettingSortField)mi2.Sort1 : SystemSettingSortField.CourseNumber;
                //var sort2 = mi2.Sort2 > 0 && mi2.Sort2 < 6 ? (SystemSettingSortField)mi2.Sort2 : SystemSettingSortField.CourseName;
                //var sort3 = mi2.Sort3 > 0 && mi2.Sort3 < 6 ? (SystemSettingSortField)mi2.Sort3 : SystemSettingSortField.CourseDate;
                //var sort4 = mi2.Sort4 > 0 && mi2.Sort4 < 6 ? (SystemSettingSortField)mi2.Sort4 : SystemSettingSortField.CourseTime;
                //var sort5 = mi2.Sort5 > 0 && mi2.Sort5 < 6 ? (SystemSettingSortField)mi2.Sort5 : SystemSettingSortField.Location;
                var filteredcourses = courses.DistinctBy(course => course.COURSEID);
                filteredcourses = NewCoursesSortLinq(filteredcourses, queryState, usersort, 0, direction);


                var pagedcourslist = filteredcourses.Skip((page - 1) * limit).Take(limit);
                int count = filteredcourses.Count();
                var result = new List<CourseModel>();
                var courseid = 0;
                int countedcourses = 0;
                foreach (var course in pagedcourslist)
                {
                    courseid = course.COURSEID;
                    if (result.Where(_result => _result.CourseId == course.COURSEID).Count() <= 0)
                    {
                        if (countedcourses < limit)
                        {
  
                                result.Add(new CourseModel(courseid));
                                countedcourses = +1;
                            
                        }
                    }
                    else
                    {
                        count = count - 1;
                    }
                }
                var model = new GridModel<CourseModel>(count, queryState);
                if (model.TotalCount > 0)
                {
                    model.Result = result;
                }
                return model;
            }
        }
        private static IEnumerable<CoursesListView> NewCoursesSortLinq(IEnumerable<CoursesListView> courses, QueryState queryState, string usersort = "", int index = 0, string direction = "ASC")
        {


            var mi2 = Settings.Instance.GetMasterInfo2();
            var sort1 = mi2.Sort1 > 0 && mi2.Sort1 < 6 ? (SystemSettingSortField)mi2.Sort1 : SystemSettingSortField.CourseNumber;
            var sort2 = mi2.Sort2 > 0 && mi2.Sort2 < 6 ? (SystemSettingSortField)mi2.Sort2 : SystemSettingSortField.CourseName;
            var sort3 = mi2.Sort3 > 0 && mi2.Sort3 < 6 ? (SystemSettingSortField)mi2.Sort3 : SystemSettingSortField.CourseDate;
            var sort4 = mi2.Sort4 > 0 && mi2.Sort4 < 6 ? (SystemSettingSortField)mi2.Sort4 : SystemSettingSortField.CourseTime;
            var sort5 = mi2.Sort5 > 0 && mi2.Sort5 < 6 ? (SystemSettingSortField)mi2.Sort5 : SystemSettingSortField.Location;
            var sort = sort1;

            var mxsort = 5;
            var userfldsort = SystemSettingSortField.CourseName;
            var userfld2sort = SystemSettingSortField.CourseTime;
            if (!string.IsNullOrWhiteSpace(usersort))
            {
                if (usersort.ToLower() == SystemSettingSortField.CourseName.ToString().ToLower()) { userfldsort = SystemSettingSortField.CourseName; mxsort = 6; }
                if (usersort.ToLower() == SystemSettingSortField.CourseNumber.ToString().ToLower()) { userfldsort = SystemSettingSortField.CourseNumber; mxsort = 6; }
                if (usersort.ToLower() == SystemSettingSortField.Location.ToString().ToLower()) { userfldsort = SystemSettingSortField.Location; mxsort = 6; }
                if (usersort.ToLower() == SystemSettingSortField.CourseTime.ToString().ToLower())
                {
                    userfld2sort = SystemSettingSortField.CourseTime;
                    userfldsort = SystemSettingSortField.CourseDate;
                    mxsort = 7;
                }
                if (usersort.ToLower() == SystemSettingSortField.CourseDate.ToString().ToLower() || usersort.ToLower() == "coursestart")
                {
                    userfld2sort = SystemSettingSortField.CourseDate;
                    userfldsort = SystemSettingSortField.CourseTime;
                    mxsort = 7;
                }

            }

            for (int i = 1; i <= mxsort; i++)
            {
                if (i == 7) { sort = userfld2sort; }
                if (i == 6) { sort = userfldsort; }
                if (i == 5) { sort = sort1; }
                if (i == 4) { sort = sort2; }
                if (i == 3) { sort = sort3; }
                if (i == 2) { sort = sort4; }
                if (i == 1) { sort = sort5; }
                if (sort == SystemSettingSortField.CourseNumber)
                {
                    if (direction == "ASC")
                    {
                        courses = courses.OrderBy(course => course.COURSENUM);
                    }
                    else
                    {
                        courses = courses.OrderByDescending(course => course.COURSENUM);
                    }
                }
                if (sort == SystemSettingSortField.CourseName)
                {
                    if (direction == "ASC")
                    {
                        courses = courses.OrderBy(course => course.COURSENAME);
                    }
                    else
                    {
                        courses = courses.OrderByDescending(course => course.COURSENAME);
                    }
                }
                if (sort == SystemSettingSortField.CourseDate)
                {
                    if (direction == "ASC")
                    {
                        courses = courses.OrderBy(course => course.coursedate);
                    }
                    else
                    {
                        courses = courses.OrderByDescending(course => course.coursedate);
                    }
                }
                if (sort == SystemSettingSortField.CourseTime)
                {
                    if (direction == "ASC")
                    {
                        courses = courses.OrderBy(course => course.coursetime);
                    }
                    else
                    {
                        courses = courses.OrderByDescending(course => course.coursetime);
                    }
                }
                if (sort == SystemSettingSortField.Location)
                {
                    if (direction == "ASC")
                    {
                        courses = courses.OrderBy(course => course.LOCATION);
                    }
                    else
                    {
                        courses = courses.OrderByDescending(course => course.LOCATION);
                    }
                }

            }



            //if (sortingField == SystemSettingSortField.CourseNumber.ToString())
            //{
            //    if (direction == "ASC")
            //    {
            //        courses = courses.OrderBy(course => course.COURSENUM);
            //    }
            //    else
            //    {
            //      courses =  courses.OrderByDescending(course => course.COURSENUM);
            //    }
            //}
            //else if (sortingField == SystemSettingSortField.CourseName.ToString())
            //{
            //    if (direction == "ASC")
            //    {
            //        courses = courses.OrderBy(course => course.COURSENAME);
            //    }
            //    else
            //    {
            //        courses = courses.OrderByDescending(course => course.COURSENAME);
            //    }
            //}
            //else if (sortingField == SystemSettingSortField.CourseTime.ToString())
            //{
            //    if (direction == "ASC")
            //    {
            //        courses = courses.OrderBy(course => course.coursetime);
            //    }
            //    else
            //    {
            //        courses = courses.OrderByDescending(course => course.coursetime);
            //    }
            //}
            //else if (sortingField == SystemSettingSortField.CourseDate.ToString())
            //{
            //    if (direction == "ASC")
            //    {
            //        courses = courses.OrderBy(course => course.coursedate);
            //    }
            //    else
            //    {
            //        courses = courses.OrderByDescending(course => course.coursedate);
            //    }
            //}
            //else if (sortingField == SystemSettingSortField.Location.ToString())
            //{
            //    if (direction == "ASC")
            //    {
            //        courses = courses.OrderBy(course => course.LOCATION);
            //    }
            //    else
            //    {
            //        courses = courses.OrderByDescending(course => course.LOCATION);
            //    }
            //}
            //else
            //{
            //    if (direction == "ASC")
            //    {
            //        courses = courses.OrderBy(course => course.coursedate);
            //    }
            //    else
            //    {
            //        courses = courses.OrderByDescending(course => course.coursedate);
            //    }
            //}

            //switch ((CourseOrderByField)queryState.OrderField)
            //{
            //    case CourseOrderByField.CourseName:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.COURSENAME);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.COURSENAME);
            //        }
            //        break;

            //    case CourseOrderByField.CourseNum:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.COURSENUM);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.COURSENUM);
            //        } break;

            //    case CourseOrderByField.CourseId:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.COURSEID);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.COURSEID);
            //        } break;

            //    case CourseOrderByField.Location:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.LOCATION);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.LOCATION);
            //        } break;

            //    case CourseOrderByField.CourseStart:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.coursestart);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.coursestart);
            //        } break;

            //    case CourseOrderByField.CourseDate:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.coursedate);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.coursedate);
            //        } break;

            //    case CourseOrderByField.CourseTime:
            //        if (direction == "ASC")
            //        {
            //            courses = courses.OrderBy(course => course.coursetime);
            //        }
            //        else
            //        {
            //            courses = courses.OrderByDescending(course => course.coursetime);
            //        } break;

            //    case CourseOrderByField.SystemDefault:
            //        courses = courses;
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}

            return courses;
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        private static string CalculateOrderBy(QueryState queryState)
        {
            var orderby = " ORDER BY [coursename] ASC";
            var direction = (queryState.OrderByDirection == OrderByDirection.Ascending ? " ASC" : " DESC");

            // 1 - course date, 2 - course number, 3 - course name, 4 - course time, 5 - location

            // sort1
            var mi2 = Settings.Instance.GetMasterInfo2();
            var sort1 = mi2.Sort1 > 0 && mi2.Sort1 < 6 ? (SystemSettingSortField)mi2.Sort1 : SystemSettingSortField.CourseNumber;
            var sort2 = mi2.Sort2 > 0 && mi2.Sort2 < 6 ? (SystemSettingSortField)mi2.Sort2 : SystemSettingSortField.CourseName;
            var sort3 = mi2.Sort3 > 0 && mi2.Sort3 < 6 ? (SystemSettingSortField)mi2.Sort3 : SystemSettingSortField.CourseDate;
            var sort4 = mi2.Sort4 > 0 && mi2.Sort4 < 6 ? (SystemSettingSortField)mi2.Sort4 : SystemSettingSortField.CourseTime;
            var sort5 = mi2.Sort5 > 0 && mi2.Sort5 < 6 ? (SystemSettingSortField)mi2.Sort5 : SystemSettingSortField.Location;

            var ordersufx = string.Empty;
            if(sort1.ToString().ToLower()!=queryState.OrderField.ToString().ToLower()){
                 ordersufx += GetSystemSettingSortFieldString(sort1) + direction;
            }
            if (sort2.ToString().ToLower() != queryState.OrderField.ToString().ToLower())
            {
                if (ordersufx != string.Empty)
                {
                    ordersufx += ", " + GetSystemSettingSortFieldString(sort2) + direction;
                }
                else
                {
                    ordersufx += " " + GetSystemSettingSortFieldString(sort2) + direction;
                }
            }
            if (sort3.ToString().ToLower() != queryState.OrderField.ToString().ToLower())
            {
                if (ordersufx != string.Empty)
                {
                    ordersufx += ", " + GetSystemSettingSortFieldString(sort3) + direction;
                }
                else
                {
                    ordersufx += " " + GetSystemSettingSortFieldString(sort3) + direction;
                }
            }
            if (sort4.ToString().ToLower() != queryState.OrderField.ToString().ToLower())
            {
                if (ordersufx != string.Empty)
                {
                    ordersufx += ", " + GetSystemSettingSortFieldString(sort4) + direction;
                }
                else
                {
                    ordersufx += " " + GetSystemSettingSortFieldString(sort4) + direction;
                }
            }

            switch ((CourseOrderByField)queryState.OrderField)
            {
                case CourseOrderByField.CourseName:
                    if (!ordersufx.ToLower().Contains("coursename"))
                    {
                        orderby = " ORDER BY [COURSENAME]" + direction + "," + ordersufx;
                    }
                    else
                    {
                        orderby = " ORDER BY [COURSENAME]" + direction;
                    }
                    break;

                case CourseOrderByField.CourseNum:
                    if (!ordersufx.ToLower().Contains("coursenum"))
                    {
                        orderby = " ORDER BY [coursenum]" + direction + "," + ordersufx;
                    }
                    else
                    {
                        orderby = " ORDER BY [coursenum]" + direction;
                    }
                    break;

                case CourseOrderByField.CourseId:
                    if (!ordersufx.ToLower().Contains("courseid"))
                    {
                        orderby = " ORDER BY [courseid]" + direction + "," + ordersufx;
                    }
                    else
                    {
                        orderby = " ORDER BY [courseid]" + direction;
                    }
                    break;

                case CourseOrderByField.Location:
                    if (!ordersufx.ToLower().Contains("location"))
                    {
                        orderby = " ORDER BY [location]" + direction + "," + ordersufx;
                    }
                    else
                    {
                        orderby = " ORDER BY [location]" + direction;
                    }
                    break;

                case CourseOrderByField.CourseStart:
                    if (!ordersufx.ToLower().Contains("coursestart"))
                    {
                        orderby = " ORDER BY [coursestart]" + direction + "," + ordersufx;
                    }

                    else
                    {
                        orderby = " ORDER BY [coursestart]" + direction;
                    }
                    break;

                case CourseOrderByField.CourseDate:
                    if (!ordersufx.ToLower().Contains("coursedate"))
                    {
                        orderby = " ORDER BY [coursedate]" + direction + "," + ordersufx;
                    }
                    else
                    {
                        orderby = " ORDER BY [coursedate]" + direction;
                    }
                    break;

                case CourseOrderByField.CourseTime:
                    if (!ordersufx.ToLower().Contains("coursetime"))
                    {
                        orderby = " ORDER BY [coursetime]" + direction + "," + ordersufx;
                    }
                    else
                    {
                        orderby = " ORDER BY [coursetime]" + direction;
                    }
                    break;

                case CourseOrderByField.SystemDefault:

                    orderby = " ORDER BY " + ordersufx;
                    break;

                default:
                    throw new NotImplementedException();
            }
            return orderby;
        }

        private static String GetSystemSettingSortFieldString(SystemSettingSortField sort)
        {
            switch (sort)
            {
                case SystemSettingSortField.CourseDate:
                    return "coursedate";

                case SystemSettingSortField.CourseName:
                    return "coursename";

                case SystemSettingSortField.CourseNumber:
                    return "coursenum";

                case SystemSettingSortField.CourseTime:
                    return "coursetime";

                case SystemSettingSortField.Location:
                    return "location";

                default:
                    throw new NotImplementedException();
            }
        }

        /*
        /// <summary>
        /// Used by the public course search.
        /// All queries here must be the same in <see cref="CourseModel">the course model class init method</see>
        /// Whatever you put in the init method must be replicated in the queries class becuase many places uses the 
        /// same features and at the moment i developed this, this was the option i had, i am sure there is better one
        /// but for now we have to stick with this
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static GridModel<CourseModel> Search(QueryState queryState, string text = null, string mainCategory = null, string subCategory = null, bool subCategoryIsSubSub = false, CourseActiveState state = CourseActiveState.All, DateTime? from = null, DateTime? until = null, CourseInternalState courseInternalState = CourseInternalState.InternalAndPublic, CourseCancelState cancelState = CourseCancelState.NotCancelled)
        {
            using (var db = new SchoolEntities())
            {
                var mi1 = Settings.Instance.GetMasterInfo();
                var mi2 = Settings.Instance.GetMasterInfo2();
                var mi3 = Settings.Instance.GetMasterInfo3();

                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                var query = from c in db.Courses
                            where c.COURSENUM != "~ZZZZZZ~" && (c.coursetype == (int)CourseType.Course || c.coursetype == null)
                            select new CourseModel()
                            {
                                Course = c,
                                CourseTimes = (from ct in db.Course_Times where ct.COURSEID == c.COURSEID orderby ct.COURSEDATE, ct.STARTTIME select ct),
                                PricingOptionsMember = (from cpo in db.CoursePricingOptions
                                                        join po in db.PricingOptions on cpo.PricingOptionId equals po.PricingOptionID
                                                        where cpo.CourseId == c.COURSEID && cpo.Type == (int)PricingOptionType.Member
                                                        select new PricingModel()
                                                        {
                                                            CoursePricingOption = cpo,
                                                            PricingOption = po,
                                                            MembershipType = Student.MembershipType.Member
                                                        }),
                                PricingOptionsNonMember = (from cpo in db.CoursePricingOptions
                                                           join po in db.PricingOptions on cpo.PricingOptionId equals po.PricingOptionID
                                                           where cpo.CourseId == c.COURSEID && cpo.Type == (int)PricingOptionType.NonMember
                                                           select new PricingModel()
                                                           {
                                                               CoursePricingOption = cpo,
                                                               PricingOption = po,
                                                               MembershipType = Student.MembershipType.NonMember
                                                           }),
                                PricingOptionsSpecial1 = (from cpo in db.CoursePricingOptions
                                                          join po in db.PricingOptions on cpo.PricingOptionId equals po.PricingOptionID
                                                          where cpo.CourseId == c.COURSEID && cpo.Type == (int)PricingOptionType.Special1
                                                          select new PricingModel()
                                                          {
                                                              CoursePricingOption = cpo,
                                                              PricingOption = po,
                                                              MembershipType = Student.MembershipType.Special1
                                                          }),
                                MainCategory = null,
                                SubCategory = null,
                                SubSubCategory = null,
                                EventInternalClass = (from eic in db.Courses where eic.COURSEID == c.eventid select eic.InternalClass).FirstOrDefault(),
                                InstructorName = (from instructor in db.Instructors where instructor.INSTRUCTORID == c.INSTRUCTORID select instructor.FIRST + " " + instructor.LAST).FirstOrDefault()
                            };

                query = query.AsNoTracking();

                // dynamic linq join with dynamic where clause
                if (!string.IsNullOrEmpty(mainCategory))
                {
                    query = query.Join(
                        db.MainCategories,
                        cm => cm.Course.COURSEID,
                        mc => mc.CourseID,
                        (cm, mc) => new CourseModel()
                        {
                            Course = cm.Course,
                            CourseTimes = cm.CourseTimes,
                            PricingOptionsMember = cm.PricingOptionsMember,
                            PricingOptionsNonMember = cm.PricingOptionsNonMember,
                            PricingOptionsSpecial1 = cm.PricingOptionsSpecial1,
                            MainCategory = mc,
                            SubCategory = null,
                            SubSubCategory = null,
                            EventInternalClass = cm.EventInternalClass,
                            InstructorName = cm.InstructorName
                        }
                    );
                    query = query.Where(r => r.MainCategory.MainCategory1 == mainCategory);


                }

                // dynamic linq join with dynamic where clause
                if (!string.IsNullOrEmpty(subCategory) && !subCategoryIsSubSub)
                {
                    query = query.Join(
                        db.SubCategories,
                        cm => cm.MainCategory.MainCategoryID,
                        sc => sc.MainCategoryID,
                        (cm, sc) => new CourseModel()
                        {
                            Course = cm.Course,
                            CourseTimes = cm.CourseTimes,
                            PricingOptionsMember = cm.PricingOptionsMember,
                            PricingOptionsNonMember = cm.PricingOptionsNonMember,
                            PricingOptionsSpecial1 = cm.PricingOptionsSpecial1,
                            MainCategory = cm.MainCategory,
                            SubCategory = sc,
                            SubSubCategory = null,
                            EventInternalClass = cm.EventInternalClass,
                            InstructorName = cm.InstructorName
                        }
                    );
                    query = query.Where(r => r.SubCategory.SubCategory1 == subCategory);
                }


                // dynamic linq join with dynamic where clause
                if (!string.IsNullOrEmpty(subCategory) && subCategoryIsSubSub)
                {
                    query = query.Join(
                        db.SubSubCategories,
                        cm => cm.MainCategory.MainCategoryID,
                        sc => sc.MainCategoryID,
                        (cm, sc) => new CourseModel()
                        {
                            Course = cm.Course,
                            CourseTimes = cm.CourseTimes,
                            PricingOptionsMember = cm.PricingOptionsMember,
                            PricingOptionsNonMember = cm.PricingOptionsNonMember,
                            PricingOptionsSpecial1 = cm.PricingOptionsSpecial1,
                            MainCategory = cm.MainCategory,
                            SubCategory = null,
                            SubSubCategory = sc,
                            EventInternalClass = cm.EventInternalClass,
                            InstructorName = cm.InstructorName
                        }
                    );
                    query = query.Where(r => r.SubSubCategory.SubSubCategory1 == subCategory);
                }
                //Check Internal Event
                query = query.Where(r => r.EventInternalClass != -1 || r.EventInternalClass == null || r.EventInternalClass == 0);

                var search = text ?? string.Empty;
                search = search.Trim();

                if (!string.IsNullOrEmpty(search))
                {
                    int classId;
                    if (int.TryParse(search, out classId))
                    {
                        query = query.Where(r => r.Course.COURSEID == classId || r.Course.COURSENAME.Contains(search) || r.Course.COURSENUM.Contains(search) || r.Course.LOCATION.Contains(search) || r.Course.DESCRIPTION.Contains(search) || r.InstructorName.Contains(search));
                    }
                    else
                    {
                        query = query.Where(r => r.Course.COURSENAME.Contains(search) || r.Course.COURSENUM.Contains(search) || r.Course.LOCATION.Contains(search) || r.Course.DESCRIPTION.Contains(search) || r.InstructorName.Contains(search));
                    }
                }

                switch (courseInternalState)
                {
                    case CourseInternalState.Internal:
                        if (Gsmu.Api.Authorization.AuthorizationHelper.VisibleInternalCourses == 1)
                        {
                            query = query.Where(r => r.Course.InternalClass == -1);
                        }
                        else
                        {
                            query = query.Where(r => r.Course.InternalClass != -1 && r.Course.InternalClass != 0);
                        }
                        break;


                    case CourseInternalState.Public:
                        query = query.Where(r => r.Course.InternalClass == 0);
                        break;
                }

                switch (cancelState)
                {
                    case CourseCancelState.Cancelled:
                        query = query.Where(r => r.Course.CANCELCOURSE == 1 || r.Course.CANCELCOURSE == -1);
                        break;

                    case CourseCancelState.NotCancelled:
                        query = query.Where(r => !(r.Course.CANCELCOURSE == 1 || r.Course.CANCELCOURSE == -1));
                        break;
                }

                // if we want to hide courses the student is enrolled in
                //var student = Authorization.AuthorizationHelper.CurrentUser as Gsmu.Api.Data.School.Entities.Student;
                //if (student != null)
                //{
                //    var rosters = student.CurrentRosters;
                //    var ids = (from r in student.CurrentRosters select r.COURSEID).ToArray();
                //    query = query.Where(r => !ids.Contains(r.Course.COURSEID));
                //}

                // dynamic course state


                /// all dates should be handled here because of the show past online courses settings
                /// all dates should be handled here because of the show past online courses settings
                /// all dates should be handled here because of the show past online courses settings
                /// all dates should be handled here because of the show past online courses settings

                var now = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                var showPastOnlineCourses = mi1.ShowPastOnlineCoursesAsBoolean;
                var allowViewPastCoursesDays = Settings.GetVbScriptBoolValue(mi3.allowviewpastcoursesdays);

                switch (state)
                {
                    case CourseActiveState.Current:
                        if (from == null)
                        {
                            from = now;
                        }
                        break;

                    case CourseActiveState.Past:
                        if (until == null)
                        {
                            until = now;
                        }
                        break;
                }


                if (from != null)
                {
                    query = query.Where(r =>
                        (
                            r.CourseTimes.Min(ct => ct.COURSEDATE) >= from
                        )

                        ||

                        (
                            allowViewPastCoursesDays
                            ?
                            r.CourseTimes.Min(ct =>
                                DbFunctions.AddDays(
                                    ct.COURSEDATE,
                                    r.Course.viewpastcoursesdays ?? 0
                                )
                            ) >= from
                            :
                            r.CourseTimes.Min(ct => ct.COURSEDATE) >= from
                        )

                        ||

                        (
                            showPastOnlineCourses
                            ?
                            r.CourseTimes.Min(ct => ct.COURSEDATE) <= from
                            &&
                            r.CourseTimes.Max(ct => ct.COURSEDATE) >= from
                            &&
                            r.Course.OnlineCourse.HasValue &&
                            (r.Course.OnlineCourse == 1 || r.Course.OnlineCourse == -1)
                            :
                            r.CourseTimes.Min(ct => ct.COURSEDATE) >= from
                        )
                    );
                }

                if (until != null)
                {
                    query = query.Where(r =>
                        (
                            r.CourseTimes.Min(ct => ct.COURSEDATE) <= until
                        )

                        ||

                        (
                            allowViewPastCoursesDays
                            ?
                            r.CourseTimes.Min(ct =>
                                DbFunctions.AddDays(
                                    ct.COURSEDATE,
                                    r.Course.viewpastcoursesdays ?? 0
                                )
                            ) <= until
                            :
                            r.CourseTimes.Min(ct => ct.COURSEDATE) <= until
                        )

                        ||

                        (
                            showPastOnlineCourses
                            ?
                            r.CourseTimes.Max(ct => ct.COURSEDATE) <= until
                            &&
                            r.Course.OnlineCourse.HasValue &&
                            (r.Course.OnlineCourse == 1 || r.Course.OnlineCourse == -1)
                            :
                            r.CourseTimes.Min(ct => ct.COURSEDATE) <= until
                        )
                    );
                }


                /// all dates should be handled above because of the show past online courses settings
                /// all dates should be handled above because of the show past online courses settings
                /// all dates should be handled above because of the show past online courses settings
                /// all dates should be handled above because of the show past online courses settings

                var model = new GridModel<CourseModel>(query.Count(), queryState);

                if (model.TotalCount > 0)
                {
                    query = ApplyOrdering(query, queryState);
                    query = model.Paginate(query);
                    model.Result = query.ToList();
                }

                return model;
            }
        }

        public static IQueryable<CourseModel> ApplyOrdering(IQueryable<CourseModel> query, QueryState state, bool alreadyOrdered = false)
        {
            //var oderedQuery = query as IOrderedQueryable<CourseModel>;

            // dynamic linq order by
            switch ((CourseOrderByField)state.OrderField)
            {

                case CourseOrderByField.CourseStart:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ?
                                query
                                .OrderBy(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE))
                                .ThenBy(cm => cm.CourseTimes.Min(ct => ct.STARTTIME))
                                :
                                (query as IOrderedQueryable<CourseModel>)
                                .ThenBy(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE))
                                .ThenBy(cm => cm.CourseTimes.Min(ct => ct.STARTTIME))
                            ;
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ?
                                query
                                .OrderByDescending(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE))
                                .ThenByDescending(cm => cm.CourseTimes.Min(ct => ct.STARTTIME))
                                :
                                (query as IOrderedQueryable<CourseModel>)
                                .ThenByDescending(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE))
                                .ThenByDescending(cm => cm.CourseTimes.Min(ct => ct.STARTTIME))
                            ;
                            break;
                    }
                    break;

                case CourseOrderByField.Location:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ? query.OrderBy(cm => cm.Course.LOCATION) : (query as IOrderedQueryable<CourseModel>).ThenBy(cm => cm.Course.LOCATION);
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ? query.OrderByDescending(cm => cm.Course.LOCATION) : (query as IOrderedQueryable<CourseModel>).ThenByDescending(cm => cm.Course.LOCATION);
                            break;
                    }
                    break;

                case CourseOrderByField.CourseId:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ? query.OrderBy(cm => cm.Course.COURSEID) : (query as IOrderedQueryable<CourseModel>).ThenBy(cm => cm.Course.COURSEID);
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ? query.OrderByDescending(cm => cm.Course.COURSEID) : (query as IOrderedQueryable<CourseModel>).ThenByDescending(cm => cm.Course.COURSEID);
                            break;
                    }
                    break;

                case CourseOrderByField.CourseName:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ? query.OrderBy(cm => cm.Course.COURSENAME) : (query as IOrderedQueryable<CourseModel>).ThenBy(cm => cm.Course.COURSENAME);
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ? query.OrderByDescending(cm => cm.Course.COURSENAME) : (query as IOrderedQueryable<CourseModel>).ThenByDescending(cm => cm.Course.COURSENAME);
                            break;
                    }
                    break;

                case CourseOrderByField.CourseNum:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ? query.OrderBy(cm => cm.Course.COURSENUM) : (query as IOrderedQueryable<CourseModel>).ThenBy(cm => cm.Course.COURSENUM);
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ? query.OrderByDescending(cm => cm.Course.COURSENUM) : (query as IOrderedQueryable<CourseModel>).ThenByDescending(cm => cm.Course.COURSENUM);
                            break;
                    }
                    break;

                case CourseOrderByField.CourseDate:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ? query.OrderBy(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE)) : (query as IOrderedQueryable<CourseModel>).ThenBy(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE));
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ? query.OrderByDescending(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE)) : (query as IOrderedQueryable<CourseModel>).ThenByDescending(cm => cm.CourseTimes.Min(ct => ct.COURSEDATE));
                            break;
                    }
                    break;

                case CourseOrderByField.CourseTime:
                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = !alreadyOrdered ? query.OrderBy(cm => cm.CourseTimes.Min(ct => ct.STARTTIME)) : (query as IOrderedQueryable<CourseModel>).ThenBy(cm => cm.CourseTimes.Min(ct => ct.STARTTIME));
                            break;
                        case OrderByDirection.Descending:
                            query = !alreadyOrdered ? query.OrderByDescending(cm => cm.CourseTimes.Min(ct => ct.STARTTIME)) : (query as IOrderedQueryable<CourseModel>).ThenByDescending(cm => cm.CourseTimes.Min(ct => ct.STARTTIME));
                            break;
                    }
                    break;

                case CourseOrderByField.SystemDefault:

                    // 1 - course date, 2 - course number, 3 - course name, 4 - course time

                    // sort1
                    var mi2 = Settings.Instance.GetMasterInfo2();
                    var sort1 = mi2.Sort1 > 0 && mi2.Sort1 < 5 ? (SystemSettingSortField)mi2.Sort1 : SystemSettingSortField.CourseNumber;
                    var sort2 = mi2.Sort2 > 0 && mi2.Sort2 < 5 ? (SystemSettingSortField)mi2.Sort2 : SystemSettingSortField.CourseName;
                    var sort3 = mi2.Sort3 > 0 && mi2.Sort3 < 5 ? (SystemSettingSortField)mi2.Sort3 : SystemSettingSortField.CourseDate;
                    var sort4 = mi2.Sort1 > 0 && mi2.Sort1 < 5 ? (SystemSettingSortField)mi2.Sort4 : SystemSettingSortField.CourseTime;

                    switch (state.OrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            query = SortBySystemSettings(query, sort1, state.OrderByDirection);
                            query = SortBySystemSettings(query, sort2, state.OrderByDirection, true);
                            query = SortBySystemSettings(query, sort3, state.OrderByDirection, true);
                            query = SortBySystemSettings(query, sort4, state.OrderByDirection, true);
                            break;

                        case OrderByDirection.Descending:
                            query = SortBySystemSettings(query, sort4, state.OrderByDirection);
                            query = SortBySystemSettings(query, sort3, state.OrderByDirection, true);
                            query = SortBySystemSettings(query, sort2, state.OrderByDirection, true);
                            query = SortBySystemSettings(query, sort1, state.OrderByDirection, true);
                            break;
                    }
                    break;
            }

            return query;
        }

        private static IQueryable<CourseModel> SortBySystemSettings(IQueryable<CourseModel> query, SystemSettingSortField sort, OrderByDirection direction = OrderByDirection.Ascending, bool alreadyOrdered = false)
        {
            switch (sort)
            {
                case SystemSettingSortField.CourseName:
                    return ApplyOrdering(query, new QueryState()
                    {
                        OrderField = CourseOrderByField.CourseName,
                        OrderByDirection = direction
                    }, alreadyOrdered);

                case SystemSettingSortField.CourseNumber:
                    return ApplyOrdering(query, new QueryState()
                    {
                        OrderField = CourseOrderByField.CourseNum,
                        OrderByDirection = direction
                    }, alreadyOrdered);


                case SystemSettingSortField.CourseDate:
                    return ApplyOrdering(query, new QueryState()
                    {
                        OrderField = CourseOrderByField.CourseDate,
                        OrderByDirection = direction
                    }, alreadyOrdered);

                case SystemSettingSortField.CourseTime:
                    return ApplyOrdering(query, new QueryState()
                    {
                        OrderField = CourseOrderByField.CourseTime,
                        OrderByDirection = direction
                    }, alreadyOrdered);

            }
            throw new NotImplementedException();
        }
        */

    
    }
}
