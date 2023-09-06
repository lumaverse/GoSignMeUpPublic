using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Student;

namespace Gsmu.Api.Authorization
{
    public abstract class AbstractSiteUser
    {
        public abstract string WelcomeName { get; }

        public abstract string LoggedInUsername { get; }

        public abstract LoggedInUserType LoggedInUserType { get; }

        public abstract MembershipType MembershipType { get; }

        public string MembershipLabel
        {
            get
            {
                switch (MembershipType)
                {
                    case MembershipType.Member:
                        return MembershipHelper.MemberLabel;

                    case MembershipType.NonMember:
                        return MembershipHelper.NonMemberLabel;

                    case MembershipType.Special1:
                        return MembershipHelper.Special1Label;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public string MembershipComment
        {
            get
            {
                switch (MembershipType)
                {
                    case MembershipType.Member:
                        return MembershipHelper.MemberComment;

                    case MembershipType.NonMember:
                        return MembershipHelper.NonMemberComment;

                    case MembershipType.Special1:
                        return MembershipHelper.Special1Comment;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public abstract string SiteUserEmailAddress
        {
            get;
        }

        public abstract bool IsLoggedIn
        {
            get;
        }

        public abstract int SiteUserId
        {
            get;
        }
    }
}
