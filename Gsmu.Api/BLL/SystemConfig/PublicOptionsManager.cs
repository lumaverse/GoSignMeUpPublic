using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gsmu.Api.Data.ViewModels.SystemConfig;
using Gsmu.Api.Data.School.Entities;

using Newtonsoft.Json;
using System.Web.Mvc;

namespace Gsmu.Api.BLL.SystemConfig
{
    public class PublicOptionsManager
    {
        public PublicOptionsViewModel GetPublicOptions() 
        {
            PublicOptionsViewModel publicOptionsModel = new PublicOptionsViewModel();

            publicOptionsModel.SystemTimeZoneHourList = new SelectList(new[] { 
                new SelectListItem { Text = "None", Value = "9999" }, 
                new SelectListItem { Text = "Pacific (0)", Value = "0" }, 
                new SelectListItem { Text = "Mountain (1)", Value = "1" }, 
                new SelectListItem { Text = "Central (2)", Value = "2" }, 
                new SelectListItem { Text = "Eastern (3)", Value = "3" },
                new SelectListItem { Text = "Arizona (4) with DST", Value = "4" },
                new SelectListItem { Text = "Arizona (5) no DST", Value = "5" },
                new SelectListItem { Text = "Gulf Standard (6)", Value = "6" }
            
            }
                , "Value", "Text");
            publicOptionsModel.PolarAnswers = new SelectList(new[] { 
                new SelectListItem { Text = "Yes", Value = "1" }, 
                new SelectListItem { Text = "No", Value = "0" }}, "Value", "Text");
            publicOptionsModel.PublicHideLinksList = new SelectList(new[] {
                new SelectListItem { Text = "Not Applicable", Value = "0" }, 
                new SelectListItem { Text = "Hide Manage Rooms Link (1)", Value = "1" },
                new SelectListItem { Text = "Hide Evaluations Area/Link (2)", Value = "2" },
                new SelectListItem { Text = "Hide 1 and 2 above", Value = "3" }
            }, "Value", "Text");
            publicOptionsModel.NameDisplayStyleList = new SelectList(new[] {
                new SelectListItem { Text = "Last, First", Value = "1" }, 
                new SelectListItem { Text = "First, Last", Value = "0" }
            }, "Value", "Text");
            publicOptionsModel.SupervisorExcludeInactiveList = new SelectList(new[] {
                new SelectListItem { Text = "Show Active and Inactive", Value = "0" }, 
                new SelectListItem { Text = "1. Exclude Inactive (Student Add/Edit Page Inactive? Field)", Value = "1" }, 
                new SelectListItem { Text = "2. Exclude by 'Inactive' (Student Registration Hidden Fields 1: Hidden Field Sample)", Value = "2" }, 
                new SelectListItem { Text = "3. Exclude by 1 and 2 above", Value = "3" }
            }, "Value", "Text");

            //shibboleth config parse part
            if (!string.IsNullOrEmpty(publicOptionsModel.ShibbolethConfiguration))
            {
                dynamic shibbolethConfiguration = JsonConvert.DeserializeObject(publicOptionsModel.ShibbolethConfiguration);
                int? login_shib_sso_gsmuonly = shibbolethConfiguration.login_shib_sso_gsmuonly ?? 0;
                int? login_shib_sso_gsmuactive = shibbolethConfiguration.login_shib_sso_gsmuactive ?? 0;
                string current_shibboleth_department_attribute = shibbolethConfiguration.current_shibboleth_department_attribute;

                publicOptionsModel.ShibbolethSSOGSMUOnly = login_shib_sso_gsmuonly;
                publicOptionsModel.ShibbolethSSOGSMUActive = login_shib_sso_gsmuactive;
                publicOptionsModel.CurrentShibbolethDepartmentAttribute = current_shibboleth_department_attribute;
            }
            //fieldspecs
            using (var db = new SchoolEntities())
            {
                var fieldSpec = db.FieldSpecs.Where(f => f.TableName == "Students").ToList();
                if (fieldSpec.Count() > 0)
                {
                    var fieldSpecSessionAttribute = fieldSpec.Where(f => f.FieldName == "SessionId").FirstOrDefault();
                    var fieldSpecDepartmentAttribute = fieldSpec.Where(f => f.FieldName == "Department").FirstOrDefault();
                    if (fieldSpecSessionAttribute != null)
                    {
                        publicOptionsModel.ShibbolethSessionIdAttribute = fieldSpecSessionAttribute.shibboleth_attribute;
                    }
                    if (fieldSpecDepartmentAttribute != null)
                    {
                        publicOptionsModel.ShibbolethDepartmentAttribute = fieldSpecDepartmentAttribute.shibboleth_attribute;
                    }
                }
            }
            return publicOptionsModel;
        }
        //Implementation of the saving publicOptionsModel
        public void SavePublicOptionsToDB(PublicOptionsViewModel publicOptionsModel)
        {
            try
            {
                using (var db = new SchoolEntities())
                {
                    MasterInfo masterinfo = db.MasterInfoes.FirstOrDefault();
                    masterinfo.PublicSignupAbilityOff = publicOptionsModel.PublicSignupAbilityOff;
                    masterinfo.HiddenStudRegField1Name = publicOptionsModel.HiddenStudRegField1Name;

                    MasterInfo2 masterinfo2 = db.MasterInfo2.FirstOrDefault();
                    masterinfo2.MinStudentNum4MultiEnrollDiscount = publicOptionsModel.MinStudentNum4MultiEnrollDiscount;
                    masterinfo2.Percent4MultiEnrollDiscount = publicOptionsModel.Percent4MultiEnrollDiscount;
                    masterinfo2.AllowParentLevel = publicOptionsModel.AllowParentLevel;
                    masterinfo2.AllowStudentMultiEnroll = publicOptionsModel.AllowStudentMultiEnroll;
                    masterinfo2.AllowModifyMultiEnroll = publicOptionsModel.AllowModifyMultiEnroll;
                    masterinfo2.ParentLevelTitle = publicOptionsModel.ParentLevelTitle;
                    masterinfo2.ChildLevelTitle = publicOptionsModel.ChildLevelTitle;
                    masterinfo2.AllowReleaseForm = publicOptionsModel.AllowReleaseForm;
                    masterinfo2.ReleaseFormTitle = publicOptionsModel.ReleaseFormTitle;
                    masterinfo2.ReleaseFormText = publicOptionsModel.ReleaseFormText;
                    masterinfo2.CommonPublicLogin = publicOptionsModel.CommonPublicLogin;
                    masterinfo2.AllowPublicBreakCommonLogin = publicOptionsModel.AllowPublicBreakCommonLogin;
                    masterinfo2.studregmask5initialtext = publicOptionsModel.StudentRegisterMaskFiveInitText;

                    MasterInfo3 masterinfo3 = db.MasterInfo3.FirstOrDefault();
                    masterinfo3.AutoApproveZeroOrder = publicOptionsModel.AutoApproveZeroOrder;
                    masterinfo3.PublicStudentEnrollment = publicOptionsModel.PublicStudentEnrollment;
                    masterinfo3.AutoPopulatePassword4CommonLogin = publicOptionsModel.AutoPopulatePassword4CommonLogin;
                    masterinfo3.PublicHideLinks = publicOptionsModel.PublicHideLinks;
                    masterinfo3.NameDisplayStyle = publicOptionsModel.NameDisplayStyle;
                    masterinfo3.SupervisorExcludeInactive = publicOptionsModel.SupervisorExcludeInactive;
                    masterinfo3.ForceAccountUpdate = publicOptionsModel.ForceAccountUpdate;
                    masterinfo3.ForceAUMsg = publicOptionsModel.ForceAUMsg;
                    masterinfo3.ForceAUDateCount = publicOptionsModel.ForceAUDateCount;
                    masterinfo3.restrictStudentMultiSignup = publicOptionsModel.RestrictStudentMultiSignup;
                    masterinfo3.google_sso_client_id = publicOptionsModel.GoogleSSOClientId;
                    masterinfo3.google_sso_client_secret = publicOptionsModel.GoogleSSOClientSecret;
                    masterinfo3.allowCrossUserUpdate = publicOptionsModel.AllowCrossUserUpdate;
                    masterinfo3.ForceUniqueStudentEmails = publicOptionsModel.ForceUniqueStudentEmails;
                    masterinfo3.OtherStrtupPage = publicOptionsModel.OtherStrtupPage;
                    masterinfo3.system_timezone_hour = publicOptionsModel.SystemTimeZoneHour;
                    masterinfo3.StrtupPage = publicOptionsModel.StartupPage;
                    masterinfo3.LDAPAuxServer = publicOptionsModel.LDAPAuxServer;


                    masterinfo4 masterinfo4 = db.masterinfo4.FirstOrDefault();
                    masterinfo4.MultipleSignUpCustomText = publicOptionsModel.MultipleSignUpCustomText;
                    masterinfo4.LoginAuthOption = publicOptionsModel.LoginAuthOption;
                    masterinfo4.shibboleth_sso_enabled = publicOptionsModel.ShibbolethSSOEnabled;
                    masterinfo4.shibboleth_allow_gsmu_login = publicOptionsModel.ShibbolethRequiredLogin;
                    masterinfo4.shibboleth_logout_link = publicOptionsModel.ShibbolethLogOutLink;

                    var shibbolethConfiguration = JsonConvert.SerializeObject(new
                    {
                        login_shib_sso_gsmuonly = publicOptionsModel.ShibbolethSSOGSMUOnly,
                        login_shib_sso_gsmuactive = publicOptionsModel.ShibbolethSSOGSMUActive,
                        current_shibboleth_department_attribute = publicOptionsModel.CurrentShibbolethDepartmentAttribute
                    });

                    masterinfo4.ShibbolethConfiguration = shibbolethConfiguration.ToString();
                    masterinfo4.AspSiteRootUrl = publicOptionsModel.AspSiteRootUrl;

                    //fieldspecs
                    var fieldSpec = db.FieldSpecs.Where(f => f.TableName == "Students").ToList();
                    if (fieldSpec.Count() > 0)
                    {
                        var fieldSpecSessionAttribute = fieldSpec.Where(f => f.FieldName == "SessionId").FirstOrDefault();
                        var fieldSpecDepartmentAttribute = fieldSpec.Where(f => f.FieldName == "Department").FirstOrDefault();

                        if (fieldSpecSessionAttribute != null)
                        {
                            fieldSpecSessionAttribute.shibboleth_attribute = publicOptionsModel.ShibbolethSessionIdAttribute;
                        }
                        else
                        {
                            db.FieldSpecs.Add(new FieldSpec()
                            {
                                TableName = "Students",
                                FieldName = "SessionId",
                                FieldReadOnly = 0,
                                FieldVisible = 0,
                                FieldRequired = 0,
                                FieldDisplaySortOrder = 0,
                                YesNo = 0,
                                FieldLabel = "SessionId",
                                Size = 30,
                                MaxLength = 50,
                                ConfirmRequired = 0,
                                FieldCertificationsId = 0,
                                shibboleth_attribute = "HTTP_SHIBSESSIONID"
                            });
                        }
                        if (fieldSpecDepartmentAttribute != null)
                        {
                            fieldSpecSessionAttribute.shibboleth_attribute = publicOptionsModel.ShibbolethDepartmentAttribute;
                        }
                        else
                        {
                            db.FieldSpecs.Add(new FieldSpec()
                            {
                                TableName = "Students",
                                FieldName = "Department",
                                FieldReadOnly = 0,
                                FieldVisible = 0,
                                FieldRequired = 0,
                                FieldDisplaySortOrder = 0,
                                YesNo = 0,
                                FieldLabel = "Department",
                                Size = 30,
                                MaxLength = 50,
                                ConfirmRequired = 0,
                                FieldCertificationsId = 0,
                                shibboleth_attribute = "HTTP_SHIBDEPARTMENTID"
                            });
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
