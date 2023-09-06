using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Web;

namespace Gsmu.Api.Data.School.Attendance
{
    public class AttendanceConfig
    {

        public static AttendanceConfig Instance
        {
            get
            {
                var instance = ObjectHelper.GetRequestObject<AttendanceConfig>(WebContextObject.AttendanceConfig);
                if (instance == null)
                {
                    ObjectHelper.SetRequestObject<AttendanceConfig>(WebContextObject.AttendanceConfig, new AttendanceConfig());
                    return Instance;
                }
                return instance;
            }
        }

        public bool AllowAttendancdeDetail
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().AllowAttendanceDetail
                );
            }
        }

        public bool DontDefaultAttendance
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().DontDefaultAttendance
                );
            }
        }

        public bool PricingOrCreditType
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().PricingOrCreditType
                );
            }
        }

        public bool ShowRebill
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().ShowRebill
                );
            }
        }

        public bool ShowRequiredFlexDays
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().ShowReqFlexDays
                );
            }
        }


        /// <summary>
        /// MM/DD format for dates, need to add year to end
        /// </summary>
        public string CreditBankingBeginDate
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().CreditBankingBeginDate;
            }
        }

        public string ActualPeriod
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().Period;
            }
        }

        public string NextPeriod
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().NextPeriod;
            }
        }


        public string DateOfCutoff
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().DateOfCutoff;
            }
        }

        public bool ShowCheckoutComments {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().ShowCheckoutComments
                );
            }
        }

        public string CheckoutCommentsLabel
        {
            get
            {
                return Settings.Instance.GetMasterInfo2().CheckoutCommentsLabel;
            }
        }

        public bool ShowParking
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().Parking
                );
            }
        }

        public bool AllowInstructorMultiPassAttendance
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().AllowInstructorMultiPassAttendance
                );
            }
        }

        public bool AllowAttendanceStatus
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().AllowAttendanceStatus
                );
            }
        }

        public bool CollectOptionalInfo
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo2().CollectOptionalInfo
                );
            }
        }

        public bool ShowGrades
        {
            get
            {
                return Settings.GetVbScriptBoolValue(
                    Settings.Instance.GetMasterInfo().GradingSystem
                );
            }
        }

    }
}
