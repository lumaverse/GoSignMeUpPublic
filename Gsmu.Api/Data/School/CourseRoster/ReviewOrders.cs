using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.ViewModels.Grid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using entities = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.CourseRoster
{
    public class ReviewOrders
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


            var start = 0;
            if (Request.QueryString["start"] != null)
            {
                start = int.Parse(Request.QueryString["start"]);
            }
            var limit = 10;
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
            var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);
            var queryState = new QueryState(start, limit)
            {
                OrderByDirection = sorterResult.Value,
                OrderFieldString = sorterResult.Key,
                Filters = filterResult,
                Page = page
            };

            return queryState;
        }
        public static Orders_Object_result ReviewOrdersList(QueryState state)
        {
            entities.SchoolEntities db = new entities.SchoolEntities();
            var orderby = "ORDER BY dateadded DESC";
            string sqlInnerAnd = "";
            string sqlAnd = "";
            string status = "";
            string paymethod = "All Payments";
            string isShowOrdersWithMaterials = "";
            string isHideZeroCharges = "";
            string groupBySubsite = "";
            string dateRangeFrom = DateTime.Now.AddMonths(-6).ToString();
            string dateRangeTo = DateTime.Now.AddMonths(1).ToString();
            string sqlDateFilter1 = "";
            string sqlDateFilter2 = "";
            string dateFilter = "";

            string district = string.Empty;
            string school = string.Empty;
            string grade = string.Empty;

            if (state.OrderFieldString != null)
            {
                switch (state.OrderFieldString)
                {
                    case "orderid":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY orderid ";
                        else
                            orderby = "ORDER BY orderid Desc ";
                        break;
                    case "dateadded":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY dateadded ";
                        else
                            orderby = "ORDER BY dateadded Desc ";
                        break;
                    case "accountnum":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY Accountnum ";
                        else
                            orderby = "ORDER BY Accountnum Desc ";
                        break;
                    case "last":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY  Last ";
                        else
                            orderby = "ORDER BY  Last Desc ";
                        break;
                    case "first":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY First ";
                        else
                            orderby = "ORDER BY First Desc ";
                        break;
                    case "totalpaid":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY TotalPaid ";
                        else
                            orderby = "ORDER BY TotalPaid Desc ";
                        break;
                    case "payment type":
                    case "paymethod":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY Paymethod ";
                        else
                            orderby = "ORDER BY Paymethod Desc ";
                        break;
                    case "paynumber":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY PayNumber ";
                        else
                            orderby = "ORDER BY PayNumber Desc ";
                        break;
                    case "ordertotal":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY ordertotal ";
                        else
                            orderby = "ORDER BY ordertotal Desc ";
                        break;
                    case "status":
                        if (state.OrderByDirection == OrderByDirection.Ascending)
                            orderby = "ORDER BY Status ";
                        else
                            orderby = "ORDER BY Status  Desc ";
                        break;
                }
            }
            else
            {

            }
            if (state.Filters != null)
            {
                if (state.Filters.ContainsKey("keyword"))
                {
                    string filterValue = state.Filters["keyword"];
                    sqlInnerAnd = sqlInnerAnd + " And (B.Last LIKE '" + filterValue + "%' OR MB.Last LIKE '" + filterValue + "%' ";
                    sqlInnerAnd = sqlInnerAnd + " OR B.First LIKE '" + filterValue + "%' OR MB.First LIKE '" + filterValue + "%' ";
                    sqlInnerAnd = sqlInnerAnd + " Or CR.OrderNumber LIKE '%" + filterValue + "%' OR CR.MasterOrderNumber LIKE '%" + filterValue + "%' ";
                    sqlInnerAnd = sqlInnerAnd + " Or CR.OrderNumber LIKE '%" + filterValue + "%' OR CR.paynumber LIKE '%" + filterValue + "%' ";
                    sqlInnerAnd = sqlInnerAnd + " Or PAYMETHOD LIKE '%" + filterValue + "%' OR COURSENAME LIKE '%" + filterValue + "%' ";
                    sqlInnerAnd = sqlInnerAnd + " Or C.COURSEID LIKE '%" + filterValue + "%' OR C.COURSENUM LIKE '%" + filterValue + "%') ";
                }
                if (state.Filters.ContainsKey("status"))
                {
                    status = state.Filters["status"];
                }
                if (state.Filters.ContainsKey("paytype"))
                {
                    string filterValue = state.Filters["paytype"];
                    paymethod = filterValue;
                }
                if (state.Filters.ContainsKey("dateFilter"))
                {
                    string filterValue = state.Filters["dateFilter"];
                    dateFilter = filterValue;
                }

                if (state.Filters.ContainsKey("hidezerocharge"))
                {
                    string filterValue = state.Filters["hidezerocharge"];
                    isHideZeroCharges = filterValue;
                }
                if (state.Filters.ContainsKey("showordermaterials"))
                {
                    string filterValue = state.Filters["showordermaterials"];
                    isShowOrdersWithMaterials = filterValue;
                }
                if (state.Filters.ContainsKey("groupby"))
                {
                    string filterValue = state.Filters["groupby"];
                    groupBySubsite = filterValue;
                }
                if (state.Filters.ContainsKey("datefrom"))
                {
                    string filterValue = state.Filters["datefrom"];

                    if (filterValue.ToLower().Trim() == "null")
                    {
                        dateRangeFrom = DateTime.Now.AddMonths(-6).ToString();
                    }
                    else
                        dateRangeFrom = filterValue;
                }
                if (state.Filters.ContainsKey("dateto"))
                {
                    string filterValue = state.Filters["dateto"];

                    if (filterValue.ToLower().Trim() == "null")
                    {
                        dateRangeTo = DateTime.Now.AddMonths(1).ToString();
                    }
                    else
                        dateRangeTo = filterValue;
                }
                if (state.Filters.ContainsKey("district"))
                {
                    string filterValue = state.Filters["district"];
                    district = filterValue;
                }
                if (state.Filters.ContainsKey("school"))
                {
                    string filterValue = state.Filters["school"];
                    school = filterValue;
                }
                if (state.Filters.ContainsKey("grade"))
                {
                    string filterValue = state.Filters["grade"];
                    grade = filterValue;
                }
            }
            if (status != "")
            {
                if (status != "All")
                {
                    if (status == "NeedsRefund")
                    {
                        sqlInnerAnd = sqlInnerAnd + " AND CR.CreditedInFull <> 1 AND (C.cancelcourse <> 0 OR CR.cancel <> 0) ";
                        //sqlInnerAnd = sqlInnerAnd + " AND ISNULL(CR.CourseSalesTaxPaid,0) + ";
                        // sqlInnerAnd = sqlInnerAnd + " ISNULL(currentmaterials.totalmaterialcosts,0) + ISNULL(currentmaterials.totalSalesTax,0) )-0 <> ";
                        // sqlInnerAnd = sqlInnerAnd + " ISNULL(CAST(CR.creditapplied AS smallmoney)-0,0) AND ISNULL(CAST(CR.totalpaid AS SMALLMONEY)-0,0) > 0 ";

                        sqlInnerAnd = sqlInnerAnd + "  AND ISNULL(CAST(CR.totalpaid AS SMALLMONEY)-0,0) > 0 ";
                        sqlInnerAnd = sqlInnerAnd + " AND (CR.paidinfull = 1 or CR.paidinfull = -1) ";
                    }
                    else if (status == "CreditOnly")
                    {
                        sqlAnd = sqlAnd + " AND TotalCreditApplied IS NOT NULL ";
                    }
                    else if (status == "Exception")
                    {
                        sqlInnerAnd = sqlInnerAnd + " AND (CR.paidinfull=1 OR CR.paidinfull=-1) ";
                        sqlInnerAnd = sqlInnerAnd + " AND CR.PayMethod IN ('Filler','Amex','Mastercard','Visa') ";
                        sqlInnerAnd = sqlInnerAnd + " AND (CR.cancel = 0) ";
                    }
                    else
                    {
                        sqlAnd = sqlAnd + " AND Status = '" + status + "' ";
                    }
                }
            }
            if (paymethod != "All Payments")
            {
                sqlAnd = sqlAnd + " AND (PAYMETHOD IN ('" + paymethod.Replace(",", "','") + "')) ";
            }

            if (isHideZeroCharges == "true")
            {
                //  sqlAnd = sqlAnd + " AND (TotalPaid-0 > 0 AND  ordertotal>0) ";
            }


            if (isShowOrdersWithMaterials == "true")
            {
                sqlInnerAnd = sqlInnerAnd + " AND (c.MaterialsRequired = -1 or c.MaterialsRequired = 1) ";
            }

            if (dateFilter != "")
            {
                if (dateFilter == "Charge Date")
                {
                    sqlDateFilter1 = " AND chargedate >= '" + dateRangeFrom + "' AND chargedate<= '" + dateRangeTo + "'";
                    sqlDateFilter2 = " AND cr.chargedate >= '" + dateRangeFrom + "' AND cr.chargedate<= '" + dateRangeTo + "'";
                }
                if (dateFilter == "Order Date")
                {
                    sqlDateFilter1 = " AND DateAdded >= '" + dateRangeFrom + "' AND Dateadded <= '" + dateRangeTo + "' ";
                    sqlDateFilter2 = " AND CR.DateAdded >= '" + dateRangeFrom + "' AND CR.Dateadded <= '" + dateRangeTo + "' ";
                }
                if (dateFilter == "Activity Order")
                {
                    sqlDateFilter1 = " AND DateAdded >= '" + dateRangeFrom + "' AND Dateadded <= '" + dateRangeTo + "' ";
                    sqlDateFilter2 = "";
                }
            }

            string additionalSelect = string.Empty;
            string additionalGroupBy = string.Empty;

            if (!string.IsNullOrEmpty(district))
            {
                sqlInnerAnd = sqlInnerAnd + " And (B.DISTRICT IN ('" + district.Replace(",", "','") + "' ))";
                orderby = orderby + ", DISTRICT";
                additionalSelect = additionalSelect + ", B.DISTRICT";
                additionalGroupBy = additionalGroupBy + ", B.DISTRICT";
            }
            if (!string.IsNullOrEmpty(school))
            {
                sqlInnerAnd = sqlInnerAnd + "  And (B.SCHOOL IN ('" + school.Replace(",", "','") + "' ))";
                orderby = orderby + ", SCHOOL";
                additionalSelect = additionalSelect + ", B.SCHOOL";
                additionalGroupBy = additionalGroupBy + ", B.SCHOOL";
            }
            if (!string.IsNullOrEmpty(grade))
            {
                sqlInnerAnd = sqlInnerAnd + " And (B.GRADE IN ('" + grade.Replace(",", "','") + "' ))";
                orderby = orderby + ", GRADE";
                additionalSelect = additionalSelect + ", B.GRADE";
                additionalGroupBy = additionalGroupBy + ", B.GRADE";
            }
            string sql = " WITH Query AS (";
            sql = sql + "SELECT * FROM (SELECT DISTINCT CR.OrderNumber AS OrderId,cast((select case when AccountNum <>'' then  AccountNum  + ' , ' else Accountnum end  from courses AC inner join [Course Roster] AR ON AC.courseid = AR.courseid where OrderNumber=CR.OrderNumber order by Accountnum desc for xml path('')) as varchar(100)) AS ACCOUNTNUM, ";
            sql = sql + " B.Last,B.First, B.studentid, case when masterordernumber<>'' then 'Multiple Order' else  CR.masterordernumber end AS masterordernumber,CR.masterordernumber AS masterordernumber2,  MAX(CR.Waiting) - MIN(CR.Waiting) ";
            sql = sql + " AS PartialWaiting, MAX(ABS(CR.Waiting)) ";
            sql = sql + " AS WaitingStatus,(Cast(cr.dateadded as datetime)+' '+ Cast(min(cr.timeadded) as TIME(0))) as DateAdded, Paymethod, CR.PayNumber, (CONVERT(MONEY,TotalPaid)) AS TotalPaid, SUM(CONVERT(MONEY,CR.CreditApplied)) as TotalRefund ";
            //sql = sql + " AS WaitingStatus,(Cast(cr.dateadded as datetime)) as DateAdded, Paymethod, CR.PayNumber, (CONVERT(MONEY,TotalPaid)) AS TotalPaid, SUM(CONVERT(MONEY,CR.CreditApplied)) as TotalRefund ";
            sql = sql + ",  c2.cancelled,c1.notcancelled, ";
            sql = sql + " CASE WHEN (c2.cancelled <= 0 OR c2.cancelled IS NULL)  THEN ";
            sql = sql + " CASE WHEN (CR.paidinfull <> 0) THEN 'Approved' ELSE 'Pending' END  ELSE ";
            sql = sql + " CASE WHEN (c1.notcancelled>0) THEN 'Partially Cancelled' ELSE case when sq6.cancel=3 then 'Failed Payment' when sq6.cancel=2 then 'Incompleted Payment' else  'Cancelled' end END END ";
            sql = sql + " AS Status,  c.subsiteID AS C_subsiteID,MAX(Cr.ChargeDate) ";
            sql = sql + " AS LatestChargeDate,CR.RespMsg, CR.Result, ";
            sql = sql + " (B.Last +', '+ B.First) AS Customer, ";
            sql = sql + " 'ptype' AS [payment type], ";
            sql = sql + " (select sum(price) from rostermaterials rm left join [course roster] cr1 on rm.rosterid=cr1.rosterid  where cr1.ordernumber = cr.ordernumber) AS mATERIALCOST , cr.CouponDiscount, ";
            sql = sql + "  (SUM(Convert(Money,coursecost))+cr.CourseSalesTaxPaid+cr.creditcardfee) AS OrderTotal " + additionalSelect;
            sql = sql + " FROM (((((((([course Roster] CR ";
            sql = sql + " LEFT JOIN students B ON CR.Studentid = B.Studentid) ";
            sql = sql + " LEFT JOIN (SELECT RosterId, SUM(OrderTransactionAmount) AS CreditApplied ";
            sql = sql + " FROM OrderTransaction GROUP BY RosterId) AS CA ON CA.RosterId = CR.RosterId) ";
            sql = sql + ") LEFT JOIN students MB ON CR.EnrollMaster = MB.Studentid)  LEFT JOIN Courses C ";
            sql = sql + " ON CR.Courseid = C.Courseid)  ";
            sql = sql + ") LEFT JOIN (SELECT COUNT(c6.cancel) ";
            sql = sql + " AS notcancelled,c6.ordernumber  FROM [course Roster] c6 WHERE c6.cancel =0 GROUP BY c6.ordernumber) ";
            sql = sql + " AS c1  ON c1.ordernumber = CR.ordernumber ) LEFT JOIN (SELECT COUNT(c7.cancel) ";
            sql = sql + " AS cancelled, c7.ordernumber FROM [course Roster] c7 WHERE c7.cancel != 0 ";
            sql = sql + " GROUP BY c7.ordernumber) AS c2 ON c2.ordernumber = CR.ordernumber ) ";
            sql = sql + " left join (SELECT max(c8.cancel) as cancel,c8.ordernumber from [course Roster] c8 where c8.cancel != 0 group by c8.ordernumber) as sq6 on sq6.ordernumber = CR.ordernumber ";

            sql = sql + " WHERE 1 = 1 ";
            sql = sql + " AND CR.ordernumber <> '' AND ISNULL(C.CourseID, 0) <> 0 AND NOT B.studentID IS NULL ";
            sql = sql + " AND B.studentID<>0 ";
            sql = sql + sqlInnerAnd;
            sql = sql + sqlDateFilter2;
            sql = sql + " GROUP BY CR.OrderNumber, B.Last, B.First, B.studentid,ACCOUNTNUM,cr.CouponDiscount,cr.CourseSalesTaxPaid,cr.creditcardfee, ";
            sql = sql + " CR.masterordernumber, CR.DateAdded,c2.cancelled,c1.notcancelled,CR.paidinfull,CR.paymethod, sq6.cancel, ";

            if (dateFilter == "Activity Order")
            {
                sql = sql + " CR.PayNumber,TotalPaid,c.subsiteID,CR.RespMsg,CA.CreditApplied,CR.Result " + additionalGroupBy;
                sql = sql + " HAVING (TotalPaid > 0 OR ISNULL(SUM(CA.CreditApplied),0) > 0)) ";
            }
            else
            {
                sql = sql + " CR.PayNumber,TotalPaid,c.subsiteID,CR.RespMsg,CR.Result " + additionalGroupBy + ") ";

            }

            sql = sql + " AS V LEFT JOIN subsite ON subsite.subsiteID=V.C_subsiteID ";
            sql = sql + " WHERE 1=1 ";
            if (isHideZeroCharges == "true")
            {
                sql = sql + " AND (ordertotal>0 or TotalPaid>0) ";
            }
            sql = sql + sqlAnd;
            sql = sql + " )";

            using (var connection = Connections.GetSchoolConnection())
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.Connection = connection;

                var page = state.Page;
                var start = (state.Page - 1) * state.PageSize;
                var limit = state.PageSize;
                int count = 0;

                string countSQL = sql;
                countSQL = countSQL + " SELECT COUNT(DISTINCT OrderId) FROM Query  ";
                var countCmd = new SqlCommand();
                countCmd.CommandText = countSQL;
                countCmd.Connection = connection;
                countCmd.CommandTimeout = 420;
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
                sql = sql + " DENSE_RANK() OVER (" + orderby + ",orderid) AS RowNumber";
                sql = sql + " FROM Query";
                sql = sql + " )";
                sql = sql + " SELECT * FROM Numbered";
                sql = sql + " WHERE RowNumber BETWEEN " + (start + 1) + " AND " + (start + limit);


                var result = new List<Orders_Object>();
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    result = (from rw in dataTable.AsEnumerable()

                              select new Orders_Object
                              {
                                  status = Convert.ToString(rw["status"]),
                                  masterordernumber = Convert.ToString(rw["masterordernumber"]),
                                  masterordernumber2 = Convert.ToString(rw["masterordernumber2"]),
                                  orderid = Convert.ToString(rw["orderid"]),
                                  dateadded = Convert.ToString(rw["dateadded"]),
                                  accountnum = Convert.ToString(rw["accountnum"]),
                                  studentid = Convert.ToInt32(rw["studentid"]),
                                  last = Convert.ToString(rw["last"]),
                                  first = Convert.ToString(rw["first"]),
                                  waitingstatus = Convert.ToInt16(rw["waitingstatus"]),
                                  paymethod = Convert.ToString(rw["paymethod"]),
                                  paynumber = Convert.ToString(rw["paynumber"]),
                                  ordertotal = (Convert.IsDBNull(rw["ordertotal"]) ? 0 : Convert.ToDecimal(rw["ordertotal"]) + (Convert.IsDBNull(rw["mATERIALCOST"]) ? 0 : Convert.ToDecimal(rw["mATERIALCOST"]))) - (Convert.IsDBNull(rw["CouponDiscount"]) ? 0 : Convert.ToDecimal(rw["CouponDiscount"])),
                                  totalpaid = (Convert.IsDBNull(rw["totalpaid"]) ? 0 : Convert.ToDecimal(rw["totalpaid"]) - (Convert.IsDBNull(rw["TotalRefund"]) ? 0 : Convert.ToDecimal(rw["TotalRefund"]))),
                                  //coursenumber = Convert.ToString(rw["coursenumber"]),
                                  // cancel = Convert.ToInt32(rw["cancel"]),
                                  // paidinfull = Convert.ToInt32(rw["paidinfull"]),
                                  //  School = Convert.ToInt32(rw["School"]),
                                  //  Grade = Convert.ToInt32(rw["Grade"]),
                                  //  District = Convert.ToInt32(rw["District"]),
                              }).ToList();
                    var model = new Gsmu.Api.Data.ViewModels.Grid.GridModel<Orders_Object>(count, state);
                    if (model.TotalCount > 0)
                    {
                        model.Result = result;
                    }

                    connection.Close();
                    Orders_Object_result Orders_Object_result = new Orders_Object_result();
                    Orders_Object_result.reviewOrders = model.Result;
                    Orders_Object_result.resultcount = model.TotalCount;
                    Orders_Object_result.totalCount = model.TotalCount;
                    Orders_Object_result.status = sql;
                    return Orders_Object_result;
                }
            }
        }

        public static Orders_Object_result GenerateReviewOrderReport(QueryState state, string folderpath)
        {
            string sqlInnerAnd = "";
            string sqlAnd = "";
            string status = "";
            string paymethod = "All Payments";
            string isShowOrdersWithMaterials = "";
            string isHideZeroCharges = "";
            string groupBySubsite = "";
            var dateRangeFrom = DateTime.Now.AddMonths(-6);
            var dateRangeTo = DateTime.Now.AddMonths(1);
            string sqlDateFilter1 = "";
            string sqlDateFilter2 = "";
            string dateFilter = "";

            if (state.Filters != null)
            {
                if (state.Filters.ContainsKey("datefrom"))
                {
                    string filterValue = state.Filters["datefrom"];

                    if (filterValue.ToLower().Trim() == "null")
                    {
                        dateRangeFrom = DateTime.Now.AddMonths(-6);
                    }
                    else
                        dateRangeFrom = DateTime.Parse(filterValue);
                }
                if (state.Filters.ContainsKey("dateto"))
                {
                    string filterValue = state.Filters["dateto"];

                    if (filterValue.ToLower().Trim() == "null")
                    {
                        dateRangeTo = DateTime.Now.AddMonths(1);
                    }
                    else
                        dateRangeTo = DateTime.Parse(filterValue);

                }
            }
            using (var db = new SchoolEntities())
            {
                var dbReport = (from roster in db.ReviewOrderViews
                                select
                                    new Orders_Object
                                    {
                                        status = (roster.Status),
                                        masterordernumber = (roster.masterordernumber2),
                                        masterordernumber2 = (roster.masterordernumber2),
                                        orderid = (roster.OrderId),
                                        full_dateadded = (roster.DateRosterAdded),
                                        accountnum = roster.ACCOUNTNUM,
                                        studentid = (roster.STUDENTID),
                                        last = (roster.LAST),
                                        first = (roster.FIRST),
                                        waitingstatus = 0,
                                        paymethod = (roster.PAYMETHOD),
                                        paynumber = (roster.payNumber),
                                        ordertotal = (((roster.OrderTotal == null) ? 0 : roster.OrderTotal)),
                                        double_materialcost = ((roster.mATERIALCOST == null) ? 0 : (roster.mATERIALCOST)),
                                        double_coupondiscount = ((roster.CouponDiscount == null) ? 0 : (roster.CouponDiscount)),
                                        totalpaid = (roster.RosterTotalPaid == null) ? 0 : (roster.RosterTotalPaid),
                                        totalarefund = ((roster.TotalRefund == null) ? 0 : (roster.TotalRefund)),
                                        Grade = roster.GRADE,
                                        School = roster.SCHOOL,
                                        District = roster.DISTRICT,
                                        GradeName = roster.GRADENAME,
                                        SchoolName = roster.SCHOOLNAME,
                                        DistrictName = roster.DISTRICTNAME,
                                        waitlistStatus = roster.WaitingStatus,
                                        cancel = roster.Cancel,
                                        coursenamenumber = roster.CourseNameNumber

                                    });
                dateRangeTo = dateRangeTo.AddHours(24);
                if (dateFilter != "")
                {
                    if (dateFilter == "Charge Date")
                    {
                        dbReport = dbReport.Where(roster => roster.full_dateadded >= dateRangeFrom && roster.full_dateadded <= dateRangeTo);
                    }
                    if (dateFilter == "Order Date")
                    {
                        dbReport = dbReport.Where(roster => roster.full_dateadded >= dateRangeFrom && roster.full_dateadded <= dateRangeTo);
                    }
                    if (dateFilter == "Activity Order")
                    {
                        dbReport = dbReport.Where(roster => roster.full_dateadded >= dateRangeFrom && roster.full_dateadded <= dateRangeTo);
                    }
                }
                else
                {
                    dbReport = dbReport.Where(roster => roster.full_dateadded >= dateRangeFrom && roster.full_dateadded <= dateRangeTo);
                }
                if (state.Filters != null)
                {
                    if (state.Filters.ContainsKey("keyword"))
                    {
                        string filterValue = state.Filters["keyword"];
                        dbReport = dbReport.Where(roster => roster.coursenamenumber.Contains(filterValue) || roster.last.Contains(filterValue) || roster.first.Contains(filterValue) || roster.orderid.Contains(filterValue) || roster.masterordernumber2.Contains(filterValue) || roster.paynumber.Contains(filterValue) || roster.paymethod.Contains(filterValue));
                    }
                    if (state.Filters.ContainsKey("status"))
                    {
                        status = state.Filters["status"];
                    }
                    if (state.Filters.ContainsKey("paytype"))
                    {
                        string filterValue = state.Filters["paytype"];

                        if (!filterValue.Contains("All"))
                        {
                            dbReport = dbReport.Where(u => filterValue.Contains(u.paymethod));
                        }

                    }
                    if (state.Filters.ContainsKey("dateFilter"))
                    {
                        string filterValue = state.Filters["dateFilter"];
                        dateFilter = filterValue;
                    }

                    if (state.Filters.ContainsKey("hidezerocharge"))
                    {
                        string filterValue = state.Filters["hidezerocharge"];
                        isHideZeroCharges = filterValue;
                    }
                    if (state.Filters.ContainsKey("showordermaterials"))
                    {
                        string filterValue = state.Filters["showordermaterials"];
                        isShowOrdersWithMaterials = filterValue;
                    }
                    if (state.Filters.ContainsKey("groupby"))
                    {
                        string filterValue = state.Filters["groupby"];
                        groupBySubsite = filterValue;
                    }
                    if (state.Filters.ContainsKey("district"))
                    {
                        string filterValue = state.Filters["district"];
                        dbReport = dbReport.Where(u => filterValue.Contains(u.District.ToString()) && u.District != 0);
                    }
                    if (state.Filters.ContainsKey("school"))
                    {
                        string filterValue = state.Filters["school"];
                        dbReport = dbReport.Where(u => filterValue.Contains(u.School.ToString()) && u.School != 0);
                    }
                    if (state.Filters.ContainsKey("grade"))
                    {
                        string filterValue = state.Filters["grade"];
                        dbReport = dbReport.Where(u => filterValue.Contains(u.Grade.ToString()) && u.Grade != 0);
                        // grade = filterValue;
                    }


                }
                List<Orders_Object> OrderResults = new List<Orders_Object>();
                if (status != "")
                {
                    if (status != "All")
                    {
                        if (status == "NeedsRefund")
                        {
                            dbReport = dbReport.Where(o => o.cancel != 0 && o.totalpaid > 0);
                        }
                        else if (status == "CreditOnly")
                        {
                            dbReport = dbReport.Where(o => o.totalarefund != 0 && o.totalarefund > 0);
                        }
                        else if (status == "Exception")
                        {
                            dbReport = dbReport.Where(o => o.paymethod.Contains("Filler") || o.paymethod.Contains("Amex") || o.paymethod.Contains("Mastercard") || o.paymethod.Contains("Visa"));
                            dbReport = dbReport.Where(o => o.cancel == 0);
                        }
                        else
                        {
                            dbReport = dbReport.Where(o => o.status == status);
                        }
                    }

                    if (status == "All" || status == "Incompleted Payment")
                    {
                        OrderResults = GenerateOrderInProgressReport(state).reviewOrders;
                    }
                }

                if (isHideZeroCharges == "true")
                {
                    //  dbReport = dbReport.Where(o => o.totalpaid > 0 && o.ordertotal > 0);
                }


                if (isShowOrdersWithMaterials == "true")
                {
                    sqlInnerAnd = sqlInnerAnd + " AND (c.MaterialsRequired = -1 or c.MaterialsRequired = 1) ";
                }

                var page = state.Page;
                var start = (state.Page - 1) * state.PageSize;
                var limit = state.PageSize;

                if (status != "Incompleted Payment")
                {

                    foreach (var item_roster in dbReport.ToList())
                    {
                        item_roster.ordertotal = item_roster.ordertotal + decimal.Parse(item_roster.double_materialcost.ToString()) - decimal.Parse(item_roster.double_coupondiscount.ToString());
                        if ((item_roster.masterordernumber != "") && (item_roster.masterordernumber != null))
                        {
                            item_roster.masterordernumber2 = item_roster.masterordernumber;
                            item_roster.masterordernumber = "Multiple Order";

                        }
                        item_roster.totalpaid = item_roster.totalpaid - item_roster.totalarefund;
                        item_roster.dateadded = item_roster.full_dateadded.ToString();
                        if (isHideZeroCharges == "true")
                        {
                            if (item_roster.ordertotal == 0 && item_roster.totalpaid == 0)
                            {
                            }
                            else
                            {
                                OrderResults.Add(item_roster);
                            }
                        }
                        else
                        {
                            OrderResults.Add(item_roster);
                        }
                    }
                }
                BuildExport(OrderResults.ToList(), folderpath);
                var model = new Gsmu.Api.Data.ViewModels.Grid.GridModel<Orders_Object>(OrderResults.ToList().Count, state);
                if (state.OrderFieldString != null)
                {
                    switch (state.OrderFieldString)
                    {
                        case "orderid":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.orderid).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.orderid).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "dateadded":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.full_dateadded).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.full_dateadded).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "last":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.last).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.last).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "first":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.first).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.first).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "totalpaid":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.totalpaid).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.totalpaid).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "payment type":
                        case "paymethod":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.paymethod).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.paymethod).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "paynumber":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.paynumber).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.paynumber).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "ordertotal":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.ordertotal).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.ordertotal).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        case "status":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                                OrderResults = OrderResults.OrderBy(o => o.status).Skip((page - 1) * limit).Take(limit).ToList();
                            else
                                OrderResults = OrderResults.OrderByDescending(o => o.status).Skip((page - 1) * limit).Take(limit).ToList();
                            break;
                        default:
                            OrderResults = OrderResults.OrderBy(o => o.full_dateadded).Skip((page - 1) * limit).Take(limit).ToList();
                            break;

                    }
                }
                else
                {
                    OrderResults = OrderResults.OrderBy(o => o.full_dateadded).Skip((page - 1) * limit).Take(limit).ToList();
                }
                Orders_Object_result Orders_Object_result = new Orders_Object_result();
                Orders_Object_result.reviewOrders = OrderResults;
                Orders_Object_result.resultcount = model.TotalCount;
                Orders_Object_result.totalCount = model.TotalCount;
                return Orders_Object_result;

            }


        }

        public static void BuildExport(List<Orders_Object> courses, string folderpath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Status").Append(",");
            sb.Append("Order ID").Append(",");
            sb.Append("Date Added").Append(",");
            sb.Append("Account Num").Append(",");
            sb.Append("Last").Append(",");
            sb.Append("First").Append(",");
            sb.Append("Wait List").Append(",");
            sb.Append("Payment Type").Append(",");
            sb.Append("Pay Number").Append(",");
            sb.Append("Order Total").Append(",");
            sb.Append("Total Paid").Append(",");
            sb.AppendLine();
            foreach (var row in courses)
            {
                sb.Append(row.status).Append(",");
                sb.Append(row.orderid).Append(",");
                sb.Append(row.dateadded).Append(",");
                sb.Append(row.accountnum).Append(",");
                sb.Append(row.last).Append(",");
                sb.Append(row.first).Append(",");
                sb.Append(row.waitlistStatus ==0?"No":"Yes").Append(",");
                sb.Append(row.paymethod).Append(",");
                sb.Append(row.paynumber).Append(",");
                sb.Append(row.ordertotal.Value.ToString("C3", CultureInfo.CurrentCulture)).Append(",");
                sb.Append(row.totalpaid.Value.ToString("C3", CultureInfo.CurrentCulture)).Append(",");
                sb.AppendLine();
            }
                string filename = "revieworder.csv";
                string path = folderpath + filename;
                File.WriteAllText(path, sb.ToString());

        }
        public static Orders_Object_result GenerateOrderInProgressReport(QueryState state)
        {
            List<Orders_Object> OrderResults = new List<Orders_Object>();
            using (var db = new SchoolEntities())
            {
                DateTime oldest = DateTime.Now.AddYears(-500);
                var dbReport = (from orderinprogress in db.OrderInProgresses
                                select
                                    new Orders_Object
                                    {
                                        status = (orderinprogress.OrderCurStatus == null ? "" : orderinprogress.OrderCurStatus),
                                        masterordernumber = orderinprogress.MasterOrderNumber,
                                        masterordernumber2 = orderinprogress.MasterOrderNumber,
                                        orderid = orderinprogress.OrderNumber,
                                        full_dateadded = (orderinprogress.orderdate == null ? oldest : orderinprogress.orderdate),
                                        accountnum = "",
                                        studentid = 0,
                                        last = "",
                                        first = "",
                                        waitingstatus = 0,
                                        paymethod = "",
                                        paynumber = "",
                                        ordertotal = 0,
                                        double_materialcost = 0,
                                        double_coupondiscount = 0,
                                        totalpaid = 0,
                                        totalarefund = 0,
                                        Grade = 0,
                                        School = 0,
                                        District = 0,
                                        orderinprogresscoursedetails = orderinprogress.coursedetails,
                                        orderinprogressuserdetails = orderinprogress.userlogindetails,
                                        OrderPrice = orderinprogress.OrderPrice,
   
                                    });

                var dateRangeFrom = DateTime.Now.AddMonths(-6);
                var dateRangeTo = DateTime.Now.AddMonths(1);
                string sqlDateFilter1 = "";
                string sqlDateFilter2 = "";
                string dateFilter = "";
                if (state.Filters != null)
                {
                    //if (state.Filters.ContainsKey("keyword"))
                    //{
                    //    string filterValue = state.Filters["keyword"];
                    //    dbReport = dbReport.Where(roster => roster.last.Contains(filterValue) || roster.first.Contains(filterValue) || roster.orderid.Contains(filterValue) || roster.masterordernumber2.Contains(filterValue) || roster.paynumber.Contains(filterValue) || roster.paymethod.Contains(filterValue) || roster.orderinprogressuserdetails.Contains(filterValue));
                    //}
                    if (state.Filters.ContainsKey("datefrom"))
                    {
                        string filterValue = state.Filters["datefrom"];

                        if (filterValue.ToLower().Trim() == "null")
                        {
                            dateRangeFrom = DateTime.Now.AddMonths(-6);
                        }
                        else
                            dateRangeFrom = DateTime.Parse(filterValue);
                    }
                    if (state.Filters.ContainsKey("dateto"))
                    {
                        string filterValue = state.Filters["dateto"];

                        if (filterValue.ToLower().Trim() == "null")
                        {
                            dateRangeTo = DateTime.Now.AddMonths(1);
                        }
                        else
                            dateRangeTo = DateTime.Parse(filterValue);

                    }
                    dateRangeTo = dateRangeTo.AddHours(24);
                    dbReport = dbReport.Where(roster => roster.full_dateadded >= dateRangeFrom && roster.full_dateadded <= dateRangeTo);
                    dbReport = dbReport.Where(roster => roster.status != "Successfully Paid");
                    dbReport = dbReport.Where(roster => roster.status != "");
                    dbReport = dbReport.Where(roster => roster.OrderPrice!="0.0000" & roster.OrderPrice!="" & roster.OrderPrice!="0.00" & roster.OrderPrice!="0");
                   
                }
                var page = state.Page;
                var start = (state.Page - 1) * state.PageSize;
                var limit = state.PageSize;
                dynamic coursedetails = JsonConvert.DeserializeObject("");
                dynamic userdetails = JsonConvert.DeserializeObject("");
                string currentorder = "";
                string sid = "0";
                Gsmu.Api.Data.School.Entities.Student student = StudentHelper.GetStudent(0);
                foreach (var item_roster in dbReport.ToList())
                {
                    if (currentorder != item_roster.orderid)
                    {
                        OrderResults.Add(item_roster);
                    }

                    currentorder = item_roster.orderid;
                }
                var model = new Gsmu.Api.Data.ViewModels.Grid.GridModel<Orders_Object>(OrderResults.ToList().Count, state);
                //OrderResults = OrderResults.OrderBy(o => o.full_dateadded).Skip((page - 1) * limit).Take(limit).ToList();
                OrderResults = OrderResults.OrderBy(o => o.full_dateadded).ToList();
                Orders_Object_result Orders_Object_result = new Orders_Object_result();
                List<Orders_Object> FinalOrderResults = new List<Orders_Object>();
                foreach (var item_roster in OrderResults.ToList())
                {
                    if (item_roster.OrderPrice != null)
                    {
                        try
                        {
                            item_roster.ordertotal = decimal.Parse(item_roster.OrderPrice);
                        }
                        catch
                        {
                            item_roster.ordertotal = 0;
                        }
                    }


                    if (item_roster.orderinprogressuserdetails != null)
                    {
                        userdetails = JsonConvert.DeserializeObject(item_roster.orderinprogressuserdetails);
                        try
                        {
                            sid = userdetails.sid;
                            student = StudentHelper.GetStudent(int.Parse(sid));
                            item_roster.first = student.FIRST;
                            item_roster.last = student.LAST;
                        }
                        catch
                        {
                            item_roster.first = userdetails.sid;
                            item_roster.last = userdetails.sid;
                        }

                    }
                    item_roster.status = "Incompleted Payment";
                    item_roster.dateadded = item_roster.full_dateadded.ToString();

                    if (state.Filters.ContainsKey("keyword"))
                    {
                        string filterValue = state.Filters["keyword"].ToLower();
                        if ((item_roster.last != null && item_roster.last.ToLower().Contains(filterValue))
                            || (item_roster.first != null && item_roster.first.ToLower().Contains(filterValue))
                            || (item_roster.orderid != null && item_roster.orderid.ToLower().Contains(filterValue))
                            || (item_roster.masterordernumber2 != null && item_roster.masterordernumber2.ToLower().Contains(filterValue))
                            || (item_roster.paynumber != null && item_roster.paynumber.ToLower().Contains(filterValue))
                            || (item_roster.paymethod != null && item_roster.paymethod.ToLower().Contains(filterValue)))
                        {
                            if ((item_roster.masterordernumber != "") && (item_roster.masterordernumber != null))
                            {
                                item_roster.masterordernumber2 = "";
                               item_roster.masterordernumber = "";

                            }
                            FinalOrderResults.Add(item_roster);
                        }
                    }
                    else
                    {
                        if ((item_roster.masterordernumber != "") && (item_roster.masterordernumber != null))
                        {
                            item_roster.masterordernumber2 = "";
                           item_roster.masterordernumber = "";

                        }
                        FinalOrderResults.Add(item_roster);
                    }
                }

                Orders_Object_result.reviewOrders = FinalOrderResults;
                Orders_Object_result.resultcount = model.TotalCount;
                Orders_Object_result.totalCount = model.TotalCount;
                return Orders_Object_result;
            }

        }

        public static List<OrDerInprogess_Object> GenerateReviewOrderInProgressDetails(string OrderNumber)
        {
            List<OrDerInprogess_Object> OrDerInprogess_Objects = new List<OrDerInprogess_Object>();
            OrDerInprogess_Object OrDerInprogess_Object = new OrDerInprogess_Object();
            using (var db = new SchoolEntities())
            {
                var orderlist = db.OrderInProgresses.Where(u => u.OrderNumber == OrderNumber || u.MasterOrderNumber == OrderNumber).ToList();
                var studentdetails = db.Students.Where(student => student.STUDENTID == 0).FirstOrDefault();
                var coursedetailsdb = db.Courses.Where(course => course.COURSEID == 0).FirstOrDefault();
                string userid = "";
                string courseid = "";
                dynamic coursedetails = JsonConvert.DeserializeObject("");
                dynamic userdetails = JsonConvert.DeserializeObject("");
                foreach (var order in orderlist)
                {

                    coursedetails = JsonConvert.DeserializeObject(order.coursedetails);
                    userdetails = JsonConvert.DeserializeObject(order.userlogindetails);
                    userid = userdetails.sid;
                    courseid = coursedetails.cid;
                    try
                    {
                        OrDerInprogess_Object.studentid = int.Parse(userid);

                        studentdetails = db.Students.Where(student => student.STUDENTID == OrDerInprogess_Object.studentid).FirstOrDefault();

                        if (studentdetails != null)
                        {
                            OrDerInprogess_Object.first = studentdetails.FIRST;
                            OrDerInprogess_Object.last = studentdetails.LAST;
                            OrDerInprogess_Object.email = studentdetails.EMAIL;
                            OrDerInprogess_Object.address = studentdetails.ADDRESS;
                            OrDerInprogess_Object.city = studentdetails.CITY;
                            OrDerInprogess_Object.state = studentdetails.STATE;
                            OrDerInprogess_Object.country = studentdetails.COUNTRY;
                            OrDerInprogess_Object.zip = studentdetails.ZIP;

                            OrDerInprogess_Object.homephone = studentdetails.HOMEPHONE;
                            OrDerInprogess_Object.workphone = studentdetails.WORKPHONE;
                            OrDerInprogess_Object.fax = studentdetails.FAX;

                            OrDerInprogess_Object.homephonelabel = db.FieldSpecs.Where(specs => specs.FieldName == "homephone" && specs.TableName == "Students").FirstOrDefault().FieldName;
                            OrDerInprogess_Object.faxlabel = db.FieldSpecs.Where(specs => specs.FieldName == "fax" && specs.TableName == "Students").FirstOrDefault().FieldName;
                            OrDerInprogess_Object.workphonelabel = db.FieldSpecs.Where(specs => specs.FieldName == "workphone" && specs.TableName == "Students").FirstOrDefault().FieldName;




                            OrDerInprogess_Object.studregfield1 = studentdetails.StudRegField1;
                            OrDerInprogess_Object.studregfield4 = studentdetails.StudRegField4;
                            OrDerInprogess_Object.studregfield5 = studentdetails.StudRegField5;
                            OrDerInprogess_Object.studregfield10 = studentdetails.StudRegField10;

                            OrDerInprogess_Object.districtname = db.Districts.Where(dist => dist.DISTID == studentdetails.DISTRICT).FirstOrDefault().DISTRICT1;
                            OrDerInprogess_Object.locationname = db.Schools.Where(loc => loc.ID == studentdetails.SCHOOL).FirstOrDefault().LOCATION;
                            OrDerInprogess_Object.gradename = db.Grade_Levels.Where(gr => gr.GRADEID == studentdetails.GRADE).FirstOrDefault().GRADE;
                        }

                    }
                    catch { }
                    try
                    {
                        OrDerInprogess_Object.courseid = int.Parse(courseid);
                        coursedetailsdb = db.Courses.Where(course => course.COURSEID == OrDerInprogess_Object.courseid).FirstOrDefault();
                        OrDerInprogess_Object.coursename = coursedetailsdb.COURSENAME;
                        OrDerInprogess_Object.coursenum = coursedetailsdb.COURSENUM;
                        OrDerInprogess_Object.coursecost = coursedetails.coursecost;
                    }
                    catch { }
                    OrDerInprogess_Object.singlerosterdiscountamount = 0;
                    OrDerInprogess_Object.totalpaid = 0;
                    OrDerInprogess_Object.ordernumber = order.OrderNumber;
                    OrDerInprogess_Objects.Add(OrDerInprogess_Object);
                    OrDerInprogess_Object = new OrDerInprogess_Object();
                }
            }
            return OrDerInprogess_Objects;
        }

        public static void GenerateProcessReviewOrderInProgresstoRoster(string id)
        {
            EnrollmentFunction Enrollment = new EnrollmentFunction();
            Enrollment.OrderInprogressToRoster(null, id);
            using (var db = new SchoolEntities())
            {
                var order_roster = (from roster in db.Course_Rosters where roster.OrderNumber == id select roster).ToList();
                foreach (var roster in order_roster)
                {
                    roster.Cancel = 0;

                }
                db.SaveChanges();
            }


        }

    }
    public class materialprice
    {
        public float? price { get; set; }
        public string ordernumber { get; set; }
    }
    public class OrDerInprogess_Object
    {
        public string ordernumber { get; set; }
        public string last { get; set; }
        public string first { get; set; }
        public int courseid { get; set; }
        public int studentid { get; set; }

        public decimal singlerosterdiscountamount { get; set; }
        public decimal totalpaid { get; set; }
        public string coursecost { get; set; }

        public string email { get; set; }
        public string username { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }

        public string mincoursedate { get; set; }
        public string maxcoursedate { get; set; }
        public string coursenum { get; set; }
        public string coursename { get; set; }


        public string districtname { get; set; }
        public string locationname { get; set; }
        public string gradename { get; set; }
        public string studregfield1 { get; set; }
        public string studregfield4 { get; set; }
        public string studregfield5 { get; set; }
        public string studregfield10 { get; set; }


        public string workphonelabel { get; set; }
        public string faxlabel { get; set; }
        public string homephonelabel { get; set; }

        public string homephone { get; set; }
        public string workphone { get; set; }
        public string fax { get; set; }

    }
    public class Orders_Object
    {
        public Int32 rosterid { get; set; }
        public string status { get; set; }
        public string masterordernumber { get; set; }
        public string orderid { get; set; }
        public DateTime? full_dateadded { get; set; }
        public string dateadded { get; set; }
        public string accountnum { get; set; }
        public int? studentid { get; set; }
        public string last { get; set; }
        public string first { get; set; }
        public short? waitingstatus { get; set; }
        public string paymethod { get; set; }
        public string paynumber { get; set; }
        public decimal? ordertotal { get; set; }
        public decimal? totalpaid { get; set; }
        public string coursenamenumber { get; set; }
        public int? cancel { get; set; }
        public int? paidinfull { get; set; }
        public int? School { get; set; }
        public int? Grade { get; set; }
        public int? District { get; set; }

        public string SchoolName { get; set; }
        public string GradeName { get; set; }
        public string DistrictName { get; set; }
        public string masterordernumber2 { get; set; }
        public double? double_ordertotal { get; set; }
        public double? double_totalpaid { get; set; }
        public string orderinprogresscoursedetails { get; set; }
        public string orderinprogressuserdetails { get; set; }
        public string OrderPrice { get; set; }
        public double? double_coupondiscount { get; set; }
        public decimal? totalarefund { get; set; }
        public double? double_materialcost { get; set; }
        public int? waitlistStatus { get; set; }

    }

    public class Orders_Object_result
    {
        public List<Orders_Object> reviewOrders { get; set; }
        public string status { get; set; }
        public int totalCount { get; set; }
        public int resultcount { get; set; }
        string sql { get; set; }
        string csvFileName { get; set; }
    }


}
