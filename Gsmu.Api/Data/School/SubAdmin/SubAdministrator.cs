using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Authorization;
using Gsmu.Api.Web;
using Gsmu.Api.Data.School.Student;

namespace Gsmu.Api.Data.School.Entities
{
    /// <summary>
    /// Make sure to call the refresh method if the user is logged in and a new enrollment is added!
    /// </summary>
    public partial class Manager : AbstractSiteUser
    {
        public override int SiteUserId
        {
            get { return this.AUTOINDEX; }
        }

        public override string SiteUserEmailAddress
        {
            get { return this.EMAIL; }
        }

        public override string WelcomeName
        {
            get {
                return "Sub Administrator";
            }
        }

        public override string LoggedInUsername
        {
            get
            {
                return this.MANAGERID;
            }
        }

        public override LoggedInUserType LoggedInUserType
        {
            get {
                return LoggedInUserType.SubAdmin;
            }
        }

        public override bool IsLoggedIn
        {
            get
            {
                var user = AuthorizationHelper.CurrentUser;
                return user.LoggedInUserType == this.LoggedInUserType && user.LoggedInUsername == this.LoggedInUsername;
            }
        }

        public override MembershipType MembershipType
        {
            get
            {
                return 0;
            }
        }



    }
}
