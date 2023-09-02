using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Reports
{
    public class CreditHoursPurchaseModel
    {
        public int TranscriptID { get; set; }
        public string StudentName { get; set; }
        public string StudentFirst { get; set; }
        public string StudentLast { get; set; }
        public string CourseName { get; set; }
        public double Amount { get; set; }
        public DateTime? DatePurchased { get; set; }
        public DateTime coursecompletiondate { get; set; }
        public string CourseCompletionDateString { get; set; }
        public double ClockHour { get; set; }
        public string IsHoursPaidInfo { get; set; }
        public string StringDatePurchased { get; set; }
        public string AmountString { get; set; }
        public string CourseNumber { get; set; }
        public string studentDistrict { get; set; }
        public int CourseId { get; set; }
        public DateTime? CourseStartDate { get; set; }
    }

    public class CreditHoursPurchaseParamenterModel
    {

        public string Keyword { get; set; }
        public string StartDate { get; set; }        
        public string EndDate { get; set; }
        public string studentDistrict { get; set; }
        public int pagestart { get; set; }
        public string datefilter { get; set; }

    }

    public class CreditHoursPurchaseResultModel
    {
        public List<CreditHoursPurchaseModel> CreditHoursPurchases { get; set; }
        public string status { get; set; }
        public int totalCount { get; set; }
        public int resultcount { get; set; }
        string sql { get; set; }
    }
}
