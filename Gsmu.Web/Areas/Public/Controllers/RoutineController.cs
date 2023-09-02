using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using web = Gsmu.Api.Web;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Data;
using System.Xml;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Commerce;
using AuthorizeNet;
using Gsmu.Api.Networking.Mail;
using Gsmu.Api.Data.School.Student;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class RoutineController : Controller
    {
        public ActionResult ANET(string secret_key)
        {
            
            string message = string.Empty;
            bool success = true;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            if (string.IsNullOrEmpty(secret_key))
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        success = false,
                        message = "No secret key found. Access denied. Err_A100"
                    }
                };
            }
            else
            {
                DateTime currDateStamp = DateTime.Now;
                string currTimestamp = currDateStamp.ToString("d");
                //unable to generate live with current time so use fix string for now
                currTimestamp = "ok2yi2j9na2b";
                PaymentsController pmtController = new PaymentsController();
                //string CurrentKey = pmtController.HMAC_MD5("6VeN2mM9T83JEB3ztR5y9E6G8b", currTimestamp);
                string CurrentKey = pmtController.HMAC_MD5(CreditCardPaymentHelper.ANLogin + CreditCardPaymentHelper.ANTranKey, currTimestamp);
                if (secret_key != CurrentKey)
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            success = false,
                            message = "Incorrect Secret Key. Access denied. Err_A100a"
                        }
                    };
                }
            }

            List<string> dataResponse = new List<string>();

            try
            {

                string ANLogin = CreditCardPaymentHelper.ANLogin;
                string ANKey = CreditCardPaymentHelper.ANTranKey;
                int count_days = CreditCardPaymentHelper.AnetRoutineTransDays;
                //count_days = 30;
                //open a call to the Gateway
                int ANETServiceMode = Settings.Instance.GetMasterInfo().ANTesting;

                var ANETGate = new ReportingGateway(ANLogin, ANKey, ANETServiceMode == 0 ? ServiceMode.Live : ServiceMode.Test);

                //FILTERING FOR THE DATE

                DateTime StartDateRange = Convert.ToDateTime(System.DateTime.Now.Date.AddDays(count_days * -1));
                DateTime EndDateRange = Convert.ToDateTime(System.DateTime.Now);
                //get all Transactions for the last xx days
                var ANETTransactions = ANETGate.GetTransactionList(StartDateRange, EndDateRange);
                if (WebConfiguration.AnetReconcileLevel == 2)
                {
                    ANETTransactions = ANETGate.GetUnsettledTransactionList();
                }
                
                string ReconcileData = "Reconcile" + WebConfiguration.AnetReconcileLevel;

                foreach (var item in ANETTransactions)
                {
                    string transactionId = item.TransactionID;
                    string cardNumber = item.CardNumber;
                    decimal settlementAmount = item.SettleAmount;
                    DateTime dateSubmitted = item.DateSubmitted;
                    string invoiceNumber = item.InvoiceNumber;
                    string itemDescription = item.Description;
                    string requestStatus = item.Status;
                    int responseCode = item.ResponseCode;
                    string authNumber = item.AuthorizationCode;
                    string paymethod = "CC";
                    string foundDupe = ",Dupe:";
                    EmailFunction emailFunction = new EmailFunction();

                    if ((requestStatus == "settledSuccessfully" || (requestStatus == "capturedPendingSettlement" && WebConfiguration.AnetReconcileLevel == 2)) && !string.IsNullOrEmpty(invoiceNumber)) //SETTLED TRANSACTIONS
                    {
                        using (SchoolEntities db = new SchoolEntities())
                        {
                            //UPDATE
                            int? course_id = 0;
                            int? student_id = 0;
                            int? rosterid = 0;
                            int sendconfirmemailnow = 0;
                            int redundant_student_enrollee_count = 0;                            
                            EnrollmentFunction EnrollmentFunction = new EnrollmentFunction();
                            CreditCardPaymentModel paymodel = new CreditCardPaymentModel();

                            EnrollmentFunction.OrderInprogressToRoster(paymodel, invoiceNumber);
                            var courseRosterData = (from cr in db.Course_Rosters where cr.OrderNumber.ToLower() == invoiceNumber.ToLower() || cr.MasterOrderNumber.ToLower() == invoiceNumber.ToLower() select cr);
                            ReconcileData = ReconcileData + "|Order:" + invoiceNumber + ",TxID:" + transactionId + ",AuthNum" + authNumber + ",TotalPaid:" + settlementAmount + ",DateAdded:" + dateSubmitted;
                            if (courseRosterData.Count() > 0 && !string.IsNullOrEmpty(courseRosterData.FirstOrDefault().MasterOrderNumber))
                            {
                                //dealing with multiple enroll masterorder# based
                                var courseRosterValue = db.Course_Rosters.Where(cr => cr.MasterOrderNumber == invoiceNumber).ToList(); // GET ROSTER INFO
                                foreach (var eachrosters in courseRosterValue)
                                {
                                    course_id = eachrosters.COURSEID;
                                    student_id = eachrosters.STUDENTID;
                                    rosterid = eachrosters.RosterID;
                                    redundant_student_enrollee_count = db.Course_Rosters.Where(cr => cr.STUDENTID == student_id && cr.COURSEID == course_id).Count();

                                    if (redundant_student_enrollee_count > 1)
                                    {
                                        foundDupe = "s" + student_id + "c" + course_id;
                                        dataResponse.Add(invoiceNumber.ToLower() + "A student (" + student_id + ") may have been enrolled multiple times in Course : " + course_id);
                                    }
                                    if (eachrosters.PAYMETHOD == "" || eachrosters.PAYMETHOD == null)
                                    {
                                        if (string.IsNullOrEmpty(transactionId) != true)
                                        {
                                            var Context = new SchoolEntities();
                                            Course_Roster eachroster = Context.Course_Rosters.First(cr => cr.RosterID == eachrosters.RosterID);
                                            eachroster.PAYMETHOD = paymethod;
                                            eachroster.payNumber = transactionId;
                                            eachroster.AuthNum = authNumber;
                                            eachroster.RefNumber = authNumber;
                                            eachroster.TotalPaid = settlementAmount;
                                            eachroster.RespMsg = "Completed"; //Approved
                                            eachroster.Result = "Completed"; //Approved
                                            eachroster.ChargeDate = dateSubmitted;
                                            eachroster.PaidInFull = -1;
                                            eachroster.Cancel = 0;
                                            Context.SaveChanges();
                                            ReconcileData = ReconcileData + "Status:Added" + foundDupe;
                                            dataResponse.Add("Updated Course Roster Information of Roster ID " + eachroster.RosterID + ".");
                                            sendconfirmemailnow = 1;
                                        }
                                    }
                                    else
                                    {
                                        ReconcileData = ReconcileData + "Status:Failed" + foundDupe;
                                    }
                                }
                                if (sendconfirmemailnow == 1)
                                {
                                    //send confirmation email after processed all record of the order.
                                    emailFunction.SendConfirmationEmail(settlementAmount.ToString(), invoiceNumber, "", "routineconfirmation");
                                    sendconfirmemailnow = 0;
                                }

                            } else if (courseRosterData.Count() > 0) {
                                //dealing with single enroll/registration or multiple enroll with no no master
                                var courseRosterValue = db.Course_Rosters.Where(cr => cr.RosterID == courseRosterData.FirstOrDefault().RosterID).SingleOrDefault(); // GET ROSTER INFO

                                //CHECKING MULTIPLE ENROLLMENT ON ROSTER
                                course_id = courseRosterValue.COURSEID;
                                student_id = courseRosterValue.STUDENTID;
                                redundant_student_enrollee_count = db.Course_Rosters.Where(cr => cr.STUDENTID == student_id && cr.COURSEID == course_id).Count();

                                if (redundant_student_enrollee_count > 1)
                                {
                                    //SEND EMAIL TO LANG AND ME HERE
                                    //letter c on instruction
                                    //string body = "This is to inform you that <b>Student: " + student_id + " </b> has been enrolled to <b>Course: " + course_id + " : multiple times";
                                    //emailFunction.SendEmail("langn@gosignmeup.com", "Multiple Student Enrollment Detected at ANET", body, "vincentb@gosignmeup.com");
                                    foundDupe = "s" + student_id + "c" + course_id;
                                    dataResponse.Add(invoiceNumber.ToLower() + "A student (" + student_id + ") may have been enrolled multiple times in Course : " + course_id);
                                }
                                // we update based on ordernumber/rosterid
                                // need to build logic to handle multple registration per ordernumber

                                //UPDATING COURSE ROSTER PHASE
                                if (courseRosterValue.PAYMETHOD == "" || courseRosterValue.PAYMETHOD == null)
                                {
                                    if (string.IsNullOrEmpty(transactionId) != true)
                                    {
                                        courseRosterValue.PAYMETHOD = paymethod;
                                        courseRosterValue.payNumber = transactionId;
                                        courseRosterValue.AuthNum = authNumber;
                                        courseRosterValue.RefNumber = authNumber;
                                        courseRosterValue.TotalPaid = settlementAmount;
                                        courseRosterValue.RespMsg = "Completed"; //Approved
                                        courseRosterValue.Result = "Completed"; //Approved
                                        courseRosterValue.ChargeDate = dateSubmitted;
                                        courseRosterValue.PaidInFull = -1;
                                        courseRosterValue.Cancel = 0;
                                        db.SaveChanges();
                                        ReconcileData = ReconcileData + "Status:Added" + foundDupe;
                                        emailFunction.SendConfirmationEmail(settlementAmount.ToString(), invoiceNumber, "", "routineconfirmation");
                                        dataResponse.Add("Updated Course Roster Information of Roster ID " + courseRosterValue.RosterID + ".");
                                    }
                                }
                                else
                                {
                                    ReconcileData = ReconcileData + "Status:Failed" + foundDupe;
                                }
                                //if (WebConfiguration.LogAuthorizeNetTransaction)
                                //{
                                //Capture post string before posting to ANET.
                                //   Gsmu.Api.Data.School.Student.EnrollmentFunction enrollment = new Gsmu.Api.Data.School.Student.EnrollmentFunction();
                                //    CreditCardPaymentModel paymentmodel = new CreditCardPaymentModel();
                                //    paymentmodel.OrderNumber = invoiceNumber;
                                //     enrollment.LogAuthorizeNetTransaction(paymentmodel, settlementAmount.ToString(), "Get Data from ANET Settled Transactions (automate routine at mid night)", "Completed", "CreditCardPayment.cs");
                                // }
                                message = "Succesfully done the routine on ANET.";

                            } //end single order
                            else
                            {
                                ReconcileData = ReconcileData + "Status:Failed" + foundDupe;
                            }
                        } //end school entities
                    } //end settledSuccessfully and not null invoice
                    else if (!string.IsNullOrEmpty(invoiceNumber)) 
                    {
                        //to record fail transactions. Declined, Rejected, MerchantPending, FDSPendingReview, MerchantFailed
                        using (SchoolEntities db = new SchoolEntities())
                        {
                            var orderlist = db.OrderInProgresses.Where(u => u.OrderNumber == invoiceNumber || u.MasterOrderNumber == invoiceNumber).ToList();
                            if (orderlist.Count > 0)
                            {
                                foreach (var order in orderlist)
                                {
                                    OrderInProgress orderinprog = db.OrderInProgresses.First(op => op.OrderInProgressId == order.OrderInProgressId);
                                    orderinprog.OrderCurStatus = requestStatus;
                                    db.SaveChanges();
                                }
                            }
                        }
                    } //end not settledSuccessfully
                } //for each
                //AUDIT TRAIL
                //tracking Routine before GSMU process response post
                AuditTrail ANETReconcileAudit = new AuditTrail();
                ANETReconcileAudit.AuditDate = DateTime.Now;
                ANETReconcileAudit.UserName = "ANET_Reconcile";
                ANETReconcileAudit.AuditAction = ReconcileData;
                ANETReconcileAudit.DetailDescription = "Succesfully done the routine on ANET.";
                ANETReconcileAudit.ShortDescription = "";
                ANETReconcileAudit.RoutineName = "RoutineController.cs";
                using (var db = new SchoolEntities())
                {
                    db.AuditTrails.Add(ANETReconcileAudit);
                    db.SaveChanges();
                }
                if ((WebConfiguration.AuditSiteAcitivityLevel.ToLower() != "off") && (WebConfiguration.AuditSiteAcitivityLevel.ToLower() != "initial"))
                {
                    //get unsettled transaction
                    var ANETunsettledTransactions = ANETGate.GetUnsettledTransactionList();
                    ReconcileData = "UnsettledTransactions";

                    foreach (var item in ANETunsettledTransactions)
                    {
                        ReconcileData = ReconcileData + "Date:" + item.DateSubmitted + "[Data:" + "TxID:" + item.TransactionID + "|Name:" + item.FirstName + " " + item.LastName + "|c-num:" + item.CardNumber;
                        ReconcileData = ReconcileData + "|TotalAmount:" + item.SettleAmount + "|OrderNum:" +item.InvoiceNumber + "|OrderDesc:" + item.Description;
                        ReconcileData = ReconcileData + "|OrderStatus:" + item.Status + "|RespCode:" + item.ResponseCode + "|Authnum:" + item.AuthorizationCode + "|TxType:" + item.TransactionType + "]";
                    }
                    ANETReconcileAudit.AuditDate = DateTime.Now;
                    ANETReconcileAudit.UserName = "ANET_UnsettledReconcile";
                    ANETReconcileAudit.AuditAction = ReconcileData;
                    ANETReconcileAudit.DetailDescription = "Succesfully done the routine on ANET.";
                    ANETReconcileAudit.ShortDescription = "";
                    ANETReconcileAudit.RoutineName = "RoutineController.cs";
                    using (var db = new SchoolEntities())
                    {
                        db.AuditTrails.Add(ANETReconcileAudit);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            return new JsonResult()
            {
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = new
                {
                    success = success,
                    message = message,
                    data = dataResponse
                }
            };
        }
    }
}
