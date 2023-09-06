using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Integration.ClubPilates
{
    public class ClubPilatesHelper
    {
        public void InsertUserToClubPilates(Student student)
        {
            ClubPilatesConfig ClubPilatesConfig = GetConfig();

            string URL = ClubPilatesConfig.requestUrl + "/api/current/users/prospect?ApiKey=" + ClubPilatesConfig.APIKey + "&StoreId=" + ClubPilatesConfig.StoreId + "&FirstName=" + student.FIRST + "&LastName=" + student.LAST + "&Email=" + student.EMAIL + "&SendEmail=true&Zip="+student.ZIP;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                                string jsonfieldfield = reader.ReadToEnd();

                                if (jsonfieldfield != null)
                                {
                                    JavaScriptSerializer j = new JavaScriptSerializer();
                                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                                    foreach (KeyValuePair<string, object> pairs in settingsconfig)
                                    {
                                        if (pairs.Key == "UserId")
                                            student.clubready_student_id=int.Parse( pairs.Value.ToString());
                                    }
                                }
            }
        }

        public ClubPilatesPackageDetails GetPackageDetails(int packageid)
        {
            ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageDetails();
            string URL = ClubPilatesConfig.requestUrl + "/api/current/sales/package/" + packageid + "?ApiKey=" + ClubPilatesConfig.APIKey + "&StoreId=" + ClubPilatesConfig.StoreId;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "GET";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                    foreach (KeyValuePair<string, object> pairs in settingsconfig)
                    {
                        if (pairs.Key == "Id")
                            ClubPilatesPackageDetails.PackageId = int.Parse(pairs.Value.ToString());
                        if (pairs.Key == "Name")
                            ClubPilatesPackageDetails.Description = pairs.Value.ToString();
                        if (pairs.Key == "Price")
                            ClubPilatesPackageDetails.Price =decimal.Parse( pairs.Value.ToString().Trim());
                    }

                }
                return ClubPilatesPackageDetails;
            }
        }
        public void SellPackageToAUser(Course_Roster roster)
        {
            ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageDetails();
            string URL = ClubPilatesConfig.requestUrl + "/api/current/sales/contract/sold?ApiKey=" + ClubPilatesConfig.APIKey + "&&StoreId=" + ClubPilatesConfig .StoreId+ "&MemberId=49124066&PackageId=282285&InstallmentId=424868&PaymentAmount=599&";
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));

                }
            }
        }

        public void CreateAPaymentProfile(string memberid, string AuthToken, string last4, string ExpMonth, string ExpYear, string accountype, string cardtype)
        {
                        ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageDetails();
            string URL = ClubPilatesConfig.requestUrl + "/api/current/sales/member/" + memberid + "/payment/profile?StoreId=" + ClubPilatesConfig.StoreId + "&ApiKey=" + ClubPilatesConfig.APIKey + "&AuthToken=" + AuthToken + "&last4=" + last4 + "&ExpMonth=" + ExpMonth + "&ExpYear=" + ExpYear + "&AcctType=" + accountype + "&CardType="+cardtype;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));

                }
            }
        }
        public List<ClubPilatesPackageInstallmentDetails> GetPackageInstallmentDetails(int packageid)
        {
            ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageInstallmentDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageInstallmentDetails();
            List<ClubPilatesPackageInstallmentDetails> ClubPilatesPackageInstallmentDetails = new List<ClubPilates.ClubPilatesPackageInstallmentDetails>();
            string URL = ClubPilatesConfig.requestUrl + "/api/current/sales/packages/" + packageid + "/installments?ApiKey=" + ClubPilatesConfig.APIKey + "&StoreId=" + ClubPilatesConfig.StoreId;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "GET";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                    foreach (var set in settingsconfig)
                    {
                        foreach (KeyValuePair<string, object> pairs in set)
                        {
                            if (pairs.Key == "Id")
                                ClubPilatesPackageDetails.installmentId = int.Parse(pairs.Value.ToString());
                            if (pairs.Key == "Name")
                                ClubPilatesPackageDetails.Name = pairs.Value.ToString();
                            if (pairs.Key == "DuePerPayment")
                                ClubPilatesPackageDetails.DueAmountPerPayment = pairs.Value.ToString();
                            if (pairs.Key == "PaymentCount")
                                ClubPilatesPackageDetails.PaymentCount = pairs.Value.ToString();
                            if (pairs.Key == "Fees")
                            {
                                var  json = new JavaScriptSerializer().Serialize(pairs.Value);
                                dynamic keypairing = j.Deserialize(json, typeof(object));
                                foreach (var keys in keypairing)
                                {
                                    foreach (KeyValuePair<string, object> pairing in keys)
                                    {
                                        if (pairing.Key == "Amount")
                                            ClubPilatesPackageDetails.DueAmountPerPayment = (double.Parse(ClubPilatesPackageDetails.DueAmountPerPayment.Replace(" ", "")) + double.Parse(pairing.Value.ToString().Replace(" ", ""))).ToString();
                                    }

                                    
                                }

                            }
                        }
                        ClubPilatesPackageInstallmentDetails.Add(ClubPilatesPackageDetails);
                        ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageInstallmentDetails();

                    }

                }
                return ClubPilatesPackageInstallmentDetails;
            }
        }
        public string CreatePaymentProfile(int studentID, CreditCardPaymentModel CreditCardPaymentModel)
        {
            string AcctToken="";
            string cardtype = "0";
            if (CreditCardPaymentModel.CardType == "Visa")
            {
                cardtype = "1";
            }
            else if (CreditCardPaymentModel.CardType == "Mastercard")
            {
                cardtype = "2";
            }
            else if (CreditCardPaymentModel.CardType == "American Express")
            {
                cardtype = "4";
            }
            else if (CreditCardPaymentModel.CardType == "Discover")
            {
                cardtype = "3";
            }


            var student = GetStudentDetails(studentID);

            ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageDetails();
            string URL = "https://www.clubreadygateway.com:443" + "/api/current/paymentprofile/create/?ApiKey=" + ClubPilatesConfig.APIKey + "&OwnerId=" + student.clubready_student_id + "&OwnerType=1&FirstName=" + student.FIRST + "&LastName=" + student.LAST + "&AccountNumber=" + CreditCardPaymentModel.CardNumber + "&AcctClass=1&AcctType="+cardtype+"&First6=" + CreditCardPaymentModel.CardNumber.Trim().Substring(0, 6) + "&Last4=" + CreditCardPaymentModel.CardNumber.Substring(CreditCardPaymentModel.CardNumber.Length - Math.Min(4, CreditCardPaymentModel.CardNumber.Length)) + "&CcExpMonth=" + CreditCardPaymentModel.ExpiryMonth + "&CcExpYear=" + CreditCardPaymentModel.ExpiryYear + "&PostalCode=" + CreditCardPaymentModel.Zip + "&CountryCode=US&State=" + CreditCardPaymentModel.State + "&City=" + CreditCardPaymentModel.City + "&Address1=" + CreditCardPaymentModel.Address;
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                    foreach (KeyValuePair<string, object> pairs in settingsconfig)
                    {
                        if (pairs.Key == "AcctToken")
                            AcctToken = pairs.Value.ToString();
                    }

                }
            }
            if (student.clubready_student_id != null)
            {
             //   if (!CheckExistingCardProfile(student.clubready_student_id.Value.ToString()))
             //   {
                try
                {

                    CreateAPaymentProfile(student.clubready_student_id.Value.ToString(), AcctToken, CreditCardPaymentModel.CardNumber.Substring(CreditCardPaymentModel.CardNumber.Length - Math.Min(4, CreditCardPaymentModel.CardNumber.Length)).ToString(), CreditCardPaymentModel.ExpiryMonth, CreditCardPaymentModel.ExpiryYear, "1", cardtype);
                }
                catch
                { }
                    //   }
            }
            return AcctToken;
        }
        public void SetDefaultPaymentProfile()
        {
        }

        public bool CheckExistingCardProfile(string memberid)
        {
            bool result = false;
            ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageDetails();
            string URL = ClubPilatesConfig.requestUrl + "/api/current/sales/member/" + memberid + "/profile/check?ApiKey=" + ClubPilatesConfig.APIKey + "&StoreId=" + ClubPilatesConfig.StoreId;   //StoreId=" + ClubPilatesConfig.StoreId + "&ApiKey=" + ClubPilatesConfig.APIKey;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "GET";
            request.ContentLength = 0;
            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                    foreach (KeyValuePair<string, object> pairs in settingsconfig)
                    {
                        if (pairs.Key == "HasCCProfile")
                            result = bool.Parse( pairs.Value.ToString());
                    }


                }
            }

            return result;
        }
        public string ContractSold(int studentID, CreditCardPaymentModel CreditCardPaymentModel,string AuthToken,Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCartItem courseitem)
        {
            string AcctToken = "";
            var student = GetStudentDetails(studentID);
            ClubPilatesConfig ClubPilatesConfig = GetConfig();
            ClubPilatesPackageDetails ClubPilatesPackageDetails = new ClubPilates.ClubPilatesPackageDetails();
            string URL = ClubPilatesConfig.requestUrl + "/api/current/sales/contract/sold?ApiKey=" + ClubPilatesConfig.APIKey + "&&StoreId=" + ClubPilatesConfig.StoreId + "&MemberId=" + student.clubready_student_id + "&PackageId=" + courseitem.Course.clubready_package_id + "&InstallmentId=" + courseitem.PricingModel.ClubReadyInstallmentId + "&PaymentAmount=" + courseitem.LineTotal + "&";
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentLength = 0;


            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                    foreach (KeyValuePair<string, object> pairs in settingsconfig)
                    {
                        if (pairs.Key == "description")
                            AcctToken = pairs.Value.ToString();

                        if (pairs.Key == "ContractSaleID")
                        {
                            CreditCardPaymentModel.AuthNum = pairs.Value.ToString();
                            CreditCardPaymentModel.PaymentNumber = pairs.Value.ToString();
                            CreditCardPaymentModel.RefNumber = pairs.Value.ToString();
                        }
                    }

                }
            }

            if (AcctToken.ToLower() == "success")
            {
                AcctToken = "Approved";
                CreditCardPaymentModel.RespMsg = "Completed";
                CreditCardPaymentModel.Result = "Approved";
            }

            else
            {

                AcctToken = "Unable to Process the Credit Card. The number may be invalid or profile error. Please try again or contact administrator.";
            }


            return AcctToken;
        }
        public static ClubPilatesConfig GetConfig()
        {
            string jsonfieldfield = Settings.Instance.GetMasterInfo4().ClubPilatesConfiguration;
            ClubPilatesConfig ClubPilatesConfig = new ClubPilatesConfig();
            if (jsonfieldfield != null && jsonfieldfield != "{}")
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                ClubPilatesConfig.requestUrl =settingsconfig["requesturl"];
                ClubPilatesConfig.APIKey = settingsconfig["apikey"];
                ClubPilatesConfig.StoreId =settingsconfig["storeid"];
                ClubPilatesConfig.useClubPilates = int.Parse(settingsconfig["useclubpilates"]);
            }
            return ClubPilatesConfig;
        }
        public Student GetStudentDetails(int sid)
        {
            using (var db = new SchoolEntities())
            {
                var student = (from _student in db.Students where _student.STUDENTID == sid select _student).FirstOrDefault();

                if (student.clubready_student_id == null)
                {
                    InsertUserToClubPilates(student);
                }
                db.SaveChanges();

                return student;
            }
        }
        public static string BuildRosterPackageInstallmentDetails(int packageid, int installmentid)
        {
            var PackageInstallmentDetails = " [{\"packageid\":" + packageid + ",\"installmentid\":" + installmentid + "}]";
            return PackageInstallmentDetails;
        }
    }
    public class ClubPilatesInstallmentPlanDetails {
        public int installmentPlanId
        {
            get;
            set;
        }
    }
    public class ClubPilatesPackageInstallmentDetails
    {
        public int installmentId { get; set; }
        public string DueAmountPerPayment { get; set; }
        public string PaymentCount { get; set; }
        public string Name { get; set; }
    }
    public class ClubPilatesPackageDetails{
        public int PackageId { get; set; }
        public decimal Price{get;set;}
        public string Description{get;set;}
    }
    public class ClubPilatesConfig
    {
        public int useClubPilates
        {
            get;
            set;
        }
        public string requestUrl
        {
            get;
            set;
        }

        public string APIKey
        {
            get;
            set;

        }
        public string StoreId
        {
            get;
            set;
        }
    }
}
