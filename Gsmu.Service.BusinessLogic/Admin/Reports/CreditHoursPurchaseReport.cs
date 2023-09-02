using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels.Grid;
using Gsmu.Service.Interface.Admin.Reports;
using Gsmu.Service.Models.Admin.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Service.BusinessLogic.Admin.Reports
{
    public class CreditHoursPurchaseReport : ICreditHoursPurchase
    {
        public CreditHoursPurchaseResultModel GetStudentCourseHoursPurchased(CreditHoursPurchaseParamenterModel CreditHoursPurchaseParamenterModel, QueryState queryState)
        {

            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                double creditcost = double.Parse(Settings.Instance.GetMasterInfo3().SingleCourseCreditCost.Value.ToString());
                var Raw_transcript = (from tran in db.Transcripts
                                  join c in db.Courses on tran.CourseId equals c.COURSEID
                                  join cr in db.Course_Rosters on tran.STUDENTID equals cr.STUDENTID
                                  join st in db.Students on tran.STUDENTID equals st.STUDENTID
                                  where
                                  cr.Cancel == 0
                                  && cr.COURSEID == tran.CourseId
                                  
                                  && c.CANCELCOURSE == 0
                                  && (tran.IsHoursPaid == 1)
                                  && tran.CustomCreditHours > 0
                                  select new CreditHoursPurchaseModel
                                  {
                                      TranscriptID = tran.TranscriptID,
                                      ClockHour = (tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value : 0),
                                      DatePurchased = cr.DATEADDED,
                                      CourseNumber = c.COURSENUM,
                                      CourseId = c.COURSEID,
                                      coursecompletiondate = tran.CourseCompletionDate,
                                      CourseName = c.COURSENAME,
                                      Amount = Math.Round((tran.CustomCreditHours.HasValue ? tran.CustomCreditHours.Value : 0) * creditcost, 2),
                                      IsHoursPaidInfo = tran.IsHoursPaidInfo,
                                      StudentName = st.FIRST + " " + st.LAST,
                                      StudentFirst = st.FIRST,
                                      StudentLast = st.LAST,
                                      studentDistrict = tran.District,
                                      CourseStartDate = tran.CourseStartDate

                                  });

                IEnumerable<CreditHoursPurchaseModel> transcript = Raw_transcript.AsEnumerable().Select(transcripted => { transcripted.DatePurchased = GetDatePurchased(transcripted.IsHoursPaidInfo); return transcripted; });
                if (CreditHoursPurchaseParamenterModel.Keyword != "")
                {
                    if (CreditHoursPurchaseParamenterModel.Keyword != null) {
                        transcript = transcript.Where(transcripts => transcripts.StudentName.ToLower().Contains(CreditHoursPurchaseParamenterModel.Keyword.ToLower()) || transcripts.CourseName.Contains(CreditHoursPurchaseParamenterModel.Keyword) || transcripts.CourseNumber.Contains(CreditHoursPurchaseParamenterModel.Keyword));
                    }
                }
                if (CreditHoursPurchaseParamenterModel.studentDistrict != "")
                {
                    if (CreditHoursPurchaseParamenterModel.studentDistrict != null)
                    {
                        transcript = transcript.Where(transcripts => transcripts.studentDistrict.ToLower().Contains(CreditHoursPurchaseParamenterModel.studentDistrict.ToLower()));
                    }
                }
                if (CreditHoursPurchaseParamenterModel.datefilter == "TranscribeDate")
                {
                    if ((CreditHoursPurchaseParamenterModel.StartDate != null) && (CreditHoursPurchaseParamenterModel.EndDate != null))
                    {
                        if (CreditHoursPurchaseParamenterModel.StartDate != "")
                        {
                            DateTime sdate = DateTime.Parse(CreditHoursPurchaseParamenterModel.StartDate);
                            transcript = transcript.Where(transcripts => transcripts.coursecompletiondate > sdate);

                        }
                        if (CreditHoursPurchaseParamenterModel.EndDate != "")
                        {
                            DateTime edate = DateTime.Parse(CreditHoursPurchaseParamenterModel.EndDate);
                            transcript = transcript.Where(transcripts => transcripts.coursecompletiondate < edate);

                        }
                    }
                }
                else if (CreditHoursPurchaseParamenterModel.datefilter == "StartDate")
                {
                    if ((CreditHoursPurchaseParamenterModel.StartDate != null) && (CreditHoursPurchaseParamenterModel.EndDate != null))
                    {
                        if (CreditHoursPurchaseParamenterModel.StartDate != "")
                        {
                            DateTime sdate = DateTime.Parse(CreditHoursPurchaseParamenterModel.StartDate);
                            transcript = transcript.Where(transcripts => transcripts.CourseStartDate > sdate);

                        }
                        if (CreditHoursPurchaseParamenterModel.EndDate != "")
                        {
                            DateTime edate = DateTime.Parse(CreditHoursPurchaseParamenterModel.EndDate);
                            transcript = transcript.Where(transcripts => transcripts.CourseStartDate < edate);

                        }
                    }
                }
                else if (CreditHoursPurchaseParamenterModel.datefilter == "DatePurchase")
                {
                    if ((CreditHoursPurchaseParamenterModel.StartDate != null) && (CreditHoursPurchaseParamenterModel.EndDate != null))
                    {
                        if (CreditHoursPurchaseParamenterModel.StartDate != "")
                        {
                            DateTime sdate = DateTime.Parse(CreditHoursPurchaseParamenterModel.StartDate);
                            transcript = transcript.Where(transcripts => transcripts.DatePurchased > sdate);

                        }
                        if (CreditHoursPurchaseParamenterModel.EndDate != "")
                        {
                            DateTime edate = DateTime.Parse(CreditHoursPurchaseParamenterModel.EndDate);
                            transcript = transcript.Where(transcripts => transcripts.DatePurchased < edate);

                        }
                    }
                }
                List<CreditHoursPurchaseModel> translist = new List<CreditHoursPurchaseModel>();
                var page = CreditHoursPurchaseParamenterModel.pagestart;
                var start = (CreditHoursPurchaseParamenterModel.pagestart - 1) * queryState.PageSize;
                var limit = queryState.PageSize;

                var model = new GridModel<CreditHoursPurchaseModel>(transcript.Count(), queryState);
                transcript = transcript.OrderBy(o => o.DatePurchased).Skip((page - 1) * limit).Take(limit);
                string paymenttype = "";
                string authnum = "";
                foreach (var item in transcript.ToList())
                {
                    item.StringDatePurchased = "";
                    if ((item.IsHoursPaidInfo != "") && (item.IsHoursPaidInfo != null))
                    {
                        JavaScriptSerializer j = new JavaScriptSerializer();
                        dynamic hourpaidinfo = j.Deserialize(item.IsHoursPaidInfo, typeof(object));
                        try
                        {
                            paymenttype = hourpaidinfo["Paymthod"];
                        }
                        catch { }
                        authnum = hourpaidinfo["AuthNum"];
                        if ((paymenttype == ""))
                        {
                            paymenttype = "Credit Card";
                        }

                        item.IsHoursPaidInfo = paymenttype + "<br>Payment Date: " + hourpaidinfo["Paydate"] + "<br> AuthNum #" + hourpaidinfo["AuthNum"] + "<br>TransactionID #" + hourpaidinfo["TransactionID"];
                        item.Amount = double.Parse(hourpaidinfo["Amount"]);
                        item.StringDatePurchased = hourpaidinfo["Paydate"];
                        paymenttype = "";
                    }
                    if (item.StringDatePurchased == "")
                    {
                        if (item.DatePurchased != null)
                        {
                            item.StringDatePurchased = item.DatePurchased.Value.ToShortDateString();
                        }
                    }
                    item.Amount = Double.Parse(String.Format("{0:0.00}", item.Amount));
                    item.CourseCompletionDateString = item.coursecompletiondate.ToShortDateString();
                    item.AmountString = String.Format("{0:C}", item.Amount);
                    translist.Add(item);
                }

                CreditHoursPurchaseResultModel CreditHoursPurchaseResultModel = new CreditHoursPurchaseResultModel();
                CreditHoursPurchaseResultModel.CreditHoursPurchases = translist;
                CreditHoursPurchaseResultModel.resultcount = model.TotalCount;
                CreditHoursPurchaseResultModel.totalCount = model.TotalCount;
                return CreditHoursPurchaseResultModel;
            }
        }

        public DateTime? GetDatePurchased(string jsonvalue)
        {

            if ((jsonvalue != "") && (jsonvalue != null))
            {
                string Paydate = "";
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic hourpaidinfo = j.Deserialize(jsonvalue, typeof(object));
                try
                {
                    Paydate = hourpaidinfo["Paydate"];
                    DateTime dt = Convert.ToDateTime(Paydate);
                    return dt;
                }
                catch
                {
                    return null;
                }

            }
            return null;
        }
    }
}
