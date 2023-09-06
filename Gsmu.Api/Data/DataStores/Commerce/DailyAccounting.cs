using Gsmu.Api.Data.ViewModels.DataStores.Commerce;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Gsmu.Api.Language;
using Gsmu.Api.Data.School.Entities;
namespace Gsmu.Api.Data.DataStores.Commerce
{
    public class DailyAccounting
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
            var queryState = new QueryState(start, limit)
            {
                OrderByDirection = sorterResult.Value,
                OrderFieldString = sorterResult.Key,
                Filters = filterResult,
                FldHeaders = FldHeaderResult,
                Page = page
            };

            return queryState;
        }
        public static string ExportDailyAccountToExcel(List<DailyAccountingViewModel> DailyAccountingReportData, QueryState queryState)
        {
            try
            {
                string exportFileName = "DailyAccountReport" + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".csv";
                string directory = AppDomain.CurrentDomain.BaseDirectory + @"Temp\";
                StringBuilder sb = new StringBuilder();
                Dictionary<string, string> FldHeadersText = queryState.FldHeaders;

                DataTable attendanceListTable = DailyAccountingReportData.ToDataTable();
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
                System.IO.File.WriteAllText(directory + exportFileName, sb.ToString());
                return exportFileName;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static DailyAccountReportResponseModel DailyAccountingReport(HttpRequestBase Request, bool isExport) 
        {
            try
            {
                QueryState queryState = BuildRequestQuery(Request);
                var page = queryState.Page;
                var start = (queryState.Page - 1) * queryState.PageSize;
                var limit = queryState.PageSize;
                string keyword = string.Empty;
                string dateFrom = string.Empty;
                string dateTo = string.Empty;

                using(var db = new SchoolEntities())
                {
                    DateTime fromDate = Convert.ToDateTime(dateFrom);
                    DateTime toDate = Convert.ToDateTime(dateTo);
                    var dailyAccountingReportData = (from cr in db.Course_Rosters
                                                     join c in db.Courses on cr.COURSEID equals c.COURSEID
                                                     join s in db.Students on cr.STUDENTID equals s.STUDENTID
                                                     join ct in db.Course_Times on c.COURSEID equals ct.COURSEID
                                                     select new { cr, c, s, ct })
                                                     .ToList()
                                                     .OrderBy(o => o.cr.OrderNumber)
                                                     .Select(da =>new DailyAccountingViewModel 
                                                     {
                                                        coursenameid = da.c.COURSENAME + da.c.COURSEID.ToString(),
                                                        //coursedateid = create a function that returns the shit
                                                        courseid = da.c.COURSEID,
                                                        coursename = da.c.COURSENAME,
                                                        coursenum = da.c.COURSENUM,
                                                        accountnum = da.c.ACCOUNTNUM,
                                                        rosterid = da.cr.RosterID,
                                                        studentid = da.cr.STUDENTID.Value,
                                                        waiting = da.cr.WAITING,
                                                        notes = da.cr.internalnote,
                                                        ordernum = da.cr.OrderNumber,
                                                        paymethod = da.cr.payNumber,
                                                        paynumber = da.cr.payNumber,
                                                        couponcode = da.cr.CouponCode,
                                                        coupondiscount = da.cr.CouponDiscount.HasValue ? da.cr.CouponDiscount.Value : 0,
                                                        coupondetails = da.cr.CouponDetails,
                                                        studentgrade = da.cr.StudentGrade,
                                                        course = float.Parse(da.cr.CourseCost),
                                                        paidinfull = da.cr.PaidInFull,
                                                        amountpaid = AmountPaid(da.cr.RosterID, db),
                                                        paiddate = da.cr.PaidInFull == 0 ? "" : da.cr.ChargeDate.Value.ToShortDateString(),
                                                        creditapplied = da.cr.CreditApplied,
                                                        cancelled = da.cr.Cancel,
                                                        attended = da.cr.ATTENDED,
                                                        invoicenumber = da.cr.InvoiceNumber,
                                                        invoicedate = da.cr.InvoiceDate.Value > DateTime.Parse("1990-01-01") ? da.cr.InvoiceDate : null,
                                                        orderdate = da.cr.DATEADDED,
                                                        first = da.s.FIRST,
                                                        last = da.s.LAST,
                                                        state = da.s.STATE,
                                                        studentschool = db.Schools.Where(sc => sc.locationid == da.s.SCHOOL).FirstOrDefault().LOCATION,
                                                        district = db.Districts.Where(ds => ds.DISTID == da.s.DISTRICT).FirstOrDefault().DISTRICT1,
                                                        cancelcourse = da.c.CANCELCOURSE,
                                                        location = da.c.LOCATION,
                                                        internalclass = da.c.InternalClass,
                                                        materials = da.c.MATERIALS,
                                                        days = da.c.DAYS,
                                                        credithours = da.c.CREDITHOURS,
                                                        courseclosedays = da.c.CourseCloseDays,
                                                        description = da.c.DESCRIPTION,
                                                        totalpaid = da.cr.TotalPaid,
                                                        chargeddate = da.cr.ChargeDate,
                                                        cancelledtxt = da.cr.Cancel == 0 ? "No" : "Yes",
                                                        attendedtxt = da.cr.ATTENDED == 0 ? "No" : "Yes",
                                                        paidfulltxt = da.cr.PaidInFull == 0 ? "No" : "Yes",
                                                        paymentstatus = da.cr.PaidInFull == 0 ? "Partial Paymment" : "Paid in Full",
                                                        orderstatus = da.cr.Cancel == 2 ? "Incompleted Payment"  : da.cr.Cancel == 3 ? "Failed Payment" : (da.cr.Cancel == 1 || da.cr.Cancel == -1) ? "Cancelled" : da.cr.PaidInFull != 0 ? "Approved" : "Pending",
                                                        credited = float.Parse(da.cr.CreditApplied),
                                                        rmcount = db.rostermaterials.Where(rm => rm.RosterID == da.cr.RosterID).Count(),
                                                        crpartialpaymentlisttxt = da.cr.CRPartialPaymentList,
                                                        materialfee = MaterialFee(da.cr.RosterID, db),
                                                        coursetotal = 0
                                                     });
                };

                return new DailyAccountReportResponseModel()
                {
                    dailyAccountReportList = null,
                    recordCount = 0,
                    exportFileName = string.Empty
                };
            }
            catch(Exception ex)
            {
                return new DailyAccountReportResponseModel()
                {
                    errorMessage = ex.Message
                };
            }
        }
        public static float AmountPaid(int rosterId, SchoolEntities db) 
        {
            return 0;
        }
        public static float MaterialFee(int rosterId, SchoolEntities db) {
            return 0;
        }
    }
}
