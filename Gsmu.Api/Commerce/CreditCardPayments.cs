using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using System.Web.UI.WebControls;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Data;
using System.Xml;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Authorization;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.Communication;
using System.Collections.Specialized;
using Gsmu.Api.Networking.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Gsmu.Api.TCSSingleService;

namespace Gsmu.Api.Commerce
{

    public class CreditCardPayments
    {

        public IEnumerable<Payment_Option> GetAcceptedCreditCards()
        {
            using (var db = new SchoolEntities())
            {
                IEnumerable<Payment_Option> item = (from m in db.Payment_Options where m.VisibleTo != 3 & m.VisibleTo != 2 & m.PaymentClass == 0 orderby m.PAYMENTTYPE descending select m).ToList();
                return item;
            }
        }

        public int CountAllPaymentTypes()
        {
            IEnumerable<Payment_Option> item = GetAllPaymentTypes();
            return item.Count();
        }

        public IEnumerable<Country> GetCountryList()
        {
            using (var db = new SchoolEntities())
            {
                IEnumerable<Country> item = (from m in db.Countries where m.disabled == 0 orderby m.countryorder, m.countryname ascending select m).ToList<Country>();
                if (item.Count() > 0)
                {
                    return item; //AddDefaultCountryValue().Concat(item);
                }
                else
                {
                    return new[] { new Country() { countrycode = "US", countryname = "United States" } }; //AddDefaultCountryValue().Concat(new[] { new Country() { countrycode = "US", countryname = "United States" } });
                }
            }
        }

        /*
        //decided to default to United States instead of use Select Country, since the jQuery Validate plugin needs an empty string for the dropdown value
        public static IEnumerable<Country> AddDefaultCountryValue()
        {
            return new[] {
                new Country() { countrycode = "", countryname = "Select Country" }
            };
        }
        */

        public IEnumerable<Payment_Option> GetAllPaymentTypes()
        {
            using (var db = new SchoolEntities())
            {
                IEnumerable<Payment_Option> item = (from m in db.Payment_Options where m.VisibleTo != 3 & m.VisibleTo != 2 & m.PaymentClass == 1 orderby m.PAYMENTTYPE descending select m).ToList();
                return AddDefaultPaymentTypeValue().Concat(item);
            }
        }
        public IEnumerable<Payment_Option> GetAllPaymentTypesforAdmin()
        {
            using (var db = new SchoolEntities())
            {
                IEnumerable<Payment_Option> item = (from m in db.Payment_Options where m.VisibleTo != 3 & m.PaymentClass == 1 orderby m.PAYMENTTYPE descending select m).ToList();
                return AddDefaultPaymentTypeValue().Concat(item);
            }
        }

        public Payment_Option GetPaymentTypeInfo(string paymenttype)
        {
            using (var db = new SchoolEntities())
            {
                if (AuthorizationHelper.CurrentAdminUser != null)
                {
                    Payment_Option item = (from m in db.Payment_Options where m.PAYMENTTYPE == paymenttype && (m.VisibleTo == 0 || m.VisibleTo == 2) select m).FirstOrDefault();
                    return item;
                }
                else
                {
                    Payment_Option item = (from m in db.Payment_Options where m.PAYMENTTYPE == paymenttype && m.VisibleTo == 0 select m).FirstOrDefault();
                    return item;
                }
                
            }
        }

        public static IEnumerable<Payment_Option> AddDefaultPaymentTypeValue()
        {
            return new[] {
                new Payment_Option() { PAYMENTTYPE = "Select Payment Type" }
            };
        }
        public string AuthorizeAccount_TouchnetTLink(string session)
        {
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            TPGSecureLink.AuthorizeAccountRequest authorize = new TPGSecureLink.AuthorizeAccountRequest();
            authorize.session = session;

            TPGSecureLink.TPGSecureLink SecureLink = new TPGSecureLink.TPGSecureLink();
            //1. Credentials
            //SecureLink.Credentials
            NetworkCredential netCredential = new NetworkCredential(CreditCardPaymentHelper.TouchnetTLinkUserName, CreditCardPaymentHelper.TouchnetTLinkPassword);
            Uri uri = new Uri(SecureLink.Url);
            ICredentials credentials = netCredential.GetCredential(uri, "Basic");
            SecureLink.Credentials = credentials;
            SecureLink.Url = CreditCardPaymentHelper.TouchnetTLinkServiceurl;

            TPGSecureLink.AuthorizeAccountResponse response = SecureLink.authorizeAccount(authorize);
            CreditCardPayments payment = new CreditCardPayments();
            payment.SaveTouchnetTLinkPaymentInfo(response.ticketName, response.creditResponse.approvalCode, response.creditResponse.receiptNumber);
            return response.ticketName;
        }

        public void SaveTouchnetTLinkPaymentInfo(string OrderNumber, string AuthNum, string PayNumber)
        {
            using (var db = new SchoolEntities())
            {
                Gsmu.Api.Data.School.Student.EnrollmentFunction enrollment = new Data.School.Student.EnrollmentFunction();

                enrollment.OrderInprogressToRoster(null, OrderNumber);
                var rosterList = db.Course_Rosters.Where(u => u.OrderNumber == OrderNumber).ToList();
                if (rosterList.Count == 0)
                {
                    rosterList = db.Course_Rosters.Where(u => u.MasterOrderNumber == OrderNumber).ToList();
                }
                foreach (var rosters in rosterList)
                {
                    var Context = new SchoolEntities();
                    Course_Roster roster = Context.Course_Rosters.First(cr => cr.RosterID == rosters.RosterID);
                    roster.PAYMETHOD = "Credit Card";

                    roster.payNumber = PayNumber;
                    roster.AuthNum = AuthNum;

                    if (Authorization.AuthorizationHelper.CurrentUser.IsLoggedIn == false)
                    {
                        if (AuthNum != null && AuthNum != "")
                        {
                            roster.PaidInFull = 1;
                            var chkout = CheckoutInfo.Instance;
                            roster.TotalPaid = chkout.PaymentTotal;
                        }
                    }
                    Context.SaveChanges();
                }
            }
        }
        public string ProcessTouchNetTlink(CreditCardPaymentModel paymentmodel, string Amount)
        {
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            TPGSecureLink.GenerateSecureLinkTicketRequest request = new TPGSecureLink.GenerateSecureLinkTicketRequest();
            request.ticketName = paymentmodel.OrderNumber;
            int pairingcount = 4;
            if (WebConfiguration.TouchNetIncludeUserEmailToAncillaryField == "true")
            {
                pairingcount = 5;
            }

            string formated_Amount = string.Format("{0:0.00}", Amount.TrimStart(new char[] { '0' }));
            TPGSecureLink.nameValuePair[] parameters = new TPGSecureLink.nameValuePair[pairingcount];
            TPGSecureLink.nameValuePair Parameter = new TPGSecureLink.nameValuePair();
            Parameter.name = "AMT";
            Parameter.value = formated_Amount;
            parameters[0] = Parameter;

            HttpContextWrapper context = new HttpContextWrapper(System.Web.HttpContext.Current);
            var url_request = context.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
            //Second Value Pair
            Parameter = new TPGSecureLink.nameValuePair();
            Parameter.name = "CANCEL_LINK";
            Parameter.value = string.Format("{0}://{1}{2}", url_request.Url.Scheme, url_request.Url.Authority, appUrl);
            parameters[1] = Parameter;
            //Third Value Pair
            Parameter = new TPGSecureLink.nameValuePair();
            Parameter.name = "ERROR_LINK";
            Parameter.value = string.Format("{0}://{1}{2}", url_request.Url.Scheme, url_request.Url.Authority, appUrl);
            parameters[2] = Parameter;
            //Fourth Value Pair
            Parameter = new TPGSecureLink.nameValuePair();
            Parameter.name = "SUCCESS_LINK";
            //Parameter.value = (System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            Parameter.value = string.Format("{0}://{1}{2}Landing", url_request.Url.Scheme, url_request.Url.Authority, appUrl);
            parameters[3] = Parameter;
            if (WebConfiguration.TouchNetIncludeUserEmailToAncillaryField == "true")
            {
                Parameter = new TPGSecureLink.nameValuePair();
                Parameter.name = "EXT_TRANS_ID";
                Parameter.value = AuthorizationHelper.CurrentUser.LoggedInUsername + ' ' + paymentmodel.OrderNumber.ToString();
                parameters[4] = Parameter;
            }
            request.nameValuePairs = parameters;
            TPGSecureLink.TPGSecureLink SecureLink = new TPGSecureLink.TPGSecureLink();
            //1. Credentials
            //SecureLink.Credentials
            NetworkCredential netCredential = new NetworkCredential(CreditCardPaymentHelper.TouchnetTLinkUserName, CreditCardPaymentHelper.TouchnetTLinkPassword);
            Uri uri = new Uri(SecureLink.Url);
            ICredentials credentials = netCredential.GetCredential(uri, "Basic");
            SecureLink.Credentials = credentials;
            SecureLink.Url = CreditCardPaymentHelper.TouchnetTLinkServiceurl;


            TPGSecureLink.GenerateSecureLinkTicketResponse response = SecureLink.generateSecureLinkTicket(request);
            return response.ticket;


        }
        public string ProcessAuthorizedNetPayment(CreditCardPaymentModel paymentmodel, string Amount)
        {
            paymentmodel.LoginID = CreditCardPaymentHelper.ANLogin;
            paymentmodel.LoginKey = CreditCardPaymentHelper.ANTranKey;
            if (CreditCardPaymentHelper.ANTesting == true)
            {
                paymentmodel.Url = "https://test.authorize.net/gateway/transact.dll";
            }
            else
            {
                paymentmodel.Url = "https://secure.authorize.net/gateway/transact.dll";
            }


            Dictionary<string, string> post_values = new Dictionary<string, string>();
            //the API Login ID and Transaction Key must be replaced with valid values
            post_values.Add("x_login", paymentmodel.LoginID);
            post_values.Add("x_tran_key", paymentmodel.LoginKey);
            post_values.Add("x_delim_data", "TRUE");
            post_values.Add("x_delim_char", "|");
            post_values.Add("x_relay_response", "FALSE");

            post_values.Add("x_type", "AUTH_CAPTURE");
            post_values.Add("x_method", "CC");
            post_values.Add("x_card_num", paymentmodel.CardNumber);
            post_values.Add("x_exp_date", paymentmodel.ExpiryMonth + paymentmodel.ExpiryYear);

            post_values.Add("x_amount", Amount);
            post_values.Add("x_description", paymentmodel.Description);
            post_values.Add("x_invoice_num", paymentmodel.OrderNumber);

            post_values.Add("x_first_name", paymentmodel.FirstName);
            post_values.Add("x_last_name", paymentmodel.LastName);
            post_values.Add("x_address", paymentmodel.Address);
            post_values.Add("x_city", paymentmodel.City);
            post_values.Add("x_state", paymentmodel.State);
            post_values.Add("x_zip", paymentmodel.Zip);
            post_values.Add("x_email", paymentmodel.Email);
            post_values.Add("x_phone", "0");
            post_values.Add("x_country", paymentmodel.Country);
            if (paymentmodel.CCV != "")
            {
                post_values.Add("x_card_code", paymentmodel.CCV);
            }
            String post_string = "";
            String audit_post = "";

            foreach (KeyValuePair<string, string> post_value in post_values)
            {
                post_string += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
                if (post_value.Key != "x_card_num")
                {
                    audit_post += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
                }
            }
            post_string = post_string.TrimEnd('&');
            audit_post = audit_post.TrimEnd('&');

            //Capture post string before posting to ANET.
            Gsmu.Api.Data.School.Student.EnrollmentFunction enrollment = new Data.School.Student.EnrollmentFunction();
            if (WebConfiguration.LogAuthorizeNetTransaction)
            {
                enrollment.LogAuthorizeNetTransaction(paymentmodel, Amount, audit_post, "", "CreditCardPayment.cs-Before_Posting-");
            }
            // create an HttpWebRequest object to communicate with Authorize.net
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(paymentmodel.Url);
            objRequest.Method = "POST";
            objRequest.ContentLength = post_string.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            // post data is sent as a stream
            StreamWriter myWriter = null;
            myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(post_string);
            myWriter.Close();

            // returned values are returned as a stream, then read into a string
            String post_response;
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
            {
                post_response = responseStream.ReadToEnd();
                responseStream.Close();
            }

            Array response_array = post_response.Split('|');
            string paymentstatus = "Not Accessible";
            if (post_response[0].ToString() == "1")
            {
                paymentstatus = "Approved.-";
                int index = 0;
                foreach (string a in response_array)
                {
                    if (index == 6)
                    {
                        paymentstatus = paymentstatus + a;
                    }
                    index = index + 1;
                }
            }
            else
            {
                switch (post_response[4].ToString())
                {
                    case "2":
                    case "3":
                        {
                            paymentstatus = "*Error: This transaction has been declined. Please try again.";
                            break;
                        }
                    case "6":
                        {
                            paymentstatus = "*Error: The credit card number is invalid. Please try again";
                            break;
                        }
                    case "7":
                    case "8":
                        {
                            paymentstatus = "*Error: The credit card expiration date is invalid or expired. Please try again.";
                            break;
                        }
                    case "11":
                        {
                            paymentstatus = "*Error: A duplicate transaction has been submitted or multiple failed transactions detected. Please try again in an hour.";
                            break;
                        }
                    default:
                        {
                            paymentstatus = "Declined. Code: " + post_response[4].ToString() + ". Please contact administrator.";
                            break;
                        }
                }
            }
            //capture after posted
            if (WebConfiguration.LogAuthorizeNetTransaction)
            {
                enrollment.LogAuthorizeNetTransaction(paymentmodel, Amount, audit_post, post_response, "CreditCardPayment.cs-");
            }
            return paymentstatus;
        }
        public string BuildBlackBoardHPPURL(CreditCardPaymentModel paymentmodel, string Amount)
        {
            string RedirectURL = "";
            string BBPaygateUsername = Settings.Instance.GetMasterInfo3().BBPaygateUsername;
            string BBPaygatePass = Settings.Instance.GetMasterInfo3().BBPaygatePass;
            string BBPaygateStoreID = Settings.Instance.GetMasterInfo3().BBPaygateStoreID;
            string BBPaygateGroupID = Settings.Instance.GetMasterInfo3().BBPaygateGroup;
            string BBPaygateServer = Settings.Instance.GetMasterInfo3().BBPaygateServer;
            string strMode = "";
            if (BBPaygateServer.Contains("test"))
            {
                strMode = "T";
            }
            else
            {
                strMode = "P";
            }
            RedirectURL = BBPaygateServer + "?mode=" + strMode;
            RedirectURL = RedirectURL + "&order_id=" + paymentmodel.OrderNumber;
            RedirectURL = RedirectURL + "&client_id=" + BBPaygateStoreID;
            RedirectURL = RedirectURL + "&user_id=" + BBPaygateUsername;
            RedirectURL = RedirectURL + "&password=" + BBPaygatePass;
            RedirectURL = RedirectURL + "&tran_amount=" + Amount;
            RedirectURL = RedirectURL + "&tran_type=Auth";
            RedirectURL = RedirectURL + "&return_url=https://dev250.gosignmeup.com";
            RedirectURL = RedirectURL + "&notification_url=https://dev250.gosignmeup.com";
            return RedirectURL;
        }
        public string ProcessBlackBoardPaygate(CreditCardPaymentModel paymentmodel, string Amount)
        {
            decimal intAmount = decimal.Parse(Amount) * 100;
            string BBPaygateUsername = Settings.Instance.GetMasterInfo3().BBPaygateUsername;
            string BBPaygatePass = Settings.Instance.GetMasterInfo3().BBPaygatePass;
            string BBPaygateStoreID = Settings.Instance.GetMasterInfo3().BBPaygateStoreID;
            string BBPaygateGroupID = Settings.Instance.GetMasterInfo3().BBPaygateGroup;
            string BBPaygateServer = Settings.Instance.GetMasterInfo3().BBPaygateServer;
            string struserId = paymentmodel.OrderNumber;
            string strMode = "";
            if (BBPaygateServer.Contains("test"))
            {
                strMode = "T";
            }
            else
            {
                strMode = "P";
            }
            string host = Dns.GetHostName();
            IPHostEntry ip = Dns.GetHostEntry(host);
            string IPAddressVal = "64.58.143.150";
            string xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xmlData = xmlData + "<EngineDocList> ";
            xmlData = xmlData + "	<DocVersion>1.0</DocVersion> ";
            xmlData = xmlData + "	<EngineDoc>";
            xmlData = xmlData + "		<ContentType>OrderFormDoc</ContentType>  ";
            xmlData = xmlData + "		<User> ";
            xmlData = xmlData + "				<Name>" + BBPaygateUsername + "</Name>     ";
            xmlData = xmlData + "				<Password>" + BBPaygatePass + "</Password>  ";
            xmlData = xmlData + "				<ClientId DataType=\"S32\">" + BBPaygateStoreID + "</ClientId> ";
            xmlData = xmlData + "		</User>   ";
            xmlData = xmlData + "		<IPAddress>" + IPAddressVal + "</IPAddress>  ";
            xmlData = xmlData + "		<Instructions> ";
            xmlData = xmlData + "			<Pipeline>PaymentNoFraud</Pipeline> ";
            xmlData = xmlData + "		</Instructions>    ";
            xmlData = xmlData + "		<OrderFormDoc>  ";
            xmlData = xmlData + "			<GroupId>" + BBPaygateGroupID + "</GroupId>  ";
            xmlData = xmlData + "			<Mode>" + strMode + "</Mode>   ";
            xmlData = xmlData + "			<UserId>" + struserId + "</UserId>";
            xmlData = xmlData + "			<OrderItemList>";

            Int32 intItemNumber = 0;
            // custom programming for BB training to set bill one order per course (related area, VBS routine/public/admin)
            //if (Settings.Instance.GetMasterInfo3().BBPaygate1tx1course == 10)
            //{
            foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
            {
                xmlData = xmlData + "				<OrderItem>	";
                xmlData = xmlData + "					<ItemNumber DataType=\"S32\" Locale=\"840\">" + intItemNumber + "</ItemNumber>";
                xmlData = xmlData + "					<Id>" + item.Course.COURSENUM + "</Id>			";
                xmlData = xmlData + "					<Desc>" + item.Course.COURSENAME + "</Desc>	";
                xmlData = xmlData + "					<ProductCode>" + item.Course.COURSEID.ToString() + "</ProductCode>	";
                xmlData = xmlData + "					<Qty DataType=\"S32\" Locale=\"840\">1</Qty>	";
                xmlData = xmlData + "					<Price DataType=\"Money\" Locale=\"840\">" + decimal.Round((item.LineTotal * 100), 0) + "</Price>	";
                xmlData = xmlData + "					<Total DataType=\"Money\" Locale=\"840\">" + decimal.Round((item.LineTotal * 100), 0) + "</Total>	";
                xmlData = xmlData + "				</OrderItem>";
                intItemNumber = intItemNumber + 1;
            }
            //}
            xmlData = xmlData + "			</OrderItemList>	";
            xmlData = xmlData + "			<Comments>" + paymentmodel.PaymentNumber + "</Comments>  ";
            xmlData = xmlData + "			<Consumer>           ";
            xmlData = xmlData + "				<PaymentMech>      ";
            xmlData = xmlData + "					<Type>CreditCard</Type>       ";
            xmlData = xmlData + "					<CreditCard>          ";
            xmlData = xmlData + "						<Number>" + paymentmodel.CardNumber + "</Number>   ";
            xmlData = xmlData + "					<Expires DataType=\"ExpirationDate\" Locale=\"840\">" + paymentmodel.ExpiryMonth + "/" + paymentmodel.ExpiryYear + "</Expires>   ";
            xmlData = xmlData + "						<Cvv2Indicator>" + Settings.Instance.GetMasterInfo2().CCVOn + "</Cvv2Indicator>       ";
            xmlData = xmlData + "						<Cvv2Val>" + paymentmodel.CCV + "</Cvv2Val>   ";
            xmlData = xmlData + "					</CreditCard>  ";
            xmlData = xmlData + "				</PaymentMech>      ";
            xmlData = xmlData + "				<BillTo>    ";
            xmlData = xmlData + "					<Location>   ";
            xmlData = xmlData + "						<TelVoice>" + paymentmodel.Telephone + "</TelVoice>   ";
            xmlData = xmlData + "						<Email>" + paymentmodel.Email + "</Email>   ";
            xmlData = xmlData + "						<Address>            ";
            xmlData = xmlData + "							<Name>" + paymentmodel.FirstName + paymentmodel.LastName + "</Name>     ";
            xmlData = xmlData + "							<Street1>" + paymentmodel.Address + "</Street1>  ";
            xmlData = xmlData + "							<City>" + paymentmodel.City + "</City>          ";
            xmlData = xmlData + "							<StateProv>" + paymentmodel.State + "</StateProv>         ";
            xmlData = xmlData + "							<PostalCode>" + paymentmodel.Zip + "</PostalCode>    ";
            xmlData = xmlData + "                           <Country>" + paymentmodel.Country + "</Country>";
            xmlData = xmlData + "						</Address>             ";
            xmlData = xmlData + "					</Location>      ";
            xmlData = xmlData + "				</BillTo>    ";
            xmlData = xmlData + "			</Consumer>     ";
            xmlData = xmlData + "			<Transaction>     ";
            if (Settings.Instance.GetMasterInfo3().PG_PreAuthFirst == 10)
            {
                xmlData = xmlData + "				<Type>PreAuth</Type>  ";
            }
            else
            {
                xmlData = xmlData + "               <Type>Auth</Type>";
            }
            xmlData = xmlData + "				<PoNumber>eCommerce-Visa-001</PoNumber>  ";
            xmlData = xmlData + "				<TaxExempt DataType=\"S32\">1</TaxExempt>";
            xmlData = xmlData + "				<InvNumber>" + paymentmodel.PaymentNumber + "</InvNumber>     ";
            xmlData = xmlData + "				<CurrentTotals>   ";
            xmlData = xmlData + "					<Totals>           ";
            xmlData = xmlData + "						<Total DataType=\"Money\" Currency=\"840\">" + decimal.Round(intAmount, 0) + "</Total>    ";
            xmlData = xmlData + "					</Totals>   ";
            xmlData = xmlData + "				</CurrentTotals>   ";
            xmlData = xmlData + "				<TerminalInputCapability DataType=\"S32\" Locale=\"840\">1</TerminalInputCapability>  ";
            xmlData = xmlData + "				<InputEnvironment DataType=\"S32\" Locale=\"840\">1</InputEnvironment>   ";
            xmlData = xmlData + "				<CardholderPresentCode DataType=\"S32\" Locale=\"840\">7</CardholderPresentCode>  ";
            xmlData = xmlData + "				<ChargeDesc1>Visa Card Not Present 001</ChargeDesc1>   ";
            xmlData = xmlData + "				<BuyerCode>VBuyerCode01</BuyerCode>     ";
            xmlData = xmlData + "			</Transaction>  ";
            xmlData = xmlData + "		</OrderFormDoc>";
            xmlData = xmlData + "	</EngineDoc>";
            xmlData = xmlData + "</EngineDocList>";

            //capture after posted
            if (WebConfiguration.LogAuthorizeNetTransaction)
            {
                Gsmu.Api.Data.School.Student.EnrollmentFunction enrollment = new Data.School.Student.EnrollmentFunction();
                enrollment.LogAuthorizeNetTransaction(paymentmodel, Amount, xmlData, "", "PayGateCreditCardPayment.cs-");
            }
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            string result = "";
            HttpWebRequest objHttpWebRequest;
            string URI = BBPaygateServer;
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            HttpWebResponse objHttpWebResponse = null;
            Stream objRequestStream = null;
            Stream objResponseStream = null;
            XmlTextReader objXMLReader;
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(xmlData);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentLength = bytes.Length;
            objHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";
            objRequestStream = objHttpWebRequest.GetRequestStream();
            objRequestStream.Write(bytes, 0, bytes.Length);
            objRequestStream.Close();


            objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
            if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                objResponseStream = objHttpWebResponse.GetResponseStream();
                objXMLReader = new XmlTextReader(objResponseStream);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(objXMLReader);

                XmlNodeList listNodes = xmldoc.GetElementsByTagName("Text");
                foreach (XmlNode node in listNodes)
                {

                    result = node.InnerText;
                }
                XmlNodeList listNodesforOId = xmldoc.GetElementsByTagName("OrderId");
                foreach (XmlNode node in listNodesforOId)
                {

                    result = result + "-" + node.InnerText;
                }

                objXMLReader.Close();
            }

            return result;
        }
        public string ProcessChasePaymentOrbital(CreditCardPaymentModel paymentmodel, string Amount)
        {
            string result = "";


            // Declare a response
            Paymentech.Response response;
            // Create an authorize transaction
            Paymentech.Transaction transaction = new Paymentech.Transaction(Paymentech.RequestType.NEW_ORDER_TRANSACTION);
            transaction["OrbitalConnectionUsername"] = CreditCardPaymentHelper.ChasePaymentUserName;
            transaction["OrbitalConnectionPassword"] = CreditCardPaymentHelper.ChasePaymentPassword;
            transaction["IndustryType"] = "EC";
            transaction["MessageType"] = "AC";
            transaction["MerchantID"] = CreditCardPaymentHelper.ChasePaymentMerchantId;
            transaction["BIN"] = "000001";
            transaction["AccountNum"] = paymentmodel.CardNumber;
            transaction["OrderID"] = paymentmodel.OrderNumber;
            transaction["Amount"] = "0";
            transaction["Exp"] = paymentmodel.ExpiryMonth + paymentmodel.ExpiryYear;
            transaction["AVSname"] = paymentmodel.FirstName + " " + paymentmodel.LastName;
            transaction["AVSaddress1"] = paymentmodel.Address;
            transaction["AVScity"] = paymentmodel.City;
            transaction["AVSstate"] = paymentmodel.State;
            transaction["AVSzip"] = paymentmodel.Zip;
            transaction["AVScountryCode"] = "US";
            transaction["CardSecVal"] = paymentmodel.CCV;
            transaction["CardSecValInd"] = "1";
            transaction["CustomerProfileFromOrderInd"] = "A";
            transaction["CustomerRefNum"] = "8994";
            transaction["CustomerProfileOrderOverrideInd"] = "NO";
            transaction["Comments"] = "This is a .Net SDK";
            transaction["testmode"] = "Test Mode";
            response = transaction.Process();



            string status = response["ProcStatus"];
            if (status == null)
                return "Exception while processing the request" + response.XML;
            if (Convert.ToInt32(status) > 0)
            {
                status = "Error processing the request - Check the data try again" + response.XML;
                return status;
            }
            status = "Approved.-" + response["CustomerRefNum"];
            // return "request;"+transaction.XML+"<br />result:" +response["ProcStatus"] +"response:"+ response.XML;
            return status;




            // Populate the required fields for the given transaction type. You can use’
            // the Paymentech Transaction Appendix to help you populate the transaction’

            //Comment to give way to API
            /*
            string result = "";
            string url="";
            string xmlrequest="";
            xmlrequest = "<?xml version=\"1.0\" encoding=\"UTF8\"?>";
			xmlrequest = xmlrequest + "<Request>";
			xmlrequest = xmlrequest + "<NewOrder>";
			xmlrequest = xmlrequest + "<OrbitalConnectionUsername>"+CreditCardPaymentHelper.ChasePaymentUserName+"</OrbitalConnectionUsername>";
			xmlrequest = xmlrequest + "<OrbitalConnectionPassword>"+CreditCardPaymentHelper.ChasePaymentPassword+"</OrbitalConnectionPassword>";
			xmlrequest = xmlrequest + "<IndustryType>EC</IndustryType>";
			xmlrequest = xmlrequest + "<MessageType>AC</MessageType>";
			xmlrequest = xmlrequest + "<BIN>000001</BIN>";
            xmlrequest = xmlrequest + "<MerchantID>" + CreditCardPaymentHelper.ChasePaymentMerchantId + "</MerchantID>";
			xmlrequest = xmlrequest + "<TerminalID>001</TerminalID>";
			xmlrequest = xmlrequest + "<CardBrand></CardBrand>";
			xmlrequest = xmlrequest + "<AccountNum>"+paymentmodel.CardNumber+"</AccountNum>";
			xmlrequest = xmlrequest + "<Exp>"+paymentmodel.ExpiryMonth+paymentmodel.ExpiryYear+"</Exp>";
			xmlrequest = xmlrequest + "<CurrencyCode>840</CurrencyCode>";
			xmlrequest = xmlrequest + "<CurrencyExponent>2</CurrencyExponent>";
			xmlrequest = xmlrequest + "<AVSzip>"+paymentmodel.Zip+"</AVSzip>";
			xmlrequest = xmlrequest + "<AVSaddress1>"+paymentmodel.Address+"</AVSaddress1>";
			xmlrequest = xmlrequest + "<AVSaddress2>"+paymentmodel.Address2+"</AVSaddress2>";
			xmlrequest = xmlrequest + "<AVScity>"+paymentmodel.City+"</AVScity>";
			xmlrequest = xmlrequest + "<AVSstate>"+paymentmodel.State+"</AVSstate>";
			xmlrequest = xmlrequest + "<AVSphoneNum>"+paymentmodel.Telephone+"</AVSphoneNum>";
			xmlrequest = xmlrequest + "<OrderID>"+paymentmodel.OrderNumber+"</OrderID>";
			xmlrequest = xmlrequest + "<Amount>"+Amount+"</Amount>";
			xmlrequest = xmlrequest + "</NewOrder>";
			xmlrequest = xmlrequest + "</Request>";
			if (CreditCardPaymentHelper.ChasePaymentServer=="0"){
				url = "https://orbitalvar2.paymentech.net/authorize";
            }
			else
            {
                url = "https://orbital1.paymentech.net/authorize";
             }
            HttpWebRequest objHttpWebRequest;
            string URI = url;
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            HttpWebResponse objHttpWebResponse = null;
            Stream objRequestStream = null;
            Stream objResponseStream = null;
            XmlTextReader objXMLReader;
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(xmlrequest);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentLength = bytes.Length;
            objHttpWebRequest.ContentType = "application/PTI58; encoding='text'";
            
            
            objRequestStream = objHttpWebRequest.GetRequestStream();
            objRequestStream.Write(bytes, 0, bytes.Length);
            objRequestStream.Close();
            try
            {
                objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
                if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    objResponseStream = objHttpWebResponse.GetResponseStream();
                    objXMLReader = new XmlTextReader(objResponseStream);
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(objXMLReader);

                    XmlNodeList listNodes = xmldoc.GetElementsByTagName("Text");
                    foreach (XmlNode node in listNodes)
                    {

                        result = node.InnerText;
                    }
                    XmlNodeList listNodesforOId = xmldoc.GetElementsByTagName("OrderId");
                    foreach (XmlNode node in listNodesforOId)
                    {

                        result = result + "-" + node.InnerText;
                    }

                    objXMLReader.Close();
                }
            }

            catch (Exception exception)
            {
                result = exception.Message + " <br /> Inner Exception:" + exception.InnerException;
            }
             * */
            return result;
        }

        public XmlDocument ProcessPaygov(CreditCardPaymentModel paymentmodel, string Amount)
        {
            XmlDocument xmldoc = new XmlDocument();
            decimal InvcourseTotal = 0;
            string GVMerchantInfo = Settings.Instance.GetMasterInfo3().PayGovMerchntID;
            string t_STATECD = "CA"; // default for GSMU.
            string t_MERCHANTID = Settings.Instance.GetMasterInfo3().PayGovMerchntID;

            if (GVMerchantInfo.IndexOf("@") != -1)
            {
                string[] GVMI = GVMerchantInfo.Split('@');
                t_STATECD = GVMI[1];
                t_MERCHANTID = GVMI[0];
            }
            string t_SERVICECODE = Settings.Instance.GetMasterInfo3().PayGovServiceCode;
            string t_MERCHANTKEY = Settings.Instance.GetMasterInfo3().PayGovMerchntKey;
            int? PayGovServer = Settings.Instance.GetMasterInfo3().PayGovServer;
            string t_DESCRIPTION = "Billing for GSMU";

            //string Common_Checkout_Web_URL;
            //string Web_service_WSDL_URL;
            string Web_service_ENDPOINT_URL;
            string t_CID = paymentmodel.OrderNumber;
            Random random = new Random();
            string t_LOCALREFID = "LRef" + t_CID + "-" + random.Next();
            string t_UNIQUETRANSID = t_CID + "-" + random.Next();
            if (PayGovServer == 1)
            {
                //Common_Checkout_Web_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/Commoncheckpage/Default.aspx";
                //Web_service_WSDL_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.wsdl";
                Web_service_ENDPOINT_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.svc";
            }
            else
            {
                //Common_Checkout_Web_URL = "https://securecheckout.cdc.nicusa.com/CommonCheckPage/Default.aspx";
                //Web_service_WSDL_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.wsdl";
                Web_service_ENDPOINT_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.svc";
            }

            string Common_Checkout_com_URL = "https://common.checkout.cdc.nicusa.com";
            string Common_Checkout_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/PreparePayment";
            //string Common_QPayment_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/QueryPayment";

            string t_HREFSUCCESS = paymentmodel.CurrentUrl[0] + "Public/Cart/PayGovConfirmation?OrderTotal=" + Amount + "&PaymentType=CC&OrderNo=" + paymentmodel.OrderNumber;
            string t_HREFFAILURE = paymentmodel.CurrentUrl[0];
            string t_HREFDUPLICATE = paymentmodel.CurrentUrl[0];
            string t_HREFCANCEL = paymentmodel.CurrentUrl[0];
            string t_NAME = paymentmodel.FirstName + " " + paymentmodel.LastName;
            string sMsg = "<soapenv:Envelope ";
            sMsg = sMsg + "xmlns:soapenv= \"http://schemas.xmlsoap.org/soap/envelope/\" ";
            sMsg = sMsg + " xmlns:com=\"" + Common_Checkout_com_URL + "\"";
            sMsg = sMsg + " xmlns:com1=\"http://schemas.datacontract.org/2004/07/Common.Payment.Common\">";
            sMsg = sMsg + "<soapenv:Header/>";
            sMsg = sMsg + "<soapenv:Body>";
            sMsg = sMsg + "<com:PreparePayment>";
            sMsg = sMsg + "<com:request>";


            sMsg = sMsg + "<com1:STATECD>" + t_STATECD + "</com1:STATECD>";
            sMsg = sMsg + "<com1:AMOUNT>" + String.Format("{0:0.00}", decimal.Parse(Amount)) + "</com1:AMOUNT>";
            sMsg = sMsg + "<com1:CID>" + t_CID + "</com1:CID>";
            sMsg = sMsg + "<com1:SERVICECODE>" + t_SERVICECODE + "</com1:SERVICECODE>";
            sMsg = sMsg + "<com1:UNIQUETRANSID>" + t_UNIQUETRANSID + "</com1:UNIQUETRANSID>";
            sMsg = sMsg + "<com1:DESCRIPTION>" + t_DESCRIPTION + "</com1:DESCRIPTION>";
            sMsg = sMsg + "<com1:SERVICENAME>fgsdfgkjfdngk fgdsff</com1:SERVICENAME>";

            sMsg = sMsg + "<com1:LOCALREFID>" + t_LOCALREFID + "</com1:LOCALREFID>";
            sMsg = sMsg + "<com1:MERCHANTID>" + t_MERCHANTID + "</com1:MERCHANTID>";
            sMsg = sMsg + "<com1:MERCHANTKEY>" + t_MERCHANTKEY + "</com1:MERCHANTKEY>";
            sMsg = sMsg + "<com1:NAME>" + t_NAME + "</com1:NAME>";


            sMsg = sMsg + "<com1:HREFSUCCESS>" + t_HREFSUCCESS + "</com1:HREFSUCCESS>";
            sMsg = sMsg + "<com1:HREFFAILURE>" + t_HREFFAILURE + "</com1:HREFFAILURE>";
            sMsg = sMsg + "<com1:HREFDUPLICATE>" + t_HREFDUPLICATE + "</com1:HREFDUPLICATE>";
            sMsg = sMsg + "<com1:HREFCANCEL>" + t_HREFCANCEL + "</com1:HREFCANCEL>";

            sMsg = sMsg + "<com1:LINEITEMS>";
            int loop = 0;
            var roster = new Gsmu.Api.Data.School.CourseRoster.OrderModel(paymentmodel.OrderNumber);
            foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
            {
                loop = loop + 1;
                sMsg = sMsg + "<com1:LINEITEM>";
                sMsg = sMsg + "<com1:ITEM_ID>" + loop + "</com1:ITEM_ID>";
                if ((Settings.Instance.GetMasterInfo3().PayGovSkuField != null) && (Settings.Instance.GetMasterInfo3().PayGovSkuField != ""))
                {
                    sMsg = sMsg + "<com1:SKU>" + item.Course.GetType().GetProperty(Settings.Instance.GetMasterInfo3().PayGovSkuField).GetValue(item.Course, null).ToString() + "</com1:SKU>";
                }
                else
                {
                    sMsg = sMsg + "<com1:SKU>000000002</com1:SKU>";
                }
                decimal coursediscount = 0;
                decimal materialtotal = 0;
                foreach (Course_Roster rosteritem in roster.CourseRosters)
                {
                    if (item.Course.COURSEID == rosteritem.COURSEID)
                    {
                        if (rosteritem.SingleRosterDiscountAmount != null)
                        {
                            coursediscount = decimal.Parse(rosteritem.SingleRosterDiscountAmount.ToString());
                        }

                        // if (rosteritem.Materials != null)
                        //{
                        // foreach (var materialitems in rosteritem.Materials)
                        // {
                        // if (materialitems.price != null)
                        // {
                        //  materialtotal = materialtotal + decimal.Parse(item.MateriaTotal.ToString());
                        //}

                        // }
                        // }
                    }
                }
                materialtotal = materialtotal + decimal.Parse(item.MateriaTotal.ToString());

                InvcourseTotal = (item.CourseTotal + materialtotal) - coursediscount;
                //rderAmount = OrderAmount+(item.CourseTotal + item.MateriaTotal) - decimal.Parse(discount.ToString());
                sMsg = sMsg + "<com1:DESCRIPTION>" + item.Course.COURSENAME + "</com1:DESCRIPTION>";
                sMsg = sMsg + "<com1:UNIT_PRICE>" + String.Format("{0:0.00}", InvcourseTotal) + "</com1:UNIT_PRICE>";
                sMsg = sMsg + "<com1:QUANTITY>1</com1:QUANTITY>";
                sMsg = sMsg + "</com1:LINEITEM>";
                InvcourseTotal = 0;
            }
            sMsg = sMsg + "</com1:LINEITEMS>";

            sMsg = sMsg + "</com:request>";
            sMsg = sMsg + "</com:PreparePayment>";
            sMsg = sMsg + "</soapenv:Body>";
            sMsg = sMsg + "</soapenv:Envelope>";

            sMsg = sMsg.Replace('\'', '\"');
            sMsg = sMsg.Replace("&", "&amp;");
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            HttpWebRequest objHttpWebRequest;
            string URI = Web_service_ENDPOINT_URL;
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            HttpWebResponse objHttpWebResponse = null;
            Stream objRequestStream = null;
            Stream objResponseStream = null;
            XmlTextReader objXMLReader;
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(sMsg);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentLength = bytes.Length;
            objHttpWebRequest.ContentType = "text/xml";
            objHttpWebRequest.Headers.Add("SOAPAction", Common_Checkout_SOAPAction);
            objRequestStream = objHttpWebRequest.GetRequestStream();
            objRequestStream.Write(bytes, 0, bytes.Length);
            objRequestStream.Close();

            string result = "";
            objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
            if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                objResponseStream = objHttpWebResponse.GetResponseStream();
                objXMLReader = new XmlTextReader(objResponseStream);
                xmldoc.Load(objXMLReader);
                result = xmldoc.InnerText;
                objXMLReader.Close();
            }

            using (StreamWriter _testData = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest.txt"), true))
            {
                _testData.WriteLine(Environment.NewLine + "----------------------" + DateTime.Now + "--------------------------" + sMsg + " ::::Token response:::: " + xmldoc.InnerText); // Write the file.
            }
            return xmldoc;
        }


        public XmlDocument ProcessPaygovTEST(CreditCardPaymentModel paymentmodel, string Amount)
        {
            XmlDocument xmldoc = new XmlDocument();
            decimal InvcourseTotal = 0;
            string GVMerchantInfo = Settings.Instance.GetMasterInfo3().PayGovMerchntID;
            string t_STATECD = "CA"; // default for GSMU.
            string t_MERCHANTID = Settings.Instance.GetMasterInfo3().PayGovMerchntID;

            if (GVMerchantInfo.IndexOf("@") != -1)
            {
                string[] GVMI = GVMerchantInfo.Split('@');
                t_STATECD = GVMI[1];
                t_MERCHANTID = GVMI[0];
            }
            string t_SERVICECODE = Settings.Instance.GetMasterInfo3().PayGovServiceCode;
            string t_MERCHANTKEY = Settings.Instance.GetMasterInfo3().PayGovMerchntKey;
            int? PayGovServer = Settings.Instance.GetMasterInfo3().PayGovServer;
            string t_DESCRIPTION = "Billing for GSMU";

            //string Common_Checkout_Web_URL;
            //string Web_service_WSDL_URL;
            string Web_service_ENDPOINT_URL;
            string t_CID = paymentmodel.OrderNumber;
            Random random = new Random();
            string t_LOCALREFID = "LRef" + t_CID + "-" + random.Next();
            string t_UNIQUETRANSID = t_CID + "-" + random.Next();
            if (PayGovServer == 1)
            {
                //Common_Checkout_Web_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/Commoncheckpage/Default.aspx";
                //Web_service_WSDL_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.wsdl";
                Web_service_ENDPOINT_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.svc";
            }
            else
            {
                //Common_Checkout_Web_URL = "https://securecheckout.cdc.nicusa.com/CommonCheckPage/Default.aspx";
                //Web_service_WSDL_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.wsdl";
                Web_service_ENDPOINT_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.svc";
            }

            string Common_Checkout_com_URL = "https://common.checkout.cdc.nicusa.com";
            string Common_Checkout_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/PreparePayment";
            //string Common_QPayment_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/QueryPayment";

            string t_HREFSUCCESS = paymentmodel.CurrentUrl[0] + "Public/Cart/PayGovConfirmation?OrderTotal=" + Amount + "&PaymentType=CC&OrderNo=" + paymentmodel.OrderNumber;
            string t_HREFFAILURE = paymentmodel.CurrentUrl[0];
            string t_HREFDUPLICATE = paymentmodel.CurrentUrl[0];
            string t_HREFCANCEL = paymentmodel.CurrentUrl[0];
            string t_NAME = paymentmodel.FirstName + " " + paymentmodel.LastName;
            string sMsg = "<soapenv:Envelope ";
            sMsg = sMsg + "xmlns:soapenv= \"http://schemas.xmlsoap.org/soap/envelope/\" ";
            sMsg = sMsg + " xmlns:com=\"" + Common_Checkout_com_URL + "\"";
            sMsg = sMsg + " xmlns:com1=\"http://schemas.datacontract.org/2004/07/Common.Payment.Common\">";
            sMsg = sMsg + "<soapenv:Header/>";
            sMsg = sMsg + "<soapenv:Body>";
            sMsg = sMsg + "<com:PreparePayment>";
            sMsg = sMsg + "<com:request>";


            sMsg = sMsg + "<com1:STATECD>" + t_STATECD + "</com1:STATECD>";
            sMsg = sMsg + "<com1:AMOUNT>" + String.Format("{0:0.00}", decimal.Parse(Amount)) + "</com1:AMOUNT>";
            sMsg = sMsg + "<com1:CID>" + t_CID + "</com1:CID>";
            sMsg = sMsg + "<com1:SERVICECODE>" + t_SERVICECODE + "</com1:SERVICECODE>";
            sMsg = sMsg + "<com1:UNIQUETRANSID>" + t_UNIQUETRANSID + "</com1:UNIQUETRANSID>";
            sMsg = sMsg + "<com1:DESCRIPTION>" + t_DESCRIPTION + "</com1:DESCRIPTION>";
            sMsg = sMsg + "<com1:SERVICENAME>fgsdfgkjfdngk fgdsff</com1:SERVICENAME>";

            sMsg = sMsg + "<com1:LOCALREFID>" + t_LOCALREFID + "</com1:LOCALREFID>";
            sMsg = sMsg + "<com1:MERCHANTID>" + t_MERCHANTID + "</com1:MERCHANTID>";
            sMsg = sMsg + "<com1:MERCHANTKEY>" + t_MERCHANTKEY + "</com1:MERCHANTKEY>";
            sMsg = sMsg + "<com1:NAME>" + t_NAME + "</com1:NAME>";


            sMsg = sMsg + "<com1:HREFSUCCESS>" + t_HREFSUCCESS + "</com1:HREFSUCCESS>";
            sMsg = sMsg + "<com1:HREFFAILURE>" + t_HREFFAILURE + "</com1:HREFFAILURE>";
            sMsg = sMsg + "<com1:HREFDUPLICATE>" + t_HREFDUPLICATE + "</com1:HREFDUPLICATE>";
            sMsg = sMsg + "<com1:HREFCANCEL>" + t_HREFCANCEL + "</com1:HREFCANCEL>";


            sMsg = sMsg + "</com:request>";
            sMsg = sMsg + "</com:PreparePayment>";
            sMsg = sMsg + "</soapenv:Body>";
            sMsg = sMsg + "</soapenv:Envelope>";

            sMsg = sMsg.Replace('\'', '\"');
            sMsg = sMsg.Replace("&", "&amp;");



            string xl = "";

            xl = xl + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:tcs=\"http://fms.treas.gov/services/tcsonline\">";
            xl = xl + "<soapenv:Header/>";
            xl = xl + "<soapenv:Body>";
            xl = xl + "<tcs:startOnlineCollection>";
            xl = xl + "<startOnlineCollectionRequest>";
            xl = xl + "<tcs_app_id>TCSDOEWAPATC</tcs_app_id>";
            xl = xl + "<agency_tracking_id>1501</agency_tracking_id>";
            xl = xl + "<transaction_type>Sale</transaction_type>";
            xl = xl + "<transaction_amount>25</transaction_amount>";
            xl = xl + "<language>en</language>";
            xl = xl + "<url_success>http://localhost:56149/application/TestPaygovSoap/ok</url_success>";
            xl = xl + "<url_cancel>http://localhost:56149/application/TestPaygovSoap/notok</url_cancel>";
            xl = xl + "</startOnlineCollectionRequest>";
            xl = xl + "</tcs:startOnlineCollection>";
            xl = xl + "</soapenv:Body>";
            xl = xl + "</soapenv:Envelope>";

            sMsg = xl;




            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            HttpWebRequest objHttpWebRequest;
            string URI = Web_service_ENDPOINT_URL;
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            HttpWebResponse objHttpWebResponse = null;
            Stream objRequestStream = null;
            Stream objResponseStream = null;
            XmlTextReader objXMLReader;
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(sMsg);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentLength = bytes.Length;
            objHttpWebRequest.ContentType = "text/xml";
            objHttpWebRequest.Headers.Add("SOAPAction", Common_Checkout_SOAPAction);
            objRequestStream = objHttpWebRequest.GetRequestStream();
            objRequestStream.Write(bytes, 0, bytes.Length);
            objRequestStream.Close();

            string result = "";
            objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
            if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                objResponseStream = objHttpWebResponse.GetResponseStream();
                objXMLReader = new XmlTextReader(objResponseStream);
                xmldoc.Load(objXMLReader);
                result = xmldoc.InnerText;
                objXMLReader.Close();
            }

            using (StreamWriter _testData = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/paygovrequest.txt"), true))
            {
                _testData.WriteLine(Environment.NewLine + "----------------------" + DateTime.Now + "--------------------------" + sMsg + " ::::Token response:::: " + xmldoc.InnerText); // Write the file.
            }
            return xmldoc;
        }


        public string CallSoapWebService()
        {
            var _url = "http://xxxxxxxxx/Service1.asmx";
            var _action = "http://xxxxxxxx/Service1.asmx?op=HelloWorld";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
               return soapResult;
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(
            @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
               xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">
        <SOAP-ENV:Body>
            <HelloWorld xmlns=""http://tempuri.org/"" 
                SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                <int1 xsi:type=""xsd:integer"">12</int1>
                <int2 xsi:type=""xsd:integer"">32</int2>
            </HelloWorld>
        </SOAP-ENV:Body>
    </SOAP-ENV:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }


        public XmlDocument FinalUpdateforPaygovPayment(string token)
        {
            XmlDocument xmldoc = new XmlDocument();
            string Web_service_ENDPOINT_URL = "";
            string Common_Checkout_com_URL = "https://common.checkout.cdc.nicusa.com";
            string Common_QPayment_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/QueryPayment";
            int? PayGovServer = Settings.Instance.GetMasterInfo3().PayGovServer;
            if (PayGovServer == 1)
            {
                Web_service_ENDPOINT_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.svc";
            }
            else
            {
                Web_service_ENDPOINT_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.svc";
            }
            string sMsg = "<soapenv:Envelope ";
            sMsg = sMsg + "xmlns:soapenv=" + "\"http://schemas.xmlsoap.org/soap/envelope/\"" + " ";
            sMsg = sMsg + "xmlns:com=\"" + Common_Checkout_com_URL + "\">";
            sMsg = sMsg + "<soapenv:Body>";
            sMsg = sMsg + "<com:QueryPayment>";
            sMsg = sMsg + "<com:token>" + token + "</com:token>";
            sMsg = sMsg + "</com:QueryPayment>";
            sMsg = sMsg + "</soapenv:Body>";
            sMsg = sMsg + "</soapenv:Envelope> ";
            HttpWebRequest objHttpWebRequest;
            string URI = Web_service_ENDPOINT_URL;
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            HttpWebResponse objHttpWebResponse = null;
            Stream objRequestStream = null;
            Stream objResponseStream = null;
            XmlTextReader objXMLReader;
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(sMsg);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentLength = bytes.Length;
            objHttpWebRequest.ContentType = "text/xml";
            objHttpWebRequest.Headers.Add("SOAPAction", Common_QPayment_SOAPAction);
            objRequestStream = objHttpWebRequest.GetRequestStream();
            objRequestStream.Write(bytes, 0, bytes.Length);
            objRequestStream.Close();

            string result = "";
            objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
            if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                objResponseStream = objHttpWebResponse.GetResponseStream();
                objXMLReader = new XmlTextReader(objResponseStream);
                xmldoc.Load(objXMLReader);
                result = xmldoc.InnerText;
                objXMLReader.Close();
            }
            return xmldoc;
        }


        public string PayflowProV2(CreditCardPaymentModel paymentmodel, string Amount)
        {
            string expirydate = "20" + paymentmodel.ExpiryYear + paymentmodel.ExpiryMonth;
            String Request = "<?xml version=\"1.0\"?><XMLPayRequest Timeout=\"45\" version=\"2.0\"><RequestData><Partner>" + Settings.Instance.GetMasterInfo().PFPartner;
            Request += "</Partner><Vendor>" + Settings.Instance.GetMasterInfo().PFVendor + "</Vendor>";
            Request += "<Transactions><Transaction><Sale><PayData><Invoice><TotalAmt Currency='USD'>" + String.Format("{0:0.00}", decimal.Parse(Amount)) + "</TotalAmt>";
            Request += "<InvNum>" + paymentmodel.OrderNumber + "</InvNum><BillTo><PONum>" + paymentmodel.OrderNumber + "</PONum>";
            Request += "<Address><Street>" + paymentmodel.Address + "</Street><Zip>" + paymentmodel.Zip + "</Zip></Address></BillTo></Invoice>";
            Request += "<Tender><Card><CardNum>" + paymentmodel.CardNumber + "</CardNum><ExpDate>" + expirydate + "</ExpDate></Card></Tender></PayData></Sale>";
            Request += "</Transaction></Transactions></RequestData>";
            Request += "<RequestAuth><UserPass><User>" + Settings.Instance.GetMasterInfo().PFUser + "</User><Password>" + Settings.Instance.GetMasterInfo().PFPWD + "</Password></UserPass></RequestAuth></XMLPayRequest>";
            PayflowNETAPI PayflowNETAPI = new PayflowNETAPI(Settings.Instance.GetMasterInfo().PFHostAddress);
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            String Response = PayflowNETAPI.SubmitTransaction(Request, PayflowUtility.RequestId);
            String TransErrors = PayflowNETAPI.TransactionContext.ToString();
            paymentmodel.AuthNum = "";
            paymentmodel.RefNumber = "";

            string Result = "";
            if (TransErrors != null && TransErrors.Length > 0)
            {
                Result = TransErrors;
            }
            else
            {
                Result = PayflowUtility.GetStatus(Response);

                if (Result.Contains("Successful"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(Response);

                    Result = "Approved";
                    XmlNodeList MessageResp1 = xmlDoc.GetElementsByTagName("AuthCode");
                    XmlNodeList MessageResp2 = xmlDoc.GetElementsByTagName("PNRef");
                    paymentmodel.AuthNum = MessageResp1[0].InnerText;
                    paymentmodel.RefNumber = MessageResp2[0].InnerText;
                    paymentmodel.PaymentNumber = MessageResp1[0].InnerText;
                    paymentmodel.RespMsg = "Approved";
                    paymentmodel.Result = "0";
                }
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(Response);

                    XmlNodeList MessageResp = xmlDoc.GetElementsByTagName("Message");
                    Result = "* " + MessageResp[0].InnerText + " - Transaction Failed. Please Try again";
                }
            }
            return Result;

        }

        public string RefundPaypal(int courseid, int studentid, string ordernumber, int rosterid, double refundamount = 0)
        {
            string amt = "0";
            decimal creditapplied = 0;
            string paymentnumber = "";
            using (var db = new SchoolEntities())
            {
                if (refundamount > 0)
                {
                    amt = refundamount.ToString();
                }
                if (rosterid != 0 && !string.IsNullOrEmpty(ordernumber))
                {
                    var roster = (from cr in db.Course_Rosters
                                  where cr.RosterID == rosterid && cr.OrderNumber == ordernumber
                                  select cr).First();
                    paymentnumber = roster.RefNumber;
                    if (refundamount == 0)
                    {
                        amt = roster.CourseCost;
                    }
                    if ((roster.CreditApplied != "") && (roster.CreditApplied != null))
                    {
                        creditapplied = Decimal.Parse(roster.CreditApplied);
                    }
                }
                else if (courseid != 0 && studentid != 0)
                {
                    var roster = (from cr in db.Course_Rosters
                           where cr.OrderNumber == ordernumber && cr.COURSEID == courseid && cr.STUDENTID == studentid
                           select cr).First();
                    paymentnumber = roster.RefNumber;
                    if (refundamount == 0)
                    {
                        amt = roster.CourseCost;
                    }
                    if ((roster.CreditApplied != "") && (roster.CreditApplied != null))
                    {
                        creditapplied = Decimal.Parse(roster.CreditApplied);
                    }
                }
                //if paymentnumber is still empty -- get the roster by OrderNumber
                if (string.IsNullOrEmpty(paymentnumber))
                {
                    var roster = db.Course_Rosters.Where(r => r.OrderNumber == ordernumber).FirstOrDefault();
                    if (roster != null)
                    {
                        paymentnumber = roster.RefNumber;
                        creditapplied = Decimal.Parse(roster.CreditApplied);
                    }
                }
            }
            try
            {
                amt = String.Format("{0:0.00}", decimal.Parse(amt));
                //coupon discount and singleroster discount
            }
            catch
            {
                amt = "0";
            }
            string partner = Settings.Instance.GetMasterInfo().PFPartner.ToString();
            string vendor = Settings.Instance.GetMasterInfo().PFVendor.ToString();
            string login = Settings.Instance.GetMasterInfo().PFUser.ToString();
            string password = Settings.Instance.GetMasterInfo().PFPWD.ToString();
            int usepaypal_payflow = Convert.ToInt32(Settings.Instance.GetMasterInfo().UsePayflow.ToString().ToLower());
            int PayPalOperateMode = -1;
            string gen_token = "MySecTokenID-" + ordernumber; //genId();
            string PaypalHostAddress = Settings.Instance.GetMasterInfo().PFHostAddress.ToString();
            if (PaypalHostAddress.ToLower().IndexOf("pilot") == -1)
            {
                PayPalOperateMode = 1;
            }



            string suffix = gen_token.Split('-')[1].ToString();
            NameValueCollection requestArray = new NameValueCollection()
                {
                    {"PARTNER", partner },
                    {"VENDOR", vendor },
                    {"USER", login },
                    {"PWD", password },
                    {"TRXTYPE", "C"},
                    {"TENDER", "C"},
                    {"BUTTONSOURCE","GoSignMeUp_SP"},
                    {"AMT", amt},
                    {"ORIGID",paymentnumber},
                    {"VERBOSITY", "HIGH"},

                };
            if (amt != "0")
            {
                NameValueCollection resp = run_payflow_call(requestArray, PayPalOperateMode);
                var refundStatusPP = resp["RESPMSG"];
                if (refundStatusPP.ToLower() == "approved")
                {
                    var Context = new SchoolEntities();
                    Course_Roster roster = Context.Course_Rosters.First(r => r.RosterID == rosterid);
                    roster.CreditApplied = (Decimal.Parse(amt) + (creditapplied)).ToString();

                    OrderTransaction ordertransaction = new OrderTransaction();
                    ordertransaction.OrderTransactionAmount = Decimal.Parse(amt);
                    ordertransaction.OrderTransactionAuthNum = resp["PNREF"];
                    ordertransaction.OrderTransactionNotes = "Admin. Online Refund/Credit";
                    ordertransaction.RosterId = rosterid;
                    ordertransaction.OrderTransactionType = 1;
                    ordertransaction.OrderTransactionPaymentType = "Paypal";
                    ordertransaction.OrderTransactionDateTime = DateTime.Now;
                    Context.OrderTransactions.Add(ordertransaction);

                    Context.SaveChanges();
                    EmailFunction EmailFunction = new EmailFunction();
                    EmailFunction.SendAutomaticCreditToCardEmail(rosterid, amt);
                    return refundStatusPP;
                }
                else
                {
                    return refundStatusPP;
                }
            }
            return string.Empty;
        }

        protected NameValueCollection run_payflow_call(NameValueCollection requestArray, int mode)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
            if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            String nvpstring = "";
            foreach (string key in requestArray)
            {
                //format:  "PARAMETERNAME[lengthofvalue]=VALUE&".  Never URL encode.
                var val = requestArray[key];
                if (val == null)
                {
                    val = string.Empty;
                }
                nvpstring += key + "[" + val.Length + "]=" + val + "&";
            }
            nvpstring = nvpstring.TrimEnd('&');

            string urlEndpoint = string.Empty;
            //if (environment == "pilot" || environment == "test" || environment == "sandbox")
            if (mode == -1)
            {
                urlEndpoint = "https://pilot-payflowpro.paypal.com/";
            }
            else if (mode == 1)
            {
                urlEndpoint = "https://payflowpro.paypal.com";
            }
            else if (mode == 0) //depends on the current environment or host //if staging, live or local
            {
                string host_environment = System.Web.HttpContext.Current.Request.Url.Host;
                if (host_environment.ToLower() == "localhost" || host_environment.ToLower() == "server")
                {
                    urlEndpoint = "https://pilot-payflowpro.paypal.com/";
                }
            }

            //send request to Payflow
            HttpWebRequest payReq = (HttpWebRequest)WebRequest.Create(urlEndpoint);
            payReq.Method = "POST";
            payReq.ContentLength = nvpstring.Length;
            payReq.ContentType = "application/x-www-form-urlencoded";

            StreamWriter sw = new StreamWriter(payReq.GetRequestStream());
            sw.Write(nvpstring);
            sw.Close();

            //get Payflow response
            HttpWebResponse payResp = (HttpWebResponse)payReq.GetResponse();
            StreamReader sr = new StreamReader(payResp.GetResponseStream());
            string response = sr.ReadToEnd();
            sr.Close();

            //parse string into array and return
            NameValueCollection dict = new NameValueCollection();
            foreach (string nvp in response.Split('&'))
            {
                string[] keys = nvp.Split('=');
                dict.Add(keys[0], keys[1]);
            }
            return dict;
        }

        public string ProcessRevtrak(string paymentId, string Amount)
        {
            var client = new HttpClient();
            var ConnexPointUrl = "https://pay.connexpoint.com/v1/payments/" + paymentId + "/execute";
            var request = new HttpRequestMessage(HttpMethod.Post, ConnexPointUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreditCardPaymentHelper.RevTrakSecretKey);
            request.Headers.Add("PCCT", CreditCardPaymentHelper.RevTrakPCCT);
            request.Content = new StringContent("amount="+Amount);
            var response = client.SendAsync(request).Result;
            return response.ToString();
        }

        public void AuthorizePayGovPaymentThruService()
        {

    //TCSOnlineService.wsdl Code -
    Gsmu.Api.TCSOnlineService.StartOnlineCollectionRequest PaygovReqField = new TCSOnlineService.StartOnlineCollectionRequest();
            Gsmu.Api.TCSOnlineService.TCSOnlineServiceClient PaygovReq = new TCSOnlineService.TCSOnlineServiceClient();
            using (TCSOnlineService.TCSOnlineServiceClient client = new TCSOnlineService.TCSOnlineServiceClient())
            {
                X509Certificate2 cert = new X509Certificate2(System.Web.HttpContext.Current.Server.MapPath("~/Certificate/paygov.pfx"), "masters3", X509KeyStorageFlags.Exportable);
               client.ClientCredentials.ClientCertificate.Certificate = cert;



                X509Certificate2Collection certcoll = new X509Certificate2Collection();
                certcoll.Add(cert);
                X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Remove(cert);
                store.Add(cert);
                client.ClientCredentials.ClientCertificate.SetCertificate(
                        StoreLocation.LocalMachine,
                        StoreName.TrustedPublisher,
                        X509FindType.FindBySubjectName,
                        "QADOEWAPATCERT0");


                client.ClientCredentials.SupportInteractive = false;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

                string useNewEncryptionProtocol = System.Configuration.ConfigurationManager.AppSettings["UseNewEncryptionProtocol"];
                //if (!string.IsNullOrEmpty(useNewEncryptionProtocol) && bool.Parse(useNewEncryptionProtocol) == true)
                //{
                //    ServicePointManager.Expect100Continue = true;
                //     ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //  //  System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                //}
                //else
                //{
                //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                //}


                PaygovReqField.tcs_app_id = "TCSDOEWAPATC";
                PaygovReqField.agency_tracking_id = "1501";
                PaygovReqField.transaction_type = TCSOnlineService.TransactionType.Sale;
                PaygovReqField.transaction_amount = decimal.Parse("90");
                PaygovReqField.url_cancel = "https://dev252.gosignmeup.com/";
                PaygovReqField.url_success = "https://dev252.gosignmeup.com/";
                PaygovReqField.language = "en";
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Open();


                client.startOnlineCollection(PaygovReqField);
 

            }



              //  PaygovReq.startOnlineCollection(PaygovReqField);



        }
        public void AuthorizePaygovPaymentSingleService()
        {

            PCSaleRequest _saleRequest = new PCSaleRequest();

            PCSaleRequestType _salePCRequest = new PCSaleRequestType();

            custom_fields _saleCustom = new custom_fields();

            PCSaleResponse _saleResponse = new PCSaleResponse();

            PCSaleResponseType _salePCResponse = new PCSaleResponseType();
        }

    }
}
