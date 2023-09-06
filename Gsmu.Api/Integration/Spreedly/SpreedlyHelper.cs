using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Networking.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Integration.Spreedly
{
    public class SpreedlyHelper
    {

        public static Stream SpreedlyRequestProcessor(string requestPath, string jsonParameter)
        {
            try
            {
                SpreedlyConfig SpreedlyConfig = SpreedlyGetConfig();
                string URL = SpreedlyConfig.spreedlyUrl + requestPath;
                System.Net.ServicePointManager.Expect100Continue = true;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
                request.Credentials = new NetworkCredential(SpreedlyConfig.spreedly_environmentkey, SpreedlyConfig.accessSecretKey);
                request.PreAuthenticate = true;
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonParameter);
                }

                System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
                return response.GetResponseStream();
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static SpreedlyConfig SpreedlyGetConfig()
        {
            string jsonfieldfield = Settings.Instance.GetMasterInfo4().SpreedlyConfiguration;
            SpreedlyConfig SpreedlyConfig = new SpreedlyConfig();

            /*Will Add this on Admin config setup later- hardcoded for now for testing purposes */

            //End
            if (jsonfieldfield != "{}" && !string.IsNullOrEmpty(jsonfieldfield))
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                SpreedlyConfig.spreedly_environmentkey = settingsconfig["spreedly_environmentkey"];
                SpreedlyConfig.use_spreedly = int.Parse(settingsconfig["use_spreedly"]);
                SpreedlyConfig.spreedlyUrl = settingsconfig["spreedly_url"]; ;// "https://core.spreedly.com/";
                SpreedlyConfig.signingSecretKey = settingsconfig["spreedly_signingSecretKey"];
                SpreedlyConfig.accessSecretKey = settingsconfig["spreedly_accessSecretKey"];
                SpreedlyConfig.GatewayToken = settingsconfig["spreedly_gatewaytoken"];

            }
            return SpreedlyConfig;
        }

        public static Stream SpreedlyGetGatewayList(string existingGatewayToken = "", bool AddedGateway = false)
        {
            string returnGWjson = "";
            string gatewayRequest = "v1/gateways_options.json";
            if (AddedGateway == false)
            {
                gatewayRequest = "v1/gateways.json";
            }
            using (Stream responseStream = SpreedlyRequestProcessor(gatewayRequest, ""))
            {
                return responseStream;
            }

        }
        public static string SpreedlyAddGateway(string jsonParams)
        {
            string SpreedlyAddedGatewayToken = "";
            using (Stream responseStream = SpreedlyRequestProcessor("/v1/gateways.json", jsonParams))
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(jsonfieldfield, typeof(object));
                    SpreedlyAddedGatewayToken = settingsconfig["token"];
                }
            }
            if (string.IsNullOrEmpty(SpreedlyAddedGatewayToken)) {
                SpreedlyAddedGatewayToken = "";
            }
            return SpreedlyAddedGatewayToken;
        }
        public static string SpreedlyProcessPayment(CreditCardPaymentModel paymentmodel, string Amount)
        {
            string result = "";
            string category = "";
            string coursenum = "";
            try
            {
                using (var db = new SchoolEntities())
                {
                    foreach (CourseShoppingCartItem item in CourseShoppingCart.Instance.Items)
                    {
                        coursenum = coursenum + "|" + item.Course.COURSENUM;
                        category = category + "|"+(from cr in db.MainCategories
                                                  where  cr.CourseID == item.CourseId 
                                                  select cr).FirstOrDefault().MainCategory1;
                    }

                }
            }
            catch
            {
            }
            float amt = float.Parse(Amount) * 100;
            string jsonData = "{\"transaction\":{\"payment_method_token\": \"" + paymentmodel.RefNumber + "\",\"amount\": " + amt + ",\"order_id\":\"";
            if (CheckoutInfo.Instance.PaymentCaller == "paynowuserdash")
            {
                Random rndnum = new Random();
                jsonData += paymentmodel.OrderNumber + '_' + rndnum.Next(10, 99);
            }
            else
            {
                jsonData += paymentmodel.OrderNumber;
            }
            jsonData += "\",\"currency_code\": \"USD\",\"retain_on_success\": false,\"gateway_specific_fields\":{\"payflow_pro\":{\"comment1\":\"" + category + "\",\"comment2\":\"" + coursenum + "\"}}}}";
            using (Stream responseStream = SpreedlyRequestProcessor("/v1/gateways/" + SpreedlyGetConfig().GatewayToken + "/purchase.json", jsonData))
            {
                if (responseStream == null)
                {
                    return "Error Processing the Payment." + jsonData;
                }
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                string jsonfieldfield = reader.ReadToEnd();
                if (jsonfieldfield != null)
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic paymentresponse = j.Deserialize(jsonfieldfield, typeof(object));
                    foreach (KeyValuePair<string, object> pairs in paymentresponse)
                    {
                        if (pairs.Key == "transaction")
                        {
                                var transactionjson = new JavaScriptSerializer().Serialize(pairs.Value);
                                dynamic transactionresponse = j.Deserialize(transactionjson, typeof(object));
                                foreach (KeyValuePair<string, object> transpair in transactionresponse)
                                {
                                    if (transpair.Key == "message_key")
                                    {
                                        if (transpair.Value.ToString().ToLower() == "messages.transaction_succeeded")
                                        {
                                            result = "success";
                                            paymentmodel.TotalPaid = double.Parse(Amount);

                                            paymentmodel.LongOrderId = "";

                                        }

                                    }
                                    if (result == "")
                                    {
                                        if (transpair.Key == "message")
                                        {
                                            result = transpair.Value.ToString();
                                        }

                                    }
                                    if (transpair.Key == "fingerprint")
                                    {
                                        paymentmodel.LongOrderId = transpair.Value.ToString();

                                    }
                                    if (transpair.Key == "token")
                                    {
                                        paymentmodel.PaymentNumber = transpair.Value.ToString();

                                    }
                                }
                            
                        }
                    }
                    if (result == "success")
                    {
                        EnrollmentFunction enrollment = new EnrollmentFunction();
                        EmailFunction email = new EmailFunction();
                        paymentmodel.PaymentType = "";
                        enrollment.ApproveEnrollment(paymentmodel, paymentmodel.OrderNumber);
                        email.SendConfirmationEmail(Amount, paymentmodel.OrderNumber, SpreedlyGetConfig().spreedlyUrl, " spreedlyconfirmation");
                        CourseShoppingCart.Instance.Empty();
                        CourseShoppingCart.Instance.MultipleStudentCourses = new List<CourseMultipleStudentItem>();
                    }

                    if ((WebConfiguration.AuditSiteAcitivityLevel.ToLower() != "off") && (WebConfiguration.AuditSiteAcitivityLevel.ToLower() != "initial"))
                    {
                        AuditTrail ASpreedlyAudit = new AuditTrail();
                        ASpreedlyAudit.AuditDate = DateTime.Now;
                        ASpreedlyAudit.UserName = "Spreedly";
                        ASpreedlyAudit.AuditAction = jsonfieldfield;
                        ASpreedlyAudit.DetailDescription = "Spreedly Response";
                        ASpreedlyAudit.ShortDescription = "";
                        ASpreedlyAudit.RoutineName = "SpreedlyHelper.cs";
                        using (var db = new SchoolEntities())
                        {
                            db.AuditTrails.Add(ASpreedlyAudit);
                            db.SaveChanges();
                        }
                    }
                    return result;
                }
            }
        return result;
        }
    }


    public class SpreedlyConfig
    {
        public int use_spreedly
        {
            get;
            set;
        }
        public string spreedly_environmentkey
        {
            get;
            set;
        }
        public string spreedlyUrl
        {
            get;
            set;
        }

        public string accessSecretKey
        {
            get;
            set;
        }
        public string signingSecretKey
        {
            get;
            set;
        }
        public string GatewayToken
        {
            get;
            set;
        }
    }
}
