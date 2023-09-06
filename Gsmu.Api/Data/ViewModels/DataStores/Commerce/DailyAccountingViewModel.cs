using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.DataStores.Commerce
{
    public class DailyAccountingViewModel
    {
        public int rosterid { get; set; }
        public int count { get; set; }
        public string coursenameid { get; set; }
        public string coursedateid { get; set; }
        public string coursename { get; set; }
        public string coursenum { get; set; }
        public int courseid { get; set; }
        public int cancelcourse { get; set; }
        public string location { get; set; }
        public int internalclass { get; set; }
        public int maxenroll { get; set; }
        public int maxwait { get; set; }
        public string room { get; set; }
        public string materials { get; set; }
        public int ? days { get; set; }
        public double ? credithours { get; set; }
        public int ? courseclosedays { get; set; }
        public string description { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string startdatestring { get; set; }
        public string enddatestring { get; set; }
        public string instructor { get; set; }
        public int studentid { get; set; }
        public string last { get; set; }
        public string first { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public int cancelled { get; set; }
        public int attended { get; set; }
        public int waiting { get; set; }
        public string cancelledtxt { get; set; }
        public string attendedtxt { get; set; }
        public string paidfulltxt { get; set; }
        public string paymentstatus { get; set; }
        public string orderstatus { get; set; }
        public int paid { get; set; }
        public int paidinfull { get; set; }
        public string paiddate { get; set; }
        public string paiddatetxt { get; set; }
        public int rmcount { get; set; }
        public List<string> materiallist { get; set; }
        public float course { get; set; }
        public float courseprice { get; set; }
        public float convcourse { get; set; }
        public float materialfee { get; set; }
        public float material { get; set; }
        public float coursetotal { get; set; }
        public float txtotal { get; set; }
        public float amountpaid { get; set; }
        public float coupondiscount { get; set; }
        public string couponcode { get; set; }
        public string coupondetails { get; set; }
        public string studentschool { get; set; }
        public string studentgradelevel { get; set; }
        public float credited { get; set; }
        public string creditapplied { get; set; }
        public string paymethod { get; set; }
        public string crpartialpaymentlisttxt { get; set; }
        public string paynumber { get; set; }
        public string ordernum { get; set; }
        public string accountnum { get; set; }
        public string invoicenumber { get; set; }
        public DateTime ? invoicedate { get; set; }
        public DateTime ? orderdate { get; set; }
        public DateTime? chargeddate { get; set; }
        public string studentgrade { get; set; }
        public string notes { get; set; }
        public string convmmatlist { get; set; }
        public string convcoursetxt { get; set; }
        public string convstatuscolor { get; set; }
        public string convpaymethod { get; set; }
        public string convcouponcode { get; set; }
        public string paymentstatushtml { get; set; }
        public float grandtotal { get; set; }
        public float grandtotalsum { get; set; }
        public float amountdue { get; set; }
        public string amountduetxt { get; set; }
        public decimal ? totalpaid { get; set; }
    }
    public class DailyAccountReportResponseModel
    {
        public List<DailyAccountingViewModel> dailyAccountReportList { get; set; }
        //make a generic class that can be inherited with these commone properties
        public int recordCount { get; set; }
        public string exportFileName { get; set; }
        public string errorMessage { get; set; }
    }
}
