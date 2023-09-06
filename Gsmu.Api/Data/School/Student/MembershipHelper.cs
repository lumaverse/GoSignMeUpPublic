using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Student
{
    public static class MembershipHelper
    {
        public static bool MembershipEnabled
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().Membership_Status;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        public static bool AllowMembersSeeTheirMembershiptype
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().allowStudentSelectMembershipOnRegistration ?? 0;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        public static bool AllowMembersChangeTheirMembershiptype
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().allowStudentEditMemberType;
                return Settings.GetVbScriptBoolValue(value);
            }
        }

        public static string MembershipName
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().Field4Name;
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "Membership";
                }
                return value;
            }
        }

        public static MembershipType DefaultNewStudentMemberType
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType;
                switch (value)
                {
                    case 2:
                        return MembershipType.Member;

                    case 1:
                        return MembershipType.NonMember;

                    case 3:
                        return MembershipType.Special1;

                    default:
                        throw new NotImplementedException();
                }

            }
        }

        public static MembershipType GetMembershipType(int distemployee)
        {
            if ((distemployee == -1)|| (distemployee==1))
            {
                return MembershipType.Member;
            }
            return (MembershipType)distemployee;
        }
        public static string GetMembershipTypeLabelByStudent(int studentid)
        {
            short distemployee = 0;
            using (var db = new SchoolEntities())
            {
                var studentdistemployee = (from s in db.Students where s.STUDENTID == studentid select s.DISTEMPLOYEE).FirstOrDefault();
                    distemployee = studentdistemployee;
            }

            if (Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCart.Instance.Items.FirstOrDefault() != null)
            {
                if (Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCart.Instance.Items.FirstOrDefault().HasMembership)
                {
                    distemployee = 1;
                }
            }
            if ((distemployee == -1) || (distemployee == 1))
            {
                return MemberLabel;
            }
            else
            {
                return NonMemberLabel;
            }
        }

        public static string MemberLabel
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().membertypememberlabel;
                if (Settings.Instance.GetMasterInfo().Membership_Status != 0)
                {
                    return string.IsNullOrEmpty(value) ? "Member" : value;
                }
                else
                {
                    return string.IsNullOrEmpty(value) ? "" : value;
                }
                
            }
        }

        public static string MemberComment
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().membertypemembercomment;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }
    
        public static string NonMemberLabel
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().membertypenonmemberlabel;
                if (Settings.Instance.GetMasterInfo().Membership_Status != 0)
                {
                    return string.IsNullOrEmpty(value) ? "Non Member" : value;
                }
                else
                {
                    return string.IsNullOrEmpty(value) ? "" : value;
                }
            }
        }

        public static string NonMemberComment
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().membertypenonmembercomment;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }
    
        public static string Special1Label
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().MemberTypeSpecialMemberLabel1;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        public static string Special1Comment
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo2().MemberTypeSpecialMemberComment1;
                return string.IsNullOrEmpty(value) ? "" : value;
            }
        }

        public static MembershipType? GetForceMembershipFlagToMembershipType(int? membershipForceFlag)
        {
            switch (membershipForceFlag)
            {
                case 1:
                    return MembershipType.NonMember;

                case 2:
                    return MembershipType.Member;

                case 3:
                    return MembershipType.Special1;

                default:
                    return null;
            }
        }

        public static void CalculateMembership(Entities.SchoolEntities db, Entities.Student st)
        {
            if (!MembershipEnabled)
            {
                var membership = GetForceMembershipFlagToMembershipType(Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType);
                    if (membership != null)
                    {
                        st.DISTEMPLOYEE = (short)membership.Value;
                        return;
                    }
               // return;
            }

            if (st.SCHOOL.HasValue && st.SCHOOL.Value > 0)
            {
                var school = (from s in db.Schools where s.locationid == st.SCHOOL.Value select s).FirstOrDefault();
                if (school != null)
                {
                    if (school.MembershipFlag.HasValue && school.MembershipFlag.Value > 0)
                    {
                        var membership = GetForceMembershipFlagToMembershipType(school.MembershipFlag);
                        if (membership != null) {
                            st.DISTEMPLOYEE = (short)membership.Value;
                            return;
                        }
                    }
                }
            }

            if (st.DISTRICT.HasValue && st.DISTRICT.Value > 0)
            {
                var district = (from d in db.Districts where d.DISTID == st.DISTRICT.Value select d).FirstOrDefault();
                if (district != null)
                {
                    if (district.MembershipFlag.HasValue && district.MembershipFlag.Value > 0)
                    {
                        var membership = GetForceMembershipFlagToMembershipType(district.MembershipFlag);
                        if (membership != null) {
                            st.DISTEMPLOYEE = (short)membership.Value;
                            return;
                        }
                    }
                }
            }
        }

        public static void CheckMembershipExpiry(Entities.Student currentstudent)
        {
            if (MembershipEnabled)
            {
               
                using (var db = new SchoolEntities())
                {
                    var latestmembership = (from course in db.Courses
                                            where course.Membership == 1 && course.CANCELCOURSE != -1
                                            orderby course.COURSEID descending
                                            select course).FirstOrDefault();

                    if (latestmembership != null)
                    {
                        var checkstudentmembership = (from roster in db.Course_Rosters
                                                      where roster.COURSEID == latestmembership.COURSEID && roster.STUDENTID == currentstudent.STUDENTID && roster.Cancel == 0
                                                      select roster).FirstOrDefault();
                        if(currentstudent.DISTRICT!=null && currentstudent.DISTRICT != 0)
                        {
                            var districtmembership = (from dist in db.Districts
                                                      where dist.DISTID == currentstudent.DISTRICT select dist).FirstOrDefault();
                            if (districtmembership != null)
                            {
                                int membershipflag = 0;
                                if(districtmembership.MembershipFlag == 2)
                                {
                                    membershipflag = -1;
                                }
                                else if (districtmembership.MembershipFlag == 1)
                                {
                                    membershipflag = 0;
                                }
                                else if (districtmembership.MembershipFlag == 3)
                                {
                                    membershipflag = 3;
                                }
                                currentstudent.DISTEMPLOYEE =short.Parse(membershipflag.ToString());
                                db.SaveChanges();
                            }

                        }

                        else if (checkstudentmembership == null)
                        {
                            if (currentstudent.DISTEMPLOYEE != 3)
                            {
                                currentstudent.DISTEMPLOYEE = 0;
                                db.SaveChanges();
                            }

                        }
                    }
                }

            }
        }
    }
}
