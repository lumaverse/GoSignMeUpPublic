using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using web = Gsmu.Api.Web;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Data;
using System.Xml;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Commerce;
using AuthorizeNet;
using Gsmu.Api.Networking.Mail;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class RoutineController : Controller
    {
        public ActionResult ANET()
        {
            string message = string.Empty;
            bool success = true;
            List<string> dataResponse = new List<string>();

            try
            {

                string ANLogin = CreditCardPaymentHelper.ANLogin;
                string ANKey = CreditCardPaymentHelper.ANTranKey;
                int count_days = CreditCardPaymentHelper.AnetRoutineTransDays;
                count_days = 30;
                //open a call to the Gateway
                int ANETServiceMode = Settings.Instance.GetMasterInfo().ANTesting;

                var ANETGate = new ReportingGateway(ANLogin, ANKey, ANETServiceMode == 0 ? ServiceMode.Live : ServiceMode.Test);

                //FILTERING FOR THE DATE

                DateTime StartDateRange = Convert.ToDateTime(System.DateTime.Now.Date.AddDays(count_days * -1));
                DateTime EndDateRange = Convert.ToDateTime(System.DateTime.Now.Date);
                //get all Transactions for the last 30 days
                var ANETTransactions = ANETGate.GetTransactionList(StartDateRange, EndDateRange);
                string ReconcileData = "Reconcile";

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

                    if (requestStatus == "settledSuccessfully") //SETTLED TRANSACTIONS
                    {
                        using (SchoolEntities db = new SchoolEntities())
                        {
                            //UPDATE
                            var courseRosterData = (from cr in db.Course_Rosters where cr.OrderNumber.ToLower() == invoiceNumber.ToLower() || cr.MasterOrderNumber.ToLower() == invoiceNumber.ToLower() select cr);
                            ReconcileData = ReconcileData + "|Order:" + invoiceNumber + ",TxID:" + transactionId + ",AuthNum" + authNumber + ",TotalPaid:" + settlementAmount + ",DateAdded:" + dateSubmitted;
                            if (courseRosterData.Count() > 0)
                            {
                                var courseRosterValue = db.Course_Rosters.Where(cr => cr.RosterID == courseRosterData.FirstOrDefault().RosterID).SingleOrDefault(); // GET ROSTER INFO

                                //CHECKING MULTIPLE ENROLLMENT ON ROSTER
                                int? course_id = courseRosterValue.COURSEID;
                                int? student_id = courseRosterValue.STUDENTID;
                                int redundant_student_enrollee_count = db.Course_Rosters.Where(cr => cr.STUDENTID == student_id && cr.COURSEID == cr.COURSEID).Count();

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
                                        emailFunction.SendConfirmationEmail(settlementAmount.ToString(), invoiceNumber, "","routineconfirmation2");
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
                            }
                            else
                            {
                                ReconcileData = ReconcileData + "Status:Failed" + foundDupe;
                            }
                        }
                    }
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
