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

namespace Gsmu.Api.Sales.ShoppingCart
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

        public IEnumerable<Payment_Option> GetAllPaymentTypes()
        {
            using (var db = new SchoolEntities())
            {
                IEnumerable<Payment_Option> item = (from m in db.Payment_Options where m.VisibleTo != 3 & m.VisibleTo != 2 & m.PaymentClass == 1 orderby m.PAYMENTTYPE descending select m).ToList();
                return item.Concat(AddDefaultValue());
            }
        }
        public Payment_Option GetPaymentTypeInfo(string paymenttype)
        {
            using (var db = new SchoolEntities())
            {
               Payment_Option item = (from m in db.Payment_Options where  m.PAYMENTTYPE==paymenttype select m).FirstOrDefault();
               return item;
            }
        }


        public static IEnumerable<Payment_Option> AddDefaultValue()
        {
            return new[] {
                new Payment_Option() { PAYMENTTYPE="Select Payment Type" },
            };
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

            post_values.Add("x_first_name", paymentmodel.FirstName);
            post_values.Add("x_last_name", paymentmodel.LastName);
            post_values.Add("x_address", paymentmodel.Address);
            post_values.Add("x_state", paymentmodel.State);
            post_values.Add("x_zip", paymentmodel.Zip);
            String post_string = "";

            foreach (KeyValuePair<string, string> post_value in post_values)
            {
                post_string += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
            }
            post_string = post_string.TrimEnd('&');
            // create an HttpWebRequest object to communicate with Authorize.net
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
            return paymentstatus;
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
            xmlData = xmlData + "						</Address>             ";
            xmlData = xmlData + "					</Location>      ";
            xmlData = xmlData + "				</BillTo>    ";
            xmlData = xmlData + "			</Consumer>     ";
            xmlData = xmlData + "			<Transaction>     ";
            xmlData = xmlData + "				<Type>PreAuth</Type>  ";
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

                    result =result+"-"+ node.InnerText;
                }
                
                objXMLReader.Close();
            }

            return result;
        }

        public XmlDocument ProcessPaygov(CreditCardPaymentModel paymentmodel, string Amount)
        {
            XmlDocument xmldoc = new XmlDocument();
            decimal InvcourseTotal = 0;
            decimal OrderAmount = 0;
            string t_STATECD = "MS";
            string t_MERCHANTID = Settings.Instance.GetMasterInfo3().PayGovMerchntID;
            string t_SERVICECODE = Settings.Instance.GetMasterInfo3().PayGovServiceCode;
            string t_MERCHANTKEY = Settings.Instance.GetMasterInfo3().PayGovMerchntKey;
            int? PayGovServer = Settings.Instance.GetMasterInfo3().PayGovServer;
            string t_DESCRIPTION = "Billing for GSMU";

            string Common_Checkout_Web_URL = "";
            string Web_service_WSDL_URL = "";
            string Web_service_ENDPOINT_URL = "";

            string t_CID = paymentmodel.OrderNumber;
            Random random = new Random();
            string t_LOCALREFID = "LRef" + t_CID + "-" + random.Next();
            string t_UNIQUETRANSID = t_CID + "-" + random.Next();
            if (PayGovServer == 1)
            {
                Common_Checkout_Web_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/Commoncheckpage/Default.aspx";
                Web_service_WSDL_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.wsdl";
                Web_service_ENDPOINT_URL = "https://stageccp.dev.cdc.nicusa.com/CommonCheckout/CCPWebService/ServiceWeb.svc";
            }
            else
            {
                Common_Checkout_Web_URL = "https://securecheckout.cdc.nicusa.com/CommonCheckPage/Default.aspx";
                Web_service_WSDL_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.wsdl";
                Web_service_ENDPOINT_URL = "https://securecheckout.cdc.nicusa.com/ccpwebservice/ServiceWeb.svc";
            }

            string Common_Checkout_com_URL = "https://common.checkout.cdc.nicusa.com";
            string Common_Checkout_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/PreparePayment";
            string Common_QPayment_SOAPAction = "https://common.checkout.cdc.nicusa.com/IServiceWeb/QueryPayment";

            string t_HREFSUCCESS = paymentmodel.CurrentUrl[0]+"Public/Cart/PayGovConfirmation?OrderTotal=" + Amount + "&PaymentType=CC&OrderNo=" + paymentmodel.OrderNumber;
            string t_HREFFAILURE = paymentmodel.CurrentUrl[0];
            string t_HREFDUPLICATE = paymentmodel.CurrentUrl[0];
            string t_HREFCANCEL = paymentmodel.CurrentUrl[0];
            string i = "1";
            string t_NAME = paymentmodel.FirstName + " " + paymentmodel.LastName;
            string sMsg = "<soapenv:Envelope ";
            sMsg = sMsg + "xmlns:soapenv= \"http://schemas.xmlsoap.org/soap/envelope/\" ";
            sMsg = sMsg + " xmlns:com=\"" + Common_Checkout_com_URL +"\"";
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
            var teem = "";
            foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
            {
                loop = loop + 1;
                sMsg = sMsg + "<com1:LINEITEM>";
                sMsg = sMsg + "<com1:ITEM_ID>" + loop + "</com1:ITEM_ID>";
                if ((Settings.Instance.GetMasterInfo3().PayGovSkuField != null) &&(Settings.Instance.GetMasterInfo3().PayGovSkuField != ""))
                {
                    teem = item.Course.GetType().GetProperty(Settings.Instance.GetMasterInfo3().PayGovSkuField).GetValue(item.Course, null).ToString();
                    sMsg = sMsg + "<com1:SKU>"+  item.Course.GetType().GetProperty(Settings.Instance.GetMasterInfo3().PayGovSkuField).GetValue(item.Course, null).ToString()+"</com1:SKU>";
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

                        if (rosteritem.Materials != null)
                        {
                            foreach (var materialitems in rosteritem.Materials)
                            {
                                if (materialitems.price != null)
                                {
                                    materialtotal = materialtotal + decimal.Parse(materialitems.price.ToString());
                                }

                            }
                        }
                    }
                }

                InvcourseTotal = (item.CourseTotal + materialtotal) - coursediscount;
               //rderAmount = OrderAmount+(item.CourseTotal + item.MateriaTotal) - decimal.Parse(discount.ToString());
                sMsg = sMsg + "<com1:DESCRIPTION>" + item.Course.COURSENAME+ "</com1:DESCRIPTION>";
                sMsg = sMsg + "<com1:UNIT_PRICE>" + String.Format("{0:0.00}",InvcourseTotal) + "</com1:UNIT_PRICE>";
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


            return xmldoc;
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
		sMsg = sMsg +"xmlns:soapenv=" +"\"http://schemas.xmlsoap.org/soap/envelope/\""+ " ";
		sMsg = sMsg +"xmlns:com=\"" +Common_Checkout_com_URL + "\">";
		   sMsg = sMsg +"<soapenv:Body>";
           sMsg = sMsg + "<com:QueryPayment>";
					sMsg = sMsg +"<com:token>"+ token +"</com:token>";
			 sMsg = sMsg +"</com:QueryPayment>";
		   sMsg = sMsg +"</soapenv:Body>";
		sMsg = sMsg +"</soapenv:Envelope> ";
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
    }
}
