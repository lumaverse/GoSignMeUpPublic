using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Commerce;
using System.Net.Mail;
using System.Web.Mail;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Networking;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using System.Xml;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using Gsmu.Api.Authorization;
using System.Net;
using Gsmu.Api.Integration.ClubPilates;
using Gsmu.Api.Integration.Spreedly;


namespace Gsmu.Web.Areas.Public.Controllers
{
    public class PaymentsController : Controller
    {
        public string AuthorizeTransactions_TouchNetTLink(string session)
        {

            CreditCardPayments payment = new CreditCardPayments();
            return payment.AuthorizeAccount_TouchnetTLink(session);
        }
        public string ProcessAndSelectPaymentMerchant(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            string strResult = "";
            if (CreditCardPaymentHelper.UseAuthorizeNet)
            {
                strResult = PaymentAuthorizedNet(CreditCardPaymentModelValues, Amount);
            }
            else if (CreditCardPaymentHelper.UseBBPaygate)
            {
                strResult = PaymentBlackBoardGateway(CreditCardPaymentModelValues, Amount);
            }

            else if (CreditCardPaymentHelper.UseChasePayment)
            {
                strResult = "Redirect";
            }
            else if (CreditCardPaymentHelper.UsePaygov)
            {

                XmlDocument xmlResult = new XmlDocument();
                xmlResult = PaymentPaygov(CreditCardPaymentModelValues, Amount);
                XmlNodeList listNodes = xmlResult.GetElementsByTagName("a:TOKEN");
                foreach (XmlNode node in listNodes)
                {

                    strResult = node.InnerText;
                }
                int? PayGovServer = Settings.Instance.GetMasterInfo3().PayGovServer;
                string Common_Checkout_Web_URL = "";
                //string Web_service_WSDL_URL = "";
                //string Web_service_ENDPOINT_URL = "";
                if (PayGovServer == 1)
                {
                    Common_Checkout_Web_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/Commoncheckpage/Default.aspx";
                    // Web_service_WSDL_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.wsdl";
                    // Web_service_ENDPOINT_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.svc";
                }
                else
                {
                    Common_Checkout_Web_URL = "https://securecheckout.cdc.nicusa.com/CommonCheckPage/Default.aspx";
                    //Web_service_WSDL_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.wsdl";
                    //Web_service_ENDPOINT_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.svc";
                }

                strResult = Common_Checkout_Web_URL + "?token=" + strResult;

                using (StreamWriter _testData = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest.txt"), true))
                {
                    _testData.WriteLine(Environment.NewLine +":::::::Redirect To:::"+strResult); // Write the file.
                }  
            }

            else if (CreditCardPaymentHelper.UseAuthorizeNetRedirect)
            {
                strResult = "Redirect";
            }
            else if (CreditCardPaymentHelper.UseTouchnetTlink)
            {
                strResult = PaymentTouchnetTLink(CreditCardPaymentModelValues, Amount);
            }
            else if (CreditCardPaymentHelper.UseNelNet)
            {
                strResult = "NelNetIFrame";
            }
            else if (CreditCardPaymentHelper.UsePayPal)
            {
                CreditCardPayments payment = new CreditCardPayments();
                strResult = payment.PayflowProV2(CreditCardPaymentModelValues, Amount);
            }
            else if (CreditCardPaymentHelper.UseCashNetRedirect)
            {
                strResult = "Redirect";
            }
            else if (CreditCardPaymentHelper.UseCybersource)
            {
                strResult = "Loading Payment form...";
            }
            else if (CreditCardPaymentHelper.UseClubPilates)
            {
                ClubPilatesHelper ClubPilatesHelper = new ClubPilatesHelper();
               string Authtoken = ClubPilatesHelper.CreatePaymentProfile(AuthorizationHelper.CurrentStudentUser.STUDENTID, CreditCardPaymentModelValues);
               foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
               {
                   if (item.Course.clubready_package_id > 0)
                   {
                       strResult =strResult+  ClubPilatesHelper.ContractSold(AuthorizationHelper.CurrentStudentUser.STUDENTID, CreditCardPaymentModelValues, Authtoken, item);
                   }
               }
            }
            else if (CreditCardPaymentHelper.UseSpreedly)
            {
                strResult = SpreedlyHelper.SpreedlyProcessPayment(CreditCardPaymentModelValues, Amount);
            }
            else
            {
                strResult = "Selected payment gateway is not yet implemented.";
            }

            return strResult;
        }
        public string PaymentTouchnetTLink(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            CreditCardPayments payment = new CreditCardPayments();
            return payment.ProcessTouchNetTlink(CreditCardPaymentModelValues, Amount);
        }

        public string PaymentRevtrak(string PaymentId, string Amount)
        {
            CreditCardPayments payment = new CreditCardPayments();
            return payment.ProcessRevtrak(PaymentId, Amount);
        }
        public string PaymentAuthorizedNet(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            CreditCardPaymentModelValues.Description ="";
            if (Settings.Instance.GetMasterInfo().anet_includedesccoursename != 0)
            {
                foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
                {
                    if (CreditCardPaymentModelValues.Description.Length < 100)
                    {
                        CreditCardPaymentModelValues.Description = CreditCardPaymentModelValues.Description + " " + item.Course.COURSENUM + " " + item.Course.COURSENAME + ",";
                    }
                }
                if (CreditCardPaymentModelValues.Description.Length > 1)
                {
                    CreditCardPaymentModelValues.Description = CreditCardPaymentModelValues.Description.Remove(CreditCardPaymentModelValues.Description.Length - 1);
                }
            }
          
            CreditCardPayments payment = new CreditCardPayments();
            return payment.ProcessAuthorizedNetPayment(CreditCardPaymentModelValues, Amount);
        }

        public string GetDescriptionForAnet()
        {
            string descriptionForAnet="";
            if (Settings.Instance.GetMasterInfo().anet_includedesccoursename != 0)
            {
                foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
                {
                    if (descriptionForAnet.Length < 100)
                    {
                        descriptionForAnet = descriptionForAnet + " " + item.Course.COURSENUM + " " + item.Course.COURSENAME + ",";
                    }
                }
                if (descriptionForAnet != "")
                {
                    descriptionForAnet = descriptionForAnet.Remove(descriptionForAnet.Length - 1);
                }
            }
            return descriptionForAnet;
        }

        public string PaymentBlackBoardGateway(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            CreditCardPaymentModel CreditCardPaymentModel = new CreditCardPaymentModel();
            CreditCardPayments payment = new CreditCardPayments();
            if (CreditCardPaymentHelper.UseBlackBoardPaygateRedirect)
            {
                return "Redirect";
            }
            else
            {
                return payment.ProcessBlackBoardPaygate(CreditCardPaymentModelValues, Amount);
            }
        }
        public string PaymentChaseOrbital(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            CreditCardPaymentModel CreditCardPaymentModel = new CreditCardPaymentModel();
            CreditCardPayments payment = new CreditCardPayments();
            return payment.ProcessChasePaymentOrbital(CreditCardPaymentModelValues, Amount);
        }

        public XmlDocument PaymentPaygov(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            CreditCardPaymentModel CreditCardPaymentModel = new CreditCardPaymentModel();
            CreditCardPayments payment = new CreditCardPayments();
            HttpContextWrapper context = new HttpContextWrapper(System.Web.HttpContext.Current);
            var request = context.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            CreditCardPaymentModelValues.CurrentUrl = new string[2];
            CreditCardPaymentModelValues.CurrentUrl[0] = baseUrl;
            CreditCardPaymentModelValues.CurrentUrl[1] = baseUrl + request.FilePath; ;

            return payment.ProcessPaygov(CreditCardPaymentModelValues, Amount);
        }

        public string UpdatePaygovOrder(string token)
        {
            CreditCardPayments payment = new CreditCardPayments();

            XmlDocument xmlResult = new XmlDocument();
            xmlResult = payment.FinalUpdateforPaygovPayment(token);
            string PayGovTransactionDetails = "";

            string pgTotalAmount = "";
            string pgCCName = "";
            string pgAuthCode = "";
            string pgProcessDate = "";
            string pgProcessTime = "";

            XmlNodeList listNodes = xmlResult.GetElementsByTagName("a:TOTALAMOUNT");
            foreach (XmlNode node in listNodes)
            {
                pgTotalAmount = node.InnerText;
            }

            listNodes = xmlResult.GetElementsByTagName("a:BillingName");
            foreach (XmlNode node in listNodes)
            {
                pgCCName = node.InnerText;
            }
            listNodes = xmlResult.GetElementsByTagName("a:AUTHCODE");
            foreach (XmlNode node in listNodes)
            {
                pgAuthCode = node.InnerText;
            }
            listNodes = xmlResult.GetElementsByTagName("a:RECEIPTDATE");
            foreach (XmlNode node in listNodes)
            {
                pgProcessDate = node.InnerText;
            }
            listNodes = xmlResult.GetElementsByTagName("a:RECEIPTTIME");
            foreach (XmlNode node in listNodes)
            {
                pgProcessTime = node.InnerText;
            }
            if (string.IsNullOrWhiteSpace(pgCCName)) { pgCCName = ""; }
            if (string.IsNullOrWhiteSpace(pgAuthCode)) { pgAuthCode = ""; }
            if (string.IsNullOrWhiteSpace(pgProcessDate)) { pgProcessDate = ""; }
            if (string.IsNullOrWhiteSpace(pgProcessTime)) { pgProcessTime = ""; }

            PayGovTransactionDetails = pgTotalAmount + "|" + pgCCName + "|" + pgAuthCode + "|" + pgProcessDate + "|" + pgProcessTime;
            return PayGovTransactionDetails;
        }

        public string PaymentCashnet(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {

            return "Approved";
        }

        public string HMAC_MD5(string key, string value)
        {
            // The first two lines take the input values and convert them
            // from strings to Byte arrays
            byte[] HMACkey = (new System.Text.ASCIIEncoding()).GetBytes(key);
            byte[] HMACdata = (new System.Text.ASCIIEncoding()).GetBytes(value);

            // create a HMACMD5 object with the key set
            HMACMD5 myhmacMD5 = new HMACMD5(HMACkey);

            //calculate the hash (returns a byte array)
            byte[] HMAChash = myhmacMD5.ComputeHash(HMACdata);

            //loop through the byte array and add append each piece to
            // a string to obtain a hash string
            string fingerprint = string.Empty;
            for (int i = 0; i < HMAChash.Length; i++)
            {
                fingerprint += HMAChash[i].ToString("x").PadLeft(2, '0');
            }

            return fingerprint;
        }

        public string PaymentNelNet(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            ///////////////////////
            ////////will need to apply API type old & new along with this custom params
            //////////////////////
            NameValueCollection requestArray = new NameValueCollection();
            //For epoch time purpose
            TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            //double total_span_time_diff = span.TotalMilliseconds + Settings.Instance.GetMasterInfo3().nelnettimedifference;
            Int32 unixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0))).TotalSeconds;
            string nelnet_timestamp = unixTimestamp.ToString() + "000";
            nelnet_timestamp = (Convert.ToDouble(nelnet_timestamp) + Settings.Instance.GetMasterInfo3().nelnettimedifference).ToString();
            string return_url = Settings.Instance.GetMasterInfo3().usernelnetredirectlink;
            //end
            //key
            string key = Settings.Instance.GetMasterInfo3().usernelnetkey;
            string orderType = Settings.Instance.GetMasterInfo3().usernelnetordertype;
            string street = CreditCardPaymentModelValues.Address;
            string city = CreditCardPaymentModelValues.City;
            string state = CreditCardPaymentModelValues.State;
            string zip = CreditCardPaymentModelValues.Zip;
            string email = CreditCardPaymentModelValues.Email;

            string NelNetBaseUrlParametersConfig = System.Configuration.ConfigurationManager.AppSettings["NelNetBaseUrlParameters"];
            double amount = Convert.ToDouble(Amount) * 100;
            string baseurlparams = buildNelNetParams(NelNetBaseUrlParametersConfig, CreditCardPaymentModelValues,amount);

            string baseUrl = Settings.Instance.GetMasterInfo3().usernelnetsubmissionlink;





            string hash2 = "";
            var fieldsparameters = HttpUtility.ParseQueryString(baseurlparams);
            foreach (string parameter in fieldsparameters.Keys)
            {
                if (parameter != "timestamp" && parameter != "hash" && parameter != "contentEmbedded")
                {
                    hash2 = hash2+ fieldsparameters[parameter];
                }
            }
            /*
            string hash2 =
             orderType2
            + onum
            + ordername
            + amount2
            + paymentMethod
            + streetOne
            + city2
            + state2
            + zip2
            + email2
            + redirecturl2
                          + redirect_parameters
                         + retriesAllowed2
            + contentEmbedded2;
             */
            hash2 = hash2 + nelnet_timestamp.Split('.')[0];
            hash2 = hash2 + key;
            //hash = "GoSignUpRegistrationC1XWZJ3C0C6Z0TW240000cc9200 Irinve Center DriveirvineCA92618Tanja@mediablend.comhttps://cmich.gosignmeup.com/admin/nelnetredirect.asptransactionType,transactionStatus,transactionId,transactionTotalAmount,transactionDate,transactionAcountType,transactionResultCode,transactionResultMessage,orderNumber,orderType,orderName,orderAmount,orderFee,accountHolderName,streetOne,streetTwo,city,state,zip,country,daytimePhone,eveningPhone,email0true14287267102411hm3mjgMio6mSyLveEc3EpKSNPf4aQrt2gSj9FLE3tQUaH4JcS1K2T1TZJvmAbAAfpKTFrqCvZG5L7Oy8bhZaTUgUBzMLMN4YJYjvKhj71mGfBOaWUEzjqdeAAqeJzRm";
            byte[] encodedPassword2 = new UTF8Encoding().GetBytes(hash2);
            var md52 = MD5.Create();
            string encoded2 = BitConverter.ToString(md52.ComputeHash(encodedPassword2)).Replace("-", "").ToLower();
            string url = baseurlparams;
            url = url.Replace("{timestamp}", nelnet_timestamp.Split('.')[0]);
            url = url.Replace("{hash}", encoded2);
            url = baseUrl + "?" + url;
            /*
            url = baseUrl + "?orderType=" + orderType2
                + "&orderNumber=" + onum
                + "&orderName=&amount=" + amount2
                + "&paymentMethod=" + paymentMethod
                + "&streetOne=" + streetOne
                + "&city=" + city2
                + "&state=" + state2
                + "&zip=" + zip2
                + "&email=" + email2 + "&redirectUrl=" + redirecturl2
                + "&redirectUrlParameters=" + redirect_parameters
                + "&retriesAllowed=" + retriesAllowed2
                + "&contentEmbedded=" + contentEmbedded2
                + "&timestamp=" + nelnet_timestamp.Split('.')[0]
                + "&hash=" + encoded2;
             */
            return url;
        }

        private string buildNelNetParams(string config, CreditCardPaymentModel creditcardmodel,double amount)
        {
            string redirect_parameters = "transactionType,transactionStatus,transactionId,transactionTotalAmount,transactionDate,transactionAcountType,transactionResultCode,transactionResultMessage,orderNumber,orderType,orderName,orderAmount,orderFee,accountHolderName,streetOne,streetTwo,city,state,zip,country,daytimePhone,eveningPhone,email";
            string return_url = Settings.Instance.GetMasterInfo3().usernelnetredirectlink;
            string orderType = Settings.Instance.GetMasterInfo3().usernelnetordertype;
            string baseUrl = Settings.Instance.GetMasterInfo3().usernelnetsubmissionlink;
            string orderType2 = orderType;
            string onum = creditcardmodel.OrderNumber;
            string ordername = "";
            string amount2 = amount.ToString();
            string paymentMethod = "cc";
            string streetOne = "9200 Irinve Center Drive";
            string city2 = "irvine";
            string state2 = "CA";
            string zip2 = "92618";
            string email2 = "Tanja@mediablend.com";

            if (AuthorizationHelper.CurrentStudentUser != null)
            {
                streetOne = AuthorizationHelper.CurrentStudentUser.ADDRESS;
                if (!string.IsNullOrEmpty(streetOne)) {
                    streetOne = streetOne.Replace("#","%23");
                }
                city2 = AuthorizationHelper.CurrentStudentUser.CITY;
                state2 = AuthorizationHelper.CurrentStudentUser.STATE;
                zip2 = AuthorizationHelper.CurrentStudentUser.ZIP;
                email2 = AuthorizationHelper.CurrentStudentUser.EMAIL;
            }
            string redirecturl2 = return_url;
            string retriesAllowed2 = "0";
            string contentEmbedded2 = "true";


            config =config.Replace("{orderType}", orderType2);
            config = config.Replace("{orderNumber}", onum);
            config = config.Replace("{orderName}", ordername);

            config = config.Replace("{amount}", amount.ToString());
            config = config.Replace("{paymentMethod}", paymentMethod);
            config = config.Replace("{streetOne}", streetOne);

            config = config.Replace("{city}", city2);
            config = config.Replace("{state}", state2);
            config = config.Replace("{zip}", zip2);


            config = config.Replace("{email}", email2);
            config = config.Replace("{redirecturl}", redirecturl2);
            config = config.Replace("{redirect_parameters}", redirect_parameters);

            config = config.Replace("{retriesAllowed}", retriesAllowed2);
            config = config.Replace("{contentEmbedded}", contentEmbedded2);
            //config.Replace("{timestamp}", redirect_parameters);
            //config.Replace("{hash}", hash);
            



            return config.Replace('|','&');
        }
        public string BuildBlackBoardHPPURL(CreditCardPaymentModel CreditCardPaymentModelValues, string Amount)
        {
            CreditCardPayments payment = new CreditCardPayments();
            return payment.BuildBlackBoardHPPURL( CreditCardPaymentModelValues, Amount);
        }


    }
}
