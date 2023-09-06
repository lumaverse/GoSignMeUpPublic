using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Update
{
    public static class Updater
    {
        public static void Execute()
        {
            new Modules.ConnectionStringsConfigFileUpdater().Execute();
            
            // every update module is attached to a system update level.
            // for example the supervisor fix is update level 0
            // if the update level is 1, it means that the supervisor fix already ran
            // if you want to add a new update routine
            // that should have the number 1 as udpate level, next is 2, but please, just comment in the code here
            // like it is shown here
            // the update level is stored in the app_data file

            // of course if your update routine can run other checks so that it does not need to use this, it is probably
            // best not having to use this

            // if possible in db routines, please try to use transactions to make it safe
            // please just make sure you implement the iupdater interface, very simple and that's all

            ExecuteUpdate(0, new Modules.SupervisorActiveFieldUpdate());

            /*
            // 2nd one
            ExecuteUpdate(1, new Modules.AnotherUpdater());             
             */
        }

        private static void ExecuteUpdate(int level, AbstractUpdater updater)
        {
            masterinfo4 mi4 = Settings.Instance.GetMasterInfo4();
            var rubyUpdateLevel = mi4.RubyUpdateLevel ?? 0;
            if (rubyUpdateLevel == level)
            {
                try
                {
                    if (updater.Execute())
                    {
                        rubyUpdateLevel++;
                        mi4.RubyUpdateLevel = rubyUpdateLevel;
                        Settings.Instance.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Gsmu.Api.Logging.LogManager.LogException("Updater", "Could not execute updater with type" + updater.GetType().ToString(), e);
                }
            }
        }

    }
}
