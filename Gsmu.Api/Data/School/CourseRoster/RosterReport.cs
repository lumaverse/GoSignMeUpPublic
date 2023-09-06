using Gsmu.Api.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Api.Data.School.CourseRoster
{
    public class RosterReport
    {
        public static QueryState BuildRequestQuery(HttpRequestBase Request)
        {
            var jresult = new JsonResult();
            var filter = @"[{'property':'keyword','value':''}]";
            if (Request.QueryString["filter"] != null)
            {
                filter = Request.QueryString["filter"];
            }
            var sort = "[{'property':'dateadded','direction':'DESC'}]";
            if (Request.QueryString["sort"] != null)
            {
                sort = Request.QueryString["sort"];
            }

            var FldHeader = "[{'property':'any','value':''}]";
            if (Request.QueryString["columns"] != null)
            {
                FldHeader = Request.QueryString["columns"];
            }

            var start = 0;
            if (Request.QueryString["start"] != null)
            {
                start = int.Parse(Request.QueryString["start"]);
            }
            var limit = 5;
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
            var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);
            var sortersResult = ExtJsDataStoreHelper.ParseSorter(sort);
            var queryState = new QueryState(start, limit)
            {
                OrderByDirection = sorterResult.Value,
                OrderFieldString = sorterResult.Key,
                Filters = filterResult,
                Sorters = sortersResult,
                FldHeaders = FldHeaderResult,
                Page = page
            };

            return queryState;
        }
        public static RosterReport_result GenerateRosterReport(HttpRequestBase Request, bool isExport, int courseid = 0)
        {
            var page = 1;
            var start = 1;
            var limit = 10;
            int records = 0;
            string export = "";
            QueryState queryState = BuildRequestQuery(Request);
            if (Request["isexport"] == "Yes")
            {
                isExport = true;
                export = "export";
            }
                using (var connection = Connections.GetSchoolConnection())
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.Connection = connection;

                    page = queryState.Page;
                    start = (queryState.Page - 1) * queryState.PageSize;
                    limit = queryState.PageSize;

                    var orderby = "ORDER BY";
                    if (queryState.Sorters.Count > 0)
                    {
                        var comma = "";
                        foreach (var sorter in queryState.Sorters)
                        {

                            String OrderKey = sorter.Key.ToString();
                            String OrderDrctn = sorter.Value == OrderByDirection.Ascending ? "ASC" : "DESC";
                            orderby += comma + "[" + OrderKey + "] " + OrderDrctn;
                            comma = ", ";
                        }
                    }
                    else
                    {
                        orderby += "  [coursedateid] DESC";
                    }

                    string where = "";
                    string having = "";
                    //List<string> columns = queryState.Columns;
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
                            string filterValue = queryState.Filters["keyword"];
                            where = where + " AND (";
                            where = where + " C.COURSEID LIKE '%" + filterValue + "%'";
                            where = where + " OR C.COURSENAME LIKE '%" + filterValue + "%'";
                            where = where + " OR C.COURSENUM LIKE '%" + filterValue + "%'";
                            where = where + " OR C.description LIKE '%" + filterValue + "%'";
                            where = where + " OR ds.District LIKE '%" + filterValue + "%'";
                            where = where + " OR sc.LOCATION LIKE '%" + filterValue + "%'";
                            where = where + " OR gl.grade LIKE '%" + filterValue + "%'";
                            where = where + " OR i1.first LIKE '%" + filterValue + "%'";
                            where = where + " OR i1.last LIKE '%" + filterValue + "%'";
                            where = where + " OR (i1.first + ' ' + i1.last) LIKE '%" + filterValue + "%'";
                            where = where + " Or (CONVERT(VARCHAR(8), startdate, 112) + cast(C.courseid as nvarchar)) LIKE '%" + filterValue + "%'";
                            where = where + ") ";
                        }
                        //filter  date-from
                        if (queryState.Filters.ContainsKey("date-from"))
                        {
                            string filterValue = queryState.Filters["date-from"];
                            having = having + " AND startdate > '" + filterValue + "'";
                        }
                        else
                        {
                            having = having + " AND startdate > '" + DateTime.Now.AddDays(-30) + "'";
                        }
                        //filter date-to
                        if (queryState.Filters.ContainsKey("date-to"))
                        {
                            string filterValue = queryState.Filters["date-to"];
                            having = having + " AND startdate <= '" + DateTime.Parse(filterValue).AddHours(24) + "'";
                        }
                        else
                        {
                            having = having + " AND startdate <= '" + DateTime.Now.AddDays(30) + "'";
                        }
                        //filter cancelledtext
                        if (queryState.Filters.ContainsKey("cancelledtxt"))
                        {
                            string filterValue = queryState.Filters["cancelledtxt"];
                            if (!string.IsNullOrEmpty(filterValue))
                            {
                                if (Convert.ToInt32(filterValue) == 1)
                                {
                                    having = having + " AND  (cr.cancel = 0)";
                                }
                            }

                        }
                        //filter category-main
                        if (queryState.Filters.ContainsKey("category-main"))
                        {
                            string filterValue = queryState.Filters["category-main"];
                            if (!string.IsNullOrEmpty(filterValue) && filterValue != "No main category selected")
                            {
                                where = where + " AND (CM.MainCategory LIKE '%" + filterValue + "%' )";
                            }
                        }
                        //filter category-sub
                        if (queryState.Filters.ContainsKey("category-sub"))
                        {
                            string filterValue = queryState.Filters["category-sub"];
                            if (!string.IsNullOrEmpty(filterValue) && filterValue != "No sub-category selected")
                            {
                                where = where + " AND  (CS.SubCategory LIKE '%" + filterValue + "%' )";
                            }
                        }
                        //filter category-subsub
                        if (queryState.Filters.ContainsKey("category-subsub"))
                        {
                            string filterValue = queryState.Filters["category-subsub"];
                            if (!string.IsNullOrEmpty(filterValue) && filterValue != "No optional sub-category selected")
                            {
                                where = where + " AND  (CSS.SubSubCategory LIKE '%" + filterValue + "%')";
                            }
                        }
                    }
                    else
                    {
                        having = having + " AND startdate <= '" + DateTime.Now.AddDays(30) + "'";
                        having = having + " AND startdate > '" + DateTime.Now.AddDays(-30) + "'";
                    }

                    if (courseid != 0)
                    {
                        where = " AND (";
                        where = where + " C.COURSEID =" + courseid + "";
                        if (queryState.Filters.ContainsKey("keyword"))
                        {
                            string filterValue = queryState.Filters["keyword"];
                            where = where + " AND( ST.first LIKE '%" + filterValue + "%'";

                            where = where + " OR ST.last LIKE '%" + filterValue + "%'";

                            where = where + " OR (ST.first + ' ' + i1.last) LIKE '%" + filterValue + "%')";
                        }
                        where = where + ") ";
                        having = "";
                        having = having + " AND  (cr.cancel = 0)";
                    }
                    if (courseid == 0 && Authorization.AuthorizationHelper.CurrentInstructorUser != null)
                    {

                        where = " AND (";
                        where = where + " (c.instructorid =" + Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID + " OR c.instructorid2 =" + Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID + " OR c.instructorid3 =" + Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID + ")";
                        if (queryState.Filters.ContainsKey("keyword"))
                        {
                            string filterValue = queryState.Filters["keyword"];
                            where = where + " AND( ST.first LIKE '%" + filterValue + "%'";
                            where = where + " OR C.COURSEID LIKE '%" + filterValue + "%'";
                            where = where + " OR C.COURSENAME LIKE '%" + filterValue + "%'";
                            where = where + " OR C.COURSENUM LIKE '%" + filterValue + "%'";
                            where = where + " OR ST.last LIKE '%" + filterValue + "%'";

                            where = where + " OR (ST.first + ' ' + i1.last) LIKE '%" + filterValue + "%')";
                        }
                        where = where + ") ";
                        having = "";
                        having = having + " AND  (cr.cancel = 0)";

                    }
                    string selectcoursefields = "";
                    string groupbyfields = "";
                    selectcoursefields = selectcoursefields + "(CASE WHEN CHARINDEX('Enrolled\":1', cr.EnrollToWaitListConfig) > 0 or CHARINDEX('Waiting\":1', cr.EnrollToWaitListConfig) > 0 THEN 'Approved' ELSE 'Not Approved' END) AS EnrollToWaitStatus, cr.EnrollToWaitListConfig,";
                    selectcoursefields = selectcoursefields + "c.cancelcourse, (c.COURSENAME + cast(C.courseid as nvarchar)) AS CourseNameID,";
                    selectcoursefields = selectcoursefields + "(CONVERT(VARCHAR(8), startdate, 112) + cast(C.courseid as nvarchar)) AS CourseDateID,";
                    selectcoursefields = selectcoursefields + "c.courseid, c.COURSENAME, c.COURSENUM, c.Location, c.internalclass, c.maxenroll, c.maxwait,";
                    selectcoursefields = selectcoursefields + "c.room, c.ACCOUNTNUM, c.instructorid, c.instructorid2, c.instructorid3,";
                    selectcoursefields = selectcoursefields + "c.materials, c.days, c.credithours, c.CourseCloseDays,";
                    selectcoursefields = selectcoursefields + "cast(c.DESCRIPTION as nvarchar) as description,";

                    //selectcoursefields = selectcoursefields + "CM.MainCategory as maincategory,CS.SubCategory as subcategory,";
                    selectcoursefields = selectcoursefields + "(SELECT TOP 1 CM.MainCategory FROM MainCategories AS CM WHERE CM.COURSEID = C.COURSEID and mainorder=1) AS maincategory,";
                    selectcoursefields = selectcoursefields + "(SELECT TOP 1 CS.SubCategory FROM MainCategories AS CM  LEFT JOIN SubCategories AS CS ON CS.MainCategoryID = CM.MainCategoryID WHERE CM.COURSEID = C.COURSEID and mainorder=1) AS subcategory,";
                    selectcoursefields = selectcoursefields + "(SELECT TOP 1 CSS.SubSubCategory FROM MainCategories AS CM  LEFT JOIN SubCategories AS CS ON CS.MainCategoryID = CM.MainCategoryID";
                    selectcoursefields = selectcoursefields + " LEFT JOIN SubSubCategories AS CSS ON CSS.MainCategoryID = CM.MainCategoryID WHERE CM.COURSEID = C.COURSEID and mainorder=1) AS subsubcategory,";

                    selectcoursefields = selectcoursefields + "cr.STUDENTID, cr.WAITING, cast(cr.internalnote as nvarchar(250)) AS internalnote,";
                    selectcoursefields = selectcoursefields + "cast(cr.enrollmentnote as nvarchar(250)) as enrollmentnote,";
                    selectcoursefields = selectcoursefields + "cr.OrderNumber as ordernum, cr.PAYMETHOD, cr.payNumber, cr.authnum, cr.refnumber,";
                    selectcoursefields = selectcoursefields + "cr.CouponCode, cr.CouponDiscount, cr.CouponDetails,";
                    selectcoursefields = selectcoursefields + "cr.StudentGrade, cast(cr.COURSECOST as money) as course, cr.PAIDINFULL,";
                    selectcoursefields = selectcoursefields + "cr.TotalPaid as amountpaid, cr.RosterID, cr.creditapplied,";
                    selectcoursefields = selectcoursefields + "cr.cancel, cr.Attended, cr.InvoiceNumber, cr.InvoiceDate,";
                    selectcoursefields = selectcoursefields + "cr.DateAdded,";
                    //selectcoursefields = selectcoursefields + " (SELECT CASE WHEN cast(cr.crinitialauditinfo as NVARCHAR(500)) LIKE '%WaitListEnrolledDate:%' THEN (SELECT Item FROM (SELECT ROW_NUMBER() OVER (ORDER BY Item DESC) AS RowNum, Item FROM dbo.SplitString('a' + cast(cr.crinitialauditinfo as NVARCHAR(500)), ':')) sub WHERE RowNum = 2) ELSE '' END) AS crinitialauditinfo, ";
                    selectcoursefields = selectcoursefields + " '' AS crinitialauditinfo,";
                    selectcoursefields = selectcoursefields + "cc.coursechoice as coursechoice,";

                    selectcoursefields = selectcoursefields + "st.first, st.last, st.username, st.STATE, st.EMAIL, st.additionalemail,";
                    selectcoursefields = selectcoursefields + "st.ADDRESS, st.CITY, st.ZIP, st.COUNTRY,";
                    selectcoursefields = selectcoursefields + "st.HOMEPHONE, st.WORKPHONE, st.FAX, ";
                    selectcoursefields = selectcoursefields + "st.StudRegField1, st.studRegfield2,";
                    selectcoursefields = selectcoursefields + "st.StudRegField3, st.studRegfield4,";
                    selectcoursefields = selectcoursefields + "st.StudRegField5, st.studRegfield6,";
                    selectcoursefields = selectcoursefields + "st.StudRegField7, st.studRegfield8,";
                    selectcoursefields = selectcoursefields + "st.StudRegField9, st.studRegfield10,";
                    selectcoursefields = selectcoursefields + "st.StudRegField11, st.studRegfield12,";
                    selectcoursefields = selectcoursefields + "st.StudRegField13, st.studRegfield14,";
                    selectcoursefields = selectcoursefields + "st.StudRegField15, st.studRegfield16,";
                    selectcoursefields = selectcoursefields + "st.StudRegField17, st.studRegfield18,";
                    selectcoursefields = selectcoursefields + "st.StudRegField19, st.studRegfield20,";
                    selectcoursefields = selectcoursefields + "st.hiddenstudregfield1, st.hiddenstudregfield2,";
                    selectcoursefields = selectcoursefields + "st.readonlystudregfield1, st.readonlystudregfield2,";
                    selectcoursefields = selectcoursefields + "st.readonlystudregfield3, st.readonlystudregfield4,";

                    selectcoursefields = selectcoursefields + "sc.LOCATION as studentschool,";
                    selectcoursefields = selectcoursefields + "ds.district,";
                    selectcoursefields = selectcoursefields + "gl.grade as studentgradelevel,";

                    selectcoursefields = selectcoursefields + "i1.first as i1first, i1.last as i1last,";
                    selectcoursefields = selectcoursefields + "i2.first as i2first, i2.last as i2last,";
                    selectcoursefields = selectcoursefields + "i3.first as i3first, i3.last as i3last ";

                    groupbyfields = groupbyfields + "EnrollToWaitListConfig, c.cancelcourse, C.courseid, c.COURSENAME, c.COURSENUM, c.Location, c.internalclass, c.maxenroll, c.maxwait,";
                    groupbyfields = groupbyfields + "c.room, c.ACCOUNTNUM, c.instructorid, c.instructorid2, c.instructorid3,";
                    groupbyfields = groupbyfields + "c.materials, c.days, c.credithours, c.CourseCloseDays,";
                    groupbyfields = groupbyfields + "c.STREET, c.CITY, c.STATE, c.ZIP,";
                    groupbyfields = groupbyfields + "cast(c.DESCRIPTION as nvarchar),";
                    //groupbyfields = groupbyfields + "CM.MainCategory, CS.SubCategory, ";

                    groupbyfields = groupbyfields + "cr.STUDENTID, cr.WAITING, cr.internalnote, cr.enrollmentnote,";
                    groupbyfields = groupbyfields + "cr.OrderNumber, cr.PAYMETHOD, cr.payNumber, cr.authnum, cr.refnumber,";
                    groupbyfields = groupbyfields + "cr.CouponCode, cr.CouponDiscount, cr.CouponDetails,";
                    groupbyfields = groupbyfields + "cr.StudentGrade, cast(cr.COURSECOST as money), cr.PAIDINFULL,";
                    groupbyfields = groupbyfields + "cr.TotalPaid, cr.RosterID, cr.creditapplied,";
                    groupbyfields = groupbyfields + "cr.cancel, cr.Attended, cr.InvoiceNumber, cr.InvoiceDate, cr.SingleRosterDiscountAmount,";
                    groupbyfields = groupbyfields + "cr.DateAdded, cast(cr.crinitialauditinfo as NVARCHAR(500)),";
                    groupbyfields = groupbyfields + "cc.coursechoice,";

                    groupbyfields = groupbyfields + "st.first, st.last, st.username, st.STATE, st.EMAIL, st.additionalemail,";
                    groupbyfields = groupbyfields + "st.ADDRESS, st.CITY, st.ZIP, st.COUNTRY,";
                    groupbyfields = groupbyfields + "st.HOMEPHONE, st.WORKPHONE, st.FAX, ";
                    groupbyfields = groupbyfields + "st.StudRegField1, st.studregfield2,";
                    groupbyfields = groupbyfields + "st.StudRegField3, st.studregfield4,";
                    groupbyfields = groupbyfields + "st.StudRegField5, st.studregfield6,";
                    groupbyfields = groupbyfields + "st.StudRegField7, st.studregfield8,";
                    groupbyfields = groupbyfields + "st.StudRegField9, st.studregfield10,";
                    groupbyfields = groupbyfields + "st.StudRegField11, st.studregfield12,";
                    groupbyfields = groupbyfields + "st.StudRegField13, st.studregfield14,";
                    groupbyfields = groupbyfields + "st.StudRegField15, st.studregfield16,";
                    groupbyfields = groupbyfields + "st.StudRegField17, st.studregfield18,";
                    groupbyfields = groupbyfields + "st.StudRegField19, st.studregfield20,";
                    groupbyfields = groupbyfields + "st.hiddenstudregfield1, st.hiddenstudregfield2,";
                    groupbyfields = groupbyfields + "st.readonlystudregfield1, st.readonlystudregfield2,";
                    groupbyfields = groupbyfields + "st.readonlystudregfield3, st.readonlystudregfield4,";

                    groupbyfields = groupbyfields + "sc.LOCATION,";
                    groupbyfields = groupbyfields + "ds.District,";
                    groupbyfields = groupbyfields + "gl.grade,";

                    groupbyfields = groupbyfields + "startdate,enddate,";
                    groupbyfields = groupbyfields + "i1.first, i1.last,";
                    groupbyfields = groupbyfields + "i2.first, i2.last,";
                    groupbyfields = groupbyfields + "i3.first, i3.last,se.daddress ";

                    selectcoursefields = selectcoursefields + ", (i1.first +' '+ i1.last) AS instructor";
                    selectcoursefields = selectcoursefields + ", (i2.first +' '+ i2.last) AS instructor2";
                    selectcoursefields = selectcoursefields + ", (i3.first +' '+ i3.last) AS instructor3";
                    ;
                    selectcoursefields = selectcoursefields + ", (case when cr.cancel =0 then 'No' else 'Yes' end) as cancelledtxt";
                    selectcoursefields = selectcoursefields + ", (case when cr.WAITING =0 then 'No' else 'Yes' end) as waitingtxt";
                    selectcoursefields = selectcoursefields + ", (case when cr.Attended =0 then 'No' else 'Yes' end) as attendedtxt";
                    selectcoursefields = selectcoursefields + ", (case when cr.PAIDINFULL =0 then 'No' else 'Yes' end) as PaidFulltxt";

                    selectcoursefields = selectcoursefields + ",(cast(CreditApplied as money)) as Credited";

                    selectcoursefields = selectcoursefields + ",(SELECT COUNT(*) from RosterMaterials WHERE RosterMaterials.RosterID = cr.RosterID) as RMCount";
                    selectcoursefields = selectcoursefields + ", cast((select case when rm.product_name<>'' then  rm.product_name + ', ' else '' end from RosterMaterials rm inner join Materials m on rm.productID = m.productID where RosterID=cr.RosterID group by rm.product_name for xml path(''))as varchar(500)) as materialnames ";

                    selectcoursefields = selectcoursefields + ",(Select sum(cast(Shipping_cost as money)) + sum(case when (taxable <> 0) then price * (salestax/100.) else 0 end) + sum(case when (Priceincluded=0) then price else 0 end) from RosterMaterials WHERE RosterMaterials.RosterID = cr.RosterID) as Material";

                    selectcoursefields = selectcoursefields + ",(cast(cr.COURSECOST as money)+ (Select ISNULL(sum(cast(Shipping_cost as money)),0) + ISNULL(sum(case when (taxable <> 0) then price * (salestax/100.) else 0 end),0) + ISNULL(sum(case when (Priceincluded=0) then price else 0 end),0) from RosterMaterials WHERE RosterMaterials.RosterID = cr.RosterID)) as CourseTotal";

                    selectcoursefields = selectcoursefields + ",cast(cr.COURSECOST as money) - ( case when (cr.SingleRosterDiscountAmount <>0) then ISNULL(cast(cr.SingleRosterDiscountAmount as money),0)  else ISNULL(cast(cr.CouponDiscount as money),0) end) +(Select ISNULL(sum(cast(Shipping_cost as money)),0) + ISNULL(sum(case when (taxable <> 0) then price * (salestax/100.) else 0 end),0) + ISNULL(sum(case when (Priceincluded=0) then price else 0 end),0) from RosterMaterials WHERE RosterMaterials.RosterID = cr.RosterID) as TxTotal";

                    selectcoursefields = selectcoursefields + ",('\"' + st.ADDRESS	+ (case when (st.CITY='') then '' else (', ' + st.CITY) end) + (case when (st.STATE='') then '' else (', ' + st.STATE) end) + (case when (st.COUNTRY is null) then '' else (', ' + st.COUNTRY) end) + ' ' + st.ZIP + '\"') as CompleteAddress";

                    selectcoursefields = selectcoursefields + ",('\"' + c.location + ' ' + c.STREET	+ (case when (c.CITY='') then '' else (', ' + c.CITY) end) + (case when (c.STATE='') then '' else (', ' + c.STATE) end) + ' ' + c.ZIP + '\"') as CourseLocation";


                    selectcoursefields = selectcoursefields + ",startdate";
                    selectcoursefields = selectcoursefields + ",enddate,daddress";
                    selectcoursefields = selectcoursefields + ",(ROW_NUMBER() OVER(PARTITION BY C.courseid ORDER BY st.last)) AS Count";



                    where = where + " AND C.CANCELCOURSE = 0 ";
                    where = where + " AND C.CourseNum <> '~ZZZZZZ~'";




                    string sql = "";
                    page = 1;
                    int count = 0;
                    string join = "";


                    if (page > 0)
                    {
                        sql = sql + " WITH Query AS (";
                    }

                    sql = sql + " SELECT ";
                    sql = sql + selectcoursefields;
                    sql = sql + " FROM [Courses] AS C";

                    sql = sql + " INNER JOIN [Course Roster] AS CR ON C.COURSEID = CR.COURSEID";
                    sql = sql + " LEFT JOIN Students AS ST ON CR.STUDENTID = ST.STUDENTID";
                    sql = sql + " LEFT JOIN Schools AS SC ON ST.SCHOOL = SC.LOCATIONID";
                    sql = sql + " LEFT JOIN Districts AS DS ON ST.DISTRICT = DS.DISTID";
                    sql = sql + " LEFT JOIN [Grade Levels] AS GL ON ST.grade = GL.gradeid";
                    sql = sql + " LEFT JOIN (select courseid, min(coursedate) +' '+ min(starttime) as startdate, max(coursedate) +' '+ min(finishtime) as enddate from [course times]  group by courseid) CT ON C.COURSEID = CT.COURSEID";
                    sql = sql + " LEFT JOIN (  select (daddress +', '+dcity+', '+ dstate +' ' +dzip) as DADDRESS,districtID from [DistrictExtraInfo] group by districtID,daddress,dcity,dstate,dzip,daddress) SE ON ST.DISTRICT  = SE.districtID";
                    sql = sql + " LEFT JOIN Instructors AS I1 ON I1.instructorid = C.instructorid";
                    sql = sql + " LEFT JOIN Instructors AS I2 ON I2.instructorid = C.instructorid2";
                    sql = sql + " LEFT JOIN Instructors AS I3 ON I3.instructorid = C.instructorid3";
                    sql = sql + " LEFT JOIN CourseChoices AS CC ON CC.CourseChoiceId = CR.studentchoicecourse";
                    //sql = sql + " LEFT JOIN MainCategories AS CM ON CM.COURSEID = C.COURSEID and mainorder=1";
                    sql = sql + " LEFT JOIN MainCategories AS CM ON CM.COURSEID = C.COURSEID";
                    sql = sql + " LEFT JOIN SubCategories AS CS ON CS.MainCategoryID = CM.MainCategoryID ";
                    sql = sql + " LEFT JOIN SubSubCategories AS CSS ON CSS.MainCategoryID = CM.MainCategoryID ";

                    sql = sql + join;
                    sql = sql + " WHERE 1=1";
                    sql = sql + where;
                    sql = sql + " GROUP BY " + groupbyfields;
                    sql = sql + " HAVING 1=1 ";
                    sql = sql + having;
                    if (page > 0)
                    {
                        sql = sql + " )";


                        string countSQL = sql;
                        countSQL = countSQL + " SELECT COUNT(DISTINCT courseid) FROM Query  ";
                        var countCmd = new SqlCommand();
                        countCmd.CommandText = countSQL;
                        countCmd.Connection = connection;

                        var cmdParams = cmd.Parameters.GetEnumerator();
                        while (cmdParams.MoveNext())
                        {
                            var param = cmdParams.Current as SqlParameter;
                            var newParam = new SqlParameter();
                            countCmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }
                        count = (int)countCmd.ExecuteScalar();

                        sql = sql + " ,Numbered AS (";
                        sql = sql + " SELECT *, ";
                        sql = sql + " DENSE_RANK() OVER (" + orderby + ") AS RowNumber";
                        sql = sql + " FROM Query";
                        sql = sql + " )";
                        sql = sql + " SELECT * FROM Numbered";
                        if (courseid != 0)
                        {
                        }
                        else
                        {
                            if (!isExport)
                            {
                                sql = sql + " WHERE RowNumber BETWEEN " + (start + 1) + " AND " + (start + limit);
                            }
                        }
                        sql = sql + " " + orderby;
                    }
                    var result = new List<RosterReport_Object>();
                    string exportFileName = "RosterReport" + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".csv";
                    string directory = AppDomain.CurrentDomain.BaseDirectory + @"Temp\";
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        result = (from rw in dataTable.AsEnumerable()
                                  select new RosterReport_Object
                                  {
                                      EnrollToWaitStatus = Convert.ToString(rw["EnrollToWaitStatus"]),
                                      EnrollToWaitListConfig = Convert.ToString(rw["EnrollToWaitListConfig"]),
                                      coursenameid = Convert.ToString(rw["CourseNameID"]),
                                      cancelcourse = Convert.ToString(rw["cancelcourse"]),
                                      coursedateid = Convert.ToString(rw["CourseDateID"]),
                                      courseid = Convert.ToString(rw["courseid"]),
                                      coursename = Convert.ToString(rw["COURSENAME"]),
                                      coursenum = Convert.ToString(rw["COURSENUM"]),
                                      Location = Convert.ToString(rw["Location"]),
                                      internalclass = Convert.ToString(rw["internalclass"]),
                                      maxenroll = Convert.ToString(rw["maxenroll"]),
                                      maxwait = Convert.ToString(rw["maxwait"]),
                                      room = Convert.ToString(rw["room"]),
                                      accountnum = Convert.ToString(rw["ACCOUNTNUM"]),
                                      instructorid = Convert.ToString(rw["instructorid"]),
                                      instructorid2 = Convert.ToString(rw["instructorid2"]),
                                      instructorid3 = Convert.ToString(rw["instructorid3"]),
                                      materials = Convert.ToString(rw["materials"]),
                                      days = Convert.ToString(rw["days"]),
                                      credithours = Convert.ToString(rw["credithours"]),
                                      CourseCloseDays = Convert.ToString(rw["CourseCloseDays"]),
                                      description = Convert.ToString(rw["description"]),
                                      maincategory = Convert.ToString(rw["maincategory"]),
                                      subcategory = Convert.ToString(rw["subcategory"]),
                                      subsubcategory = Convert.ToString(rw["subsubcategory"]),
                                      studentid = Convert.ToString(rw["STUDENTID"]),
                                      WAITING = Convert.ToString(rw["WAITING"]),
                                      internalnote = Convert.ToString(rw["internalnote"]),
                                      enrollmentnote = Convert.ToString(rw["enrollmentnote"]),
                                      ordernum = Convert.ToString(rw["ordernum"]),
                                      paymethod = Convert.ToString(rw["PAYMETHOD"]),
                                      paynumber = Convert.ToString(rw["payNumber"]),
                                      authnum = Convert.ToString(rw["authnum"]),
                                      refnumber = Convert.ToString(rw["refnumber"]),
                                      couponcode = Convert.ToString(rw["CouponCode"]),
                                      coupondiscount = Convert.ToString(rw["CouponDiscount"]),
                                      coupondetails = Convert.ToString(rw["CouponDetails"]),
                                      studentgrade = Convert.ToString(rw["StudentGrade"]),
                                      course = Convert.ToString(rw["course"]),
                                      paidinfull = Convert.ToString(rw["PAIDINFULL"]),
                                      amountpaid = Convert.ToString(rw["amountpaid"]),
                                      rosterid = Convert.ToString(rw["RosterID"]),
                                      creditapplied = Convert.ToString(rw["creditapplied"]),
                                      cancel = Convert.ToString(rw["cancel"]),
                                      attended = Convert.ToString(rw["Attended"]),
                                      invoicenumber = Convert.ToString(rw["InvoiceNumber"]),
                                      invoicedate = Convert.ToString(rw["InvoiceDate"]),
                                      dateadded = String.Format("{0:M/d/yyyy}", (rw["DateAdded"])),
                                      crinitialauditinfo = Convert.ToString(rw["crinitialauditinfo"]),
                                      coursechoice = Convert.ToString(rw["coursechoice"]),
                                      first = Convert.ToString(rw["first"]),
                                      last = Convert.ToString(rw["last"]),
                                      username = Convert.ToString(rw["username"]),
                                      state = Convert.ToString(rw["STATE"]),
                                      email = Convert.ToString(rw["EMAIL"]),
                                      additionalemail = Convert.ToString(rw["AdditionalEMAIL"]),
                                      address = Convert.ToString(rw["ADDRESS"]),
                                      city = Convert.ToString(rw["CITY"]),
                                      zip = Convert.ToString(rw["ZIP"]),
                                      country = rw["COUNTRY"] == null ? "" : Convert.ToString(rw["COUNTRY"]),
                                      homephone = Convert.ToString(rw["HOMEPHONE"]),
                                      workphone = Convert.ToString(rw["WORKPHONE"]),
                                      fax = Convert.ToString(rw["FAX"]),
                                      studregfield1 = Convert.ToString(rw["studregfield1"]),
                                      studregfield2 = Convert.ToString(rw["studregfield2"]),
                                      studregfield3 = Convert.ToString(rw["studregfield3"]),
                                      studregfield4 = Convert.ToString(rw["studregfield4"]),
                                      studregfield5 = Convert.ToString(rw["studregfield5"]),
                                      studregfield6 = Convert.ToString(rw["studregfield6"]),
                                      studregfield7 = Convert.ToString(rw["studregfield7"]),
                                      studregfield8 = Convert.ToString(rw["studregfield8"]),
                                      studregfield9 = Convert.ToString(rw["studregfield9"]),
                                      studregfield10 = Convert.ToString(rw["studregfield10"]),
                                      studregfield11 = Convert.ToString(rw["studregfield11"]),
                                      studregfield12 = Convert.ToString(rw["studregfield12"]),
                                      studregfield13 = Convert.ToString(rw["studregfield13"]),
                                      studregfield14 = Convert.ToString(rw["studregfield14"]),
                                      studregfield15 = Convert.ToString(rw["studregfield15"]),
                                      studregfield16 = Convert.ToString(rw["studregfield16"]),
                                      studregfield17 = Convert.ToString(rw["studregfield17"]),
                                      studregfield18 = Convert.ToString(rw["studregfield18"]),
                                      studregfield19 = Convert.ToString(rw["studregfield19"]),
                                      studregfield20 = Convert.ToString(rw["studregfield20"]),
                                      hiddenstudregfield1 = Convert.ToString(rw["hiddenstudregfield1"]),
                                      hiddenstudregfield2 = Convert.ToString(rw["hiddenstudregfield2"]),
                                      readonlystudregfield1 = Convert.ToString(rw["readonlystudregfield1"]),
                                      readonlystudregfield2 = Convert.ToString(rw["readonlystudregfield2"]),
                                      readonlystudregfield3 = Convert.ToString(rw["readonlystudregfield3"]),
                                      readonlystudregfield4 = Convert.ToString(rw["readonlystudregfield4"]),
                                      studentschool = Convert.ToString(rw["studentschool"]),
                                      district = Convert.ToString(rw["district"]),
                                      studentgradelevel = Convert.ToString(rw["studentgradelevel"]),
                                      i1first = Convert.ToString(rw["i1first"]),
                                      i1last = Convert.ToString(rw["i1last"]),
                                      i2first = Convert.ToString(rw["i2first"]),
                                      i2last = Convert.ToString(rw["i2last"]),
                                      i3first = Convert.ToString(rw["i3first"]),
                                      i3last = Convert.ToString(rw["i3last"]),
                                      instructor = Convert.ToString(rw["instructor"]),
                                      instructor2 = Convert.ToString(rw["instructor2"]),
                                      instructor3 = Convert.ToString(rw["instructor3"]),
                                      cancelledtxt = Convert.ToString(rw["cancelledtxt"]),
                                      waitingtxt = Convert.ToString(rw["waitingtxt"]),
                                      attendedtxt = Convert.ToString(rw["attendedtxt"]),
                                      PaidFulltxt = Convert.ToString(rw["PaidFulltxt"]),
                                      Credited = Convert.ToString(rw["Credited"]),
                                      RMCount = Convert.ToString(rw["RMCount"]),
                                      materialnames = Convert.ToString(rw["materialnames"]),
                                      Material = Convert.ToString(rw["Material"]),
                                      CourseTotal = Convert.ToString(rw["CourseTotal"]),
                                      TxTotal = Convert.ToString(rw["TxTotal"]),
                                      CompleteAddress = Convert.ToString(rw["CompleteAddress"]),
                                      CourseLocation = Convert.ToString(rw["CourseLocation"]),
                                      startdate = Convert.ToString(rw["startdate"]),
                                      enddate = Convert.ToString(rw["enddate"]),
                                      daddress = Convert.ToString(rw["daddress"]),
                                      count = Convert.ToInt64(rw["Count"]),
                                      RowNumber = Convert.ToInt64(rw["RowNumber"]),


                                  }).ToList();

                        if (courseid != 0)
                        {
                            page = queryState.Page;
                            records = result.Count();
                            result = result.Skip((page - 1) * limit).Take(limit).ToList();

                        }
                        if (isExport || (Authorization.AuthorizationHelper.CurrentInstructorUser != null))
                        {

                            StringBuilder sb = new StringBuilder();
                            Dictionary<string, string> FldHeadersText = queryState.FldHeaders;

                            foreach (var dta in dataTable.Columns.Cast<DataColumn>())
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


                            foreach (DataRow row in dataTable.Rows)
                            {
                                var fields = new List<string>();
                                foreach (var col in queryState.FldHeaders)
                                {
                                    try
                                    {
                                        string dta = row[col.Key].ToString().Replace(",", " ");
                                        fields.Add(dta);
                                    }
                                    catch
                                    {
                                    }
                                }
                                sb.AppendLine(string.Join(",", fields.ToArray()));
                            }
                            if ((Authorization.AuthorizationHelper.CurrentInstructorUser != null))
                            {
                                exportFileName = "RosterReport_" + export + courseid + "_" + Authorization.AuthorizationHelper.CurrentInstructorUser.INSTRUCTORID + ".csv";
                            }
                            else
                            {
                            }
                            System.IO.File.WriteAllText(directory + exportFileName, sb.ToString());
                        }
                        reader.Close();
                    }

                    var model = new Gsmu.Api.Data.ViewModels.Grid.GridModel<RosterReport_Object>(count, queryState);
                    if (model.TotalCount > 0)
                    {
                        model.Result = result;
                    }

                    connection.Close();
                    RosterReport_result RosterReportResult = new RosterReport_result();
                    if (isExport)
                    {
                        //RosterReportResult.rosters = null;
                    }
                    else
                    {
                        //RosterReportResult.rosters = model.Result;
                        exportFileName = null;
                    }
                    RosterReportResult.rosters = model.Result;
                    if (courseid != 0)
                    {
                        RosterReportResult.recordCount = records;
                    }
                    else
                    {
                        RosterReportResult.recordCount = model.TotalCount;
                    }
                    RosterReportResult.exportFileName = exportFileName;
                    return RosterReportResult;


            }
        }

}
    public class RosterReport_Object
    {
        internal string username;
        public string EnrollToWaitStatus { get; set; }
        public string EnrollToWaitListConfig { get; set; }
        public string cancelcourse { get; set; }
        public string coursenameid { get; set; }
        public string coursedateid { get; set; }
        public string courseid { get; set; }
        public string coursename { get; set; }
        public string coursenum { get; set; }
        public string Location { get; set; }
        public string internalclass { get; set; }
        public string maxenroll { get; set; }
        public string maxwait { get; set; }
        public string room { get; set; }
        public string accountnum { get; set; }
        public string instructorid { get; set; }
        public string instructorid2 { get; set; }
        public string instructorid3 { get; set; }
        public string materials { get; set; }
        public string days { get; set; }
        public string credithours { get; set; }
        public string CourseCloseDays { get; set; }
        public string description { get; set; }
        public string studentid { get; set; }
        public string WAITING { get; set; }
        public string internalnote { get; set; }
        public string enrollmentnote { get; set; }
        public string ordernum { get; set; }
        public string paymethod { get; set; }
        public string paynumber { get; set; }
        public string authnum { get; set; }
        public string refnumber { get; set; }
        public string couponcode { get; set; }
        public string coupondiscount { get; set; }
        public string coupondetails { get; set; }
        public string studentgrade { get; set; }
        public string course { get; set; }
        public string paidinfull { get; set; }
        public string amountpaid { get; set; }
        public string rosterid { get; set; }
        public string creditapplied { get; set; }
        public string cancel { get; set; }
        public string attended { get; set; }
        public string invoicenumber { get; set; }
        public string invoicedate { get; set; }
        public string dateadded { get; set; }
        public string crinitialauditinfo { get; set; }
        public string coursechoice { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string state { get; set; }
        public string email { get; set; }
        public string additionalemail { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string homephone { get; set; }
        public string workphone { get; set; }
        public string fax { get; set; }
        public string studregfield1 { get; set; }
        public string studregfield2 { get; set; }
        public string studregfield3 { get; set; }
        public string studregfield4 { get; set; }
        public string studregfield5 { get; set; }
        public string studregfield6 { get; set; }
        public string studregfield7 { get; set; }
        public string studregfield8 { get; set; }
        public string studregfield9 { get; set; }
        public string studregfield10 { get; set; }
        public string studregfield11 { get; set; }
        public string studregfield12 { get; set; }
        public string studregfield13 { get; set; }
        public string studregfield14 { get; set; }
        public string studregfield15 { get; set; }
        public string studregfield16 { get; set; }
        public string studregfield17 { get; set; }
        public string studregfield18 { get; set; }
        public string studregfield19 { get; set; }
        public string studregfield20 { get; set; }
        public string hiddenstudregfield1 { get; set; }
        public string hiddenstudregfield2 { get; set; }
        public string readonlystudregfield1 { get; set; }
        public string readonlystudregfield2 { get; set; }
        public string readonlystudregfield3 { get; set; }
        public string readonlystudregfield4 { get; set; }
        public string studentschool { get; set; }
        public string district { get; set; }
        public string studentgradelevel { get; set; }
        public string i1first { get; set; }
        public string i1last { get; set; }
        public string i2first { get; set; }
        public string i2last { get; set; }
        public string i3first { get; set; }
        public string i3last { get; set; }
        public string instructor { get; set; }
        public string instructor2 { get; set; }
        public string instructor3 { get; set; }
        public string cancelledtxt { get; set; }
        public string waitingtxt { get; set; }
        public string attendedtxt { get; set; }
        public string PaidFulltxt { get; set; }
        public string Credited { get; set; }
        public string RMCount { get; set; }
        public string materialnames { get; set; }
        public string Material { get; set; }
        public string CourseTotal { get; set; }
        public string TxTotal { get; set; }
        public string CompleteAddress { get; set; }
        public string CourseLocation { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string daddress { get; set; }
        public string maincategory { get; set; }
        public string subcategory { get; set; }
        public Int64 count { get; set; }
        public Int64 RowNumber { get; set; }
        public string subsubcategory { get; set; }
    }
    public class RosterReport_result
    {
        public List<RosterReport_Object> rosters { get; set; }
        public int recordCount { get; set; }
        public string exportFileName { get; set; }
    }
}
