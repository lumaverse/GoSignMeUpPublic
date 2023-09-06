using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Update.Modules
{
    internal class SupervisorActiveFieldUpdate : AbstractUpdater
    {
        internal override bool Execute()
        {
            using (var db = new SchoolEntities())
            {
                using (var trx = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Database.ExecuteSqlCommand("UPDATE Supervisors SET Active = 1 WHERE Active = 0");
                        db.Database.ExecuteSqlCommand("UPDATE Supervisors SET Active = 0 WHERE Active = -1");
                        db.SaveChanges();
                        trx.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        trx.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
