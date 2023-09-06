using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Integration.ClubPilates;
using Gsmu.Api.Integration.Spreedly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Data
{
    public class CreditCardPaymentHelper
    {

        public static string ActiveCCProcessing
        {
            get
            {
                // please include new CC processing in the list

                if (UseTouchnetRedirect) { return "TouchnetRedirect"; }
                else if (UseNelNet) { return "NelNet"; }
                else if (UseTouchnetTlink) { return "TouchnetTlink"; }
                else if (UseAuthorizeNet) { return "AuthorizeNet"; }
                else if (UseAuthorizeNetRedirect) { return "AuthorizeNetRedirect"; }
                else if (UsePayPal) { return "PayPal"; }
                else if (UseBBPaygate) { return "BBPaygate"; }
                else if (UsePaygov) { return "Paygov"; }
                else if (UsePaygovTCS) { return "PaygovTCS"; }
                else if (ANTesting) { return "ANTesting"; }
                else if (AnetUsePostMethod) { return "AnetPostMethod"; }
                else if (UsePriorAuthAndCapture) { return "PriorAuthAndCapture"; }
                else if (UseChasePayment) { return "ChasePayment"; }
                else if (UseSquare) { return "SquarePayment"; }
                else if (UseCybersource) { return "CybersourcePayment"; }
                else if (UseAdyen) { return "AdyenPayment"; }
                else if (UseFirstData) { return "FirstDataPayment"; }
                else if (UseClubPilates) { return "ClubReady"; }
                else if (UseSpreedly) { return "Spreedly"; }
                else { return "none"; }
            }
        }


        /// <summary>
        /// If true, TouchNet is the Pyment Merchant
        /// </summary>
        public static bool UseTouchnetRedirect
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().UseTouchnet;
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        public static bool UseCashNetRedirect
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                if (value != "" && value != null)
                {
                    int UseCashnet = 0;
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        UseCashnet = int.Parse(cashnetconfiguration["UseCashnet"]);
                    }
                    catch { }

                    return Settings.GetVbScriptBoolValue(UseCashnet);
                }
                else
                {
                    return false;
                }
            }
        }
        public static string CashNetOperator
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                string CashNetOperatorValue = "";
                if (value != "" && value != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        CashNetOperatorValue = cashnetconfiguration["CashNetOperator"];
                    }
                    catch { }


                }
                return CashNetOperatorValue;
            }
        }
        public static string CashNetPassword
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                string CashNetPasswordValue = "";
                if (value != "" && value != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        CashNetPasswordValue = cashnetconfiguration["CashNetPassword"];
                    }
                    catch { }


                }
                return CashNetPasswordValue;
            }
        }

        public static string cashnetCustomerCode
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                string cashnetCustomerCodeValue = "";
                if (value != "" && value != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        cashnetCustomerCodeValue = cashnetconfiguration["cashnetCustomerCode"];
                    }
                    catch { }


                }
                return cashnetCustomerCodeValue;
            }
        }
        public static string cashnetItemCode
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                string cashnetItemCodeValue = "";
                if (value != "" && value != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        cashnetItemCodeValue = cashnetconfiguration["cashnetItemCode"];
                    }
                    catch { }


                }
                return cashnetItemCodeValue;
            }
        }
        public static string cashnetItemCodeDesc
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                string cashnetItemCodeDescValue = "";
                if (value != "" && value != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        cashnetItemCodeDescValue = cashnetconfiguration["cashnetItemCodeDesc"];
                    }
                    catch { }


                }
                return cashnetItemCodeDescValue;
            }
        }
        public static string cashnetserver
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().CashNetConfiguration;
                string cashnetserverValue = "";
                if (value != "" && value != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic cashnetconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        cashnetserverValue = cashnetconfiguration["cashnetserver"];
                    }
                    catch { }


                }
                return cashnetserverValue;
            }
        }
        public static bool UseIpay
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().UseiPay;
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        public static bool UseNelNet
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo3().usenelnetpayment;
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        /// <summary>
        /// If true, TouchNet is the Pyment Merchant
        /// </summary>
        public static string TouchnetTLinkUserName
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().TouchnetTLinkUserName;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        /// <summary>
        /// If true, TouchNet is the Pyment Merchant
        /// </summary>
        public static string TouchnetTLinkServiceurl
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().TouchnetTLinkServiceurl;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        /// <summary>
        /// If true, TouchNet is the Pyment Merchant
        /// </summary>
        public static string TouchnetTLinkPassword
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().TouchnetTLinkPassword;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        /// <summary>
        /// If true, TouchNet is the Pyment Merchant
        /// </summary>
        public static bool UseTouchnetTlink
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().UseTouchnetTlink;
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        /// <summary>
        /// If true, Authorize Net is the Pyment Merchant
        /// </summary>
        public static bool UseAuthorizeNet
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().UseAuthorizeNet;
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        /// <summary>
        /// If true, Authorize Net Redirect is the Pyment Merchant
        /// </summary>
        public static bool UseAuthorizeNetRedirect
        {
            get
            {
                int value = int.Parse(Settings.Instance.GetMasterInfo().UseAuthorizeNetRedirect.ToString());
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        public static bool UsePayPal
        {
            get
            {
                bool result = false;
                int value = int.Parse(Settings.Instance.GetMasterInfo().UsePayflow.ToString());
                if (value == 1) // Positive 1 is assign to PayFlow Pro redirect while -1 value is for PaypalAdvance
                {
                    result = true;
                }
                return result;
            }
        }

        public static bool UsePayPalAdvance
        {
            get
            {
                bool result = false;
                int value = int.Parse(Settings.Instance.GetMasterInfo().UsePayflow.ToString());
                if (value == -1) // Positive 1 is assign to PayFlow Pro redirect while -1 value is for PaypalAdvance
                {
                    result = true;
                }
                return result;
            }
        }


        public static bool UseBBPaygate
        {
            get
            {
                int value = 0;
                if (Settings.Instance.GetMasterInfo3().use_BBPaygate == 10)
                {
                    value = 1;
                };
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        public static bool UseBlackBoardPaygateRedirect 
        {
            get
            {
                int value = 0;
                if (Settings.Instance.GetMasterInfo3().use_BBPaygate == 20)
                {
                    value = 1;
                };
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        public static bool UsePaygov
        {
            get
            {
                int value = int.Parse(Settings.Instance.GetMasterInfo3().usePayGovRedirect.ToString());
                if (Settings.Instance.GetMasterInfo3().usePayGovRedirect == 10)
                {
                    value = 1;
                };
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        public static bool UsePaygovTCS
        {
            get
            {
                try
                {
                    int value = int.Parse(Settings.Instance.GetMasterInfo3().usePayGovTCS.ToString());
                    return Settings.GetVbScriptBoolValue(value);
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// If true, Authorize Net is in Testing Mode
        /// </summary>
        public static bool ANTesting
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().ANTesting;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        /// <summary>
        /// If true, Authorize Net is using PostMethod
        /// </summary>
        public static bool AnetUsePostMethod
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().AnetUsePostMethod;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        /// <summary>
        /// If true, Authorize Net is using PostMethod and UsePriorAuthAndCapture
        /// </summary>
        public static bool UsePriorAuthAndCapture
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().UsePriorAuthAndCapture;
                return Settings.GetVbScriptBoolValue(value);
            }
        }
        /// <summary>
        /// Value is the Login Key for AuthorizeNet
        /// </summary>
        public static string ANLogin
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().ANLogin;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }
        /// <summary>
        /// Value is the Login Key for AuthorizeNet
        /// </summary>
        public static string ANTranKey
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().ANTranKey;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        /// <summary>
        /// Value is the Submission Link for AuthorizeNet
        /// </summary>
        public static string ANRedirectSubmissionLink
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().ANRedirectSubmissionLink;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        /// <summary>
        /// Value is the Reciept Link for AuthorizeNet
        /// </summary>
        public static string AnetReceiptLinkURL
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().ANRedirectSubmissionLink;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        public static int AnetRoutineTransDays
        {
            get
            {
                int? value = Settings.Instance.GetMasterInfo().AnetRoutineTransDays;
                if (value == null)
                    return 0;
                return Convert.ToInt32(value);
            }
        }

        public static bool UseChasePayment
        {
            get
            {

                int value = int.Parse(Settings.Instance.GetMasterInfo4().UseOrbital.ToString());
                return Settings.GetVbScriptBoolValue(value);
            }

        }

        public static string ChasePaymentUserName
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().OrbitalUsername;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }
        /// <summary>
        /// Value is the ChasePaymentPassword or hostedSecureAPIToken
        /// </summary>
        public static string ChasePaymentPassword
        {
            get
            {

                var value = Settings.Instance.GetMasterInfo4().OrbitalPassword;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        public static string ChasePaymentServer
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().OrbitalServer.ToString();
                string result = string.IsNullOrEmpty(value) ? "" : value;

                if (result == "0")
                {
                    return "https://www.chasepaymentechhostedpay-var.com/securepayments/a1/cc_collection.php";
                }
                else
                {
                    return "https://www.chasepaymentechhostedpay.com/securepayments/a1/cc_collection.php";
                }

            }
        }

        public static string ChasePaymentAbstractionAddress
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().OrbitalServer.ToString();
                string result = string.IsNullOrEmpty(value) ? "" : value;

                if (result == "0")
                {
                    return "https://www.chasepaymentechhostedpay-var.com/direct/services/request/init/";
                }
                else
                {
                    return "https://www.chasepaymentechhostedpay.com/direct/services/request/init/";
                }

            }
        }

        public static string ChasePaymentMerchantId
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo4().OrbitalMerchantID.ToString();
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        public static string ElavonServer
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo3().ElavonServer;
                return value;
            }
        }
        public static string ElavonUserId
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo3().ElavonUserId;
                return value;
            }
        }

        public static string ElavonMerchantId {
            get {
                var value = Settings.Instance.GetMasterInfo3().ElavonMerchantId;
                return value;
            }
        }

        public static string ElavonPin
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo3().ElavonPin;
                return value;
            }
        }
        public static bool UseElavon
        {
            get 
            {
                var value = Settings.Instance.GetMasterInfo3().CreditCardProcessorToUse;
                if (value.HasValue && value == 10)
                {
                    return true;
                    //return Settings.GetVbScriptBoolValue(value);
                }
                return false;
            }
        }


        public static bool UseSquare
        {
            get
            {

                var value = Settings.Instance.GetMasterInfo().UseSquare;
                if (value.HasValue && (value == 1 || value == -1))
                {
                    return true;
                    //return Settings.GetVbScriptBoolValue(value);
                }
                return false;
            }

        }

        public static bool UseFirstData
        {
            get
            {

                var value = Settings.Instance.GetMasterInfo3().use_FirstDataGateway;
                if (value.HasValue && (value == 1 || value == -1 || value == 10))
                {
                    return true;
                }
                return false;
            }

        }
        public static bool UseCybersource
        {
            get
            {

                var value = Settings.Instance.GetMasterInfo().UseCybersource;
                if (value.HasValue && (value == 1 || value == -1))
                {
                    return true;
                }
                return false;
            }

        }
        public static bool UseClubPilates
        {
            get
            {
                if (ClubPilatesHelper.GetConfig().useClubPilates == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool UseSpreedly
        {
            get
            {
                if (SpreedlyHelper.SpreedlyGetConfig().use_spreedly == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static string SpreedlyEnvironmentKey
        {
            get
            {
                return SpreedlyHelper.SpreedlyGetConfig().spreedly_environmentkey;
            }
        }


        public static bool UseAdyen
        {
            get
            {

                var value = 0;
                string AdyenPaymentGateway = Settings.Instance.GetMasterInfo().AdyenPaymentGateway;
                if ((AdyenPaymentGateway != "") && (AdyenPaymentGateway != null))
                {
                    if (AdyenPaymentGateway.Length > 10)
                    {
                        JavaScriptSerializer j = new JavaScriptSerializer();
                        dynamic settingsconfig = j.Deserialize(AdyenPaymentGateway, typeof(object));
                        value = settingsconfig["UseAdyen"];
                    }

                }


                if (value == 1 || value == -1)
                {
                    return true;
                }
                return false;
            }

        }

        public static bool UseRevtrak
        {
            get
            {
                var value = "";
                if (value != "" && value != null)
                {
                    int UseRevtrak = 0;
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic revtrakconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        UseRevtrak = int.Parse(revtrakconfiguration["use_revtrak"]);
                    }
                    catch { }

                    return Settings.GetVbScriptBoolValue(UseRevtrak);
                }
                else
                {
                    return false;
                }
            }
        }

        public static string RevTrakPCCT
        {
            get
            {
                var value = "";
                if (value != "" && value != null)
                {
                    string pcct = "";
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic revtrakconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        pcct = int.Parse(revtrakconfiguration["pcct"]);
                    }
                    catch { }

                    return pcct;
                }
                else
                {
                    return "";
                }
            }
        }

        public static string RevTrakSecretKey
        {
            get
            {
                var value = "";
                if (value != "" && value != null)
                {
                    string revtraksecretkey = "";
                    System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic revtrakconfiguration = JSSerializeObj.Deserialize(value, typeof(object));
                    try
                    {
                        revtraksecretkey = int.Parse(revtrakconfiguration["revtraksecretkey"]);
                    }
                    catch { }

                    return revtraksecretkey;
                }
                else
                {
                    return "";
                }
            }
        }

    }
}