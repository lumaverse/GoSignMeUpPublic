using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Student;

namespace Gsmu.Api.Data.School.Entities
{
    /// <summary>
    /// Make sure to call the refresh method if the user is logged in and a new enrollment is added!
    /// </summary>
    public partial class Supervisor : AbstractSiteUser
    {
        public override int SiteUserId
        {
            get { return this.SUPERVISORID; }
        }
        public override string SiteUserEmailAddress
        {
            get { return this.EMAIL; }
        }

        public override string WelcomeName
        {
            get
            {
                return this.FIRST + " " + this.LAST;
            }
        }

        public override string LoggedInUsername
        {
            get
            {
                return this.UserName;
            }
        }

        public override LoggedInUserType LoggedInUserType
        {
            get
            {
                return LoggedInUserType.Supervisor;
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
                return MembershipType.Member;
            }
        }
        public void UpdateSupervisorLastLogin()
        {
            using (var db = new SchoolEntities())
            {
                var supervisorLastLogin = (from sup in db.Supervisors where sup.SUPERVISORID == this.SUPERVISORID select sup).SingleOrDefault();
                if (supervisorLastLogin != null)
                {
                    try
                    {
                        supervisorLastLogin.LastLogin = DateTime.Now.Date;
                        supervisorLastLogin.UserSessionId = this.UserSessionId;
                        try
                        {
                            if (supervisorLastLogin.PASSWORD.Length >= 50)
                            {
                                supervisorLastLogin.PASSWORD = supervisorLastLogin.PASSWORD.Substring(0, 49);
                            }
                            db.SaveChanges();
                        }
                        catch {
                            supervisorLastLogin.PASSWORD = "Auto Assigned/Catch Assigned";
                            db.SaveChanges();
                        }
                        
                    }
                    catch(DbEntityValidationException e)
                    {
                        string error = "";
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            error = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state \"{1}\" has the following validation errors:";
                            foreach (var ve in eve.ValidationErrors)
                            {
                                error= error+"- Property:"+ ve.PropertyName + ", Error:"+ ve.ErrorMessage;
                            }
                        }
                        throw new Exception("Your account has invalid session" + error);
                    }
                }
            }

        }
    }
}
