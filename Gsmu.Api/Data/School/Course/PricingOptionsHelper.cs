using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Student;
using Newtonsoft.Json;

namespace Gsmu.Api.Data.School.Course
{
    public class PricingOptionsHelper
    {
        public static string PricingOptionsLabel
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().PricingOptionsLabel;
                return string.IsNullOrEmpty(value) ? "" : "Pricing Options";

            }
        }

        /// <summary>
        /// If true, pricing options are enabled
        /// </summary>
        public static bool PricingVisible
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().PricingVisible;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        /// <summary>
        /// If ture, the options range is allowed
        /// </summary>
        public static bool PricingOptionRange
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().PricingOptRange ?? 0;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        /// <summary>
        /// if true, courses can override the pricing option price
        /// </summary>
        public static bool PricingOptionOverride
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().PricingOptionOverride;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        public static PricingOptionType MembershipTypeToPricingOptionType(MembershipType membership)
        {
            switch (membership)
            {
                case MembershipType.Member:
                    return PricingOptionType.Member;

                case MembershipType.NonMember:
                    return PricingOptionType.NonMember;

                case MembershipType.Special1:
                    return PricingOptionType.Special1;

                default:
                    throw new NotImplementedException();
            }
        }

        public static MembershipType DefaultmembershipType
        {
            get
            {
                if (Settings.Instance.GetMasterInfo2().DefaultPublicPricingType == 0)
                {
                    return MembershipType.Member;
                }
                return MembershipType.NonMember;                
            }
        }

        public static string ExtraParticipantLabel
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().CollectionStyleLabel;
                return value;
            }
        }

        public static bool ExtraParticipantCollectionEnabled
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExtraParticipantLabel))
                {
                    return false;
                }
                return true;
            }
        }

        public static string ExtraParticipantCustomFieldLabel
        {
            get
            {
                var label = Settings.Instance.GetMasterInfo3().householdFieldLabel;
                if (!string.IsNullOrWhiteSpace(label))
                {
                    if (label.Contains(":") && label.Contains("{") && label.Contains("}"))
                    {
                        dynamic jsonString = JsonConvert.DeserializeObject(label);
                        return jsonString.label;
                    }
                    return label;
                }
                return null;
            }
        }

        public static bool ExtraParticipantCustomFieldLabelRequired
        {
            get
            {
                var label = Settings.Instance.GetMasterInfo3().householdFieldLabel;
                if (!string.IsNullOrWhiteSpace(label))
                {
                    if (label.Contains(":") && label.Contains("{") && label.Contains("}"))
                    {
                        dynamic jsonString = JsonConvert.DeserializeObject(label);
                        if (jsonString.required == "on") 
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
        }

        public static bool ExtraParticipantCollectCustomField
        {
            get
            {
                return ExtraParticipantCustomFieldLabel != null;
            }
        }
    }
}
