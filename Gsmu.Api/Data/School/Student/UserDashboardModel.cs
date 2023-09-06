using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Student
{
    public class UserDashboardModel
    {

            public string actionemail { get; set; }

            public string actionmerge { get; set; }

            public string actionmakepayment { get; set; }

            public string actionenroll { get; set; }

            public string actionrefund { get; set; }
    }

    class GradelevelModel
    {
        public int gradeid { get; set; }

        public string grade { get; set; }
    }

    public class OtherUserEnrolledModel
    {
        public int? courseid { get; set; }
        public string courseName { get; set; }
        public string MasterOrderNumber { get; set; }
        public string OrderNumber { get; set; }
        public string StudentName { get; set; }
        public double OrderTotal { get; set; }
        public decimal? TotalPaid { get; set; }
        public DateTime? CourseDate { get; set; }
        public string FormatedDate { get; set; }
        public string FormattedTotalPaid { get; set; }
        public int Rosterid { get; set; }

    }
}
