using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Reports
{

        public class RosterReport_Object
        {
            public string EnrollToWaitStatus { get; set; }
            public string EnrollToWaitListConfig { get; set; }
            public short cancelcourse { get; set; }
            public string coursenameid { get; set; }
            public string coursedateid { get; set; }
            public int? courseid { get; set; }
            public string coursename { get; set; }
            public string coursenum { get; set; }
            public string Location { get; set; }
            public short internalclass { get; set; }
            public int? maxenroll { get; set; }
            public int? maxwait { get; set; }
            public string room { get; set; }
            public string accountnum { get; set; }
            public int? instructorid { get; set; }
            public int? instructorid2 { get; set; }
            public int? instructorid3 { get; set; }
            public string materials { get; set; }
            public int? days { get; set; }
            public double? credithours { get; set; }
            public int? CourseCloseDays { get; set; }
            public string description { get; set; }
            public string canvascoursesection { get; set; }
            public int? studentid { get; set; }
            public short? WAITING { get; set; }
            public string internalnote { get; set; }
            public string enrollmentnote { get; set; }
            public string ordernum { get; set; }
            public string masterordernum { get; set; }
            public string paymethod { get; set; }
            public string paynumber { get; set; }
            public string authnum { get; set; }
            public string refnumber { get; set; }
            public string couponcode { get; set; }
            public float? coupondiscount { get; set; }
            public string coupondetails { get; set; }
            public string studentgrade { get; set; }
            public decimal? course { get; set; }
            public short? paidinfull { get; set; }
            public decimal? amountpaid { get; set; }
            public int rosterid { get; set; }
            public string creditapplied { get; set; }
            public short cancel { get; set; }
            public short attended { get; set; }
            public string invoicenumber { get; set; }
            public DateTime? invoicedate { get; set; }
            public DateTime? dateadded { get; set; }
            public string crinitialauditinfo { get; set; }
            public string coursechoice { get; set; }
            public string first { get; set; }
            public string last { get; set; }
            public string username { get; set; }
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
            public decimal? Credited { get; set; }
            public int? RMCount { get; set; }
            public string materialnames { get; set; }
            public double? Material { get; set; }
            public double? CourseTotal { get; set; }
            public double? TxTotal { get; set; }
            public string CompleteAddress { get; set; }
            public string CourseLocation { get; set; }
            public DateTime? rawstartdate { get; set; }
            public DateTime? rawenddate { get; set; }
            public string startdate { get; set; }
            public string enddate { get; set; }
            public string daddress { get; set; }
            public string maincategory { get; set; }
            public string subcategory { get; set; }
            public Int64? count { get; set; }
            public Int64 RowNumber { get; set; }
            public string subsubcategory { get; set; }
        public long? TotalCount { get; set; }
        public long? RowNum { get; set; }
    }
        public class RosterReportResult
        {
            public List<RosterReport_Object> rosters { get; set; }
            public int recordCount { get; set; }
            public string exportFileName { get; set; }
        }

        public class RosterReportinGroupings
        {
            public RosterReport_Object roster { get; set; }
            public int index { get; set; }
        }
    
}
