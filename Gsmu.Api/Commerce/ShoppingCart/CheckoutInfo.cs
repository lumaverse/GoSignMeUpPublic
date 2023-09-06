using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Web;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.Supervisor;
using Gsmu.Api.Data.School.CourseRoster;

namespace Gsmu.Api.Commerce.ShoppingCart
{
    public class CheckoutInfo
    {
        #region Static

        public static CheckoutInfo Instance
        {
            get
            {
                var checkout = ObjectHelper.GetSessionObject<CheckoutInfo>(WebContextObject.CheckoutInfo);
                if (checkout == null)
                {
                    checkout = new CheckoutInfo();
                    ObjectHelper.SetSessionObject<CheckoutInfo>(WebContextObject.CheckoutInfo, checkout);
                }
                return checkout;
            }
        }

        #endregion

        public string ActiveCCProcessing
        {
            get
            {
                return CreditCardPaymentHelper.ActiveCCProcessing;
            }
        }

        public string PaymentMode
        {
            get
            {
                if (RemainderAmount > 0)
                {
                    return "partialpayment";
                }
                else
                {
                    return "fullpayment";
                }
            }
        }

        public decimal PaymentTotal { get; set; }

        public string transactionid { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal RemainderAmount { get; set; }
        public bool HasPartialPayment { get; set; }
        public decimal OrderTotal
        {
            get
            {
                return new OrderModel(OrderNumber).OrderTotal;
            }
        }

        public decimal OrderinProgressTotal
        {
            get
            {
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var OrderInProgressRoster = (from oprogress in db.OrderInProgresses where (oprogress.OrderNumber == OrderNumber || oprogress.MasterOrderNumber == OrderNumber) && oprogress.OrderCurStatus == "Pending" select oprogress).ToList();
                    if (OrderInProgressRoster != null)
                    {
                        decimal OrderinprogressTotal = 0;
                        decimal SingleRosterDiscount = 0;
                        foreach (var orderinprog in OrderInProgressRoster)
                        {
                            OrderinprogressTotal = decimal.Parse(orderinprog.OrderPrice);
                            try
                            {
                                dynamic coursedetails = Newtonsoft.Json.JsonConvert.DeserializeObject(orderinprog.coursedetails);
                                if (decimal.Parse(coursedetails.CouponDiscount.ToString()) > 0)
                                {
                                    OrderinprogressTotal = decimal.Parse(orderinprog.OrderPrice) - decimal.Parse(coursedetails.CouponDiscount.ToString());
                                }
                                else
                                {

                                    SingleRosterDiscount = SingleRosterDiscount + decimal.Parse(coursedetails.SingleRosterDiscountAmount.ToString());
                                }
                            }
                            catch
                            {
                            }
                        }

                        return OrderinprogressTotal - SingleRosterDiscount;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public string GatherStudentNameInCart(string checkOrderNumber)
        {
            var db = new SchoolEntities();
            string StudentName = "";
            /*var CourseRostersName = (from cr in db.Course_Rosters
                                     group cr by cr.STUDENTID into NewGroup
                                     join stds in db.Students on NewGroup.FirstOrDefault().STUDENTID equals stds.STUDENTID
                                     where NewGroup.FirstOrDefault().OrderNumber == checkOrderNumber || NewGroup.FirstOrDefault().MasterOrderNumber == checkOrderNumber
                                     select new NewStudentGroup
                                     {
                                         curStudentFirst = stds.FIRST,
                                         curStudentLast = stds.LAST
                                     }).ToList();
            */
            var MultipleStudentCourses = new List<CourseMultipleStudentItem>();

            if (CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent != 0)
            {
                foreach (var s in CourseShoppingCart.Instance.MultipleStudentCourses)
                {
                    if (s.StudentId != 0)
                    {
                        StudentName += s.FirstName + " " + s.LastName + ", ";
                    }
                }
                StudentName = StudentName.Remove(StudentName.Length - 2, 2);
            }
            else
            {
                var student = AuthorizationHelper.CurrentStudentUser;
                StudentName = student.FIRST + " " + student.LAST;
            }
            return StudentName;
        }
        public bool IsValidOrderNumber(string checkOrderNumber)
        {
            var db = new SchoolEntities();
            var CourseRosters  = (from cr in db.Course_Rosters where cr.OrderNumber == checkOrderNumber || cr.MasterOrderNumber == checkOrderNumber select cr).ToList();
            if (CourseRosters.Count() > 0)
            {
                return true;
            }
            else
            {


                var OrderInProgress = (from cr in db.OrderInProgresses where cr.OrderNumber == checkOrderNumber || cr.MasterOrderNumber == checkOrderNumber select cr).ToList();
                if (OrderInProgress.Count() > 0)
                {
                    int transid;
                    var isNumericValue = int.TryParse(checkOrderNumber, out  transid);
                    if (isNumericValue)
                    {
                        var Transcript = (from trans in db.Transcripts where trans.TranscriptID == transid select trans).FirstOrDefault();
                        if (Transcript != null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string OrderNumber { get; set; }

        public string PaymentCaller { get; set; }



        public int CountPayOptions
        {
            get
            {
                return PayOptionList.Count();
            }
        }

        public List<PayOption> PayOptionList
        {
            get
            {
                var dlist = new List<PayOption>();
                if (CreditCardPaymentHelper.ActiveCCProcessing != "none")
                {
                    dlist.Add(new PayOption { title = "Credit Card", value = "Credit Card", subtitle = CreditCardPaymentHelper.ActiveCCProcessing });
                }

                if (Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo().ShowOtherPayment))
                {
                    var othrpayoptions = new CreditCardPayments();
                    IEnumerable<Payment_Option> items = othrpayoptions.GetAllPaymentTypes();
                    {
                        foreach (Payment_Option payoptn in items)
                            if (payoptn.PAYMENTTYPE != "Select Payment Type")
                            {
                                dlist.Add(new PayOption { title = payoptn.PAYMENTTYPE, value = payoptn.PAYMENTTYPE, subtitle = "" });
                            }
                    }
                }
                return dlist;
            }
        }


        public string SelectedPayOption { get; set; }

        public string PaymentStatus {
            get
            {
                if (PaymentCompleteResult.Contains("Approved") || PaymentResult.Contains("Approved"))
                {
                    return "Approved";
                }else{
                    return "Declined";
                }
            }
        }

        public string PaymentCompleteResult { get; set; }

        public string PaymentResult { get; set; }

        public decimal PrvTotalPaid { get; set; }

        public string AnetReceiptLink { get; set; }

        public string TranscriptID { get; set; }
        public string responsecode { get; set; }
        public string responsemessage { get; set; }

        public string ReturnLink { get; set; }
        public string PaypalTempData { get; set; }

        public bool? HasActivePaymentProcess { get; set; }

        private class NewStudentGroup
        {
            public string curStudentFirst { get; set; }
            public string curStudentLast { get; set; }
        }
    }

  
}
