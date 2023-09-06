using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.BaseModels
{
    public class MasterInfoBaseModel
    {
        #region MasterInfo Base ViewModel Class
        //This viewmodel class will server as the base class for the sub viewmodel classes
        //that will use the properties of the masterinfo. SystemConfig pages uses masterinfo table
        //so they can just inherit an use the selected properties associated to them
        #endregion
        public MasterInfoBaseModel()
        {
        }
        #region MASTERINFO1
        public int PublicSignupAbilityOff
        {
            get
            {
                return Settings.Instance.GetMasterInfo().PublicSignupAbilityOff;
            }
            set 
            {
                Settings.Instance.GetMasterInfo().PublicSignupAbilityOff = value;
            }
        }
        public string HiddenStudRegField1Name 
        {
            get
            {
                return Settings.Instance.GetMasterInfo().HiddenStudRegField1Name;
            }
            
        }
        #endregion
        #region MASTERINFO2
        public int MinStudentNum4MultiEnrollDiscount 
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().MinStudentNum4MultiEnrollDiscount ?? 0;
            }
            set 
            {
                Settings.Instance.GetMasterInfo2().MinStudentNum4MultiEnrollDiscount = value;
            }
        }
        public int? Percent4MultiEnrollDiscount 
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().Percent4MultiEnrollDiscount ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().Percent4MultiEnrollDiscount = value;
            }
        }
        public short AllowParentLevel
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().AllowParentLevel;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().AllowParentLevel = value;
            }
        }
        public int? AllowStudentMultiEnroll
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().AllowStudentMultiEnroll ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().AllowStudentMultiEnroll = value;
            }
        }
        public int? AllowModifyMultiEnroll
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().AllowModifyMultiEnroll ?? 0;
            }
            set 
            {
                Settings.Instance.GetMasterInfo2().AllowModifyMultiEnroll = value;
            }
        }
        public string ParentLevelTitle
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().ParentLevelTitle;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().ParentLevelTitle = value;
            }
        }
        public string ChildLevelTitle
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().ChildLevelTitle;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().ChildLevelTitle = value;
            }
        }
        public int AllowReleaseForm
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().AllowReleaseForm;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().AllowReleaseForm = value;
            }
        }
        public string ReleaseFormTitle
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().ReleaseFormTitle;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().ReleaseFormTitle = value;
            }
        }
        [DataType(DataType.MultilineText)]
        public string ReleaseFormText
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().ReleaseFormText;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().ReleaseFormText = value;
            }
        }
        public int? CommonPublicLogin
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().CommonPublicLogin ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().CommonPublicLogin = value;
            }
        }
        public int? AllowPublicBreakCommonLogin
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().AllowPublicBreakCommonLogin ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().AllowPublicBreakCommonLogin = value;
            }
        }
        public string StudentRegisterMaskFiveInitText
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().studregmask5initialtext;
            }
            set
            {
                Settings.Instance.GetMasterInfo2().studregmask5initialtext = value;
            }
        }
        #endregion
        #region MASTERINFO3
        public int? AutoApproveZeroOrder
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().AutoApproveZeroOrder ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().AutoApproveZeroOrder = value;
            }
        }
        public int? PublicStudentEnrollment
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().PublicStudentEnrollment ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().PublicStudentEnrollment = value;
            }
        }
        public int? AutoPopulatePassword4CommonLogin
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().AutoPopulatePassword4CommonLogin ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().AutoPopulatePassword4CommonLogin = value;
            }
        }
        public int? PublicHideLinks
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().PublicHideLinks ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().PublicHideLinks = value;
            }
        }
        public int? NameDisplayStyle
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().NameDisplayStyle ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().NameDisplayStyle = value;
            }
        }
        public int? SupervisorExcludeInactive
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().SupervisorExcludeInactive ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().SupervisorExcludeInactive = value;
            }
        }
        public int? ForceAccountUpdate
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().ForceAccountUpdate ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().ForceAccountUpdate = value;
            }
        }
        public string ForceAUMsg
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().ForceAUMsg;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().ForceAUMsg = value;
            }
        }
        public int? ForceAUDateCount
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().ForceAUDateCount ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().ForceAUDateCount = value;
            }
        }
        public int? RestrictStudentMultiSignup
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().restrictStudentMultiSignup ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().restrictStudentMultiSignup = value;
            }
        }
        public int? GoogleSSOEnabled
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().google_sso_enabled ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().google_sso_enabled = value;
            }
        }
        public string GoogleSSOClientId 
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().google_sso_client_id;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().google_sso_client_id = value;
            }
        }
        public string GoogleSSOClientSecret
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().google_sso_client_secret;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().google_sso_client_secret = value;
            }
        }
        public int? AllowCrossUserUpdate
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().allowCrossUserUpdate ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().allowCrossUserUpdate = value;
            }
        }
        public int? ForceUniqueStudentEmails
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().ForceUniqueStudentEmails ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().ForceUniqueStudentEmails = value;
            }
        }
        public int? StartupPage
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().StrtupPage ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().StrtupPage = value;
            }
        }
        public string OtherStrtupPage
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OtherStrtupPage;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().OtherStrtupPage = value;
            }
        }
        public int ? SystemTimeZoneHour
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().system_timezone_hour ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo3().system_timezone_hour = value;
            }
        }
        public string LDAPAuxServer 
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OtherStrtupPage;
            }
            
        }
        #endregion
        #region MASTERINFO4
        public string MultipleSignUpCustomText
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().MultipleSignUpCustomText;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().MultipleSignUpCustomText = value;
            }
        }
        public int? LoginAuthOption
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().LoginAuthOption ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().LoginAuthOption = value;
            }
            
        }
        public int? ShibbolethSSOEnabled
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled = value;
            }
        }
        public int? ShibbolethRequiredLogin
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().shibboleth_required_login ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().shibboleth_required_login = value;
            }
        }
        public string ShibbolethLogOutLink
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().shibboleth_logout_link;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().shibboleth_logout_link = value;
            }
        }
        public string ShibbolethSessionIdAttribute
        {
            get;
            set;

        }

        public string ShibbolethDepartmentAttribute
        {
            get;
            set;

        }
        public string CurrentShibbolethDepartmentAttribute
        {
            get;
            set;

        }
        public int? ShibbolethAllowGSMULogin 
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().shibboleth_allow_gsmu_login ?? 0;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().shibboleth_allow_gsmu_login = value;
            }
        }
        public string ShibbolethConfiguration 
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().ShibbolethConfiguration;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().ShibbolethConfiguration = value;
            }
        }
        public int? ShibbolethSSOGSMUOnly
        {
            get;
            set;
        }
        public int? ShibbolethSSOGSMUActive
        {
            get;
            set;
        }
        public string ShibbolethSSOGSMUDepartmentAttribute
        {
            get;
            set;
        }
        public string AspSiteRootUrl
        {
            get {
                return Settings.Instance.GetMasterInfo4().AspSiteRootUrl;
            }
            set
            {
                Settings.Instance.GetMasterInfo4().AspSiteRootUrl = value;
            }
        }
        public string AspSiteGoogleRedirectUrl 
        {
            get
            {
                return Settings.Instance.GetMasterInfo4().AspSiteRootUrl + "google-sso.asp";
            }
            set
            {
                Settings.Instance.GetMasterInfo4().AspSiteRootUrl = value;
            }
        }
        #endregion
    }
}
