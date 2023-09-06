using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data;

namespace Gsmu.Api.Authorization
{
    public class GuestUser : AbstractSiteUser
    {
        public override int SiteUserId
        {
            get { return 0; }
        }



        public override string WelcomeName
        {
            get {
                return "Visitor";
            }
        }

        public override string LoggedInUsername
        {
            get {
                return "Guest";
            }
        }

        public override LoggedInUserType LoggedInUserType
        {
            get {
                return LoggedInUserType.Guest;
            }
        }

        public override MembershipType MembershipType
        {
            get
            {
                return PricingOptionsHelper.DefaultmembershipType;
            }
        }

        public override string SiteUserEmailAddress
        {
            get { throw new NotImplementedException("The Gues user does not have an e-mail address, please do a type check before retrieving this."); }
        }

        public override bool IsLoggedIn
        {
            get {
                return false;
            }
        }
    }
}
