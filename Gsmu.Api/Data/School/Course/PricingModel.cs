using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Commerce.ShoppingCart;

namespace Gsmu.Api.Data.School.Course
{
    public class PricingModel
    {
        public CoursePricingOption CoursePricingOption
        {
            get;
            set;
        }

        public PricingOption PricingOption
        {
            get;
            set;
        }

        public MembershipType MembershipType
        {
            get;
            set;
        }

        public bool IsOption
        {
            get
            {
                return CoursePricingOption != null && PricingOption != null;
            }
        }

        public bool Disabled
        {
            get;
            set;
        }

        public decimal NonOptionPrice
        {
            get;
            set;
        }

        public string Label
        {
            get
            {
                if (IsOption)
                {
                    return PricingOption.PriceTypedesc;
                }
                else
                {
                    switch (MembershipType)
                    {
                        case Student.MembershipType.Member:
                            return MembershipHelper.MemberLabel;

                        case Student.MembershipType.NonMember:
                            return MembershipHelper.NonMemberLabel;

                        case Student.MembershipType.Special1:
                            return MembershipHelper.Special1Label;

                        default:
                            if (Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType == 2)
                            {
                                return MembershipHelper.MemberLabel;
                            }
                            else
                            {
                                return MembershipHelper.NonMemberLabel;
                            }
                          //  throw new NotImplementedException();

                    }
                }
            }
        }
        public string ClubReadyPriceDescription
        {
            get;
            set;
        }
        public int ClubReadyInstallmentId
        {
            get;
            set;
        }
        public decimal EffectivePrice
        {
            get
            {
                if (!IsOption)
                {
                    return NonOptionPrice;
                }
                if (PricingOptionsHelper.PricingOptionOverride)
                {
                    return CoursePricingOption.Price;
                }
                else
                {
                    switch (MembershipType)
                    {
                        case Student.MembershipType.Member:
                            return PricingOption.Price;

                        case Student.MembershipType.NonMember:
                            return PricingOption.NonPrice;

                        case Student.MembershipType.Special1:
                            return PricingOption.SpecialMemberPrice1;

                        default:
                            if (Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType == 2)
                            {
                                return PricingOption.Price;
                            }
                            else
                            {
                                return PricingOption.NonPrice;
                            }
                        //throw new NotImplementedException();
                    }
                }
            }
        }

        public bool IsAvailable(CourseModel model)
        {
            if (!IsOption)
            {
                if (!PricingOptionsHelper.PricingVisible)
                {
                    return true;
                }
                else if (Settings.GetVbScriptBoolValue(model.Course.DisplayPrice))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (!PricingOptionsHelper.PricingVisible)
                {
                    return false;
                }
                else
                {
                    if (PricingOptionsHelper.PricingOptionRange)
                    {
                        return IsPricingOptionAvailableByRange(model);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        private bool IsPricingOptionAvailableByRange(CourseModel model)
        {
            int rangeStart = PricingOption.rangestart ?? 0;
            int rangeEnd = PricingOption.rangeend ?? 0;
            if (CoursePricingOption.rangestart != 0 || CoursePricingOption.rangeend != 0)
            {
                rangeStart = CoursePricingOption.rangestart ?? 0;
                rangeEnd = CoursePricingOption.rangeend ?? 0;
            }
            if (rangeStart == 0 && rangeEnd == 0)
            {
                return true;
            }

            var courseStartDate = model.CourseTimes.FirstOrDefault();
            if (courseStartDate == null || courseStartDate.COURSEDATE == null)
            {
                return true;
            }
            var date = courseStartDate.COURSEDATE.Value;
            var today = DateTime.Now;
            var dateRangeStart = date.AddDays(-rangeStart);
            var dateRangeEnd = date.AddDays(-rangeEnd);
            dateRangeEnd = dateRangeEnd.AddHours(24);
            return today >= dateRangeStart && today <= dateRangeEnd;
        }
    }
}
