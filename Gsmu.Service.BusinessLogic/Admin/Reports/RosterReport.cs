using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Service.Models.Admin.Reports;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.Interface.Admin.Reports;
using Gsmu.Api.Data;
using Gsmu.Service.BusinessLogic.GlobalTools;

namespace Gsmu.Service.BusinessLogic.Admin.Reports
{
    public class RosterReport : IRosterReport
    {
        public RosterReportResult GenerateRosterReport(Gsmu.Api.Data.QueryState QueryState)
        {
            RosterReportResult RosterReportResult = new RosterReportResult();


            using (var db = new SchoolEntities())
            {
                var dateRangeFrom = DateTime.Now.AddDays(-30);
                var dateRangeTo = DateTime.Now.AddDays(30);
                var rosters = (from _rosters in db.RosterReportViews

 
                               select new RosterReport_Object
                               {
                                   EnrollToWaitStatus = _rosters.EnrollToWaitStatus,
                                   EnrollToWaitListConfig = _rosters.EnrollToWaitListConfig,
                                   coursenameid = _rosters.CourseNameID,
                                   cancelcourse = _rosters.cancelcourse,
                                   coursedateid = _rosters.CourseDateID,
                                   courseid = _rosters.courseid,
                                   coursename = _rosters.COURSENAME,
                                   coursenum = _rosters.COURSENUM,
                                   Location = _rosters.Location,
                                   internalclass = _rosters.internalclass,
                                   maxenroll = _rosters.maxenroll,
                                   maxwait = _rosters.maxwait,
                                   room = _rosters.room,
                                 //  accountnum = _rosters.ACCOUNTNUM,
                                   instructorid = _rosters.instructorid,
                                   instructorid2 = _rosters.instructorid2,
                                   instructorid3 = _rosters.instructorid3,
                                   materials = _rosters.materials,
                                   days = _rosters.days,
                                   credithours = _rosters.credithours,
                                   CourseCloseDays = _rosters.CourseCloseDays,
                                   description = _rosters.description,
                                   canvascoursesection = _rosters.canvassectioncourse,
                                   maincategory = _rosters.maincategory,
                                   subcategory = _rosters.subcategory,
                                   subsubcategory = _rosters.subsubcategory,
                                   studentid = _rosters.STUDENTID,
                                   WAITING = _rosters.WAITING,
                                   internalnote = _rosters.internalnote,
                                   enrollmentnote = _rosters.enrollmentnote,
                                   ordernum = _rosters.ordernum,
                                   masterordernum = _rosters.masterordernumber,
                                   paymethod = _rosters.PAYMETHOD,
                                   paynumber = _rosters.payNumber,
                                   authnum = _rosters.authnum,
                                   refnumber = _rosters.refnumber,
                                   couponcode = _rosters.CouponCode,
                                   coupondiscount = _rosters.CouponDiscount,
                                   coupondetails = _rosters.CouponDetails,
                                   studentgrade = _rosters.StudentGrade,
                                   course = _rosters.course,
                                   paidinfull = _rosters.PAIDINFULL,
                                   amountpaid = _rosters.amountpaid,
                                   rosterid = _rosters.RosterID,
                                   creditapplied = _rosters.creditapplied,
                                   cancel = _rosters.cancel,
                                   attended = _rosters.Attended,
                                   invoicenumber = _rosters.InvoiceNumber,
                                   invoicedate = _rosters.InvoiceDate,
                                   dateadded = _rosters.DateAdded,
                                   crinitialauditinfo = _rosters.crinitialauditinfo,
                                   coursechoice = _rosters.coursechoice,
                                   first = _rosters.first,
                                   last = _rosters.last,
                                   state = _rosters.STATE,
                                   email = _rosters.EMAIL,
                                   address = _rosters.ADDRESS,
                                   city = _rosters.CITY,
                                   zip = _rosters.ZIP,
              
                                   homephone = _rosters.HOMEPHONE,
                                   workphone = _rosters.WORKPHONE,
                                   fax = _rosters.FAX,
                                   studregfield1 = _rosters.StudRegField1,
                                   studregfield2 = _rosters.studRegfield2,
                                   studregfield3 = _rosters.StudRegField3,
                                   studregfield4 = _rosters.studRegfield4,
                                   studregfield5 = _rosters.StudRegField5,
                                   studregfield6 = _rosters.studRegfield6,
                                   studregfield7 = _rosters.StudRegField7,
                                   studregfield8 = _rosters.studRegfield8,
                                   studregfield9 = _rosters.StudRegField9,
                                   studregfield10 = _rosters.studRegfield10,
                                   studregfield11 = _rosters.StudRegField11,
                                   studregfield12 = _rosters.studRegfield12,
                                   studregfield13 = _rosters.StudRegField13,
                                   studregfield14 = _rosters.studRegfield14,
                                   studregfield15 = _rosters.StudRegField15,
                                   studregfield16 = _rosters.studRegfield16,
                                   studregfield17 = _rosters.StudRegField17,
                                   studregfield18 = _rosters.studRegfield18,
                                   studregfield19 = _rosters.StudRegField19,
                                   studregfield20 = _rosters.studRegfield20,
                                   hiddenstudregfield1 = _rosters.hiddenstudregfield1,
                                   hiddenstudregfield2 = _rosters.hiddenstudregfield2,
                                   readonlystudregfield1 = _rosters.readonlystudregfield1,
                                   readonlystudregfield2 = _rosters.readonlystudregfield2,
                                   readonlystudregfield3 = _rosters.readonlystudregfield3,
                                   readonlystudregfield4 = _rosters.readonlystudregfield4,
                                   studentschool = _rosters.studentschool,
                                   district = _rosters.district,
                                   studentgradelevel = _rosters.studentgradelevel,
                                   i1first = _rosters.i1first,
                                   i1last = _rosters.i1last,
                                   i2first = _rosters.i2first,
                                   i2last = _rosters.i2last,
                                   i3first = _rosters.i3first,
                                   i3last = _rosters.i3last,
                                   instructor = _rosters.instructor,
                                   instructor2 = _rosters.instructor2,
                                   instructor3 = _rosters.instructor3,
                                   cancelledtxt = _rosters.cancelledtxt,
                                   waitingtxt = _rosters.waitingtxt,
                                   attendedtxt = _rosters.attendedtxt,
                                   PaidFulltxt = _rosters.PaidFulltxt,
                                   Credited = _rosters.Credited,
                                   RMCount = _rosters.RMCount,
                                   materialnames = _rosters.materialnames,
                                   Material = _rosters.Material,
                                   CourseTotal = _rosters.CourseTotal,
                                   TxTotal = _rosters.TxTotal,
                                   CompleteAddress = _rosters.CompleteAddress,
                                   CourseLocation = _rosters.CourseLocation,
                                   rawstartdate = _rosters.startdate,
                                   rawenddate = _rosters.enddate,
                                   daddress = _rosters.daddress,
                                   count = _rosters.Count,


                               });

                //for instructor dashboard
                if (QueryState.FilterbyInstructor)
                {
                    var instructorid = QueryState.FilterInstructorID;
                    rosters = rosters.Where(roster => roster.instructorid2 == instructorid ||roster.instructorid == instructorid || roster.instructorid3 == instructorid);
                }

                if (QueryState.Filters != null)
                {


                    //filter keyword
                        if (QueryState.Filters.ContainsKey("keyword"))
                    {
                        string filterValue = QueryState.Filters["keyword"];

                        rosters = rosters.Where(roster => roster.i1first.Contains(filterValue) ||
                            roster.i1last.Contains(filterValue) ||
                            roster.Location.Contains(filterValue) ||
                            roster.district.Contains(filterValue) ||
                            roster.description.Contains(filterValue) ||
                            roster.coursenum.Contains(filterValue)
                            || roster.coursedateid.Contains(filterValue)
                            || roster.coursenameid.Contains(filterValue)
                            || roster.coursename.Contains(filterValue)
                            || roster.first.Contains(filterValue)
                            || roster.last.Contains(filterValue)
                            );

                    }
                    //filter  date-from
                    if (QueryState.Filters.ContainsKey("date-from"))
                    {
                        string filterValue = QueryState.Filters["date-from"];
                        dateRangeFrom = DateTime.Parse(filterValue);
                        rosters = rosters.Where(roster => roster.rawstartdate >= dateRangeFrom);
                    }
                    else
                    {
                        if (!QueryState.FilterbyInstructor)// no date range if instructors dashboard
                        {
                           rosters = rosters.Where(roster => roster.rawstartdate >= dateRangeFrom);
                        }
                     }
                    //filter date-to
                    if (QueryState.Filters.ContainsKey("date-to"))
                    {
                        string filterValue = QueryState.Filters["date-to"];
                        dateRangeTo = DateTime.Parse(filterValue).AddHours(24);
                        rosters = rosters.Where(roster => roster.rawstartdate <= dateRangeTo);
                    }
                    else
                    {
                        if (!QueryState.FilterbyInstructor) // no date range if instructors dashboard
                        {
                            rosters = rosters.Where(roster => roster.rawstartdate <= dateRangeTo);
                        }
                            
                    }
                    //filter cancelledtext
                    if (QueryState.Filters.ContainsKey("cancelledtxt"))
                    {
                        string filterValue = QueryState.Filters["cancelledtxt"];
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            if (Convert.ToInt32(filterValue) == 1)
                            {
                                rosters = rosters.Where(roster => roster.cancel == 0);
                            }
                        }

                    }
                    //filter category-main
                    if (QueryState.Filters.ContainsKey("category-main"))
                    {
                        string filterValue = QueryState.Filters["category-main"];
                        if (!string.IsNullOrEmpty(filterValue) && filterValue != "No main category selected")
                        {
                            rosters = rosters.Where(roster => roster.maincategory.Contains(filterValue));
                        }
                    }
                    //filter category-sub
                    if (QueryState.Filters.ContainsKey("category-sub"))
                    {
                        string filterValue = QueryState.Filters["category-sub"];
                        if (!string.IsNullOrEmpty(filterValue) && filterValue != "No sub-category selected")
                        {
                            rosters = rosters.Where(roster => roster.subcategory.Contains(filterValue));
                        }
                    }
                    //filter category-subsub
                    if (QueryState.Filters.ContainsKey("category-subsub"))
                    {
                        string filterValue = QueryState.Filters["category-subsub"];
                        if (!string.IsNullOrEmpty(filterValue) && filterValue != "No optional sub-category selected")
                        {
                            rosters = rosters.Where(roster => roster.subsubcategory.Contains(filterValue));
                        }
                    }
                }
                else
                {
                    if (!QueryState.FilterbyInstructor) // no date range if instructors dashboard
                    {
                        rosters = rosters.Where(roster => roster.rawstartdate <= dateRangeTo);
                        rosters = rosters.Where(roster => roster.rawstartdate >= dateRangeFrom);
                    }

                }

                // Read data to export
                
                if (QueryState.DownloadGridCSV == "true")
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var col in QueryState.colinfo) { sb = GridExportManager.AppendValue(col.title, sb); }
                    sb.AppendLine();
                    foreach (var data in rosters.OrderBy(s => s.coursename))
                    {
                        foreach (var col in QueryState.columns)
                        {
                            var vlu = data.GetType().GetProperty(col.data).GetValue(data, null);
                            sb = GridExportManager.AppendValue(vlu, sb);
                        }
                        sb.AppendLine();
                    }
                    string path = GridExportManager.BuildExportFile(QueryState.DownloadGridFilename, sb);
                }
                

                var group1roster = rosters.GroupBy(roster => roster.coursedateid);
                int count = 1;
                int currentindex = 0;
                var groupedrosters = group1roster.AsEnumerable().Select((g, i) => new { g, i, index = i + 1 })
                .SelectMany(x =>
                x.g.Select((_roster1, a) => new RosterReportinGroupings { roster = _roster1, index = a }
                ).Select(_rosters => new
                RosterReport_Object
                {
                    EnrollToWaitStatus = _rosters.roster.EnrollToWaitStatus,
                    EnrollToWaitListConfig = _rosters.roster.EnrollToWaitListConfig,
                    coursenameid = _rosters.roster.coursenameid,
                    cancelcourse = _rosters.roster.cancelcourse,
                    coursedateid = _rosters.roster.coursedateid,
                    courseid = _rosters.roster.courseid,
                    coursename = _rosters.roster.coursename,
                    coursenum = _rosters.roster.coursenum,
                    Location = _rosters.roster.Location,
                    internalclass = _rosters.roster.internalclass,
                    maxenroll = _rosters.roster.maxenroll,
                    maxwait = _rosters.roster.maxwait,
                    room = _rosters.roster.room,
                    accountnum = _rosters.roster.accountnum,
                    instructorid = _rosters.roster.instructorid,
                    instructorid2 = _rosters.roster.instructorid2,
                    instructorid3 = _rosters.roster.instructorid3,
                    materials = _rosters.roster.materials,
                    days = _rosters.roster.days,
                    credithours = _rosters.roster.credithours,
                    CourseCloseDays = _rosters.roster.CourseCloseDays,
                    description = _rosters.roster.description,
                    canvascoursesection = _rosters.roster.canvascoursesection,
                    maincategory = _rosters.roster.maincategory,
                    subcategory = _rosters.roster.subcategory,
                    subsubcategory = _rosters.roster.subsubcategory,
                    studentid = _rosters.roster.studentid,
                    WAITING = _rosters.roster.WAITING,
                    internalnote = _rosters.roster.internalnote,
                    enrollmentnote = _rosters.roster.enrollmentnote,
                    ordernum = _rosters.roster.ordernum,
                    masterordernum = _rosters.roster.masterordernum,
                    paymethod = _rosters.roster.paymethod,
                    paynumber = _rosters.roster.paynumber,
                    authnum = _rosters.roster.authnum,
                    refnumber = _rosters.roster.refnumber,
                    couponcode = _rosters.roster.couponcode,
                    coupondiscount = _rosters.roster.coupondiscount,
                    coupondetails = _rosters.roster.coupondetails,
                    studentgrade = _rosters.roster.studentgrade,
                    course = _rosters.roster.course,
                    paidinfull = _rosters.roster.paidinfull,
                    amountpaid = _rosters.roster.amountpaid,
                    rosterid = _rosters.roster.rosterid,
                    creditapplied = _rosters.roster.creditapplied,
                    cancel = _rosters.roster.cancel,
                    attended = _rosters.roster.attended,
                    invoicenumber = _rosters.roster.invoicenumber,
                    invoicedate = _rosters.roster.invoicedate,
                    dateadded = _rosters.roster.dateadded,
                    crinitialauditinfo = _rosters.roster.crinitialauditinfo,
                    coursechoice = _rosters.roster.coursechoice,
                    first = _rosters.roster.first,
                    last = _rosters.roster.last,
                    state = _rosters.roster.state,
                    email = _rosters.roster.email,
                    address = _rosters.roster.address,
                    city = _rosters.roster.city,
                    zip = _rosters.roster.zip,
                    country = _rosters.roster.country,
                    homephone = _rosters.roster.homephone,
                    workphone = _rosters.roster.workphone,
                    fax = _rosters.roster.fax,
                    studregfield1 = _rosters.roster.studregfield1,
                    studregfield2 = _rosters.roster.studregfield2,
                    studregfield3 = _rosters.roster.studregfield3,
                    studregfield4 = _rosters.roster.studregfield4,
                    studregfield5 = _rosters.roster.studregfield5,
                    studregfield6 = _rosters.roster.studregfield6,
                    studregfield7 = _rosters.roster.studregfield7,
                    studregfield8 = _rosters.roster.studregfield8,
                    studregfield9 = _rosters.roster.studregfield9,
                    studregfield10 = _rosters.roster.studregfield10,
                    studregfield11 = _rosters.roster.studregfield11,
                    studregfield12 = _rosters.roster.studregfield12,
                    studregfield13 = _rosters.roster.studregfield13,
                    studregfield14 = _rosters.roster.studregfield14,
                    studregfield15 = _rosters.roster.studregfield15,
                    studregfield16 = _rosters.roster.studregfield16,
                    studregfield17 = _rosters.roster.studregfield17,
                    studregfield18 = _rosters.roster.studregfield18,
                    studregfield19 = _rosters.roster.studregfield19,
                    studregfield20 = _rosters.roster.studregfield20,
                    hiddenstudregfield1 = _rosters.roster.hiddenstudregfield1,
                    hiddenstudregfield2 = _rosters.roster.hiddenstudregfield2,
                    readonlystudregfield1 = _rosters.roster.readonlystudregfield1,
                    readonlystudregfield2 = _rosters.roster.readonlystudregfield2,
                    readonlystudregfield3 = _rosters.roster.readonlystudregfield3,
                    readonlystudregfield4 = _rosters.roster.readonlystudregfield4,
                    studentschool = _rosters.roster.studentschool,
                    district = _rosters.roster.district,
                    studentgradelevel = _rosters.roster.studentgradelevel,
                    i1first = _rosters.roster.i1first,
                    i1last = _rosters.roster.i1last,
                    i2first = _rosters.roster.i2first,
                    i2last = _rosters.roster.i2last,
                    i3first = _rosters.roster.i3first,
                    i3last = _rosters.roster.i3last,
                    instructor = _rosters.roster.instructor,
                    instructor2 = _rosters.roster.instructor2,
                    instructor3 = _rosters.roster.instructor3,
                    cancelledtxt = _rosters.roster.cancelledtxt,
                    waitingtxt = _rosters.roster.waitingtxt,
                    attendedtxt = _rosters.roster.attendedtxt,
                    PaidFulltxt = _rosters.roster.PaidFulltxt,
                    Credited = _rosters.roster.Credited,
                    RMCount = _rosters.roster.RMCount,
                    materialnames = _rosters.roster.materialnames,
                    Material = _rosters.roster.Material,
                    CourseTotal = _rosters.roster.CourseTotal,
                    TxTotal = _rosters.roster.TxTotal,
                    CompleteAddress = _rosters.roster.CompleteAddress,
                    CourseLocation = _rosters.roster.CourseLocation,
                    startdate = _rosters.roster.rawstartdate.ToString(),
                    enddate = _rosters.roster.rawenddate.ToString(),
                    daddress = _rosters.roster.daddress,
                    count = _rosters.index + 1,

                    RowNumber = x.i,
                }
                ));
                var page = 1;
                var start = 1;
                var limit = 10;
                page = QueryState.Page;
                start = (QueryState.Page - 1) * QueryState.PageSize;
                limit = QueryState.PageSize;
                groupedrosters = groupedrosters.OrderBy(roster => roster.startdate);
                if(QueryState.Sorters != null)
                {
                    if (QueryState.Sorters.Count > 0)
                    {

                        foreach (var sorter in QueryState.Sorters)
                        {
                            String OrderKey = sorter.Key.ToString();
                            if (OrderKey == "coursenameid")
                            {
                                if (sorter.Value == OrderByDirection.Ascending)
                                    groupedrosters = groupedrosters.OrderBy(roster => roster.coursenameid);
                                else
                                    groupedrosters = groupedrosters.OrderByDescending(roster => roster.coursenameid);
                            }
                            else
                            {
                                if (sorter.Value == OrderByDirection.Ascending)
                                    groupedrosters = groupedrosters.OrderBy(roster => roster.coursedateid);
                                else
                                    groupedrosters = groupedrosters.OrderByDescending(roster => roster.coursedateid);
                            }

                            break;

                        }
                    }

                }
                groupedrosters = groupedrosters.Where(roster => roster.RowNumber >= (start) && roster.RowNumber <= (start + limit));
                var model = new Gsmu.Api.Data.ViewModels.Grid.GridModel<RosterReport_Object>(group1roster.Count(), QueryState);

                if (model.TotalCount > 0)
                {

                    model.Result = groupedrosters.ToList();
                }

                RosterReportResult.rosters = model.Result;
                RosterReportResult.recordCount = model.TotalCount;

            }
            return RosterReportResult;
        }
    }
}
