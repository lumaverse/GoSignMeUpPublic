using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    /*     
    To show credits condition:
    if (coursetype=0 and PublicHideAllcredit<> "-1" and (DontDisplayCredithours <> -1 or ShowInserviceTime=1 or bShowCustomCreditType=true or bShowCEUandGraduateCreditCourses= true or bShowOptionalCredithours1 = true)) and hidecreditinpublic = "0" then

    coursetype - course, Lang will answer this
    hidecreditinpublic = courses showcreditinpublic used as hidecreditinpublic, Darren -> notice, value = 0 is to show, 1 not to show. (programmer mixed up (was it me mixed up there???))

    All Variables:
    x DontDisplayCredithours = rsmi1
    x ShowInserviceTime = rsmi1

    x CreditHoursName = mi2
    x InserviceHoursName = mi2
    x ShowCustomCreditType = mi2
    x CustomCreditTypeName = mi2
    x ShowCEUandGraduateCreditCourses = mi2
    x CEUCreditLabel = mi2
    x pricinghourtype = mi2 - bind credit type to pricing options

    x OptionalcredithoursLabel1 = mi3
    x OptionalcredithoursLabel2 = mi3
    x OptionalcredithoursLabel3 = mi3
    x OptionalcredithoursLabel4 = mi3
    x OptionalcredithoursLabel5 = mi3
    x OptionalcredithoursLabel6 = mi3
    x OptionalcredithoursLabel7 = mi3
    x OptionalcredithoursLabel8 = mi3
    x OptionalCredithoursvisible1 = mi3
     
    x PublicHideAllcredit = mi3
    x DefaultCreditType = mi3
    x OptionalCreditHideSpCategory = mi3
    x SingleCourseCreditCost = mi3

    Logic for setting variable names:
    if not isnull(rsMI2inc("CreditHoursName")) and len(rsMI2inc("CreditHoursName")) > 1 then
        strCreditHoursName=rsMI2inc("CreditHoursName")
    else
        strCreditHoursName="Credit"
    end if

    if not isnull(rsMI2inc("InserviceHoursName")) and len(rsMI2inc("InserviceHoursName")) > 1 then
        strInserviceHoursName=rsMI2inc("InserviceHoursName")
    else
        strInserviceHoursName="Inservice"
    end if

    if not isnull(rsMI2inc("CustomCreditTypeName")) and len(rsMI2inc("CustomCreditTypeName")) > 1 then
        strCustomCreditTypeName = rsMI2inc("CustomCreditTypeName")
    else
        strCustomCreditTypeName = "Credit Type"
    end if
 
    if rsMI2inc("ShowCustomCreditType") = 1 then
        bShowCustomCreditType = true
        ShowCustomCreditType = true
    else
        bShowCustomCreditType = false
        ShowCustomCreditType = false
    end if
 

    if rsMI2inc("ShowCEUandGraduateCreditCourses") = 1 then
        bShowCEUandGraduateCreditCourses = true
    else
        bShowCEUandGraduateCreditCourses = false
    end if
        strCEUCreditLabel = rsMI2inc("CEUCreditLabel")

    OptionalcredithoursLabel1 = rsMI3inc("OptionalcredithoursLabel1")
    OptionalcredithoursLabel2 = rsMI3inc("OptionalcredithoursLabel2")
    OptionalcredithoursLabel3 = rsMI3inc("OptionalcredithoursLabel3")
    OptionalcredithoursLabel4 = rsMI3inc("OptionalcredithoursLabel4")
    OptionalcredithoursLabel5 = rsMI3inc("OptionalcredithoursLabel5")
    OptionalcredithoursLabel6 = rsMI3inc("OptionalcredithoursLabel6")
    OptionalcredithoursLabel7 = rsMI3inc("OptionalcredithoursLabel7")
    OptionalcredithoursLabel8 = rsMI3inc("OptionalcredithoursLabel8")
    if rsMI3inc("OptionalCredithoursvisible1") = 1 then
        bShowOptionalCredithours1 = true
    else
        bShowOptionalCredithours1 = false
    end if

    Not related:
    UsePurchaseCredit = mi3
    LateFeeCharge = mi3
    NoofDaysToPurchaseHours = mi3
	*/
    public static class CourseCreditHelper
    {

        public static string GetCreditLabel(CourseCreditType credit)
        {
            switch (credit)
            {
                case CourseCreditType.Credit:
                    return CreditHoursLabel;

                case CourseCreditType.Ceu:
                    return CeuCreditLabel;

                case CourseCreditType.Graduate:
                    return GraduateCreditLabel;

                case CourseCreditType.Custom:
                    return CustomCreditTypeLabel;

                case CourseCreditType.InService:
                    return InserviceHoursLabel;

                case CourseCreditType.Optional1:
                    return OptionalCreditHoursLabel1;

                case CourseCreditType.Optional2:
                    return OptionalCreditHoursLabel2;

                case CourseCreditType.Optional3:
                    return OptionalCreditHoursLabel3;

                case CourseCreditType.Optional4:
                    return OptionalCreditHoursLabel4;

                case CourseCreditType.Optional5:
                    return OptionalCreditHoursLabel5;

                case CourseCreditType.Optional6:
                    return OptionalCreditHoursLabel6;

                case CourseCreditType.Optional7:
                    return OptionalCreditHoursLabel7;

                case CourseCreditType.Optional8:
                    return OptionalCreditHoursLabel8;

                default:
                    return null;
            }
        }


        public static CourseCreditType? GetCreditTypeByCustomDropDownString(string value)
        {
            value = value.ToLower();
            switch (value)
            {
                case "customoption1":
                    return CourseCreditType.Optional1;

                case "customoption2":
                    return CourseCreditType.Optional2;

                case "customoption3":
                    return CourseCreditType.Optional3;

                case "customoption4":
                    return CourseCreditType.Optional4;

                case "customoption5":
                    return CourseCreditType.Optional5;

                case "customoption6":
                    return CourseCreditType.Optional6;

                case "customoption7":
                    return CourseCreditType.Optional7;

                case "customoption8":
                    return CourseCreditType.Optional8;

                case "ceucredit9":
                    return CourseCreditType.Ceu;

                case "gradcredit10":
                    return CourseCreditType.Graduate;

                case "optionalcredit11":
                    return CourseCreditType.Custom;

                case "inservice12":
                    return CourseCreditType.InService;

                case "credithours13":
                    return CourseCreditType.Credit;

                default:
                    return null;
            }
        }

        public static List<CourseCreditType> GetAvailableCreditTypesForCourse(Entities.Course course, bool admin = false)
        {
            List<CourseCreditType> list = new List<CourseCreditType>();

            if (!admin && (course.HideCreditInPublic  || PublicHideAllCredit || course.CourseType != CourseType.Course))
            {
                return list;
            }

            if (!string.IsNullOrWhiteSpace(course.CustomDropDownValueSequence) && OptionalCreditHoursVisible)
            {
                var creditsList = course.CustomDropDownValueSequence.Split('|');
                if (creditsList.Length > 0)
                {
                    foreach (var creditString in creditsList)
                    {
                        if (!string.IsNullOrWhiteSpace(creditString))
                        {
                            var credit = GetCreditTypeByCustomDropDownString(creditString);
                            if (credit != null && IsCreditVisible(credit.Value))
                            {
                                list.Add(credit.Value);
                            }
                        }
                    }
                    return list;
                }
            }

            foreach(var enumValue in Enum.GetValues(typeof(CourseCreditType))) {
                var credit = (CourseCreditType)enumValue;
                if (IsCreditVisible(credit)) {
                    list.Add(credit);
                }
            }

            return list;
        }

        public static bool IsCreditVisible(CourseCreditType credit, bool admin = false)
        {
            if (!admin && PublicHideAllCredit) {
                return false;
            }

            switch (credit)
            {
                case CourseCreditType.Credit:
                    return !DontDisplayCreditHours;

                case CourseCreditType.Ceu:
                    return ShowCeauAndGraduateCreditCourses;

                case CourseCreditType.Graduate:
                    return ShowCeauAndGraduateCreditCourses;

                case CourseCreditType.Custom:
                    return ShowCustomCreditType;

                case CourseCreditType.InService:
                    return ShowInserviceTime;

                case CourseCreditType.Optional1:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel1);

                case CourseCreditType.Optional2:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel2);

                case CourseCreditType.Optional3:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel3);

                case CourseCreditType.Optional4:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel4);

                case CourseCreditType.Optional5:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel5);

                case CourseCreditType.Optional6:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel6);

                case CourseCreditType.Optional7:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel7);

                case CourseCreditType.Optional8:
                    return OptionalCreditHoursVisible && !string.IsNullOrWhiteSpace(OptionalCreditHoursLabel8);

                default:
                    throw new NotImplementedException(credit.ToString() + " is not implemented for IsCreditVisible.");
            }
        }

        public static bool ShowCreditHours
        {
            get
            {
                return DontDisplayCreditHours == false;
            }
        }

        public static bool DontDisplayCreditHours
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo().DontDisplayCreditHours);
            }
        }

        public static bool ShowInserviceTime
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo().ShowInservice);
            }
        }

        public static string CreditHoursLabel
        {
            get
            {
                return Settings.GetStringOrDefault(Settings.Instance.GetMasterInfo2().CreditHoursName, "Credit");
            }
        }

        public static string InserviceHoursLabel
        {
            get
            {
                return Settings.GetStringOrDefault(Settings.Instance.GetMasterInfo2().InserviceHoursName, "Inservice");
            }
        }

        public static string CustomCreditTypeLabel
        {
            get
            {
                return Settings.GetStringOrDefault(Settings.Instance.GetMasterInfo2().CustomCreditTypeName, "Credit Type");
            }
        }

        public static bool ShowCustomCreditType
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo2().ShowCustomCreditType);
            }
        }

        public static bool ShowCeauAndGraduateCreditCourses
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo2().ShowCEUandGraduateCreditCourses);
            }
        }

        public static string CeuCreditLabel
        {
            get
            {
                return Settings.GetStringOrDefault(Settings.Instance.GetMasterInfo2().CEUCreditLabel, "CEU Credit");
            }

        }

        public static string GraduateCreditLabel
        {
            get
            {
                return "Graduate Credit";
            }

        }

        public static bool OptionalCreditHoursVisible
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo3().OptionalCredithoursvisible1);
            }
        }

        public static string OptionalCreditHoursLabel1
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1;
            }
        }

        public static string OptionalCreditHoursLabel2
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2;
            }
        }

        public static string OptionalCreditHoursLabel3
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3;
            }
        }

        public static string OptionalCreditHoursLabel4
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4;
            }
        }

        public static string OptionalCreditHoursLabel5
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5;
            }
        }

        public static string OptionalCreditHoursLabel6
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6;
            }
        }

        public static string OptionalCreditHoursLabel7
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7;
            }
        }

        public static string OptionalCreditHoursLabel8
        {
            get
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8;
            }
        }

        public static bool PricingHourType
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo2().PricingHourType);
            }
        }

        public static bool BindCreditTypeToPricingOptions
        {
            get
            {
                return PricingHourType;
            }
        }

        public static bool HideCreditSpecialCategory
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo3().OptionalCreditHideSpCategory);
            }
        }

        public static int DefaultCreditType
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo3().DefaultCreditType;
                return value.HasValue ? value.Value : 0;
            }
        }

        /*
		<option value=0>Course Default</option>
		<option value=1 <% if trim(DefaultCreditType) = "1" then %> selected <% end if %>>NONE</option>
		<option value=2 <% if trim(DefaultCreditType) = "2" then %> selected <% end if %>>ALL/BOTH</option>
		<option value=3 <% if trim(DefaultCreditType) = "3" then %> selected <% end if ' ch%>><%=CredithoursName%> (CH)</option>
		<option value=4 <% if trim(DefaultCreditType) = "4" then %> selected <% end if 'cch %>><%=CustomCreditTypeName%> (CCH)</option>
		<option value=5 <% if trim(DefaultCreditType) = "5" then %> selected <% end if ' ish %>><%=InservicehoursName%> (ISH)</option>
		<option value=6 <% if trim(DefaultCreditType) = "6" then %> selected <% end if ' och %>><%=OptionalcredithoursLabel1%> (Optional CH)</option>
        */
        public static string DefaultCreditTypeLabel
        {
            get
            {
                string result = null;
                switch (DefaultCreditType)
                {
                    case 0:
                        result =  "Course default";
                        break;

                    case 1:
                        result = "None";
                        break;

                    case 2:
                        result = "All/Both";
                        break;

                    case 3:
                        result = CreditHoursLabel;
                        break;

                    case 4:
                        result = CustomCreditTypeLabel;
                        break;

                    case 5:
                        result = InserviceHoursLabel;
                        break;

                    case 6:
                        result = OptionalCreditHoursLabel1;
                        break;

                }
                return result;
            }
        }

        public static decimal SingleCourseCreditCost
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo3().SingleCourseCreditCost;
                return value.HasValue ? value.Value : 0;
            }
        }

        public static bool PublicHideAllCredit
        {
            get
            {
                return Settings.GetVbScriptBoolValue(Settings.Instance.GetMasterInfo3().PublicHideAllcredit);
            }
        }
    }
}
